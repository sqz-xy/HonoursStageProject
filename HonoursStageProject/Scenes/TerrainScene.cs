using HonoursStageProject.Managers;
using HonoursStageProject.Objects;
using HonoursStageProject.Shaders;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace HonoursStageProject.Scenes;

public class TerrainScene : Scene
{
    private Shader _shader;
    
    private Matrix4 _modelMatrix;
    private TerrainMesh _terrainMesh;
    private int _meshTextureIndex;
    
    private Camera _camera;
    

    // Camera
    public TerrainScene(SceneManager sceneManager) : base(sceneManager)
    {
        SceneManager.Title = "Terrain Scene";
        
        sceneManager.Renderer = Render;
        sceneManager.Updater = Update;
        sceneManager.MouseMoveEvent += MouseMove;
        sceneManager.MouseClickEvent = null;
        sceneManager.KeyPressEvent += KeyPress;
        sceneManager.CursorVisible = false;
        sceneManager.CursorGrabbed = true;
        
        Initialize();
    }

    public override void Initialize()
    {
        GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
        
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace);
        GL.CullFace(CullFaceMode.Front); // Ask about this, show difference
        
        // Camera and Shader initialization
        _camera = new Camera();
        _shader = new Shader(@"Shaders/terrainscene.vert", @"Shaders/terrainscene.frag");

        // Object initialization
        VertexManager.Initialize(1, 1, 1);
        _terrainMesh = new TerrainMesh(100, 100, 10);
        _terrainMesh.Index = VertexManager.BindVertexData(_terrainMesh.Vertices, _terrainMesh.Indices, 0, 1, 2);
        _meshTextureIndex = TextureManager.BindTextureData("Textures/button.png");
        _modelMatrix = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(0.0f)); // Changed this to see correct side of mesh
        
        GL.UseProgram(_shader.Handle);
        
        UpdateMatrices();
    }

    private void UpdateMatrices()
    {
        int uModel = GL.GetUniformLocation(_shader.Handle, "uModel");
        GL.UniformMatrix4(uModel, true, ref _modelMatrix);

        int uView = GL.GetUniformLocation(_shader.Handle, "uView");
        GL.UniformMatrix4(uView, true, ref _camera.View);

        int uProjection = GL.GetUniformLocation(_shader.Handle, "uProjection");
        GL.UniformMatrix4(uProjection, true, ref _camera.Projection);
        
        int uTexLocation1 = GL.GetUniformLocation(_shader.Handle, "uTextureSampler1");
        GL.Uniform1(uTexLocation1, _meshTextureIndex);
    }

    public override void Render(FrameEventArgs e)
    {
        _shader.UseShader();
        
        GL.BindVertexArray(VertexManager.GetVaoAtIndex(_terrainMesh.Index));
        GL.DrawElements((PrimitiveType) PrimitiveType.TriangleStrip, _terrainMesh.Indices.Length, DrawElementsType.UnsignedInt, 0);
    }

    public override void Update(FrameEventArgs e)
    {
        _camera.RotateCamera(Mouse.GetState());
        _camera.UpdateCamera();
        UpdateMatrices();
    }

    private void MouseMove(MouseEventArgs e)
    {
        if (SceneManager.Focused)
        {
            // Center Mouse after movement
            Mouse.SetPosition(e.X + SceneManager.SWidth / 2f, e.Y + SceneManager.SHeight / 2f);
        }
    }
    
    private void KeyPress(KeyPressEventArgs e)
    {
        if (e.KeyChar == 'a') 
        { 
            _camera.MoveCamera(Direction.LEFT, 0.1f);
        }
        else if (e.KeyChar == 'd') 
        { 
            _camera.MoveCamera(Direction.RIGHT, 0.1f);
        }
        else if (e.KeyChar == 'w') 
        { 
            _camera.MoveCamera(Direction.FORWARD,0.1f);
        }
        else if (e.KeyChar == 's') 
        { 
            _camera.MoveCamera(Direction.BACKWARD,0.1f);
        }
        else if (e.KeyChar == 'q')
        {
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        }
        else if (e.KeyChar == 'e')
        {
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
        }
    }

    public override void Close()
    {
        VertexManager.ClearData();
        TextureManager.ClearData();
    }
}