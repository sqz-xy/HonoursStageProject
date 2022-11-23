using OpenTK.Platform.Windows;

namespace HonoursStageProject.Algorithms;

public class DiamondSquare : Algorithm
{
    public DiamondSquare(int pSize) : base(pSize)
    {
    }
    
    /* https://craftofcoding.wordpress.com/tag/diamond-square-algorithm/ */


    public override float[,] GenerateData(int pSeed, float pRoughness, float pFalloff)
    {
        // Initialize random
        Random rnd = new Random(100);
        float randomRange = 10;

        Size = 4;
        
        // Initialize grid
        int size = Size;
        float[,] heightData = new float[size + 1, size + 1];

        // Seed initial values
        heightData[0, 0] = NextFloat(rnd, randomRange);
        heightData[0, size] = NextFloat(rnd, randomRange);
        heightData[size, 0] = NextFloat(rnd, randomRange);
        heightData[size, size] = NextFloat(rnd, randomRange);

        int stepSize = size;

        // Main Loop
        while (stepSize > 1)
        {
            SquareStep(ref heightData, stepSize, randomRange, rnd);
            
            DiamondStep(ref heightData, stepSize, randomRange);
            
            stepSize /= 2; // Half the step size for the next iteration
            randomRange *= (float)Math.Pow(2.0f, -pRoughness);
        }
#if  DEBUG
        PrintData(heightData);
#endif

        return heightData;
    }
    
    private void SquareStep(ref float[,] pHeightData, int pStepSize, float pRandomRange, Random pRandom)
    {
        float bottomRight = 0, bottomLeft = 0, topLeft = 0, topRight = 0;
        int halfStep = pStepSize / 2;
        
        for (int i = 0; i < pStepSize; i += pStepSize)
        for (int j = 0; j < pStepSize; j += pStepSize)
        {
            topLeft = pHeightData[0, 0];
            topRight = pHeightData[0, pStepSize];
            bottomLeft = pHeightData[pStepSize, 0];
            bottomRight = pHeightData[pStepSize, pStepSize];

            float avg = (topLeft + topRight + bottomLeft + bottomRight) / 4;
            pHeightData[halfStep, halfStep] = avg + NextFloat(pRandom, pRandomRange);
        }
    }

    private void DiamondStep(ref float[,] pHeightData, int pStepSize, float pRandomRange)
    {
        Random rnd = new Random();
        float bottom = 0, left = 0, top = 0, right = 0;
        
        // for
        //   for 
        
    }
    
    private void PrintData(float[,] heightData)
    {
        for (int i = 0; i < heightData.GetLength(0); i++)
        {
            Console.Write("\n\n");
            for (int j = 0; j < heightData.GetLength(1); j++)
            {
                Console.Write($"{(int)heightData[i, j]}  ");
            }
        }
    }
    



}