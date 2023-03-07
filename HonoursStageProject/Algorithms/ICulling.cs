using System.Numerics;
using HonoursStageProject.Objects;

namespace HonoursStageProject.Algorithms;

public interface ICulling
{
    public bool Cull(Chunk pChunk, Camera pCamera);
}