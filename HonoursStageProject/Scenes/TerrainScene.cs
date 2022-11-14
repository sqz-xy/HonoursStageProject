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
    
    private Matrix4 _modelMatrix;
    private TerrainMesh _terrainMesh;
    private int _meshTextureIndex;
    
    private Camera _camera;
    

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
        VertexManager.Initialize(1, 1, 1);
        _terrainMesh = new TerrainMesh(100, 100, 10);
        _terrainMesh.Index = VertexManager.BindVertexData(_terrainMesh.Vertices, _terrainMesh.Indices, true);
        _meshTextureIndex = TextureManager.BindTextureData("Textures/button.png");
        _modelMatrix = Matrix4.Identity; 
        
        GL.UseProgram(_shader.Handle);
        
        UpdateValues();
    }

    private void UpdateValues()
    {
        var uModel = GL.GetUniformLocation(_shader.Handle, "uModel");
        GL.UniformMatrix4(uModel, true, ref _modelMatrix);

        var uView = GL.GetUniformLocation(_shader.Handle, "uView");
        GL.UniformMatrix4(uView, true, ref _camera.View);

        var uProjection = GL.GetUniformLocation(_shader.Handle, "uProjection");
        GL.UniformMatrix4(uProjection, true, ref _camera.Projection);
        
        var uTexLocation1 = GL.GetUniformLocation(_shader.Handle, "uTextureSampler1");
        GL.Uniform1(uTexLocation1, _meshTextureIndex);
    }

    public override void Render(FrameEventArgs pE)
    {
        _shader.UseShader();
        
        GL.BindVertexArray(VertexManager.GetVaoAtIndex(_terrainMesh.Index));
        GL.DrawElements((PrimitiveType) _terrainMesh.PrimitiveType, _terrainMesh.Indices.Length, DrawElementsType.UnsignedInt, 0);
    }

    public override void Update(FrameEventArgs pE)
    {
        _camera.RotateCamera(Mouse.GetState());
        _camera.UpdateCamera();
        UpdateValues();
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
        }
    }

    public override void Close()
    {
        VertexManager.ClearData();
        TextureManager.ClearData();
    }
}