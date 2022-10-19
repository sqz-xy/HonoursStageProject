using HonoursStageProject.Managers;
using OpenTK;
using OpenTK.Graphics.OpenGL;
namespace HonoursStageProject.Scenes;

public class MainMenuScene : Scene
{
    private float[] _triangleVertices = {
        -0.5f, -0.5f, 0.0f, //Bottom-left vertex
        0.5f, -0.5f, 0.0f, //Bottom-right vertex
        0.0f,  0.5f, 0.0f  //Top vertex
    };

    private int _VBO;
    
    public MainMenuScene(SceneManager sceneManager) : base(sceneManager)
    {
        sceneManager.Title = "Main Menu";

        sceneManager._renderer = Render;
        sceneManager._updater = Update;
    }

    public override void Initialize()
    {
        _VBO = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
        GL.BufferData(BufferTarget.ArrayBuffer, _triangleVertices.Length * sizeof(float), _triangleVertices, BufferUsageHint.StaticDraw);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.DeleteBuffer(_VBO);
    }

    public override void Render(FrameEventArgs e)
    {

    }

    public override void Update(FrameEventArgs e)
    {

    }

    public override void Close()
    {
        throw new NotImplementedException();
    }
}