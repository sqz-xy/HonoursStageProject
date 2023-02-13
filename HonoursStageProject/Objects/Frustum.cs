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
        
        private float[] _clipMatrix = new float[ 16 ];
        
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
            Matrix4 ClipMatrix;
            
            // Create the clipping matrix
            _clipMatrix[ 0 ] = (pView.M11 * pProj.M11) + (pView.M12 * pProj.M21) + (pView.M13 * pProj.M31) + (pView.M14 * pProj.M41);
            _clipMatrix[ 1 ] = (pView.M11 * pProj.M12) + (pView.M12 * pProj.M22) + (pView.M13 * pProj.M32) + (pView.M14 * pProj.M42);
            _clipMatrix[ 2 ] = (pView.M11 * pProj.M13) + (pView.M12 * pProj.M23) + (pView.M13 * pProj.M33) + (pView.M14 * pProj.M43);
            _clipMatrix[ 3 ] = (pView.M11 * pProj.M14) + (pView.M12 * pProj.M24) + (pView.M13 * pProj.M34) + (pView.M14 * pProj.M44);

            _clipMatrix[ 4 ] = (pView.M21 * pProj.M11) + (pView.M22 * pProj.M21) + (pView.M23 * pProj.M31) + (pView.M24 * pProj.M41);
            _clipMatrix[ 5 ] = (pView.M21 * pProj.M12) + (pView.M22 * pProj.M22) + (pView.M23 * pProj.M32) + (pView.M24 * pProj.M42);
            _clipMatrix[ 6 ] = (pView.M21 * pProj.M13) + (pView.M22 * pProj.M23) + (pView.M23 * pProj.M33) + (pView.M24 * pProj.M43);
            _clipMatrix[ 7 ] = (pView.M21 * pProj.M14) + (pView.M22 * pProj.M24) + (pView.M23 * pProj.M34) + (pView.M24 * pProj.M44);

            _clipMatrix[ 8 ] = (pView.M31 * pProj.M11) + (pView.M32 * pProj.M21) + (pView.M33 * pProj.M31) + (pView.M34 * pProj.M41);
            _clipMatrix[ 9 ] = (pView.M31 * pProj.M12) + (pView.M32 * pProj.M22) + (pView.M33 * pProj.M32) + (pView.M34 * pProj.M42);
            _clipMatrix[ 10 ] = (pView.M31 * pProj.M13) + (pView.M32 * pProj.M23) + (pView.M33 * pProj.M33) + (pView.M34 * pProj.M43);
            _clipMatrix[ 11 ] = (pView.M31 * pProj.M14) + (pView.M32 * pProj.M24) + (pView.M33 * pProj.M34) + (pView.M34 * pProj.M44);

            _clipMatrix[ 12 ] = (pView.M41 * pProj.M11) + (pView.M42 * pProj.M21) + (pView.M43 * pProj.M31) + (pView.M44 * pProj.M41);
            _clipMatrix[ 13 ] = (pView.M41 * pProj.M12) + (pView.M42 * pProj.M22) + (pView.M43 * pProj.M32) + (pView.M44 * pProj.M42);
            _clipMatrix[ 14 ] = (pView.M41 * pProj.M13) + (pView.M42 * pProj.M23) + (pView.M43 * pProj.M33) + (pView.M44 * pProj.M43);
            _clipMatrix[ 15 ] = (pView.M41 * pProj.M14) + (pView.M42 * pProj.M24) + (pView.M43 * pProj.M34) + (pView.M44 * pProj.M44);

            
            // Right Plane
            _frustum[0, 0] = _clipMatrix[3] - _clipMatrix[0];
            _frustum[0, 1] = _clipMatrix[7] - _clipMatrix[4] ;
            _frustum[0, 2] = _clipMatrix[11] - _clipMatrix[8];
            _frustum[0, 3] = _clipMatrix[15] - _clipMatrix[12];
            NormaliseFace(_frustum , 0);

            // Left Plane
            _frustum[1, 0] = _clipMatrix[3] + _clipMatrix[0];
            _frustum[1, 1] = _clipMatrix[7] + _clipMatrix[4];
            _frustum[1, 2] = _clipMatrix[11] + _clipMatrix[8];
            _frustum[1, 3] = _clipMatrix[15] + _clipMatrix[12];
            NormaliseFace(_frustum , 1);

            // Bottom Plane
            _frustum[2, 0] = _clipMatrix[3] + _clipMatrix[1];
            _frustum[2, 1] = _clipMatrix[7] + _clipMatrix[5];
            _frustum[2, 2] = _clipMatrix[11] + _clipMatrix[9];
            _frustum[2, 3] = _clipMatrix[15] + _clipMatrix[13];
            NormaliseFace(_frustum, 2);

            // Top Plane
            _frustum[3, 0] = _clipMatrix[3] - _clipMatrix[1];
            _frustum[3, 1] = _clipMatrix[7] - _clipMatrix[5];
            _frustum[3, 2] = _clipMatrix[11] - _clipMatrix[9];
            _frustum[3, 3] = _clipMatrix[15] - _clipMatrix[13];
            NormaliseFace(_frustum , 3);

            // Back Plane
            _frustum[4, 0] = _clipMatrix[3] - _clipMatrix[2];
            _frustum[4, 1] = _clipMatrix[7] - _clipMatrix[6];
            _frustum[4, 2] = _clipMatrix[11] - _clipMatrix[10];
            _frustum[4, 3] = _clipMatrix[15] - _clipMatrix[14];
            NormaliseFace( _frustum , 4);

            // Front Plane
            _frustum[5 , 0 ] = _clipMatrix[ 3 ] + _clipMatrix[ 2 ];
            _frustum[5 , 1 ] = _clipMatrix[ 7 ] + _clipMatrix[ 6 ];
            _frustum[5 , 2 ] = _clipMatrix[ 11 ] + _clipMatrix[ 10 ];
            _frustum[5 , 3 ] = _clipMatrix[ 15 ] + _clipMatrix[ 14 ];
            NormaliseFace( _frustum , 5);
        }
    }
    