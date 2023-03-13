using HonoursStageProject.Algorithms;

namespace HonoursStageProject.Objects;

public struct Settings
{
    public int MapSize;
    public int ChunkSize;
    public float MapScale;
    public int Seed;
    public String FileName;
    public float RenderDistance;
    public List<IAlgorithm> TerrainAlgorithms;
    public List<ICulling> CullingAlgorithms;
}