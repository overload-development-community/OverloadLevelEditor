#include "PxPhysicsAPI.h"
#include <stdlib.h>
#include <stdint.h>
#include <map>

typedef std::map< uint32_t, physx::PxRigidStatic* > map_actor;

extern "C" {
	__declspec(dllexport) int __cdecl editor_physics_initialize();
	__declspec(dllexport) void __cdecl editor_physics_shutdown();
	__declspec(dllexport) uint32_t __cdecl editor_register_trianglemesh(uint32_t num_verts, float *verts, uint32_t num_tris, uint32_t *indices32, uint32_t layer_mask);
	__declspec(dllexport) void __cdecl editor_deregister_trianglemesh(uint32_t id);
	__declspec(dllexport) void __cdecl editor_deregister_all_trianglemeshes();
	__declspec(dllexport) int __cdecl editor_do_linecast(float ox, float oy, float oz, float dx, float dy, float dz, float max_dist, uint32_t layer_mask);
}


class physics_allocator : public physx::PxAllocatorCallback
{
public:
	void* allocate(size_t size, const char* /*typeName*/, const char* /*filename*/, int /*line*/)
	{
		return _aligned_malloc(size, 16);
	}

	void deallocate(void* ptr)
	{
		_aligned_free(ptr);
	}
};

class physics_error_callback : public physx::PxErrorCallback
{
public:
	void reportError(physx::PxErrorCode::Enum /*code*/, const char* message, const char* /*file*/, int /*line*/)
	{
		puts(message);
	}
};

static physics_error_callback gDefaultErrorCallback;
static physics_allocator      gDefaultAllocatorCallback;
static physx::PxFoundation *gFoundation = nullptr;
static physx::PxPvd *gPvd = nullptr;
static physx::PxPhysics *gPhysics = nullptr;
static physx::PxCooking *gCooking = nullptr;
static physx::PxScene *gScene = nullptr;
static physx::PxMaterial *gMaterial = nullptr;
static map_actor gActorMap;
static uint32_t gActorId = 1;

static physx::PxTolerancesScale get_default_tolerance_scale()
{
	physx::PxTolerancesScale scale;
	scale.length = 1.0f;      // typical length of an object
	scale.mass = 1000.0f;
	scale.speed = 9.81f;      // typical speed of an object, gravity*1s is a reasonable choice
	return scale;
}

int editor_physics_initialize()
{
	//
	// Setup Foundation
	//
	gFoundation = PxCreateFoundation(PX_FOUNDATION_VERSION, gDefaultAllocatorCallback, gDefaultErrorCallback);
	if (!gFoundation) {
		return 0;
	}

	//gPvd = PxCreatePvd(*gFoundation);
	//physx::PxPvdTransport* transport = physx::PxDefaultPvdSocketTransportCreate(physx::PVD_HOST, 5425, 10);
	//gPvd->connect(*transport, physx::PxPvdInstrumentationFlag::eALL);

	// 
	// Setup Physics
	//
	const bool recordMemoryAllocations = false;
	gPhysics = PxCreatePhysics(PX_PHYSICS_VERSION, *gFoundation, physx::PxTolerancesScale(), recordMemoryAllocations, gPvd);
	if (!gPhysics) {
		gFoundation->release();
		gFoundation = nullptr;
		return 0;
	}

	//
	// Setup Cooking
	//
	gCooking = PxCreateCooking(PX_PHYSICS_VERSION, *gFoundation, physx::PxCookingParams(get_default_tolerance_scale()));
	if (!gCooking) {
		gPhysics->release();
		gFoundation->release();
		gPhysics = nullptr;
		gFoundation = nullptr;
		return 0;
	}

	//
	// Setup Scene
	//
	physx::PxSceneDesc scene_desc(get_default_tolerance_scale());
	gScene = gPhysics->createScene(scene_desc);
	if (!gScene) {
		gCooking->release();
		gPhysics->release();
		gFoundation->release();
		gCooking = nullptr;
		gPhysics = nullptr;
		gFoundation = nullptr;
		return 0;
	}

	//
	// Setup Material
	//
	gMaterial = gPhysics->createMaterial(0.0f, 0.0f, 0.0f);
	if (!gMaterial) {
		gScene->release();
		gCooking->release();
		gPhysics->release();
		gFoundation->release();
		gScene = nullptr;
		gCooking = nullptr;
		gPhysics = nullptr;
		gFoundation = nullptr;
		return 0;
	}

	return 1;
}

void cleanup_actor(physx::PxRigidStatic *actor)
{
	gScene->removeActor(*actor);

	physx::PxShape *shape;
	actor->getShapes(&shape, 1);

	physx::PxTriangleMeshGeometry geom;
	shape->getTriangleMeshGeometry(geom);

	shape->release();
	geom.triangleMesh->release();
	actor->release();
}

void editor_physics_shutdown()
{
	editor_deregister_all_trianglemeshes();

	if (gMaterial) {
		gMaterial->release();
		gMaterial = nullptr;
	}

	if (gScene) {
		gScene->release();
		gScene = nullptr;
	}

	if (gCooking) {
		gCooking->release();
		gCooking = nullptr;
	}

	if (gPhysics) {
		gPhysics->release();
		gPhysics = nullptr;
	}

	if (gFoundation) {
		gFoundation->release();
		gFoundation = nullptr;
	}
}

uint32_t editor_register_trianglemesh( uint32_t num_verts, float *verts, uint32_t num_tris, uint32_t *indices32, uint32_t layer_mask )
{
	physx::PxCookingParams params(get_default_tolerance_scale());
	params.meshPreprocessParams |= physx::PxMeshPreprocessingFlag::eWELD_VERTICES;
	params.meshPreprocessParams |= physx::PxMeshPreprocessingFlag::eDISABLE_ACTIVE_EDGES_PRECOMPUTE;
	params.meshCookingHint = physx::PxMeshCookingHint::eSIM_PERFORMANCE;
	gCooking->setParams(params);

	physx::PxTriangleMeshDesc meshDesc;
	meshDesc.points.count  = num_verts;
	meshDesc.points.stride = sizeof(physx::PxVec3);
	meshDesc.points.data   = verts;

	meshDesc.triangles.count  = num_tris;
	meshDesc.triangles.stride = 3 * sizeof(physx::PxU32);
	meshDesc.triangles.data   = indices32;

#ifdef _DEBUG
	// mesh should be validated before cooked without the mesh cleaning
	bool res = gCooking->validateTriangleMesh(meshDesc);
	PX_ASSERT(res);
#endif

	physx::PxTransform pose(physx::PxIdentity);
	physx::PxRigidStatic *actor = gPhysics->createRigidStatic(pose);
	actor->setActorFlag(physx::PxActorFlag::eDISABLE_SIMULATION, true);

	physx::PxTriangleMeshGeometry geom;
	geom.triangleMesh = gCooking->createTriangleMesh(meshDesc, gPhysics->getPhysicsInsertionCallback());

	physx::PxFilterData filterData(layer_mask, 0, 0, 0);
	physx::PxShape *shape = gPhysics->createShape(geom, *gMaterial, true, physx::PxShapeFlag::eSCENE_QUERY_SHAPE);
	shape->setQueryFilterData(filterData);
	actor->attachShape(*shape);
	shape->release();

	gScene->addActor(*actor);

	uint32_t id = gActorId++;
	gActorMap.insert(map_actor::value_type(id, actor));

	return id;
}

void editor_deregister_trianglemesh(uint32_t id)
{
	auto it = gActorMap.find(id);
	if (it == gActorMap.end()) {
		return;
	}

	physx::PxRigidStatic *actor = it->second;
	gActorMap.erase(it);
	cleanup_actor(actor);
}

void editor_deregister_all_trianglemeshes()
{
	for (auto it = gActorMap.begin(), end_it = gActorMap.end(); it != end_it; ++it) {
		physx::PxRigidStatic *actor = it->second;
		cleanup_actor(actor);
	}
	gActorMap.clear();
}

int editor_do_linecast(float ox, float oy, float oz, float dx, float dy, float dz, float max_dist, uint32_t layer_mask)
{
	physx::PxFilterData filter(layer_mask, 0, 0, 0);
	physx::PxQueryFilterData filterData(filter, physx::PxQueryFlag::eSTATIC);

	physx::PxRaycastBuffer hit;
	bool res = gScene->raycast(physx::PxVec3(ox, oy, oz), physx::PxVec3(dx, dy, dz), max_dist, hit, physx::PxHitFlag::eMESH_ANY, filterData);
	return res ? 1 : 0;
}