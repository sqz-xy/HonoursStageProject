using System.Reflection.Metadata;
using HonoursStageProject.Managers;
using HonoursStageProject.Objects;
using HonoursStageProject.Shaders;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace HonoursStageProject.Scenes;

public class TerrainScene : Scene
{
    private Shader _shader;
    
    private Matrix4 _modelMatrix;
    private Matrix4 _viewMatrix;
    private Matrix4 _projectionMatrix;

    private Quadrilateral _square;
    public TerrainScene(SceneManager sceneManager) : base(sceneManager)
    {
        SceneManager.Title = "Terrain Scene";
        
        sceneManager.Renderer = Render;
        sceneManager.Updater = Update;
        sceneManager.MouseMoveEvent = null;
        sceneManager.MouseClickEvent = null;
        
        Initialize();
    }

    public override void Initialize()
    {
        GL.ClearColor(0.0f, 1.0f, 0.0f, 1.0f);
        //GL.Enable(EnableCap.DepthTest);
        //GL.DepthMask(true);

        VertexManager.Initialize(1, 1, 1);
        
        _shader = new Shader(@"Shaders/terrainscene.vert", @"Shaders/terrainscene.frag");
        _modelMatrix = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(-55.0f));
        _viewMatrix = Matrix4.CreateTranslation(0.0f, 0.0f, -3.0f);
        _projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), SceneManager.Width / SceneManager.Height, 0.1f, 100.0f);
        
        _square = new Quadrilateral(new Vector2(0.0f, 0.0f),0.2f, 0.5f, Vector4.One)
        {
            Colour = Vector4.One,
        };
        
        _square.Index = VertexManager.BindVertexData(_square.Vertices, _square.Indices, 0, 0, 0);
        
        GL.UseProgram(_shader.Handle);
        
        int uModel = GL.GetUniformLocation(_shader.Handle, "uModel");
        GL.UniformMatrix4(uModel, true, ref _modelMatrix);
        
        int uView = GL.GetUniformLocation(_shader.Handle, "uView");
        GL.UniformMatrix4(uView, true, ref _viewMatrix);
        
        int uProjection = GL.GetUniformLocation(_shader.Handle, "uProjection");
        GL.UniformMatrix4(uProjection, true, ref _projectionMatrix);
    }

    public override void Render(FrameEventArgs e)
    {
        //Console.WriteLine("Rendering");
        
        _shader.UseShader();
        
        GL.BindVertexArray(VertexManager.GetVaoAtIndex(_square.Index));
        GL.DrawElements((PrimitiveType) _square.PrimitiveType, _square.Indices.Length, DrawElementsType.UnsignedInt, 0);
    }

    public override void Update(FrameEventArgs e)
    {
        int vertexColorLocation = GL.GetUniformLocation(_shader.Handle, "uColour");
        GL.Uniform4(vertexColorLocation, _square.Colour);
    }

    public override void Close()
    {
        
    }
}