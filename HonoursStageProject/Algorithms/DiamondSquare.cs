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
        Random rnd = new Random();
        int randomRange = 10;
        
        // Initialize grid
        int size = _size;
        float[,] heightData = new float[size + 1, size + 1];

        // Seed initial values
        heightData[0, 0] = rnd.Next(randomRange);
        heightData[0, size - 1] = rnd.Next(randomRange);
        heightData[size - 1, 0] = rnd.Next(randomRange);
        heightData[size - 1, size - 1] = rnd.Next(randomRange);

        int stepSize = size - 1;

        // Main Loop
        while (stepSize > 1)
        {
            SquareStep(ref heightData, stepSize, randomRange);
            
            DiamondStep(ref heightData, stepSize, randomRange);
            
            stepSize /= 2; // Half the step size for the next iteration
            randomRange /= 2; // Lower the random range
        }
        return heightData;
    }
    
    private void SquareStep(ref float[,] pHeightData, int pStepSize, int pRandomRange)
    {
        Random rnd = new Random();
        float bottomRight = 0, bottomLeft = 0, topLeft = 0, topRight = 0;
        
        // Step = distance between points
        
        for (int columnIndex = 0; columnIndex < pHeightData.GetLength(0); columnIndex += pStepSize)
        for (int rowIndex = 0; rowIndex < pHeightData.GetLength(1); rowIndex += pStepSize)
        {
            try
            {
                bottomRight = pHeightData[rowIndex + pStepSize, columnIndex + pStepSize];
                bottomLeft = pHeightData[rowIndex + pStepSize, columnIndex];
                topLeft = pHeightData[rowIndex, columnIndex];
                topRight = pHeightData[rowIndex, columnIndex + pStepSize];
                
                float average = (bottomRight + bottomLeft + topLeft + topRight) / 4;
                average += rnd.Next(pRandomRange);
                pHeightData[columnIndex + pStepSize / 2, rowIndex + pStepSize / 2] = average;
            }
            catch (Exception e) { }
        }
    }

    private void DiamondStep(ref float[,] pHeightData, int pStepSize, int pRandomRange)
    {
        Random rnd = new Random();
        float bottom = 0, left = 0, top = 0, right = 0;
        
        // for
        //   for 
        
    }
    



}