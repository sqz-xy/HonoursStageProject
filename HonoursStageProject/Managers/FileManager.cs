namespace HonoursStageProject.Managers;

public abstract class FileManager
{
    public abstract float[,] ReadHeightData(string pFileName);
    
    public abstract void SaveHeightData(string pFileName, int pMapSize, int pMapScale, int pSeed);
}