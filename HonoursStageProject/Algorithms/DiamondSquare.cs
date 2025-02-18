﻿using System.Diagnostics;
using System.Net.Http.Headers;
using HonoursStageProject.Managers;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform.Windows;

namespace HonoursStageProject.Algorithms;

//TODO: Scaling Multiplier, scroll wheel
public class DiamondSquare : Algorithm
{
    
    //TODO: ROUGHNESS AND FALLOFF ADD TO SETTINGS https://gorbitsprojects.wordpress.com/2016/12/28/diamond-square-algorithm/
    public DiamondSquare(int pSize) : base(pSize)
    {
        Size = pSize;   
    }

    public DiamondSquare()
    {
        
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
    /// <param name="pRoughness">The falloff of the roughness</param>
    /// <returns>A 2D array of height values</returns>
    public override float[,] GenerateData(int pSeed, float pScale, float pRoughness)
    {
        Data = null;

        if (IsValidSize(Size, Size))
        {
            FileManager.LogMessage("Generate Data - Invalid Grid Dimensions for diamond square, Size must be a power of 2 + 1");
            return new float[Size, Size];
        }
        
        // Initialize random
        var rnd = new Random(pSeed);
        var randomRange = 1.0f;

        // Initialize Grid, Size has to be odd
        
        Data = new float[Size, Size];

        // Seed initial values
        
        Data[0, 0] = (NextFloat(rnd, -randomRange, randomRange));
        Data[0, Size - 1] = (NextFloat(rnd, -randomRange, randomRange));
        Data[Size - 1, 0] = (NextFloat(rnd, -randomRange, randomRange));
        Data[Size - 1, Size - 1] = (NextFloat(rnd, -randomRange, randomRange));

        // Run Algorithm
        DiamondSquareAlgorithm(rnd, randomRange, pRoughness, pScale);
        
        return Data;
    }


    /// <summary>
    /// Generates the data for the algorithm
    /// </summary>
    /// <param name="pSeed">The random seed</param>
    /// <param name="pScale">The terrain roughness</param>
    /// <param name="pRoughness">The falloff of the roughness</param>
    /// <param name="pPreSeed">Seeded Data</param>
    /// <returns>A 2D array of height values</returns>
    public override float[,] GenerateData(int pSeed, float pScale, float pRoughness, float[,] pPreSeed, bool pSeedCorners)
    {
        if (IsValidSize(pPreSeed.GetLength(0), pPreSeed.GetLength(1)))
        {
            FileManager.LogMessage("Generate Data Pre Seed - Invalid Grid Dimensions for diamond square, Size must be a power of 2 + 1");
            return new float[pPreSeed.GetLength(0), pPreSeed.GetLength(1)];
        }
        
        Data = pPreSeed;
        
        // Initialize random
        var rnd = new Random(pSeed);
        var randomRange = 1.0f;

        // Seed the corners of the first chunk
        // Some corners for other chunks will have 0s on the corner because they dont get pre seeded, talk to darren about this
        // Chunk sizes can only be 1, 2, 3, 8, 16, 21, 64, 128 
        
        // Seed corners based on avg of adjacents, this is a limitation of ds algorithm, corners are supposed to be pre-seeded so they don't get hit by algorithm
        // This doesn't affect the source chunk because this gets seeded straight away
        if (pSeedCorners)
        {
            if (Data[0, 0] == 0) {Data[0, 0] = (Data[1, 0] + Data[0, 1] + Data[1, 1]) / 3;}
            if (Data[0, Size - 1] == 0) {Data[0, Size - 1] = (Data[0, Size - 2] + Data[1, Size - 1] + Data[1, Size - 2]) / 3;}
            if (Data[Size - 1, 0] == 0) {Data[Size - 1, 0] = (Data[Size - 2, 0] + Data[Size - 1, 1] + Data[Size - 2, 1]) / 3;}
            if (Data[Size - 1, Size - 1] == 0) {Data[Size - 1, Size - 1] = (Data[Size - 2, Size - 1] + Data[Size - 1, Size - 2] + Data[Size - 2, Size - 2]) / 3;}
        }
        //PrintData(pPreSeed);
        
        // Run Algorithm

        try
        {
            DiamondSquareAlgorithm(rnd, randomRange, pRoughness, pScale);
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message} \n Diamond square size incorrect, must be 2n + 1! \n Retrying with default chunk size of 17");
            Size = 17;
            DiamondSquareAlgorithm(rnd, randomRange, pRoughness, pScale);
        }
        
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
    /// <param name="pRoughness">Random falloff</param>
    private void DiamondSquareAlgorithm(Random pRnd, float pRandomRange, float pRoughness, float pScale)
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
                topLeft = Data[x, y];
                topRight = Data[x + step, y];
                bottomLeft = Data[x, y + step];
                bottomRight = Data[x + step, y + step];

                // Average of four points plus a random displacement
                var avg = (topLeft + topRight + bottomLeft + bottomRight) / 4;
                Data[x + halfStep, y + halfStep] = avg + (float)(pRnd.NextDouble() * 2 - 1) * pRoughness * pScale;
            }
            
            float top = 0, bottom = 0, left = 0, right = 0;

            // Square Step, Loop though a diamond shape using the step and halfstep to get the horizontals and verticals, then seed the edges
            for (var x = 0; x < Size - 1; x += halfStep)
            for (var y = (x + halfStep) % step; y < Size - 1; y += step)
            {
                top = Data[(x - halfStep + Size - 1) % (Size - 1), y];
                bottom = Data[(x + halfStep) % (Size - 1), y];
                left = Data[x, (y + halfStep) % (Size - 1)];
                right = Data[x, (y - halfStep + Size - 1) % (Size - 1)];

                // Average of four points plus a random displacement
                var avg = (top + bottom + left + right) / 4;
                Data[x, y] = avg + (float)(pRnd.NextDouble() * 2 - 1) * pRoughness * pScale;

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

    bool IsValidSize(int pWidth, int pHeight)
    {
        if (pWidth == 0 || pHeight == 0)
            return false;
        
        if (pWidth != pHeight)
            return false;

        return (pWidth & (pWidth - 1)) == 0;
    }
}