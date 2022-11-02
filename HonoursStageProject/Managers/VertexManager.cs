using OpenTK.Graphics.OpenGL;

namespace HonoursStageProject.Managers;

static class VertexManager
{
    private static int[] _vboIDs;
    private static  int[] _vaoIDs;
    private static  int[] _eboIDs;

    private static int _vboIndex;
    private static int _vaoIndex;
    private static int _eboIndex;
    
    public static void Initialize(int pVboSize, int pVaoSize, int pEboSize)
    {
        _vboIDs = new int[pVboSize];
        _vaoIDs = new int[pVaoSize];
        _eboIDs = new int[pEboSize];

        _vboIndex = 0;
        _vaoIndex = 0;
        _eboIndex = 0;

        // Generates the Vertex arrays and buffers on handler initialization
        GL.GenVertexArrays(_vaoIDs.Length, _vaoIDs);
        GL.GenBuffers(_vboIDs.Length, _vboIDs);
        GL.GenBuffers(_eboIDs.Length, _eboIDs);
    }
    
    /// <summary>
    /// Returns the VAO At a specified Index
    /// </summary>
    /// <param name="pVaoIndex">The index to return</param>
    /// <returns>An integer Value</returns>
    public static int GetVaoAtIndex(int pVaoIndex)
    {
        return _vaoIDs[pVaoIndex];
    }

    /// <summary>
    /// Binds and Buffers data to the graphics card
    /// </summary>
    /// <param name="pVertices">The vertices to bind</param>
    /// <param name="pIndices">The indices to bind</param>
    /// <param name="pPositionLocation">The vertex position information from the shader</param>
    /// <param name="pNormalLocation">The vertex normal information from the shader</param>
    /// <returns>The VAO index of the bound vertices</returns>
    public static int BindVertexData(float[] pVertices, uint[] pIndices, int pPositionLocation, int pNormalLocation, int pTextureLocation)
    {
        GL.BindVertexArray(_vaoIDs[_vaoIndex]);
        _vaoIndex++;
        
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vboIDs[_vboIndex]);
        GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(pVertices.Length * sizeof(float)), pVertices, BufferUsageHint.StaticDraw);
        _vboIndex++;
        
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _eboIDs[_eboIndex]);
        GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(pIndices.Length * sizeof(uint)), pIndices, BufferUsageHint.StaticDraw);
        _eboIndex++;
        
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

        GL.BindVertexArray(0);
        return _vaoIndex - 1;
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
        GL.DeleteBuffers(_vboIndex, _vboIDs);
        GL.DeleteBuffers(_eboIndex, _eboIDs);
        GL.DeleteVertexArrays(_vaoIndex, _vaoIDs);
        
        _vboIndex = 0;
        _vaoIndex = 0;
        _eboIndex = 0;
        
        Array.Clear(_vboIDs);
        Array.Clear(_eboIDs);
        Array.Clear(_vaoIDs);
    }
}