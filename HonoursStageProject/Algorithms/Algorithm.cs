namespace HonoursStageProject.Algorithms;

public abstract class Algorithm : IAlgorithm
{
    protected int Size;
    
    protected Algorithm(int pSize)
    {
        Size = pSize;
    }
    
    public abstract float[,] GenerateData(int pSeed, float pRoughness, float pFalloff);
    
    public static float NextFloat(Random pRandom, float pMax)
    {
        return (float)(pMax * 2.0f * (pRandom.NextDouble() - 0.5f));
    }
    
}