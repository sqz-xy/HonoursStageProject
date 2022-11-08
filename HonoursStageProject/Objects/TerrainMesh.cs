using OpenTK;
using OpenTK.Graphics.ES11;

namespace HonoursStageProject.Objects;

public class TerrainMesh : Shape
{
    public int Width { get; }
    public int Height { get; }
    public int Resolution { get; }

    public TerrainMesh(int pWidth, int pHeight, int pResolution)
    {
        GenerateMesh(pWidth, pHeight, pResolution);
        PrimitiveType = PrimitiveType.TriangleStrip;
    }

    private void GenerateMesh(int pWidth, int pHeight, int pResolution)
    {
        // Make these arrays again
        List<float> vertices = new List<float>();
        List<uint> indices = new List<uint>();
        
        Random rand = new Random();
        
        for (int heightIndex = 0; heightIndex < pHeight; heightIndex++)
        for (int widthIndex = 0; widthIndex < pWidth; widthIndex++)
        {
            // Starts top left corner, ends bottom right , -width/2 to +width/2. -Dim/2 centres the mesh on 0,0, add i to move along the dimension
            // Divide by resolution, 1 is the base res, i.e. larger res with larger dimensions == more detail
            vertices.Add(((-pHeight / 2.0f) + heightIndex) / pResolution);       // X, the range of the X dimension
            vertices.Add((float) rand.NextDouble() / pResolution);               // Y - Height will be added later
            //vertices.Add(0.2f);   
            vertices.Add(((-pWidth / 2.0f) + widthIndex) / pResolution); // Z, the range of the Z dimension
        }
        
        for (int heightIndex = 0; heightIndex < pHeight - 1; heightIndex++) // for each row
            for (int widthIndex = 0; widthIndex < pWidth; widthIndex++) // for each column
            for (int sideIndex = 0; sideIndex < 2; sideIndex++) // for each side of the trianglestrip
            {
                // Triangle strip vertices are ordered from top row to bottom row
                // Alternate between row i and i + 1, j is the columns
                // j + pWidth is current column + width, i + k is alternating between the rows
                // int test = i + k;
                
                //                        Across Columns * Across Row
                indices.Add((uint) (widthIndex + pWidth * (heightIndex + sideIndex)));
            }

        Vertices = vertices.ToArray();
        Indices = indices.ToArray();
    }
}