using OpenTK.Platform.Windows;

namespace HonoursStageProject.Algorithms;

public class DiamondSquare : Algorithm
{
    public DiamondSquare(int pSize) : base(pSize)
    {
    }
    
    /* https://craftofcoding.wordpress.com/tag/diamond-square-algorithm/ */
    /* https://yonatankra.com/how-to-create-terrain-and-heightmaps-using-the-diamond-square-algorithm-in-javascript/ */
    /* https://asyncdrink.com/blog/diamond-square-algorithm */
    /* https://medium.com/@nickobrien/diamond-square-algorithm-explanation-and-c-implementation-5efa891e486f */


    public override float[,] GenerateData(int pSeed, float pRoughness, float pFalloff)
    {
        // Initialize random
        var rnd = new Random();
        float randomRange = 100;
        
        // Initialize grid

        // Size has to be odd
        var size = Size;
        
        if (size % 2 == 0)
            size++;

        var heightData = new float[size, size];

        // Seed initial values
        heightData[0, 0] = NextFloat(rnd, randomRange);
        heightData[0, size - 1] = NextFloat(rnd, randomRange);
        heightData[size - 1, 0] = NextFloat(rnd, randomRange);
        heightData[size - 1 , size - 1] = NextFloat(rnd, randomRange);

        var step = size - 1;

        while (step > 1)
        {
            SquareStep(step, size, ref heightData, rnd, randomRange);
            
            step /= 2;
            randomRange /= 2;
        }
        
#if  DEBUG
        PrintData(heightData);
#endif
        return heightData;
    }

    private void SquareStep(int pStep, int pSize, ref float[,] pHeightData, Random pRnd, float pRandomRange)
    {
        float bottomRight = 0, bottomLeft = 0, topLeft = 0, topRight = 0;
        int halfStep = pStep / 2;
        
        for (int i = 0; i < pStep; i += pStep)
        for (int j = 0; j < pStep; j += pStep)
        {
            topLeft = pHeightData[0, 0];
            topRight = pHeightData[0, pStep];
            bottomLeft = pHeightData[pStep, 0];
            bottomRight = pHeightData[pStep, pStep];

            float avg = (topLeft + topRight + bottomLeft + bottomRight) / 4;
            pHeightData[halfStep, halfStep] = avg + NextFloat(pRnd, pRandomRange);
        }
    }

    private void DiamondStep(int pStep, int pSize, ref float[,] pHeightData, Random pRnd, float pRandomRange)
    {

    }
    
    private void PrintData(float[,] pHeightData)
    {
        for (var i = 0; i < pHeightData.GetLength(0); i++)
        {
            Console.Write("\n\n");
            for (var j = 0; j < pHeightData.GetLength(1); j++)
            {
                Console.Write($"{(int)pHeightData[i, j]}  ");
            }
        }
    }
    



}