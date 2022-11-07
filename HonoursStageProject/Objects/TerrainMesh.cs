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
    }

    private void GenerateMesh(int pWidth, int pHeight, int pResolution)
    {
        const int stride = 3;
        int size = (pWidth * pHeight);
        
        Vertices = new float[size * stride];
        Indices = new uint[size * stride]; // Can't figure out what size to make this

        PrimitiveType = PrimitiveType.TriangleStrip;
        
        int vertexPointer = 0;
        Random rand = new Random();
        
        for (int i = 0; i < pHeight; i++)
        for (int j = 0; j < pWidth; j++)
        {
            // Starts top left corner, ends bottom right , -width/2 to +width/2. -Dim/2 centres the mesh on 0,0, add i to move along the dimension
            Vertices[vertexPointer * stride] = (-pHeight / 2.0f) + i; // X, the range of the X dimension
            Vertices[(vertexPointer * stride) + 1] = 0; //(float) rand.NextDouble();                // Y - Height will be added later
            Vertices[(vertexPointer * stride) + 2] = (-pWidth / 2.0f) + j; // Z, the range of the Z dimension

            vertexPointer++;
        }

        int indicesPointer = 0;
        
        for (int i = 0; i < pHeight - 1; i++) // for each row
            for (int j = 0; j < pWidth; j++) // for each column
            for (int k = 0; k < 2; k++) // for each side of the trianglestrip
            {
                // Triangle strip vertices are ordered from top row to bottom row
                // Alternate between row i and i + 1, j is the columns
                // j + pWidth is current column + width, i + k is alternating between the rows
                // int test = i + k;
                
                //                        Across Columns * Across Row
                Indices[indicesPointer] = (uint) (j + pWidth * (i + k));
                indicesPointer++;
            }
    }
}