using OpenTK.Graphics.OpenGL;

namespace HonoursStageProject.Managers;

static class VertexManager
{
    private static int[] _VBO_IDs;
    private static  int[] _VAO_IDs;
    private static  int[] _EBO_IDs;

    private static int _VBOIndex;
    private static int _VAOIndex;
    private static int _EBOIndex;

    static VertexManager()
    {
        
    }

    public static void Initialize(int pVBOSize, int pVAOSize, int pEBOSize)
    {
        _VBO_IDs = new int[pVBOSize];
        _VAO_IDs = new int[pVAOSize];
        _EBO_IDs = new int[pEBOSize];

        _VBOIndex = 0;
        _VAOIndex = 0;
        _EBOIndex = 0;

        // Generates the Vertex arrays and buffers on handler initialization
        GL.GenVertexArrays(_VAO_IDs.Length, _VAO_IDs);
        GL.GenBuffers(_VBO_IDs.Length, _VBO_IDs);
        GL.GenBuffers(_EBO_IDs.Length, _EBO_IDs);
    }
    
    /// <summary>
    /// Returns the VAO At a specified Index
    /// </summary>
    /// <param name="pVAOIndex">The index to return</param>
    /// <returns>An integer Value</returns>
    public static int GetVAOAtIndex(int pVAOIndex)
    {
        return _VAO_IDs[pVAOIndex];
    }

    /// <summary>
    /// Binds and Buffers data to the graphics card
    /// </summary>
    /// <param name="pVertices">The vertices to bind</param>
    /// <param name="pIndices">The indices to bind</param>
    /// <param name="pPositionLocation">The vertex position information from the shader</param>
    /// <param name="pNormalLocation">The vertex normal information from the shader</param>
    /// <returns>The VAO index of the bound vertices</returns>
    public static int BindVertexData(float[] pVertices, int[] pIndices, int pPositionLocation, int pNormalLocation, int pTextureLocation)
    {
        GL.BindVertexArray(_VAO_IDs[_VAOIndex]);
        _VAOIndex++;
        
        GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO_IDs[_VBOIndex]);
        GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(pVertices.Length * sizeof(float)), pVertices, BufferUsageHint.StaticDraw);
        _VBOIndex++;
        
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _EBO_IDs[_EBOIndex]);
        GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(pIndices.Length * sizeof(uint)), pIndices, BufferUsageHint.StaticDraw);
        _EBOIndex++;
        
        // Make sure data is buffered correctly
        GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out int size);
        if (pVertices.Length * sizeof(float) != size)
        {
            throw new ApplicationException("Vertex data not loaded onto graphics card correctly");
        }

        GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);
        if (pIndices.Length * sizeof(float) != size)
        {
            throw new ApplicationException("Index data not loaded onto graphics card correctly");
        }
        
        // No normals or textures yet
        EnableVertexAttributes(pPositionLocation, 0, 0);

        return _VAOIndex - 1;
    }

    /// <summary>
    /// Enables vertex attributes for the shapes and shaders
    /// </summary>
    /// <param name="pPositionLocation">Position location in the shader</param>
    /// <param name="pNormalLocation">Normal location in the shader</param>
    /// <param name="pTextureLocation">Texture location in the shader</param>
    private static void EnableVertexAttributes(int pPositionLocation, int pNormalLocation, int pTextureLocation)
    {
        GL.EnableVertexAttribArray(pPositionLocation);
        
        //Stride is three because only vertex coords are present
        GL.VertexAttribPointer(pPositionLocation, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
    }
    
    /// <summary>
    /// Deletes all buffered data
    /// </summary>
    public static void ClearData()
    {
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        GL.BindVertexArray(0);
        GL.DeleteBuffers(_VBOIndex, _VBO_IDs);
        GL.DeleteBuffers(_EBOIndex, _EBO_IDs);
        GL.DeleteVertexArrays(_VAOIndex, _VAO_IDs);
        
        _VBOIndex = 0;
        _VAOIndex = 0;
        _EBOIndex = 0;
        
        Array.Clear(_VBO_IDs);
        Array.Clear(_EBO_IDs);
        Array.Clear(_VAO_IDs);
    }
}