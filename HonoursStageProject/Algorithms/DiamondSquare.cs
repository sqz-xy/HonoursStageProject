using OpenTK.Graphics.OpenGL;
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
        float randomRange = 128;
        
        // Initialize grid

        Size = 4;
        
        // Size has to be odd
        var heightMapWidth = Size;
        
        if (heightMapWidth % 2 == 0)
            heightMapWidth++;

        var heightData = new float[heightMapWidth, heightMapWidth];

        // Seed initial values
        heightData[0, 0] = NextFloat(rnd, randomRange);
        heightData[0, heightMapWidth - 1] = NextFloat(rnd, randomRange);
        heightData[heightMapWidth - 1, 0] = NextFloat(rnd, randomRange);
        heightData[heightMapWidth - 1 , heightMapWidth - 1] = NextFloat(rnd, randomRange);

        var tileWidth = heightMapWidth - 1;

        while (tileWidth > 1)
        {
            //SquareStep(step, size, ref heightData, rnd, randomRange);

            var halfSide = tileWidth / 2;
            
            float bottomRight = 0, bottomLeft = 0, topLeft = 0, topRight = 0;
            for (var x = 0; x < heightMapWidth - 1; x += tileWidth)
            for (var y = 0; y < heightMapWidth - 1; y += tileWidth)
            {
                topLeft = heightData[x, y];
                topRight = heightData[x + tileWidth, y];
                bottomLeft = heightData[x, y + tileWidth];
                bottomRight = heightData[x + tileWidth, y + tileWidth];
                
                var avg = (topLeft + topRight + bottomLeft + bottomRight) / 4;
                heightData[x + halfSide, y + halfSide] = avg + NextFloat(rnd, randomRange);
            }
            
            float top = 0, bottom = 0, left = 0, right = 0;
            for (var x = 0; x < heightMapWidth - 1; x += halfSide)
            for (var y = (x + halfSide) % heightMapWidth; y < heightMapWidth - 1; y += tileWidth)
            {
                top = heightData[(x - halfSide + heightMapWidth - 1) % (heightMapWidth - 1), y];
                bottom = heightData[(x + halfSide) % (heightMapWidth - 1), y];
                left = heightData[x, (y + halfSide) % (heightMapWidth - 1)];
                right = heightData[x, (y - halfSide + heightMapWidth - 1) % (heightMapWidth - 1)];
                
                var avg = (top + bottom + left + right) / 4;
                heightData[x, y] = avg + NextFloat(rnd, randomRange);

                if (x == 0)
                    heightData[heightMapWidth - 1, y] = avg;
                if (y == 0)
                    heightData[x, heightMapWidth - 1] = avg;
            }
            
            randomRange = Math.Max(randomRange / 2, 1);
            tileWidth /= 2;
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