using System.Drawing.Printing;
using OpenTK;
using OpenTK.Graphics.ES11;

namespace HonoursStageProject.Objects;

public class TerrainMesh : Shape
{
    public TerrainMesh(int pSize, int pResolution)
    {

        GenerateTriangleMesh(pSize, pResolution);
    }

    /// <summary>
    /// Generates a terrain mesh
    /// </summary>
    /// <param name="pSize">Width of the mesh</param>
    /// <param name="pResolution">Resolution of the mesh</param>
    private void GenerateTriangleStripMesh(int pSize, int pResolution)
    {
        PrimitiveType = PrimitiveType.TriangleStrip;

        // Make these arrays again
        var vertices = new List<float>();
        var indices = new List<uint>();
        
        var rand = new Random();

        for (var heightIndex = 0; heightIndex < pSize; heightIndex++)
        for (var widthIndex = 0; widthIndex < pSize; widthIndex++)
        {
            // Starts top left corner, ends bottom right , -width/2 to +width/2. -Dim/2 centres the mesh on 0,0, add i to move along the dimension
            // Divide by resolution, 1 is the base res, i.e. larger res with larger dimensions == more detail
            
            // Strange lines across terrain, maybe due to temporary noise
            // Vertices
            vertices.Add(((-pSize / 2.0f) + heightIndex) / pResolution);       // X, the range of the X dimension
            vertices.Add((float) rand.NextDouble()/ pResolution);                // Y - Height will be added later
            vertices.Add(((-pSize / 2.0f) + widthIndex) / pResolution);         // Z, the range of the Z dimension  
            
            // Normals
            vertices.Add(0); 
            vertices.Add(1); 
            vertices.Add(0);
            
            // Texture Coords
            vertices.Add(((-pSize / 2.0f) + heightIndex) / pResolution);       // X, the range of the X dimension
            vertices.Add(((-pSize / 2.0f) + widthIndex) / pResolution);         // Z, the range of the Z dimension  
        }
        
        for (var heightIndex = 0; heightIndex < pSize - 1; heightIndex++) // for each row
            for (var widthIndex = 0; widthIndex < pSize; widthIndex++) // for each column
            for (var sideIndex = 0; sideIndex < 2; sideIndex++) // for each side of the trianglestrip
            {
                // Triangle strip vertices are ordered from top row to bottom row
                // Alternate between row i and i + 1, j is the columns
                // j + pWidth is current column + width, i + k is alternating between the rows
                // int test = i + k;
                
                //                        Across Columns * Across Row
                indices.Add((uint) (widthIndex + pSize * (heightIndex + sideIndex)));
            }

        Vertices = vertices.ToArray();
        Indices = indices.ToArray();
    }

    /// <summary>
    /// Generates a terrain mesh
    /// </summary>
    /// <param name="pSize">Height of the mesh</param>
    /// <param name="pResolution">Resolution of the mesh</param>
    private void GenerateTriangleMesh(int pSize, int pResolution)
    {
        PrimitiveType = PrimitiveType.Triangles;

        // https://www.youtube.com/watch?v=DJk-aTmrAlQ
        
        // Make these arrays again
        var vertices = new List<float>();
        var indices = new List<uint>();
        
        var rand = new Random();
        
        for (var heightIndex = 0; heightIndex < pSize; heightIndex++)
        for (var widthIndex = 0; widthIndex < pSize; widthIndex++)
        {
           
            vertices.Add(((-pSize / 2.0f) + heightIndex) / pResolution);       // X, the range of the X dimension
            vertices.Add(0);         // Y - Height will be added later
            vertices.Add(((-pSize / 2.0f) + widthIndex) / pResolution);         // Z, the range of the Z dimension  
            
            // Normals
            vertices.Add(0); 
            vertices.Add(1); 
            vertices.Add(0);
            
            // Texture Coords
            vertices.Add(((-pSize / 2.0f) + heightIndex) / pResolution);       // X, the range of the X dimension
            vertices.Add(((-pSize / 2.0f) + widthIndex) / pResolution);         // Z, the range of the Z dimension  
        }

        // Indices bug with extra lines
        
        for (var heightIndex = 0; heightIndex - 1 < pSize; heightIndex++)
        for (var widthIndex = 0; widthIndex - 1 < pSize; widthIndex++)
        {
            indices.Add((uint) ((uint) pSize * heightIndex + widthIndex)); // Top left corner
            indices.Add((uint) ((uint) ((uint) pSize * heightIndex + widthIndex) + pSize)); // Bottom left corner
            indices.Add((uint) ((uint) ((uint) pSize * heightIndex + widthIndex) + pSize + 1)); // Bottom right corner
            
            indices.Add((uint) ((uint) pSize * heightIndex + widthIndex)); // Top left corner
            indices.Add((uint) ((uint) ((uint) pSize * heightIndex + widthIndex) + pSize + 1)); // Bottom right corner
            indices.Add((uint) ((uint) pSize * heightIndex + widthIndex) + 1); // Top right corner
        }

        Indices = indices.ToArray();
        Vertices = vertices.ToArray();
    }
}