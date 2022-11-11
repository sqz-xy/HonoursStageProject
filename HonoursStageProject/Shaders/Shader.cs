using OpenTK.Graphics.OpenGL;
namespace HonoursStageProject.Shaders;

public sealed class Shader : IDisposable
{
    public int Handle { get; private set; }
    private bool _disposedValue;

    public Shader(string pVertexPath, string pFragmentPath)
    {
        var vertexShader = LoadShader(pVertexPath, ShaderType.VertexShader);
        var fragmentShader = LoadShader(pFragmentPath, ShaderType.FragmentShader);
        
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
        var shader = GL.CreateShader(pType);
        
        using (var sr = new StreamReader(pFileName))
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

        GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out var success);
        if (success == 0)
        {
            var infoLog = GL.GetProgramInfoLog(Handle);
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
    /// <param name="pDisposing"></param>
    private void Dispose(bool pDisposing)
    {
        if (!_disposedValue)
        {
            GL.DeleteProgram(Handle);

            // Object disposed
            _disposedValue = true;
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
    /// Calls the IDisposable implemented method
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}