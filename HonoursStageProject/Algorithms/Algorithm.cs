namespace HonoursStageProject.Algorithms;

public abstract class Algorithm : IAlgorithm
{
    protected int _size;
    
    protected Algorithm(int pSize)
    {
        _size = pSize;
    }
    
    public abstract float[,] GenerateData(int pSeed, float pRoughness, float pFalloff);
    
}