using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform.Windows;

namespace HonoursStageProject.Algorithms;

public class DiamondSquare : Algorithm
{
    public DiamondSquare(int pSize) : base(pSize)
    {
        if (pSize % 2 == 0)
            pSize++;

        Size = pSize;   
    }
    
    /* https://craftofcoding.wordpress.com/tag/diamond-square-algorithm/ */
    /* https://yonatankra.com/how-to-create-terrain-and-heightmaps-using-the-diamond-square-algorithm-in-javascript/ */
    /* https://asyncdrink.com/blog/diamond-square-algorithm */
    /* https://medium.com/@nickobrien/diamond-square-algorithm-explanation-and-c-implementation-5efa891e486f */
    /* https://learn.64bitdragon.com/articles/computer-science/procedural-generation/the-diamond-square-algorithm Fixed my final issue using this*/


    /// <summary>
    /// Generates the data for the algorithm
    /// </summary>
    /// <param name="pSeed">The random seed</param>
    /// <param name="pScale">The terrain roughness</param>
    /// <param name="pFalloff">The falloff of the roughness</param>
    /// <returns>A 2D array of height values</returns>
    public override float[,] GenerateData(int pSeed, float pScale, float pFalloff)
    {
        Data = null;
        // Utilize roughness, and add a seed check
        // Fix high size crash
        // Allow meshes to seed adjacent meshes
        
        // Initialize random
        var rnd = new Random(pSeed);
        var randomRange = 1.0f;
        //float randomRange = 1;
        
        // Initialize Grid, Size has to be odd
        
        Data = new float[Size, Size];

        // Seed initial values
        Data[0, 0] = (NextFloat(rnd, -randomRange, randomRange));
        Data[0, Size - 1] = (NextFloat(rnd, -randomRange, randomRange));
        Data[Size - 1, 0] = (NextFloat(rnd, -randomRange, randomRange));
        Data[Size - 1, Size - 1] = (NextFloat(rnd, -randomRange, randomRange));

        // Run Algorithm
        DiamondSquareAlgorithm(rnd, randomRange, pFalloff, pScale);

#if  DEBUG
        //PrintData(Data);
#endif
        return Data;
        //return Normalise(Data);
    }
    
    
    /// <summary>
    /// Generates the data for the algorithm
    /// </summary>
    /// <param name="pSeed">The random seed</param>
    /// <param name="pScale">The terrain roughness</param>
    /// <param name="pFalloff">The falloff of the roughness</param>
    /// <returns>A 2D array of height values</returns>
    public override float[,] GenerateData(int pSeed, float pScale, float pFalloff, float[,] pPreSeed)
    {
        Data = null;

        // Initialize random
        var rnd = new Random(pSeed);
        var randomRange = 1.0f;
        
        Data = pPreSeed;
        
        // Run Algorithm
        DiamondSquareAlgorithm(rnd, randomRange, pFalloff, pScale);

#if  DEBUG
        //PrintData(Data);
#endif
        return Data;
        //return Normalise(Data);
    }

    /// <summary>
    /// The diamond square algorithm
    /// </summary>
    /// <param name="pRnd">The random instance</param>
    /// <param name="pRandomRange">The random range</param>
    /// <param name="pFalloff">Random falloff</param>
    private void DiamondSquareAlgorithm(Random pRnd, float pRandomRange, float pFalloff, float pScale)
    {
        var step = Size - 1;

        while (step > 1)
        {
            var halfStep = step / 2;

            float bottomRight = 0, bottomLeft = 0, topLeft = 0, topRight = 0;

            // Diamond Step, Loop through a square using the step to get to corners, seed the middle value [halfstep, halfstep]
            for (var x = 0; x < Size - 1; x += step)
            for (var y = 0; y < Size - 1; y += step)
            {
                if (Data[x + halfStep, y + halfStep] != 0)
                    continue;

                // Working on crash fix
                if (x + step >= Size || y + step >= Size)
                    continue;
                
                topLeft = Data[x, y];
                topRight = Data[x + step, y];
                bottomLeft = Data[x, y + step];
                bottomRight = Data[x + step, y + step];

                // Average of four points plus a random displacement
                var avg = (topLeft + topRight + bottomLeft + bottomRight) / 4;
                Data[x + halfStep, y + halfStep] = avg + (float)(pRnd.NextDouble() * 2 - 1) * pFalloff * pScale;
                
            }


            float top = 0, bottom = 0, left = 0, right = 0;

            // Square Step, Loop though a diamond shape using the step and halfstep to get the horizontals and verticals, then seed the edges
            for (var x = 0; x < Size - 1; x += halfStep)
            for (var y = (x + halfStep) % step; y < Size - 1; y += step)
            {
                if (Data[x, y] != 0)
                    continue;
                
                top = Data[(x - halfStep + Size - 1) % (Size - 1), y];
                bottom = Data[(x + halfStep) % (Size - 1), y];
                left = Data[x, (y + halfStep) % (Size - 1)];
                right = Data[x, (y - halfStep + Size - 1) % (Size - 1)];

                // Average of four points plus a random displacement
                var avg = (top + bottom + left + right) / 4;
                Data[x, y] = avg + (float)(pRnd.NextDouble() * 2 - 1) * pFalloff * pScale;

                if (x == 0)
                    Data[Size - 1, y] = avg;
                if (y == 0)
                    Data[x, Size - 1] = avg;
            }

            // Reduces the random range and half the step
            pRandomRange /= Math.Max(pRandomRange / 2, 1);
            step /= 2;
        }
    }
}