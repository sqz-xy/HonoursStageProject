using OpenTK;

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
        Vertices = new float[(pWidth * pHeight) * 3];
        Indices = new uint[((pWidth * pHeight) * 3) / 2]; // There will always be 2 intersecting points per triangle, so half as many indices
            
        // Set the origin of the mesh to the top left corner of the mesh bounds
        Vector2 origin = new Vector2(-Width, -Height);
        const int stride = 3;
        int triangleIndex = 0;
        int vertexPointer = 0;
        
        for (int widthIndex = (int)origin.X; widthIndex < +origin.X; widthIndex++)
        for (int heightIndex = (int) origin.Y; heightIndex < +origin.Y; heightIndex++)
        {
            if (widthIndex % stride == 0 && heightIndex % stride == 0)
            {
                // Not sure on setting the x and z of the vertex, struggling to visualize it
                // Struggling to visualise indices too
                
                
                Vertices[vertexPointer * stride] = pWidth - triangleIndex; // X
                Vertices[(vertexPointer * stride) + 1] = 0;                // Y - Height will be added later
                Vertices[(vertexPointer * stride) + 2] = pHeight - triangleIndex; // Z
                
                // To add normals increase stride
                //Vertices[vertexPointer * stride] = 0;
                //Vertices[(vertexPointer * stride) + 3] = 1;               
                //Vertices[(vertexPointer * stride) + 4] = 0;
                
                triangleIndex++;
                vertexPointer++;
            }
        }
        
        
    }
}