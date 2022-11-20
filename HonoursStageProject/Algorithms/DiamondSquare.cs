using OpenTK.Platform.Windows;

namespace HonoursStageProject.Algorithms;

public class DiamondSquare : Algorithm
{
    public DiamondSquare(int pSize) : base(pSize)
    {
    }
    
    /*def diamond_square(width, height):
      # Set up the array of z-values
      let A = a width*height 2D array of 0s
      pre-seed four corners of A with a value

      let step_size = width - 1
      let r = a random number within a range

      # Main loop
      while step_size > 1:
        loop over A
          do diamond_step for each square in A 

        loop over A
          do square_step for each diamond in A

        step_size /= 2
        reduce random range for r

    def diamond_step(x, y, step_size, r):
      # Note: this assumes x, y is top-left of square
      # but you can implement it how you like
      let avg = average of square corners step_size apart
      A[x + step_size/2][y + step_size/2] = avg + r

    def square_step(x, y, step_size, r):
      # Note: x, y here are the middle of a diamond
      let avg = average of four corners of diamond 
      A[x][y] = avg + r */


    public override float[,] GenerateData(int pSeed, float pRoughness, float pFalloff)
    {
      Random rnd = new Random(pSeed);
      int size = _size;
      float[,] heightData = new float[size + 1, size + 1];

      heightData[0, 0] = (float) rnd.NextDouble();
      heightData[0, size] = (float) rnd.NextDouble();
      heightData[size, 0] = (float) rnd.NextDouble();
      heightData[size, size] = (float) rnd.NextDouble();

      
      return heightData;
    }

}