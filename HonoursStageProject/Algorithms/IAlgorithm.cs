namespace HonoursStageProject.Algorithms;

public interface IAlgorithm
{
    public float[,] GenerateData(int pSeed, float pScale, float pFalloff);
    
    public float[,] GenerateData(int pSeed, float pScale, float pFalloff, float[,] pPreSeed);
    
}