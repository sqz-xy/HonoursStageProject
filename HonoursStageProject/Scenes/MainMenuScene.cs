using HonoursStageProject.Managers;
using HonoursStageProject.Objects;
using HonoursStageProject.Shaders;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace HonoursStageProject.Scenes;

public sealed class MainMenuScene : Scene
{
    private Shader _shader;
    private Quadrilateral2D _button;

    public MainMenuScene(SceneManager sceneManager) : base(sceneManager)
    {
        sceneManager.Title = "Main Menu";

        sceneManager.Renderer = Render;
        sceneManager.Updater = Update;
        sceneManager.MouseMoveEvent += MouseMovement;
        sceneManager.MouseClickEvent += MouseClick;
        
        Initialize();
    }

    public override void Initialize()
    {
        // Initialise variables
        VertexManager.Initialize(1, 1, 1);
        
        _shader = new Shader(@"Shaders/vs.vert", @"Shaders/fs.frag");
        
        _button = new Quadrilateral2D(new Vector2(0.0f, 0.0f),0.2f, 0.1f, Vector4.One)
        {
            Colour = Vector4.One,
        };
        
        _button.Index = VertexManager.BindVertexData(_button.Vertices, _button.Indices, 0, 0, 0);
        
        // Shader stuff
        _shader.UseShader();
        
    }

    public override void Render(FrameEventArgs e)
    {
        //Console.WriteLine("Rendering");
        
        _shader.UseShader();
        
        GL.BindVertexArray(VertexManager.GetVaoAtIndex(_button.Index));
        GL.DrawElements((PrimitiveType) _button.PrimitiveType, _button.Indices.Length, DrawElementsType.UnsignedInt, 0);
    }

    private void MouseMovement(MouseEventArgs e)
    {
        Vector2 mousePos = new Vector2((float) (-1.0 + 2.0 * (double) e.X / SceneManager.Width),
            (float) (1.0 - 2.0 * (double) e.Y / SceneManager.Height));

        if (Shape2D.CheckSquareIntersection(_button, mousePos))
            _button.Colour = new Vector4(1.0f, 0.0f, 0.0f, 0.0f);
        else
            _button.Colour = Vector4.One;
    }

    private void MouseClick(MouseEventArgs e)
    {
        Vector2 mousePos = new Vector2((float) (-1.0 + 2.0 * (double) e.X / SceneManager.Width),
            (float) (1.0 - 2.0 * (double) e.Y / SceneManager.Height));

        if (Shape2D.CheckSquareIntersection(_button, mousePos))
            SceneManager.ChangeScene(SceneTypes.SceneTerrain);
    }

    public override void Update(FrameEventArgs e)
    {
        int vertexColorLocation = GL.GetUniformLocation(_shader.Handle, "uColour");
        GL.Uniform4(vertexColorLocation, _button.Colour);
    } 

    public override void Close()
    {
        VertexManager.ClearData();
    }
}