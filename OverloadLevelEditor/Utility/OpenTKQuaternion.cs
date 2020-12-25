/*
Copyright (c) 2006 - 2008 The Open Toolkit library.

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
of the Software, and to permit persons to whom the Software is furnished to do
so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 */
using System;
using OpenTK;

namespace OpenTKExtensions
{
    // Fixed version of ExtractRotation from OpenTK 1.1 stable-5 2014-07-23
    // The trace limit is increased to 1e-4 and a few matrix indices are reversed, mirroring this change in Blender:
    // https://github.com/dfelinto/blender/commit/744f691af42ffeadd306180048c51edad65a5f06 (and 1e-6 to 1e-4 followup)
    class OpenTKQuaternion
    {
       /// <summary>
        /// Returns the rotation component of this instance. Quite slow.
        /// </summary>
        /// <param name="row_normalise">Whether the method should row-normalise (i.e. remove scale from) the Matrix. Pass false if you know it's already normalised.</param>
        public static Quaternion ExtractRotation(Matrix4 m, bool row_normalise = true)
        {
            var row0 = m.Row0.Xyz;
            var row1 = m.Row1.Xyz;
            var row2 = m.Row2.Xyz;

            if (row_normalise)
            {
                row0 = row0.Normalized();
                row1 = row1.Normalized();
                row2 = row2.Normalized();
            }

            // code below adapted from Blender

            Quaternion q = new Quaternion();
            double trace = 0.25 * (row0[0] + row1[1] + row2[2] + 1.0);

            if (trace > 1e-4)
            {
                double sq = Math.Sqrt(trace);

                q.W = (float)sq;
                sq = 1.0 / (4.0 * sq);
                q.X = (float)((row1[2] - row2[1]) * sq);
                q.Y = (float)((row2[0] - row0[2]) * sq);
                q.Z = (float)((row0[1] - row1[0]) * sq);
            }
            else if (row0[0] > row1[1] && row0[0] > row2[2])
            {
                double sq = 2.0 * Math.Sqrt(1.0 + row0[0] - row1[1] - row2[2]);

                q.X = (float)(0.25 * sq);
                sq = 1.0 / sq;
                q.W = (float)((row1[2] - row2[1]) * sq);
                q.Y = (float)((row1[0] + row0[1]) * sq);
                q.Z = (float)((row2[0] + row0[2]) * sq);
            }
            else if (row1[1] > row2[2])
            {
                double sq = 2.0 * Math.Sqrt(1.0 + row1[1] - row0[0] - row2[2]);

                q.Y = (float)(0.25 * sq);
                sq = 1.0 / sq;
                q.W = (float)((row2[0] - row0[2]) * sq);
                q.X = (float)((row1[0] + row0[1]) * sq);
                q.Z = (float)((row2[1] + row1[2]) * sq);
            }
            else
            {
                double sq = 2.0 * Math.Sqrt(1.0 + row2[2] - row0[0] - row1[1]);

                q.Z = (float)(0.25 * sq);
                sq = 1.0 / sq;
                q.W = (float)((row0[1] - row1[0]) * sq);
                q.X = (float)((row2[0] + row0[2]) * sq);
                q.Y = (float)((row2[1] + row1[2]) * sq);
            }

            q.Normalize();
            return q;
        }
    }
}
