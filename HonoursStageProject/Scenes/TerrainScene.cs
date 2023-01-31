using HonoursStageProject.Algorithms;
using HonoursStageProject.Managers;
using HonoursStageProject.Objects;
using HonoursStageProject.Shaders;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Object = HonoursStageProject.Objects.Object;

namespace HonoursStageProject.Scenes;

public sealed class TerrainScene : Scene
{
    private Shader _shader;
    
    private TerrainMesh _terrainMesh;
    private TerrainMesh _terrainMesh2;
    private TerrainMesh _terrainMesh3;

    private Camera _camera;

    private Algorithm _diamondSquare;

    private List<TerrainMesh> _meshes;

    private int _textureIndex;

    private ChunkManager _chunkManager;


    // Camera
    public TerrainScene(SceneManager pSceneManager) : base(pSceneManager)
    {
        SceneManager.Title = "Terrain Scene";
        
        pSceneManager.Renderer = Render;
        pSceneManager.Updater = Update;
        pSceneManager.MouseMoveEvent += MouseMove;
        pSceneManager.MouseClickEvent = null;
        pSceneManager.KeyPressEvent += KeyPress;
        pSceneManager.CursorVisible = false;
        pSceneManager.CursorGrabbed = true;

        _meshes = new List<TerrainMesh>();
        
        Initialize();
    }

    public override void Initialize()
    {
        var rnd = new Random();
        
        GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
        
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace);
        GL.CullFace(CullFaceMode.Front); // Changed due to triangle strip
        
        // Camera and Shader initialization
        _camera = new Camera();
        _shader = new Shader(@"Shaders/terrainscene.vert", @"Shaders/terrainscene.frag");

        // Texture Initialization
        _textureIndex = TextureManager.BindTextureData("Textures/button.png");
        
        // Object initialization (Terrain mesh) Buffer size cannot be modified during runtime
        VertexManager.Initialize(100000);
        TextureManager.Initialize(2);
        
        _meshes.Add(new TerrainMesh(new Vector3(0.0f, 2, 0.0f), 16, 1));
        _meshes.Add(new TerrainMesh(new Vector3(16.0f,2, 0.0f), 16, 1));
        _meshes.Add(new TerrainMesh(new Vector3(-16.0f, 2, -0.0f), 32, 0.5f));
        _meshes.Add(new TerrainMesh(new Vector3(32.0f,2, 0.0f), 16, 1));
        
        // Testing
        _terrainMesh = _meshes[0];
        
        // TODO: Fix bug "If the size is greater than 25 or less than 64 it breaks???"
        
        // Initialize mesh values
        foreach (var mesh in _meshes)
        {
            var terrainMesh = (TerrainMesh) mesh;
            _diamondSquare = new DiamondSquare(terrainMesh.Size);
            terrainMesh.AddHeightData(_diamondSquare.GenerateData(rnd.Next(), terrainMesh.Scale, 0.5f)); 
            terrainMesh.TextureIndex = _textureIndex;
            terrainMesh.BufferData();
        }
        
        // Chunk Manager, putting this before the other meshes causes a crash?
        // Need to fix scaling chunk boundry issue
        
        _chunkManager = new ChunkManager();
        _chunkManager.GenerateMap(2, 16, 1.0f);

        var uViewPosLocation = GL.GetUniformLocation(_shader.Handle, "uViewPos");
        GL.Uniform3(uViewPosLocation, _camera.Position);
        
        GL.UseProgram(_shader.Handle);
    }
    

    public override void Render(FrameEventArgs pE)
    {
        _shader.UseShader();

        foreach (var mesh in _meshes)
        {
           //mesh.Render(_shader.Handle); // COMMENTED OUT FOR TESTING
        }
        
        _chunkManager.RenderMap(_shader.Handle);
    }

    public override void Update(FrameEventArgs pE)
    {
        _camera.RotateCamera(Mouse.GetState());
        _camera.UpdateCamera(_shader.Handle);
    }

    /// <summary>
    /// Mouse movement logic, resets the mouse to the centre of the screen on movement, used for the camera
    /// </summary>
    /// <param name="pMouseEventArgs">The mouse event arguments</param>
    private void MouseMove(MouseEventArgs pMouseEventArgs)
    {
        if (SceneManager.Focused)
        {
            // Center Mouse after movement
            Mouse.SetPosition(pMouseEventArgs.X + SceneManager.SWidth / 2f, pMouseEventArgs.Y + SceneManager.SHeight / 2f);
        }
    }
    
    /// <summary>
    /// Basic camera movement, needs to be refactored into keyboard state
    /// </summary>
    /// <param name="pKeyPressEventArgs">The key press event arguments</param>
    private void KeyPress(KeyPressEventArgs pKeyPressEventArgs)
    {
        var rnd = new Random();
        switch (pKeyPressEventArgs.KeyChar)
        {
            case 'a':
                _camera.MoveCamera(Direction.Left, 0.3f);
                break;
            case 'd':
                _camera.MoveCamera(Direction.Right, 0.3f);
                break;
            case 'w':
                _camera.MoveCamera(Direction.Forward,0.3f);
                break;
            case 's':
                _camera.MoveCamera(Direction.Backward,0.3f);
                break;
            case 'q':
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                break;
            case 'e':
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                break;
            case 'f':
                SceneManager.Exit();
                break;
            case 'z':
                // Limit is bufferSize, Make a mesh, IMGUI configurable
                _meshes.Remove(_terrainMesh);
                _terrainMesh = new TerrainMesh(new Vector3(0.0f, 2, 0.0f), 16, 1);
                _diamondSquare = new DiamondSquare(_terrainMesh.Size);
                _terrainMesh.AddHeightData(_diamondSquare.GenerateData(rnd.Next(), 1, 0.5f)); 
                _meshes.Add(_terrainMesh);
                _terrainMesh.BufferData(_terrainMesh.BufferIndex);
                break;
        }
    }

    public override void Close()
    {
        VertexManager.ClearData();
        TextureManager.ClearData();
    }
}