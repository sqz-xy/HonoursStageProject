﻿using OpenTK.Graphics.OpenGL;

namespace HonoursStageProject.Managers;

static class VertexManager
{
    private static int[] _vboIDs;
    private static  int[] _vaoIDs;
    private static  int[] _eboIDs;

    private static int _vboIndex;
    private static int _vaoIndex;
    private static int _eboIndex;

    private static int _bufferSize;
    
    public static void Initialize(int pBufferSize)
    {
        _bufferSize = pBufferSize;
        
        _vboIDs = new int[_bufferSize];
        _vaoIDs = new int[_bufferSize];
        _eboIDs = new int[_bufferSize];
        
        _vaoIndex = 0;

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
    /// <param name="pIsTextured">Is the object textured?</param>
    /// <returns>The VAO index of the bound vertices</returns>
    public static int BindVertexData(float[] pVertices, uint[] pIndices, bool pIsTextured)
    {
        if (_vaoIndex == _vaoIDs.Length)
        {
            FileManager.LogMessage("Not enough vertex buffer space preallocated");
            return -1;  
        }
        
        GL.BindVertexArray(_vaoIDs[_vaoIndex]);

        GL.BindBuffer(BufferTarget.ArrayBuffer, _vboIDs[_vaoIndex]);
        GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(pVertices.Length * sizeof(float)), pVertices, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _eboIDs[_vaoIndex]);
        GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(pIndices.Length * sizeof(uint)), pIndices, BufferUsageHint.StaticDraw);

        _vaoIndex++;
        // Make sure data is buffered correctly
        GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out int size);
        
        if (pVertices.Length * sizeof(float) != size)
            FileManager.LogMessage("Vertex data not loaded onto graphics card correctly");
        

        GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);
        
        if (pIndices.Length * sizeof(float) != size)
            FileManager.LogMessage("Index data not loaded onto graphics card correctly");
        
        // No normals or textures yet
        EnableVertexAttributes(pIsTextured);

        GL.BindVertexArray(0);
        return _vaoIndex - 1;
    }

    /// <summary>
    /// Binds and Buffers data to a passed in buffer
    /// </summary>
    /// <param name="pVertices">The vertices to bind</param>
    /// <param name="pIndices">The indices to bind</param>
    /// <param name="pIsTextured">Is the object textured?</param>
    /// <param name="pBufferTarget">The buffer target</param>
    /// <returns>The VAO index of the bound vertices</returns>
    public static int BindVertexData(float[] pVertices, uint[] pIndices, bool pIsTextured, int pBufferTarget)
    {
        GL.BindVertexArray(_vaoIDs[pBufferTarget]);

        GL.BindBuffer(BufferTarget.ArrayBuffer, _vboIDs[pBufferTarget]);
        GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(pVertices.Length * sizeof(float)), pVertices, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _eboIDs[pBufferTarget]);
        GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(pIndices.Length * sizeof(uint)), pIndices, BufferUsageHint.StaticDraw);
        
        // Make sure data is buffered correctly
        GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out int size);
        
        if (pVertices.Length * sizeof(float) != size)
            FileManager.LogMessage("Vertex data not loaded onto graphics card correctly");

        GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);
        
        if (pIndices.Length * sizeof(float) != size)
            FileManager.LogMessage("Index data not loaded onto graphics card correctly");
        
        // No normals or textures yet
        EnableVertexAttributes(pIsTextured);

        GL.BindVertexArray(0);
        return pBufferTarget;
    }

    /// <summary>
    /// Enables vertex attributes for models
    /// </summary>
    /// <param name="pIsTextured">Is the object textured?</param>
    private static void EnableVertexAttributes(bool pIsTextured)
    {
        // Locations are hard coded into shader layout
        // Account for textures using increased stride
        if (pIsTextured)
        {
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, true, 8 * sizeof(float), 3 * sizeof(float));

            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
            return;
        }

        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, true, 6 * sizeof(float), 3 * sizeof(float));
         
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

    public static void RemoveBuffer(int pBufferTarget)
    {
        GL.DeleteBuffer(pBufferTarget);
    }
}