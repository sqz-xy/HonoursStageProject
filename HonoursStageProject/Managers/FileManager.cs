using HonoursStageProject.Objects;

namespace HonoursStageProject.Managers;

public abstract class FileManager
{
    public abstract bool ReadHeightData(string pFileName, int pTextureIndex, out Chunk[,] pChunkGrid);
    
    public abstract void SaveHeightData(string pFileName, int pMapSize, float pF, int pSeed, Chunk pSourceChunk);
}