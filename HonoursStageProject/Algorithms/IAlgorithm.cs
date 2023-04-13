namespace HonoursStageProject.Algorithms;

public interface IAlgorithm
{
    public float[,] GenerateData(int pSeed, float pScale, float pRoughness);
    
    public float[,] GenerateData(int pSeed, float pScale, float pRoughness, float[,] pPreSeed, bool pSeedCorners);
}