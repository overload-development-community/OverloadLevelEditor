#include "unitycrunch.h"
#include <stdint.h>
#include <algorithm>
#include "unitycrunch/crn_decomp.h"
using namespace unitycrnd;

bool unity_crunch_unpack_level(const uint8_t* data, uint32_t data_size, uint32_t level_index, void** ret, uint32_t* ret_size) {
	unitycrnd::crn_texture_info tex_info;
	if (!unitycrnd::crnd_get_texture_info(data, data_size, &tex_info))
	{
		return false;
	}

	unitycrnd::crnd_unpack_context pContext = unitycrnd::crnd_unpack_begin(data, data_size);
	if (!pContext)
	{
		return false;
	}

	const crn_uint32 width = std::max(1U, tex_info.m_width >> level_index);
	const crn_uint32 height = std::max(1U, tex_info.m_height >> level_index);
	const crn_uint32 blocks_x = std::max(1U, (width + 3) >> 2);
	const crn_uint32 blocks_y = std::max(1U, (height + 3) >> 2);
	const crn_uint32 row_pitch = blocks_x * unitycrnd::crnd_get_bytes_per_dxt_block(tex_info.m_format);
	const crn_uint32 total_face_size = row_pitch * blocks_y;
	*ret = new uint8_t[total_face_size];
	*ret_size = total_face_size;
	if (!unitycrnd::crnd_unpack_level(pContext, ret, total_face_size, row_pitch, level_index))
	{
		unitycrnd::crnd_unpack_end(pContext);
		return false;
	}
	unitycrnd::crnd_unpack_end(pContext);
	return true;
}

void *unity_crunch_unpack_init(const void* hdr_data, uint32_t hdr_size, int level_index, uint32_t* pDataOfs, uint32_t* pDataSize, uint32_t* ret_size)
{
	const crn_header *hdr = static_cast<const crn_header*>(hdr_data);
	uint32_t need_hdr_size = crnd_get_segmented_file_size(hdr_data, hdr->m_data_size);
	if (need_hdr_size == 0 || hdr_size < need_hdr_size)
	{
		*ret_size = need_hdr_size;
		return NULL;
	}

	unitycrnd::crnd_unpack_context pContext = unitycrnd::crnd_unpack_begin(hdr_data, hdr->m_data_size);
	if (!pContext)
	{
		*ret_size = 0;
		return NULL;
	}

	const crn_uint32 width = std::max(1U, hdr->m_width >> level_index);
	const crn_uint32 height = std::max(1U, hdr->m_height >> level_index);
	const crn_uint32 blocks_x = std::max(1U, (width + 3) >> 2);
	const crn_uint32 blocks_y = std::max(1U, (height + 3) >> 2);
	const crn_uint32 row_pitch = blocks_x * unitycrnd::crnd_get_bytes_per_dxt_block(static_cast<crn_format>((int)hdr->m_format));
	const crn_uint32 total_face_size = row_pitch * blocks_y;
	*ret_size = total_face_size;

	uint32 cur_level_ofs = hdr->m_level_ofs[level_index];

	uint32 next_level_ofs = hdr->m_data_size;
	if ((level_index + 1) < (hdr->m_levels))
		next_level_ofs = hdr->m_level_ofs[level_index + 1];

	*pDataOfs = cur_level_ofs;
	*pDataSize = next_level_ofs - cur_level_ofs;

	return pContext;
}

bool unity_crunch_unpack_level_data(void* pContext, int level_index, const void* pData, uint32_t pDataSize, void *pDst, int pDstSize)
{
	crn_unpacker* pUnpacker = static_cast<crn_unpacker*>(pContext);

	if (!pUnpacker->is_valid())
		return false;

	const crn_header *hdr = pUnpacker->get_header();
	const crn_uint32 width = std::max(1U, hdr->m_width >> level_index);
	const crn_uint32 height = std::max(1U, hdr->m_height >> level_index);
	const crn_uint32 blocks_x = std::max(1U, (width + 3) >> 2);
	const crn_uint32 blocks_y = std::max(1U, (height + 3) >> 2);
	const crn_uint32 row_pitch = blocks_x * unitycrnd::crnd_get_bytes_per_dxt_block(static_cast<crn_format>((int)hdr->m_format));
	const crn_uint32 total_face_size = row_pitch * blocks_y;

	return pUnpacker->unpack_level(pData, pDataSize, &pDst, pDstSize, row_pitch, level_index);
}

void unity_crunch_unpack_done(void *pContext)
{
	unitycrnd::crnd_unpack_end(pContext);
}
