using System.Drawing.Drawing2D;
using HonoursStageProject.Managers;
using HonoursStageProject.Objects;
using HonoursStageProject.Shaders;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using PrimitiveType = OpenTK.Graphics.ES11.PrimitiveType;

namespace HonoursStageProject.Objects;

public class Chunk : Object
{
    public int Size { get; }
    public float Scale { get; }
    public Chunk[] Adjacents { get; }
    public Vector2 GridPos { get; }
    public float[,] HeightData { get; set; }
    
    public int ID { get; set; }

    public Chunk(Vector3 pPosition, int pSize, float pScale, Vector2 pGridPos, int pTextureIndex)
    {
        Size = pSize;
        Scale = pScale;
        Adjacents = new Chunk[4];
        GridPos = pGridPos;
        TextureIndex = pTextureIndex;
        Position = pPosition;
        BaseColour = Vector4.One;
        HeightData = new float[pSize, pSize];
        
        GenerateTriangleMesh(pSize, pScale);
    }

    // Buffers the Data to the GPU
    public override void BufferData()
    {
        BufferIndex = VertexManager.BindVertexData(Vertices, Indices, true);
        //TextureIndex = TextureManager.BindTextureData("Textures/button.png");
    }
    
    public override void BufferData(int pBufferTarget)
    {
        BufferIndex = VertexManager.BindVertexData(Vertices, Indices, true, pBufferTarget);
        //TextureIndex = TextureManager.BindTextureData("Textures/button.png");
    }
    
    /// <summary>
    /// Generates the vertices for the mesh
    /// </summary>
    /// <param name="pSize">Size of the mesh</param>
    /// <param name="pScale">Resolution of the mesh</param>
    /// <param name="pVertices">Mesh Vertices</param>
    private static void GenerateVertices(int pSize, float pScale, List<float> pVertices)
    {
        for (var heightIndex = 0; heightIndex < pSize; heightIndex++)
        for (var widthIndex = 0; widthIndex < pSize; widthIndex++)
        {
            pVertices.Add(((-pSize / 2.0f) + heightIndex) * pScale); // X, the range of the X dimension
            pVertices.Add(0); // Y - Height will be added later
            pVertices.Add(((-pSize / 2.0f) + widthIndex) * pScale); // Z, the range of the Z dimension  

            // Normals
            pVertices.Add(0);
            pVertices.Add(1);
            pVertices.Add(0);

            // Texture Coords
            pVertices.Add(((-pSize / 2.0f) + heightIndex) * pScale); // X, the range of the X dimension
            pVertices.Add(((-pSize / 2.0f) + widthIndex) * pScale); // Z, the range of the Z dimension  
        }
    }

    
    /// <summary>
    /// Generates a terrain mesh
    /// </summary>
    /// <param name="pSize">Height of the mesh</param>
    /// <param name="pScale">Resolution of the mesh</param>
    private void GenerateTriangleMesh(int pSize, float pScale)
    {
        PrimitiveType = PrimitiveType.Triangles;
        
        // Will initialize an initial grid of chunks first,
        // then work on generating new ones
        
        // Size 20
        // 3200 / 8 = 40 Vertices per mesh

        // https://www.youtube.com/watch?v=DJk-aTmrAlQ
        
        // Make these arrays again
        var vertices = new List<float>();
        var indices = new List<uint>();
        
        var rand = new Random();
        
        GenerateVertices(pSize, pScale, vertices);

        // Indices bug with extra lines
        // Dividing this fixes it by reducing the number of loops, because less indices per vertex, so repeated values

        for (var heightIndex = 0; heightIndex - 1 < pSize / 1.15; heightIndex++)
        for (var widthIndex = 0; widthIndex - 1 < pSize / 1.15; widthIndex++)
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

    
    public void AddHeightData(float[,] pHeightData)
    {
        HeightData = pHeightData;
        var stride = 8;
        var yPointer = 1;
        
        for (var i = 0; i < pHeightData.GetLength(0); i++)
        for (var j = 0; j < pHeightData.GetLength(0); j++)
        {
            Vertices[yPointer] = pHeightData[i, j];
            yPointer += stride;
        }
    }

    public void PrintHeightData()
    {
        for (var i = 0; i < HeightData.GetLength(0); i++)
        {
            Console.WriteLine("");
            for (var j = 0; j < HeightData.GetLength(0); j++)
            {
                Console.Write($"{HeightData[i, j]} ");
            }   
        }

    }
    
    public override void Render(int pShaderHandle)
    {
        Update(pShaderHandle);
        
        GL.BindVertexArray(VertexManager.GetVaoAtIndex(BufferIndex));
        GL.DrawElements((OpenTK.Graphics.OpenGL.PrimitiveType) PrimitiveType, Indices.Length, DrawElementsType.UnsignedInt, 0);
    }

    public override void Update(int pShaderHandle)
    {
        var uModel = GL.GetUniformLocation(pShaderHandle, "uModel");
        var translation = Matrix4.CreateTranslation(Position);
        GL.UniformMatrix4(uModel, true, ref translation);
        
        var vertexColorLocation = GL.GetUniformLocation(pShaderHandle, "uColour");
        GL.Uniform4(vertexColorLocation, BaseColour);

        var uTextureSamplerLocation = GL.GetUniformLocation(pShaderHandle, "uTextureSampler1");
        GL.Uniform1(uTextureSamplerLocation, TextureIndex);
    }
}