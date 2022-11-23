using OpenTK.Platform.Windows;

namespace HonoursStageProject.Algorithms;

public class DiamondSquare : Algorithm
{
    public DiamondSquare(int pSize) : base(pSize)
    {
    }
    
    /* https://craftofcoding.wordpress.com/tag/diamond-square-algorithm/ */
    /* https://yonatankra.com/how-to-create-terrain-and-heightmaps-using-the-diamond-square-algorithm-in-javascript/ */


    public override float[,] GenerateData(int pSeed, float pRoughness, float pFalloff)
    {
        // Initialize random
        var rnd = new Random();
        float randomRange = 2;
        
        // Initialize grid
        var size = Size;

        size = 4; // Going above 5 causes crash
        
        var heightData = new float[size + 1, size + 1];

        // Seed initial values
        heightData[0, 0] = NextFloat(rnd, randomRange);
        heightData[0, size] = NextFloat(rnd, randomRange);
        heightData[size, 0] = NextFloat(rnd, randomRange);
        heightData[size, size] = NextFloat(rnd, randomRange);

        var step = size / 2;

        for (var i = 0; i < size / 2; i++)
        {
            SquareStep(step, size, ref heightData, rnd, randomRange);

            DiamondStep(step, size, ref heightData, rnd, randomRange);
            
            step /= 2;
            randomRange *= pFalloff;
        }
        
#if  DEBUG
        PrintData(heightData);
#endif
        return heightData;
    }

    private void SquareStep(int pStep, int pSize, ref float[,] pHeightData, Random pRnd, float pRandomRange)
    {
        for (var x = pStep; x < pSize; x += 2 * pStep)
        for (var y = pStep; y < pSize; y += 2 * pStep)
        {
            var topLeft = pHeightData[x - pStep, y - pStep];
            var topRight = pHeightData[x - pStep, y + pStep];
            var bottomleft = pHeightData[x + pStep, y - pStep];
            var bottomRight = pHeightData[x + pStep, y + pStep];

            pHeightData[x, y] = (topLeft + topRight + bottomleft + bottomRight) / 4 + NextFloat(pRnd, pRandomRange);
        }
    }

    private void DiamondStep(int pStep, int pSize, ref float[,] pHeightData, Random pRnd, float pRandomRange)
    {
        // Loop through the diagonals
        for (int x = 0; x < pSize + 1; x += pStep)
        for (int y = ((x / pStep) % 2 == 0 ? pStep : 0); y < pSize + 1; y += pStep * 2)
        {
            float top = 0, bottom = 0, left = 0, right = 0;

            // Handle out of bounds, if its out of bounds, leave it as 0
            if (x - pStep >= 0)
                left = pHeightData[x - pStep, y];

            if (x + pStep < pSize)
                right = pHeightData[x + pStep, y];

            if (y - pStep >= 0)
                bottom = pHeightData[x, y - pStep];

            if (y + pStep < pSize)
                top = pHeightData[x, y + pStep];

            pHeightData[x, y] = (top + bottom + left + right) / 4 + NextFloat(pRnd, pRandomRange);
        }
    }
    
    private void PrintData(float[,] pHeightData)
    {
        for (var i = 0; i < pHeightData.GetLength(0); i++)
        {
            Console.Write("\n\n");
            for (var j = 0; j < pHeightData.GetLength(1); j++)
            {
                Console.Write($"{pHeightData[i, j]}  ");
            }
        }
    }
    



}