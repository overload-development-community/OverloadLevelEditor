/*
THE COMPUTER CODE CONTAINED HEREIN IS THE SOLE PROPERTY OF REVIVAL
PRODUCTIONS, LLC ("REVIVAL").  REVIVAL, IN DISTRIBUTING THE CODE TO
END-USERS, AND SUBJECT TO ALL OF THE TERMS AND CONDITIONS HEREIN, GRANTS A
ROYALTY-FREE, PERPETUAL LICENSE TO SUCH END-USERS FOR USE BY SUCH END-USERS
IN USING, DISPLAYING,  AND CREATING DERIVATIVE WORKS THEREOF, SO LONG AS
SUCH USE, DISPLAY OR CREATION IS FOR NON-COMMERCIAL, ROYALTY OR REVENUE
FREE PURPOSES.  IN NO EVENT SHALL THE END-USER USE THE COMPUTER CODE
CONTAINED HEREIN FOR REVENUE-BEARING PURPOSES.  THE END-USER UNDERSTANDS
AND AGREES TO THE TERMS HEREIN AND ACCEPTS THE SAME BY USE OF THIS FILE.  
COPYRIGHT 2015-2020 REVIVAL PRODUCTIONS, LLC.  ALL RIGHTS RESERVED.
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace OverloadLevelEditor.Clipping
{
	/// <summary>
	/// Generic interface for vertices through the clipper
	/// This allows us to clip any kind of polygon no matter how much data is contain in the vertex (as long as it has a Position)
	/// </summary>
	public interface IVertex
	{
		OpenTK.Vector3 Position{ get; }

		/// <summary>
		/// Must create a new IVertex and return it. The result should be the vertex from [this,other] using t, which is [0,1]
		/// </summary>
		/// <param name="other">The vertex to lerp towards (represented by t==1.0)</param>
		/// <param name="t">The lerp factor</param>
		/// <returns>A *new* IVertex object with the lerped vertex</returns>
		IVertex LerpTo(IVertex other, float t);
	}

	/// <summary>
	/// Classification to a ClipPlane
	/// </summary>
	public enum ClipSide
	{
		Back,
		Front,
		CoPlanar,
	}

	public class ClipPlane
	{
		public float A;
		public float B;
		public float C;
		public float D;

		/// <summary>
		/// Given a normal for the plane and a point on the plane, this creates a ClipPlane
		/// </summary>
		/// <param name="normal"></param>
		/// <param name="pt"></param>
		/// <returns></returns>
		public static ClipPlane CreateFromNormalAndPoint(OpenTK.Vector3 normal, OpenTK.Vector3 pt)
		{
			return new ClipPlane {A = normal.X, B = normal.Y, C = normal.Z, D = -OpenTK.Vector3.Dot( normal, pt )};
		}

		public static ClipPlane CreateFrom3Points(OpenTK.Vector3 p1, OpenTK.Vector3 p2, OpenTK.Vector3 p3)
		{
			OpenTK.Vector3 normal = Utility.FindNormal(p1, p2, p3);
			return CreateFromNormalAndPoint(normal, p1);
		}

		/// <summary>
		/// The normal of the plane
		/// </summary>
		public OpenTK.Vector3 PlaneNormal
		{
			get { return new OpenTK.Vector3( this.A, this.B, this.C ); }
		}

		/// <summary>
		/// A point on the plane
		/// </summary>
		public OpenTK.Vector3 PlanePoint
		{
			get
			{
				var vals = new[] {this.A, this.B, this.C};

				// Find the major axis of the normal
				var largestIndex = 0;
				if( Math.Abs( vals[1] ) > Math.Abs( vals[0] ) ) {
					largestIndex = 1;
				}
				if( Math.Abs( vals[2] ) > Math.Abs( vals[largestIndex] ) ) {
					largestIndex = 2;
				}

				// Calculate the point on the plane, remember that:
				//  Ax + By + Cz + D = 0
				// Leave two of the 3 axes 0, and calculate the 3rd as necessary to solve the equation
				var pt = OpenTK.Vector3.Zero;
				pt[largestIndex] = -this.D / vals[largestIndex];

				return pt;
			}
		}


		/// <summary>
		/// Classifies a point as to which side of the ClipPlane it is
		/// </summary>
		/// <param name="pt"></param>
		/// <returns></returns>
		public ClipSide ClassifyPoint(OpenTK.Vector3 pt)
		{
			const float epsilon = 0.0001f;

			float dist = GetDistanceToPlane(pt);
			if( dist >= epsilon ) {
				return ClipSide.Front;
			} else if( dist <= -epsilon ) {
				return ClipSide.Back;
			}
			return ClipSide.CoPlanar;
		}

		/// <summary>
		/// Returns the distance the given point is to the plane. This will be
		/// positive if the point is in front of the plane, negative if it is
		/// behind.
		/// </summary>
		/// <param name="pt"></param>
		/// <returns></returns>
		public float GetDistanceToPlane(OpenTK.Vector3 pt)
		{
			return OpenTK.Vector3.Dot(this.PlaneNormal, pt) + this.D;
		}

		/// <summary>
		/// Helper function that will return a new vertex that is on the plane
		/// intersected with the edge defined by frontVertex and backVertex
		/// </summary>
		/// <param name="frontVertex">The vertex in front of the plane</param>
		/// <param name="backVertex">The vertex in back of the plane</param>
		/// <returns></returns>
		public IVertex IntersectEdge(IVertex frontVertex, IVertex backVertex)
		{
			// Find t, which is a value [0,1], which defines how far along the edge [frontVertex,backVertex]
			// the intersection with the plane is.
			var planeNormal = this.PlaneNormal;
			var planePoint = this.PlanePoint;
			var frontToPlaneVec = planePoint - frontVertex.Position;
			var frontToBackVec = backVertex.Position - frontVertex.Position;
			var numer = OpenTK.Vector3.Dot(planeNormal, frontToPlaneVec);
			var denom = OpenTK.Vector3.Dot(planeNormal, frontToBackVec);
			float t = numer / denom;

			// Ask the vertex to lerp itself
			return frontVertex.LerpTo(backVertex, t);
		}
	}

	public static class Clipper
	{
		public static int CheckColinear( OpenTK.Vector3 a, OpenTK.Vector3 b, OpenTK.Vector3 c )
		{
			const double EpsilonDist = 0.00001;
			const double SqEpsilon = EpsilonDist * EpsilonDist;
			const double DotEpsilon = 1.0;

            double e0x = b.X - a.X;
            double e0y = b.Y - a.Y;
            double e0z = b.Z - a.Z;
            double e1x = c.X - a.X;
            double e1y = c.Y - a.Y;
            double e1z = c.Z - a.Z;

            double e0SqLen = e0x * e0x + e0y * e0y + e0z * e0z;
            double e1SqLen = e1x * e1x + e1y * e1y + e1z * e1z;

			if( e0SqLen <= SqEpsilon ) {
				// a and b are on top of each other, remove b
				return 1;
			}

			if( e1SqLen <= SqEpsilon ) {
				// a and c are on top of each other, remove b (it will resolve on a future loop)
				return 1;
			}

            // normalize and check the dot of the two vectors
            double dot = ( e0x * e1x + e0y * e1y + e0z * e1z ) / ( Math.Sqrt( e0SqLen ) * Math.Sqrt( e1SqLen ) );
			if( dot >= DotEpsilon ) {
				// a -> b -> c  are colinear
				// remove b
				return 1;
			}

			if( dot <= -DotEpsilon ) {
				// b -> a -> c are colinear
				// remove a
				return 0;
			}

			// Looks good
			return -1;
		}

		public static IEnumerable<IVertex> CleanDegenerates( IEnumerable<IVertex> verts )
		{
			List<IVertex> workingList = verts.ToList();

			// Repeatedly filter the workingList while it has 3 or more points
			// by removing successive points that are on top of each other, or,
			// the middle vertex of 3 colinear verts.
			bool keepChecking = true;
			while( workingList.Count >= 3 && keepChecking ) {
				int numVerts = workingList.Count;

				// Loop around the edge loop
				keepChecking = false;
				int count = 0;
				for( int prevVert = numVerts - 1, currVert = 0, nextVert = 1; count < numVerts; prevVert = currVert, currVert = nextVert, nextVert = ( nextVert + 1 ) % numVerts, ++count ) {

					int checkColinearRes = CheckColinear( workingList[prevVert].Position, workingList[currVert].Position, workingList[nextVert].Position );
					if( checkColinearRes == -1 ) {
						continue;
					}

					// The three verts are colinear, we need to remove one
					keepChecking = true;
					switch( checkColinearRes ) {
					case 0:
						workingList.RemoveAt( prevVert );
						break;
					case 1:
						workingList.RemoveAt( currVert );
						break;
					case 2:
						workingList.RemoveAt( nextVert );
						break;
					}
					break;
				}
			}

			if( workingList.Count < 3 ) {
				// Not enough for a triangle
				yield break;
			}

			// Return the valid polygon's verts
			foreach( var v in workingList ) {
				yield return v;
			}
		}


		/// <summary>
		/// Generic implementation of Sutherland–Hodgman clipping algorithm
		/// </summary>
		/// <param name="vertList"></param>
		/// <returns></returns>
		public static IEnumerable<IVertex> ClipToPlaneRaw(IEnumerable<IVertex> vertList, ClipPlane plane)
		{
			if (plane == null) {
				throw new ArgumentNullException("plane");
			}

			if (vertList == null) {
				yield break;
			}

			ClipSide firstPointSide = ClipSide.CoPlanar;
			ClipSide lastPointSide = ClipSide.CoPlanar;
			IVertex lastVertex = null;
			IVertex firstVertex = null;

			foreach (var v in vertList) {

				if (lastVertex == null) {
					// This is the first vertex in the list, so it is specially handled
					// because we have no prior state, and, we'll need to refer to it with
					// regards to the final vertex
					firstVertex = lastVertex = v;
					firstPointSide = lastPointSide = plane.ClassifyPoint(v.Position);

					// If this point is in front of the plane, emit it
					if (firstPointSide != ClipSide.Back) {
						yield return v;
					}
					continue;
				}

				// Classify the current point to the plane
				ClipSide currPointSide = plane.ClassifyPoint(v.Position);

				// We have 4 possible situations:
				//   1) Last point in front of the plane, current point in front of the plane:
				//       In this situation emit the current point
				//   2) Last point in front of the plane, current point is behind the plane:
				//       In this situation emit the intersection point of the plane and the edge (last, curr)
				//   3) Last point in back of the plane, current point is behind the plane:
				//       In this situation do not emit anything
				//   4) Last point in back of the plane, current point in front of the plane:
				//       In this situation emit the intersection point of the plane and the edge (curr, last), and
				//       then emit the current point
				//
				// One thing to be mindful of is to generate the intersection points in a consistent manner by 
				// calling the intersection function with the verts in the same order.
				if (lastPointSide != ClipSide.Back) {
					if (currPointSide != ClipSide.Back) {
						// Situation 1
						yield return v;
					}
					else {
						// Situation 2
						yield return plane.IntersectEdge(lastVertex, v);
					}
				}
				else {
					if (currPointSide != ClipSide.Back) {
						// Situation 4
						yield return plane.IntersectEdge(v, lastVertex);
						yield return v;
					}
				}

				lastPointSide = currPointSide;
				lastVertex = v;
			}

			// To finish up we need to handle the edge from the last vertex back to the first vertex
			// This is only necessary if the last point and the first point were on different sides of the plane
			bool firstInFront = firstPointSide != ClipSide.Back;
			bool lastInFront = lastPointSide != ClipSide.Back;
			if (firstInFront != lastInFront) {
				if (firstInFront) {
					yield return plane.IntersectEdge(firstVertex, lastVertex);
				}
				else {
					yield return plane.IntersectEdge(lastVertex, firstVertex);
				}
			}
		}

		public static IEnumerable<IVertex> ClipToPlane( IEnumerable<IVertex> vertList, ClipPlane plane )
		{
			return CleanDegenerates( ClipToPlaneRaw( vertList, plane ) );
		}

		public struct SplitPolygon
		{
			public IVertex[] front;
			public IVertex[] back;
			public IVertex[] coplaner;
		}

		// Split a polygon by a plane, this is a bit faster than clipping twice with the flipped
		// plane, and will guarantee that points are exact
		public static SplitPolygon SplitPolygonByPlane( IEnumerable<IVertex> vertList, ClipPlane plane )
		{
			if( plane == null ) {
				throw new ArgumentNullException( "plane" );
			}

			if( vertList == null ) {
				throw new ArgumentNullException( "vertList" );
			}

			List<IVertex> front_list = new List<IVertex>();
			List<IVertex> back_list = new List<IVertex>();

			ClipSide firstPointSide = ClipSide.CoPlanar;
			ClipSide lastPointSide = ClipSide.CoPlanar;
			IVertex lastVertex = null;
			IVertex firstVertex = null;
			bool isCoPlanar = true;

			foreach( var v in vertList ) {

				if( lastVertex == null ) {
					// This is the first vertex in the list, so it is specially handled
					// because we have no prior state, and, we'll need to refer to it with
					// regards to the final vertex
					firstVertex = lastVertex = v;
					firstPointSide = lastPointSide = plane.ClassifyPoint( v.Position );
					isCoPlanar = isCoPlanar && firstPointSide == ClipSide.CoPlanar;

					// Emit the point to both sides if coplanar, otherwise emit
					// it to the particular side it is
					if( firstPointSide == ClipSide.CoPlanar ) {
						front_list.Add( v );
						back_list.Add( v );
					} else if( firstPointSide == ClipSide.Back ) {
						back_list.Add( v );
					} else {
						front_list.Add( v );
					}

					continue;
				}

				// Classify the current point to the plane
				ClipSide currPointSide = plane.ClassifyPoint( v.Position );
				isCoPlanar = isCoPlanar && currPointSide == ClipSide.CoPlanar;

				// One thing to be mindful of is to generate the intersection points in a consistent manner by 
				// calling the intersection function with the verts in the same order.
				if( lastPointSide == ClipSide.CoPlanar ) {
					// The last point was on the split plane
					if( currPointSide == ClipSide.CoPlanar ) {
						// Still on the split plane
						front_list.Add( v );
						back_list.Add( v );
					} else if( currPointSide == ClipSide.Back ) {
						// Going from the split plane to the backside
						back_list.Add( v );
					} else {
						// Going from the split plane to the frontside
						front_list.Add( v );
					}
				} else if( lastPointSide == ClipSide.Front ) {
					// Last point was in the front
					if( currPointSide == ClipSide.CoPlanar ) {
						// Front -> coplanar
						front_list.Add( v );
						back_list.Add( v );
					} else if( currPointSide == ClipSide.Back ) {
						// Front -> Back
						var intersected_pt = plane.IntersectEdge( lastVertex, v );
						front_list.Add( intersected_pt );
						back_list.Add( intersected_pt );
						back_list.Add( v );
					} else {
						// Front -> Front
						front_list.Add( v );
					}
				} else {
					// Last point was in the back
					if( currPointSide == ClipSide.CoPlanar ) {
						// Back -> coplanar
						front_list.Add( v );
						back_list.Add( v );
					} else if( currPointSide == ClipSide.Back ) {
						// Back -> Back
						back_list.Add( v );
					} else {
						// Back -> Front
						var intersected_pt = plane.IntersectEdge( v, lastVertex );
						back_list.Add( intersected_pt );
						front_list.Add( intersected_pt );
						front_list.Add( v );
					}
				}

				lastPointSide = currPointSide;
				lastVertex = v;
			}

			// To finish up we need to handle the edge from the last vertex back to the first vertex
			// This is only necessary if the last point and the first point were on different sides of the plane
			if( firstPointSide != lastPointSide ) {
				// We ended up in a different area then the first point
				if( firstPointSide == ClipSide.CoPlanar || lastPointSide == ClipSide.CoPlanar ) {
					// If we started or ended on a co-planar point, then we don't have to add more
				} else if( firstPointSide == ClipSide.Back ) {
					// We started in the back, we ended in the front
					var intersection_pt = plane.IntersectEdge( lastVertex, firstVertex );
					front_list.Add( intersection_pt );
					back_list.Add( intersection_pt );
				} else if( firstPointSide == ClipSide.Front ) {
					// We started in the front, we ended in the back
					var intersection_pt = plane.IntersectEdge( firstVertex, lastVertex );
					front_list.Add( intersection_pt );
					back_list.Add( intersection_pt );
				}
			}

			var res = new SplitPolygon();
			if( isCoPlanar ) {
				front_list = CleanDegenerates( front_list ).ToList();
				res.coplaner = front_list.Count > 2 ? front_list.ToArray() : null;
				res.front = null;
				res.back = null;
			} else {
				// If there we some verts co-planar, but not all, and the remaining
				// verts are on one side, then the other side will have some verts, but not a full triangle
				front_list = CleanDegenerates( front_list ).ToList();
				back_list = CleanDegenerates( back_list ).ToList();
				res.front = front_list.Count > 2 ? front_list.ToArray() : null;
				res.back = back_list.Count > 2 ? back_list.ToArray() : null;
				res.coplaner = null;
			}
			return res;
		}

		// Return true if any verts in the list would be clipped
		public static bool WouldBeClipped(IEnumerable<IVertex> vertList, ClipPlane plane)
		{
			return vertList.Any( v => plane.ClassifyPoint( v.Position ) == ClipSide.Back );
		}
	}

	public class CTriangle
	{
		public CVertex[] original_verts;
		public CVertex[] clipped_verts;
		public int tex_index;
		public int flags;

		public CTriangle(DTriangle dtri, OpenTK.Vector3 v1, OpenTK.Vector3 v2, OpenTK.Vector3 v3)
		{
			// Always start with 3 verts
			original_verts = new CVertex[3];

			tex_index = dtri.tex_index;
			flags = dtri.flags;

			for (int i = 0; i < 3; i++) {
				original_verts[i] = new CVertex();
				original_verts[i].Normal = dtri.normal[i];
				original_verts[i].UV = dtri.tex_uv[i];
			}

			original_verts[0].Position = v1;
			original_verts[1].Position = v2;
			original_verts[2].Position = v3;
		}

		public CTriangle(OpenTK.Vector3[] v)
		{
			original_verts = new CVertex[4];

			tex_index = 0;
			flags = 0;

			for (int i = 0; i < 4; i++) {
				original_verts[i] = new CVertex();
				original_verts[i].Normal = new OpenTK.Vector3(0f, 1f, 0f);
				original_verts[i].UV = new OpenTK.Vector2(0f, 0f);
				original_verts[i].Position = v[i];
			}
		}

		public CTriangle(CVertex[] verts, int tex_index, int flags)
		{
			this.original_verts = verts;
			this.tex_index = tex_index;
			this.flags = flags;
		}

		public CTriangle ExtractClippedTriangle(int tri_idx)
		{
			CVertex[] extracted_verts = new CVertex[3];
			extracted_verts[0] = new CVertex { Position = clipped_verts[0].Position, Normal = clipped_verts[0].Normal, UV = clipped_verts[0].UV };
			extracted_verts[1] = new CVertex { Position = clipped_verts[tri_idx + 1].Position, Normal = clipped_verts[tri_idx + 1].Normal, UV = clipped_verts[tri_idx + 1].UV };
			extracted_verts[2] = new CVertex { Position = clipped_verts[tri_idx + 2].Position, Normal = clipped_verts[tri_idx + 2].Normal, UV = clipped_verts[tri_idx + 2].UV };

			return new CTriangle(extracted_verts, this.tex_index, this.flags);
		}

		/// <summary>
		/// Returns true if any triangles are degenerate
		/// </summary>
		/// <returns></returns>
		public bool HasDegenerateTriangles(bool use_clipped_verts)
		{
			const float kLengthEpsilon = 0.000001f;
            const double kDotEpsilon = 0.99999999999;

			CVertex[] vert_data = use_clipped_verts ? clipped_verts : original_verts;
			int num_triangles = vert_data.Length - 2;

			OpenTK.Vector3[] verts = new OpenTK.Vector3[3];
			for (int tri_idx = 0; tri_idx < num_triangles; ++tri_idx) {

				verts[0] = vert_data[0].Position;
				verts[1] = vert_data[tri_idx + 1].Position;
				verts[2] = vert_data[tri_idx + 2].Position;

				OpenTK.Vector3 edge0 = verts[0] - verts[1];
				OpenTK.Vector3 edge1 = verts[2] - verts[1];
				OpenTK.Vector3 edge2 = verts[0] - verts[2];

				if (edge0.Length <= kLengthEpsilon) {
					return true;
				}
				if (edge1.Length <= kLengthEpsilon) {
					return true;
				}
				if (edge2.Length <= kLengthEpsilon) {
					return true;
				}

                double e0x = edge0.X;
                double e0y = edge0.Y;
                double e0z = edge0.Z;
                double e1x = edge1.X;
                double e1y = edge1.Y;
                double e1z = edge1.Z;
                double e0Mag = Math.Sqrt( e0x * e0x + e0y * e0y + e0z * e0z );
                double e1Mag = Math.Sqrt( e1x * e1x + e1y * e1y + e1z * e1z );
                double edgeDot = Math.Abs( (e0x * e1x + e0y * e1y + e0z * e1z) / (e0Mag * e1Mag) );
                if( edgeDot >= kDotEpsilon ) {
                    return true;
                }
			}

			return false;
		}

		public void ClipFacePlane(ClipPlane cp)
		{
			clipped_verts = Clipper.ClipToPlane(original_verts, cp)
				.Cast<CVertex>()
				.ToArray();
		}

		public bool WouldBeClipped(ClipPlane cp)
		{
			return Clipper.WouldBeClipped(original_verts, cp);
		}

		// Turn this back into DTriangle inside GMesh
	}

	// Define a vertex that implements the IVertex interface
	public class CVertex : IVertex
	{
		public OpenTK.Vector3 Position
		{
			get;
			set;
		}

		public OpenTK.Vector3 Normal
		{
			get;
			set;
		}

		public OpenTK.Vector2 UV
		{
			get;
			set;
		}

		public IVertex LerpTo(IVertex _other, float t)
		{
			CVertex other = (CVertex)_other;

			var newPos = OpenTK.Vector3.Lerp(this.Position, other.Position, t);
			var newNormal = OpenTK.Vector3.Lerp(this.Normal, other.Normal, t).Normalized();
			var newUV = OpenTK.Vector2.Lerp(this.UV, other.UV, t);
			return new CVertex { Position = newPos, Normal = newNormal, UV = newUV };
		}

		public override string ToString()
		{
			return string.Format("Pos: [{0}, {1}, {2}]  Nor: [{3}, {4}, {5}]   UV:[{6}, {7}]",
				this.Position.X, this.Position.Y, this.Position.Z,
				this.Normal.X, this.Normal.Y, this.Normal.Z,
				this.UV.X, this.UV.Y);
		}
	}

	/*public static void TestArray()
	{
		// This is the XY plane, facing +Z, going through (0,0,1)
		var clipPlane = ClipPlane.CreateFromNormalAndPoint(OpenTK.Vector3.UnitZ, new OpenTK.Vector3(0.0f, 0.0f, 1.0f));

		// Get a "source polygon" which is a triangle on the X=1 axis that cuts through the Z=1 clipping plane
		CVertex[] sourcePoints = new CVertex[3];
		sourcePoints[0] = new CVertex();
		sourcePoints[0].Normal = OpenTK.Vector3.UnitX;
		sourcePoints[0].Position = new OpenTK.Vector3(1.0f, 1.0f, 3.0f);
		sourcePoints[0].UV = new OpenTK.Vector2(0.0f, 0.0f);
		sourcePoints[1] = new CVertex();
		sourcePoints[1].Normal = OpenTK.Vector3.UnitX;
		sourcePoints[1].Position = new OpenTK.Vector3(1.0f, 0.3f, 0.5f);
		sourcePoints[1].UV = new OpenTK.Vector2(1.0f, 0.5f);
		sourcePoints[2] = new CVertex();
		sourcePoints[2].Normal = OpenTK.Vector3.UnitX;
		sourcePoints[2].Position = new OpenTK.Vector3(1.0f, -0.5f, 2.0f);
		sourcePoints[2].UV = new OpenTK.Vector2(0.3f, 1.0f);

		// Call to the clipper to clip our points, cast each result back to a TestVertex, and bring everything back
		// into an array -- representing the clipped vertices against the plane
		CVertex[] clippedPoints = Clipper.ClipToPlane(sourcePoints, clipPlane)
			.Cast<CVertex>()
			.ToArray();

		// Note: The above little piece of 'magic source code' is a LINQ (using System.Linq) shortcut for the following
		// piece of code:
		//    List<TestVertex> clippedPointList = new List<TestVertex>();
		//    foreach (var clippedIVertex in Clipper.ClipToPlane(sourcePoints, clipPlane)) {
		//        clippedPointList.Add((TestVertex)clippedIVertex);
		//    }
		//    TestVertex[] clippedPoints = clippedPointList.ToArray();

		// Display the results - no points should have a Z value < 1.0f
		foreach (var v in clippedPoints) {
			Console.WriteLine(v.ToString());
		}
	}*/
}
