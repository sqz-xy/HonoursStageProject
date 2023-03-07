using OpenTK;

namespace HonoursStageProject.Objects;

public struct Plane
{
    public Vector4 Points;

    public Plane()
    {
        // Ax + By + Cz + D
        Points = new Vector4();
    }
}

public class Frustum
    {
        /*
         * Used this http://www.lighthouse3d.com/tutorials/view-frustum-culling/clip-space-approach-extracting-the-planes/
         * This Helped https://stackoverflow.com/questions/25821037/opentk-opengl-frustum-culling-clipping-too-soon
         */
        
        // 6 Faces, 4 Points per face
        
        /*
            +-----------+                        
            | \       / |
            |   +---+   |
            |   |   |   |
            |   +---+   |
            | /       \ |
            +-----------+ 
        */
        
        public Plane[] ViewFrustum = new Plane[6];
        
        public void GenerateViewFrustum(Matrix4 pProj , Matrix4 pView)
        {
            // Maths from here http://www.lighthouse3d.com/tutorials/view-frustum-culling/clip-space-approach-extracting-the-planes/
            var clipMatrix = new Matrix4
            {
                // Create the clipping matrix
                M11 = (pView.M11 * pProj.M11) + (pView.M12 * pProj.M21) + (pView.M13 * pProj.M31) + (pView.M14 * pProj.M41), 
                M12 = (pView.M11 * pProj.M12) + (pView.M12 * pProj.M22) + (pView.M13 * pProj.M32) + (pView.M14 * pProj.M42), 
                M13 = (pView.M11 * pProj.M13) + (pView.M12 * pProj.M23) + (pView.M13 * pProj.M33) + (pView.M14 * pProj.M43), 
                M14 = (pView.M11 * pProj.M14) + (pView.M12 * pProj.M24) + (pView.M13 * pProj.M34) + (pView.M14 * pProj.M44), 
                M21 = (pView.M21 * pProj.M11) + (pView.M22 * pProj.M21) + (pView.M23 * pProj.M31) + (pView.M24 * pProj.M41), 
                M22 = (pView.M21 * pProj.M12) + (pView.M22 * pProj.M22) + (pView.M23 * pProj.M32) + (pView.M24 * pProj.M42), 
                M23 = (pView.M21 * pProj.M13) + (pView.M22 * pProj.M23) + (pView.M23 * pProj.M33) + (pView.M24 * pProj.M43), 
                M24 = (pView.M21 * pProj.M14) + (pView.M22 * pProj.M24) + (pView.M23 * pProj.M34) + (pView.M24 * pProj.M44), 
                M31 = (pView.M31 * pProj.M11) + (pView.M32 * pProj.M21) + (pView.M33 * pProj.M31) + (pView.M34 * pProj.M41), 
                M32 = (pView.M31 * pProj.M12) + (pView.M32 * pProj.M22) + (pView.M33 * pProj.M32) + (pView.M34 * pProj.M42), 
                M33 = (pView.M31 * pProj.M13) + (pView.M32 * pProj.M23) + (pView.M33 * pProj.M33) + (pView.M34 * pProj.M43), 
                M34 = (pView.M31 * pProj.M14) + (pView.M32 * pProj.M24) + (pView.M33 * pProj.M34) + (pView.M34 * pProj.M44), 
                M41 = (pView.M41 * pProj.M11) + (pView.M42 * pProj.M21) + (pView.M43 * pProj.M31) + (pView.M44 * pProj.M41), 
                M42 = (pView.M41 * pProj.M12) + (pView.M42 * pProj.M22) + (pView.M43 * pProj.M32) + (pView.M44 * pProj.M42), 
                M43 = (pView.M41 * pProj.M13) + (pView.M42 * pProj.M23) + (pView.M43 * pProj.M33) + (pView.M44 * pProj.M43), 
                M44 = (pView.M41 * pProj.M14) + (pView.M42 * pProj.M24) + (pView.M43 * pProj.M34) + (pView.M44 * pProj.M44) 
            };

            // Right Plane
            var rightPlane = new Plane
            {
                Points =
                {
                    [0] = clipMatrix.M14 - clipMatrix.M11,
                    [1] = clipMatrix.M24 - clipMatrix.M21,
                    [2] = clipMatrix.M34 - clipMatrix.M31,
                    [3] = clipMatrix.M44 - clipMatrix.M41
                }
            };
            rightPlane = NormalisePlane(rightPlane);
            ViewFrustum[0] = rightPlane;

            // Left Plane
            var leftPlane = new Plane
            {
                Points =
                {
                    [0] = clipMatrix.M14 + clipMatrix.M11,
                    [1] = clipMatrix.M24 + clipMatrix.M21,
                    [2] = clipMatrix.M34 + clipMatrix.M31,
                    [3] = clipMatrix.M44  + clipMatrix.M41
                }
            };
            leftPlane = NormalisePlane(leftPlane);
            ViewFrustum[1] = leftPlane;

            // Bottom Plane
            var bottomPlane = new Plane
            {
                Points =
                {
                    [0] = clipMatrix.M14 + clipMatrix.M12,
                    [1] = clipMatrix.M24 + clipMatrix.M22,
                    [2] = clipMatrix.M34 + clipMatrix.M32,
                    [3] = clipMatrix.M44 + clipMatrix.M42
                }
            };
            bottomPlane = NormalisePlane(bottomPlane);
            ViewFrustum[2] = bottomPlane;

            // Top Plane
            var topPlane = new Plane
            {
                Points =
                {
                    [0] = clipMatrix.M14 - clipMatrix.M12,
                    [1] = clipMatrix.M24 - clipMatrix.M22,
                    [2] = clipMatrix.M34 - clipMatrix.M32,
                    [3] = clipMatrix.M44 - clipMatrix.M42
                }
            };
            topPlane = NormalisePlane(topPlane);
            ViewFrustum[3] = topPlane;

            // Far Plane
            var farPlane = new Plane
            {
                Points =
                {
                    [0] = clipMatrix.M14 - clipMatrix.M13,
                    [1] = clipMatrix.M24 - clipMatrix.M23,
                    [2] = clipMatrix.M34 - clipMatrix.M33,
                    [3] = clipMatrix.M44 - clipMatrix.M43
                }
            };
            farPlane = NormalisePlane(farPlane);
            ViewFrustum[4] = farPlane;

            // Near Plane
            var nearPlane = new Plane
            {
                Points =
                {
                    [0] = clipMatrix.M14 + clipMatrix.M13,
                    [1] = clipMatrix.M24 + clipMatrix.M23,
                    [2] = clipMatrix.M34 + clipMatrix.M33,
                    [3] = clipMatrix.M44 + clipMatrix.M43
                }
            };
            nearPlane = NormalisePlane(nearPlane);
            ViewFrustum[5] = nearPlane;
        }
        
        private Plane NormalisePlane(Plane pPlane)
        {
            // Magnitude of the four points
            var magnitude = (float) Math.Sqrt(   
                (pPlane.Points[0] * pPlane.Points[0]) 
                + (pPlane.Points[1] * pPlane.Points[1])
                + (pPlane.Points[2] * pPlane.Points[2]) 
                + (pPlane.Points[3] * pPlane.Points[3]));
            
            // Normalise by dividing by the magnitude
            for (var i = 0; i < 4; i++)
                pPlane.Points[i] /= magnitude;
            
            return pPlane;
        }
    }
    