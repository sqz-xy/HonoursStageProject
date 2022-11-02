using System.Data;
using System.Drawing;
using HonoursStageProject.Managers;
using HonoursStageProject.Shaders;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace HonoursStageProject.Scenes;

public class MainMenuScene : Scene
{
    private readonly float[] _triangleVertices =
    {
        0.5f,  0.5f, 0.0f, // top right
        0.5f, -0.5f, 0.0f, // bottom right
        -0.5f, -0.5f, 0.0f, // bottom left
        -0.5f,  0.5f, 0.0f, // top left
    };
    
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

    private Vector4 buttonColour;
    private int buttonIndex;

    public MainMenuScene(SceneManager sceneManager) : base(sceneManager)
    {
        sceneManager.Title = "Main Menu";

        sceneManager._renderer = Render;
        sceneManager._updater = Update;
        sceneManager._mouseEvent += MouseMovement;
        
        buttonColour = Vector4.One;
        
        Initialize();
    }

    public override void Initialize()
    {
        // Initialise variables
        _shader = new Shader(@"Shaders/vs.vert", @"Shaders/fs.frag");
        VertexManager.Initialize(1, 1, 1);

        buttonIndex = VertexManager.BindVertexData(_triangleVertices, _triangleIndices, 0, 0, 0);
        
        // Shader stuff
        _shader.UseShader();
        
    }

    public override void Render(FrameEventArgs e)
    {
        //Console.WriteLine("Rendering");
        
        _shader.UseShader();
        
        GL.BindVertexArray(VertexManager.GetVAOAtIndex(buttonIndex));
        GL.DrawElements(PrimitiveType.Triangles, _triangleIndices.Length, DrawElementsType.UnsignedInt, 0);
    }

    private void MouseMovement(MouseEventArgs e)
    {
        double normalizedX = -1.0 + 2.0 * (double)e.X / _sceneManager.Width; 
        double normalizedY = 1.0 - 2.0 * (double)e.Y / _sceneManager.Height; 
        Console.WriteLine($"{normalizedX} {normalizedY}");

        if ((normalizedX >= -0.5f && normalizedX <= 0.5f) && (normalizedY >= -0.5f && normalizedY <= 0.5f))
            //buttonColour = new Vector4(1.0f, 0.0f, 0.0f, 0.0f);
            _sceneManager.ChangeScene(SceneTypes.SCENE_TERRAIN);
        else
            buttonColour = Vector4.One;
    }
    
    public override void Update(FrameEventArgs e)
    {
        int vertexColorLocation = GL.GetUniformLocation(_shader.Handle, "uColour");
        GL.Uniform4(vertexColorLocation, buttonColour);
    } 

    public override void Close()
    {
        VertexManager.ClearData();
    }
}