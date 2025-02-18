﻿using HonoursStageProject.Algorithms;
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
    private Camera _camera;
    private Settings _settings;
    
    public TerrainScene(SceneManager pSceneManager) : base(pSceneManager)
    {
        _sceneManager.Title = "Terrain Generated";
        
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
        
        // Camera and Shader initialization
        _camera = new Camera();
        _shader = new Shader(@"Shaders/terrainscene.vert", @"Shaders/terrainscene.frag");

        // Object initialization (Terrain mesh) Buffer size cannot be modified during runtime
        VertexManager.Initialize(100000);
        TextureManager.Initialize(2);
        
        _settings = _sceneManager._fileManager.LoadSettings("Resources/settings.txt");

        // Chunk sizes can only be 2, 3, 4, 9, 17, 33, 65, 129 
        _sceneManager._chunkManager.GenerateMap(_settings);
        
        GL.UseProgram(_shader.Handle);
        
        _shader.UseShader();
        _sceneManager._chunkManager.BufferMap(_shader.Handle);
        
    }
    
    public override void Render(FrameEventArgs pE)
    {
        _shader.UseShader();
        _sceneManager._chunkManager.RenderMap(_shader.Handle, _camera);
    }

    public override void Update(FrameEventArgs pE)
    {
        var input = Keyboard.GetState();
        
        if (input.IsKeyDown(Key.Escape))
            _sceneManager.Exit();
        
        _shader.UseShader();
        _camera.RotateCamera(Mouse.GetState());
        _camera.UpdateCamera(_shader.Handle);
    }

    /// <summary>
    /// Mouse movement logic, resets the mouse to the centre of the screen on movement, used for the camera
    /// </summary>
    /// <param name="pMouseEventArgs">The mouse event arguments</param>
    private void MouseMove(MouseEventArgs pMouseEventArgs)
    {
        if (_sceneManager.Focused)
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
                _sceneManager.Exit();
                break;
            case 'z':
                _sceneManager._chunkManager.SaveData("OutputData.txt");
                break;
            case '.':
                _sceneManager._chunkManager.ScaleChunkHeight(1.1f);
                break;
            case ',':
                _sceneManager._chunkManager.ScaleChunkHeight(0.9f);
                break;
            case 'g':
                if (_settings.FileName != String.Empty) {break;}
                _sceneManager._chunkManager.RegenerateMap();
                break;
        }
    }

    public override void Close()
    {
        VertexManager.ClearData();
        TextureManager.ClearData();
    }
}