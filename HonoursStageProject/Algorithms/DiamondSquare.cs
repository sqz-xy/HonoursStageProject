using OpenTK;
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

        Size = 16;
        
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
            var halfStep = step / 2;
            
            float bottomRight = 0, bottomLeft = 0, topLeft = 0, topRight = 0;
            
            // Diamond Step, Loop through a square using the step to get to corners, seed the middle value [halfstep, halfstep]
            for (var x = 0; x < size - 1; x += step)
            for (var y = 0; y < size - 1; y += step)
            {
                topLeft = heightData[x, y];
                topRight = heightData[x + step, y];
                bottomLeft = heightData[x, y + step];
                bottomRight = heightData[x + step, y + step];
                
                // Average of four points plus a random displacement
                var avg = (topLeft + topRight + bottomLeft + bottomRight) / 4;
                heightData[x + halfStep, y + halfStep] = avg + NextFloat(rnd, randomRange);
            }
            
            
            float top = 0, bottom = 0, left = 0, right = 0;
            
            // Square Step, Loop though a diamond shape using the step and halfstep to get the horizontals and verticals, then seed the edges
            for (var x = 0; x < size - 1; x += halfStep)
            for (var y = (x + halfStep) % step; y < size - 1; y += step)
            {
                top = heightData[(x - halfStep + size - 1) % (size - 1), y];
                bottom = heightData[(x + halfStep) % (size - 1), y];
                left = heightData[x, (y + halfStep) % (size - 1)];
                right = heightData[x, (y - halfStep + size - 1) % (size - 1)];
                
                // Average of four points plus a random displacement
                var avg = (top + bottom + left + right) / 4;
                heightData[x, y] = avg + NextFloat(rnd, randomRange);

                if (x == 0)
                    heightData[size - 1, y] = avg;
                if (y == 0)
                    heightData[x, size - 1] = avg;
            }
            
            // Reduces the random range and half the step
            randomRange = Math.Max(randomRange / 2, 1);
            step /= 2;
        }
        
#if  DEBUG
        PrintData(heightData);
#endif
        return Normalise(heightData);
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

    private float[,] Normalise(float[,] pHeightData)
    {
        return pHeightData;
    }
    



}