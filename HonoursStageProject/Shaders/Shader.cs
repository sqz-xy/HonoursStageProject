
using System;
using System.IO;
using System.Collections.Generic;
using HonoursStageProject.Scenes;
using OpenTK.Graphics.OpenGL;
namespace HonoursStageProject.Shaders;

public class Shader : IDisposable
{
    public int Handle { get; set; }
    private bool disposedValue = false;

    public Shader(string pVertexPath, string pFragmentPath)
    {
        int vertexShader = LoadShader(pVertexPath, ShaderType.VertexShader);
        int fragmentShader = LoadShader(pFragmentPath, ShaderType.FragmentShader);
        
        CreateProgram(vertexShader, fragmentShader);
        DetatchShaders(vertexShader, fragmentShader);
    }

    /// <summary>
    /// Uses the shader object
    /// </summary>
    public void UseShader()
    {
        GL.UseProgram(Handle);
    }

    /// <summary>
    /// Loads a shader
    /// </summary>
    /// <param name="pFileName">Shader filename</param>
    /// <param name="pType">Type of shader</param>
    /// <returns></returns>
    private int LoadShader(string pFileName, ShaderType pType)
    {
        int shader = GL.CreateShader(pType);
        
        using (StreamReader sr = new StreamReader(pFileName))
        {
            GL.ShaderSource(shader, sr.ReadToEnd());
        }

        GL.CompileShader(shader);
        Console.WriteLine(GL.GetProgramInfoLog(shader));
        return shader;
    }

    /// <summary>
    /// Creates the shader program using the vertex and fragment shader
    /// </summary>
    /// <param name="pVertexShader">The vertex shader to link</param>
    /// <param name="pFragmentShader">The fragment shader to link</param>
    private void CreateProgram(int pVertexShader, int pFragmentShader)
    {
        Handle = GL.CreateProgram();

        GL.AttachShader(Handle, pVertexShader);
        GL.AttachShader(Handle, pFragmentShader);

        GL.LinkProgram(Handle);

        GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int success);
        if (success == 0)
        {
            string infoLog = GL.GetProgramInfoLog(Handle);
            Console.WriteLine(infoLog);
        }
    }

    /// <summary>
    /// Detatches the individual shaders from the context as they have been linked already
    /// </summary>
    /// <param name="pVertexShader">The vertex shader to detach</param>
    /// <param name="pFragmentShader">The fragment shader to detach</param>
    private void DetatchShaders(int pVertexShader, int pFragmentShader)
    {
        GL.DetachShader(Handle, pVertexShader);
        GL.DetachShader(Handle, pFragmentShader);
        GL.DeleteShader(pFragmentShader);
        GL.DeleteShader(pVertexShader);
    }

    /// <summary>
    /// Delete the shader program once the shader is no longer needed
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            GL.DeleteProgram(Handle);

            // Object disposed
            disposedValue = true;
        }
    }

    /// <summary>
    /// Destructor
    /// </summary>
    ~Shader()
    {
        GL.DeleteProgram(Handle);
    }


    /// <summary>
    /// Calles the IDisposable implemented method
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}