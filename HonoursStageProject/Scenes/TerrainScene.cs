using HonoursStageProject.Algorithms;
using HonoursStageProject.Managers;
using HonoursStageProject.Objects;
using HonoursStageProject.Shaders;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace HonoursStageProject.Scenes;

public sealed class TerrainScene : Scene
{
    private Shader _shader;
    
    private TerrainMesh _terrainMesh;
    private TerrainMesh _terrainMesh2;

    private Camera _camera;

    private Algorithm _diamondSquare;
    

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
        
        Initialize();
    }

    public override void Initialize()
    {
        GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
        
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace);
        GL.CullFace(CullFaceMode.Front); // Changed due to triangle strip
        
        // Camera and Shader initialization
        _camera = new Camera();
        _shader = new Shader(@"Shaders/terrainscene.vert", @"Shaders/terrainscene.frag");

        // Object initialization (Terrain mesh)
        VertexManager.Initialize(2, 2, 2);
        TextureManager.Initialize(2);

        _terrainMesh = new TerrainMesh(new Vector3(0.0f, 0, 0.0f), 16, 1);
        _terrainMesh2 = new TerrainMesh(new Vector3(-16.0f, 0, 0.0f), 16, 1);

        // Algorithm Initialization
        _diamondSquare = new DiamondSquare(_terrainMesh.Size);
        _terrainMesh.AddHeightData(_diamondSquare.GenerateData(10, 10, 10)); 
        _terrainMesh2.AddHeightData(_diamondSquare.GenerateData(11, 10, 10)); 
        
        // Buffer Data
        _terrainMesh.BufferData();
        _terrainMesh2.BufferData();
        
        GL.UseProgram(_shader.Handle);
    }
    

    public override void Render(FrameEventArgs pE)
    {
        _shader.UseShader();
        _terrainMesh.Render(_shader.Handle);
        _terrainMesh2.Render(_shader.Handle);
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
        switch (pKeyPressEventArgs.KeyChar)
        {
            case 'a':
                _camera.MoveCamera(Direction.Left, 0.1f);
                break;
            case 'd':
                _camera.MoveCamera(Direction.Right, 0.1f);
                break;
            case 'w':
                _camera.MoveCamera(Direction.Forward,0.1f);
                break;
            case 's':
                _camera.MoveCamera(Direction.Backward,0.1f);
                break;
            case 'q':
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                break;
            case 'e':
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                break;
            case 'f':
                _diamondSquare.GenerateData(10, 10, 1);
                _terrainMesh.AddHeightData(_diamondSquare.GenerateData(10, 10, 10)); 
                break;
        }
    }

    public override void Close()
    {
        VertexManager.ClearData();
        TextureManager.ClearData();
    }
}