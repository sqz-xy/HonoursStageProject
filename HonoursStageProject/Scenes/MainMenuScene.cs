using HonoursStageProject.Managers;
using HonoursStageProject.Shaders;
using OpenTK;
using OpenTK.Graphics.OpenGL;
namespace HonoursStageProject.Scenes;

public class MainMenuScene : Scene
{
    // We modify the vertex array to include four vertices for our rectangle.
    private readonly float[] _triangleVertices =
    {
        0.5f,  0.5f, 0.0f, // top right
        0.5f, -0.5f, 0.0f, // bottom right
        -0.5f, -0.5f, 0.0f, // bottom left
        -0.5f,  0.5f, 0.0f, // top left
    };

    // Then, we create a new array: indices.
    // This array controls how the EBO will use those vertices to create triangles
    private readonly uint[] _triangleIndices =
    {
        // Note that indices start at 0!
        0, 1, 3, // The first triangle will be the top-right half of the triangle
        1, 2, 3  // Then the second will be the bottom-left half of the triangle
    };

    
    private int _VBO;
    private int _VAO;
    private int _EBO;
    private Shader _shader;
    
    public MainMenuScene(SceneManager sceneManager) : base(sceneManager)
    {
        sceneManager.Title = "Main Menu";

        sceneManager._renderer = Render;
        sceneManager._updater = Update;
        Initialize();
    }

    public override void Initialize()
    {
        // Initialise variables
        _shader = new Shader(@"Shaders/vs.vert", @"Shaders/fs.frag");
        _VBO = GL.GenBuffer();
        _VAO = GL.GenVertexArray();
        _EBO = GL.GenBuffer();
        
        GL.BindVertexArray(_VAO);

        GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
        GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(_triangleVertices.Length * sizeof(float)), _triangleVertices, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _EBO);
        GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(_triangleIndices.Length * sizeof(uint)), _triangleIndices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindVertexArray(0);
        
        _shader.UseShader();

    }

    public override void Render(FrameEventArgs e)
    {
        Console.WriteLine("Rendering");
        
        _shader.UseShader();
        
        GL.BindVertexArray(_VAO);
        GL.DrawElements(PrimitiveType.Triangles, _triangleIndices.Length, DrawElementsType.UnsignedInt, 0);
    }

    public override void Update(FrameEventArgs e)
    {

    }

    public override void Close()
    {
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        GL.BindVertexArray(0);
    }
}