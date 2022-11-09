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
    private Matrix4 _viewMatrix;
    private Matrix4 _projectionMatrix;

    private Quadrilateral _square;
    private TerrainMesh _terrainMesh;

    private float[] _cubeVertices = new float[]
    {
        -0.2f, -0.2f, -0.2f, 
        0.2f, -0.2f, -0.2f, 
        -0.2f, 0.2f, -0.2f, 
        0.2f, 0.2f, -0.2f,
        -0.2f, -0.2f, 0.2f, 
        0.2f, -0.2f, 0.2f,
        -0.2f, 0.2f, 0.2f, 
        0.2f, 0.2f, 0.2f, 
        0.2f, -0.2f, -0.2f,
        0.2f, -0.2f, 0.2f,
        0.2f, 0.2f, -0.2f, 
        0.2f, 0.2f, 0.2f,
        -0.2f, -0.2f, -0.2f, 
        -0.2f, -0.2f, 0.2f, 
        -0.2f, 0.2f, -0.2f, 
        -0.2f, 0.2f, 0.2f,
        -0.2f, -0.2f, -0.2f, 
        -0.2f, -0.2f, 0.2f, 
        0.2f, -0.2f, -0.2f, 
        0.2f, -0.2f, 0.2f, 
        -0.2f, 0.2f, -0.2f,
        -0.2f, 0.2f, 0.2f,
        0.2f, 0.2f, -0.2f,
        0.2f, 0.2f, 0.2f
    };

    private uint[] _cubeIndices = new uint[]
    {
        1, 0, 2,
        1, 2, 3,
        4, 5, 6,
        6, 5, 7,
        9, 8, 10,
        9, 10, 11,
        12, 13, 14,
        14, 13, 15,
        17, 16, 18,
        17, 18, 19,
        20, 21, 22,
        22, 21, 23
    };

    private int _cubeIndex;
    private Camera _camera;
    

    // Camera
    public TerrainScene(SceneManager sceneManager) : base(sceneManager)
    {
        SceneManager.Title = "Terrain Scene";
        
        sceneManager.Renderer = Render;
        sceneManager.Updater = Update;
        sceneManager.MouseMoveEvent = null;
        sceneManager.MouseClickEvent = null;
        sceneManager.KeyPressEvent += KeyPress;
        
        Initialize();
    }

    public override void Initialize()
    {
        //GL.ClearColor(0.0f, 1.0f, 0.0f, 1.0f);
        GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
        
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace);
        GL.CullFace(CullFaceMode.Front); // Ask about this, show difference
        
        // Add toggle for this
        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        
        // Camera and Shader initialization
        _camera = new Camera();
        _shader = new Shader(@"Shaders/terrainscene.vert", @"Shaders/terrainscene.frag");
        
        VertexManager.Initialize(1, 1, 1);
        _terrainMesh = new TerrainMesh(100, 100, 10);
        _terrainMesh.Index = VertexManager.BindVertexData(_terrainMesh.Vertices, _terrainMesh.Indices, 0, 0, 0);
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
    }

    public override void Render(FrameEventArgs e)
    {
        _shader.UseShader();
        
        GL.BindVertexArray(VertexManager.GetVaoAtIndex(_terrainMesh.Index));
        GL.DrawElements((PrimitiveType) PrimitiveType.TriangleStrip, _terrainMesh.Indices.Length, DrawElementsType.UnsignedInt, 0);
    }

    public override void Update(FrameEventArgs e)
    {
        UpdateMatrices();
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
    }

    public override void Close()
    {
        VertexManager.ClearData();
    }
}