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

        GenerateTriangleMesh(pWidth, pHeight, pResolution);
    }

    /// <summary>
    /// Generates a terrain mesh
    /// </summary>
    /// <param name="pWidth">Width of the mesh</param>
    /// <param name="pHeight">Height of the mesh</param>
    /// <param name="pResolution">Resolution of the mesh</param>
    private void GenerateTriangleStripMesh(int pWidth, int pHeight, int pResolution)
    {
        PrimitiveType = PrimitiveType.TriangleStrip;

        // Make these arrays again
        var vertices = new List<float>();
        var indices = new List<uint>();
        
        var rand = new Random();

        for (var heightIndex = 0; heightIndex < pHeight; heightIndex++)
        for (var widthIndex = 0; widthIndex < pWidth; widthIndex++)
        {
            // Starts top left corner, ends bottom right , -width/2 to +width/2. -Dim/2 centres the mesh on 0,0, add i to move along the dimension
            // Divide by resolution, 1 is the base res, i.e. larger res with larger dimensions == more detail
            
            
            // Strange lines across terrain, maybe due to temporary noise
            // Vertices
            vertices.Add(((-pHeight / 2.0f) + heightIndex) / pResolution);       // X, the range of the X dimension
            vertices.Add((float) rand.NextDouble()/ pResolution);                // Y - Height will be added later
            vertices.Add(((-pWidth / 2.0f) + widthIndex) / pResolution);         // Z, the range of the Z dimension  
            
            // Normals
            vertices.Add(0); 
            vertices.Add(1); 
            vertices.Add(0);
            
            // Texture Coords
            vertices.Add(((-pHeight / 2.0f) + heightIndex) / pResolution);       // X, the range of the X dimension
            vertices.Add(((-pWidth / 2.0f) + widthIndex) / pResolution);         // Z, the range of the Z dimension  
        }
        
        for (var heightIndex = 0; heightIndex < pHeight - 1; heightIndex++) // for each row
            for (var widthIndex = 0; widthIndex < pWidth; widthIndex++) // for each column
            for (var sideIndex = 0; sideIndex < 2; sideIndex++) // for each side of the trianglestrip
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

    /// <summary>
    /// Generates a terrain mesh
    /// </summary>
    /// <param name="pWidth">Width of the mesh</param>
    /// <param name="pHeight">Height of the mesh</param>
    /// <param name="pResolution">Resolution of the mesh</param>
    private void GenerateTriangleMesh(int pWidth, int pHeight, int pResolution)
    {
        PrimitiveType = PrimitiveType.Triangles;

        //https://stackoverflow.com/questions/48614373/how-to-generate-a-plane-using-gl-triangles

        // Make these arrays again
        var vertices = new List<float>();
        var indices = new List<uint>();

        for (var heightIndex = 0; heightIndex < pHeight; heightIndex++)
            for (var widthIndex = 0; widthIndex < pWidth; widthIndex++)
            {
                vertices.Add((heightIndex - pHeight / 2) * 1); // X
                vertices.Add(0);    // Y
                vertices.Add((widthIndex - pWidth / 2) * 1);// Z
            }

        Vertices = vertices.ToArray();
        Indices = indices.ToArray();
    }
}