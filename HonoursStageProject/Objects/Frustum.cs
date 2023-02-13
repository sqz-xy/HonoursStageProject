using OpenTK;

namespace HonoursStageProject.Objects;

struct Plane
{
    
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
        
        private float[ , ] _frustum = new float[ 6 , 4 ];

        private void NormaliseFace( float[,] pFrustum , int pSide )
        {
            // Magnitude of the four points
            float magnitude = (float) Math.Sqrt(   
                ( pFrustum[ pSide , 0 ] * pFrustum[ pSide , 0 ] ) 
              + ( pFrustum[ pSide , 1 ] * pFrustum[ pSide , 1 ] )
              + ( pFrustum[ pSide , 2 ] * pFrustum[ pSide , 2 ] ) 
              + ( pFrustum[ pSide , 3 ] * pFrustum[ pSide , 3 ] ));
            
            // Normalise by dividing by the magnitude
            for (var i = 0; i < 4; i++)
                pFrustum[pSide, i] /= magnitude;
        }
        
        public bool IsPointIntersecting( Vector3 pLocation )
        {
            // If the sum of each point * corresponding vector location is less than 0 then the object is outside of the frustum
            for (var i = 0; i < 6; i++ )
                if ((_frustum[i , 0] * pLocation.X) + 
                    (_frustum[i , 1] * pLocation.Y) + 
                    (_frustum[i , 2] * pLocation.Z) + 
                    (_frustum[i , 3]) <= 0 )
                    return false;
            return true;
        }
        
        public void GenerateViewFrustum(Matrix4 pProj , Matrix4 pView)
        {
            // Maths from here http://www.lighthouse3d.com/tutorials/view-frustum-culling/clip-space-approach-extracting-the-planes/
            Matrix4 clipMatrix = new Matrix4();
            
            // Create the clipping matrix
            clipMatrix.M11 = (pView.M11 * pProj.M11) + (pView.M12 * pProj.M21) + (pView.M13 * pProj.M31) + (pView.M14 * pProj.M41); // 0
            clipMatrix.M12 = (pView.M11 * pProj.M12) + (pView.M12 * pProj.M22) + (pView.M13 * pProj.M32) + (pView.M14 * pProj.M42); // 1
            clipMatrix.M13 = (pView.M11 * pProj.M13) + (pView.M12 * pProj.M23) + (pView.M13 * pProj.M33) + (pView.M14 * pProj.M43); // 2
            clipMatrix.M14 = (pView.M11 * pProj.M14) + (pView.M12 * pProj.M24) + (pView.M13 * pProj.M34) + (pView.M14 * pProj.M44); // 3

            clipMatrix.M21 = (pView.M21 * pProj.M11) + (pView.M22 * pProj.M21) + (pView.M23 * pProj.M31) + (pView.M24 * pProj.M41); // 4
            clipMatrix.M22 = (pView.M21 * pProj.M12) + (pView.M22 * pProj.M22) + (pView.M23 * pProj.M32) + (pView.M24 * pProj.M42); // 5
            clipMatrix.M23 = (pView.M21 * pProj.M13) + (pView.M22 * pProj.M23) + (pView.M23 * pProj.M33) + (pView.M24 * pProj.M43); // 6
            clipMatrix.M24 = (pView.M21 * pProj.M14) + (pView.M22 * pProj.M24) + (pView.M23 * pProj.M34) + (pView.M24 * pProj.M44); // 7

            clipMatrix.M31 = (pView.M31 * pProj.M11) + (pView.M32 * pProj.M21) + (pView.M33 * pProj.M31) + (pView.M34 * pProj.M41); // 8
            clipMatrix.M32 = (pView.M31 * pProj.M12) + (pView.M32 * pProj.M22) + (pView.M33 * pProj.M32) + (pView.M34 * pProj.M42); // 9
            clipMatrix.M33 = (pView.M31 * pProj.M13) + (pView.M32 * pProj.M23) + (pView.M33 * pProj.M33) + (pView.M34 * pProj.M43); // 10
            clipMatrix.M34 = (pView.M31 * pProj.M14) + (pView.M32 * pProj.M24) + (pView.M33 * pProj.M34) + (pView.M34 * pProj.M44); // 11

            clipMatrix.M41 = (pView.M41 * pProj.M11) + (pView.M42 * pProj.M21) + (pView.M43 * pProj.M31) + (pView.M44 * pProj.M41); // 12
            clipMatrix.M42 = (pView.M41 * pProj.M12) + (pView.M42 * pProj.M22) + (pView.M43 * pProj.M32) + (pView.M44 * pProj.M42); // 13
            clipMatrix.M43 = (pView.M41 * pProj.M13) + (pView.M42 * pProj.M23) + (pView.M43 * pProj.M33) + (pView.M44 * pProj.M43); // 14
            clipMatrix.M44 = (pView.M41 * pProj.M14) + (pView.M42 * pProj.M24) + (pView.M43 * pProj.M34) + (pView.M44 * pProj.M44); // 15

            
            // Right Plane
            _frustum[0, 0] = clipMatrix.M14 - clipMatrix.M11;
            _frustum[0, 1] = clipMatrix.M24 - clipMatrix.M21;
            _frustum[0, 2] = clipMatrix.M34 - clipMatrix.M31;
            _frustum[0, 3] = clipMatrix.M44 - clipMatrix.M41;
            NormaliseFace(_frustum , 0);

            // Left Plane
            _frustum[1, 0] = clipMatrix.M14 + clipMatrix.M11;
            _frustum[1, 1] = clipMatrix.M24 + clipMatrix.M21;
            _frustum[1, 2] = clipMatrix.M34 + clipMatrix.M31;
            _frustum[1, 3] = clipMatrix.M44  + clipMatrix.M41;
            NormaliseFace(_frustum , 1);

            // Bottom Plane
            _frustum[2, 0] = clipMatrix.M14 + clipMatrix.M12;
            _frustum[2, 1] = clipMatrix.M24 + clipMatrix.M22;
            _frustum[2, 2] = clipMatrix.M34 + clipMatrix.M32;
            _frustum[2, 3] = clipMatrix.M44 + clipMatrix.M42;
            NormaliseFace(_frustum, 2);

            // Top Plane
            _frustum[3, 0] = clipMatrix.M14 - clipMatrix.M12;
            _frustum[3, 1] = clipMatrix.M24 - clipMatrix.M22;
            _frustum[3, 2] = clipMatrix.M34 - clipMatrix.M32;
            _frustum[3, 3] = clipMatrix.M44 - clipMatrix.M42;
            NormaliseFace(_frustum , 3);

            // Far Plane
            _frustum[4, 0] = clipMatrix.M14 - clipMatrix.M13;
            _frustum[4, 1] = clipMatrix.M24 - clipMatrix.M23;
            _frustum[4, 2] = clipMatrix.M34 - clipMatrix.M33;
            _frustum[4, 3] = clipMatrix.M44 - clipMatrix.M43;
            NormaliseFace( _frustum , 4);

            // Near Plane
            _frustum[4, 0] = clipMatrix.M14 + clipMatrix.M13;
            _frustum[4, 1] = clipMatrix.M24 + clipMatrix.M23;
            _frustum[4, 2] = clipMatrix.M34 + clipMatrix.M33;
            _frustum[4, 3] = clipMatrix.M44 + clipMatrix.M43;
            NormaliseFace( _frustum , 5);
        }
    }
    