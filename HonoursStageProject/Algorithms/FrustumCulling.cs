using HonoursStageProject.Objects;

namespace HonoursStageProject.Algorithms;

public class FrustumCulling : ICulling
{
    public bool Cull(Chunk pChunk, Camera pCamera, Settings pSettings)
    {
        /*
        * dist = A*rx + B*ry + C*rz + D
        */
        for (var i = 0; i < 6; i++ )
            if ((pCamera.ViewFrustum.ViewFrustum[i].Points[0] * pChunk.Position.X) + 
                (pCamera.ViewFrustum.ViewFrustum[i].Points[1] * pChunk.Position.Y) + 
                (pCamera.ViewFrustum.ViewFrustum[i].Points[2] * pChunk.Position.Z) + 
                (pCamera.ViewFrustum.ViewFrustum[i].Points[3]) <= 0)
                return false;
        return true;
    }
}