namespace HonoursStageProject.Algorithms;

public abstract class Algorithm : IAlgorithm
{
    protected int Size;
    protected float[,] Data;
    
    protected Algorithm(int pSize)
    {
        Size = pSize;
    }
    
    public abstract float[,] GenerateData(int pSeed, float pScale, float pFalloff, float[,] pPreSeed, bool pSeedCorners);
    
    public abstract float[,] GenerateData(int pSeed, float pScale, float pFalloff);
    
    /// <summary>
    /// Prints stored data to the console
    /// </summary>
    /// <param name="pHeightData">The Data to print</param>
    protected void PrintData(float[,] pHeightData)
    {
        for (var i = 0; i < pHeightData.GetLength(0); i++)
        {
            Console.Write("\n\n");
            for (var j = 0; j < pHeightData.GetLength(1); j++)
            {
                Console.Write($"{pHeightData[i, j]}  ");
            }
        }
        Console.WriteLine(" // NEW CHUNK // ");
    }

    /// <summary>
    /// Normalizes input data between 0 and 1
    /// </summary>
    /// <param name="pHeightData">The data to normalize</param>
    /// <returns>A normalized dataset</returns>
    protected float[,] Normalise(float[,] pHeightData)
    {
        // Change this to normalize between different values
        
        var max = pHeightData.Cast<float>().Max();
        var min = pHeightData.Cast<float>().Min();
        
        for (var i = 0; i < pHeightData.GetLength(0); i++)
        {
            for (var j = 0; j < pHeightData.GetLength(1); j++)
            {
                pHeightData[i, j] = (pHeightData[i, j] - min) / (max - min);
            }
        }
        return pHeightData;
    }
    
    public static float NextFloat(Random pRandom, float pMax)
    {
        return (float)(pMax * 2.0f * (pRandom.NextDouble() - 0.5f));
    }

    public static float NextFloat(Random pRandom, float pMin, float pMax)
    {
        return (float) Math.Floor(pRandom.NextDouble() * (pMax - pMin + 1) + pMin);
    }
    
}