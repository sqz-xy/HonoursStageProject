using System;
using HonoursStageProject.Scenes;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace HonoursStageProject.Managers;

public class SceneManager : GameWindow
{
    public static int SWidth = 1280, SHeight = 720;

    private Scene _currentScene;
    public delegate void SceneDelegate(FrameEventArgs e);
    public SceneDelegate Renderer;
    public SceneDelegate Updater;

    public delegate void MouseDelegate(MouseEventArgs e);

    public MouseDelegate? MouseEvent;

    // Stack of render delegates
    // Update delegate stays the same
    // Input manager for camera movement
    
    public SceneManager() : base(SWidth, SHeight, new GraphicsMode(new ColorFormat(8, 8, 8, 8), 16))
    {

    }
    
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        //GL.Enable(EnableCap.DepthTest);
        GL.DepthMask(true);
        //GL.Enable(EnableCap.CullFace);
        //GL.CullFace(CullFaceMode.Back);
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        
        _currentScene = new MainMenuScene(this);
    }
    
    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        
        GL.Viewport(0, 0, Width, Height);
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);
        Updater(e);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        
        Renderer(e);

        GL.Flush();
        SwapBuffers();
    }

    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
        MouseEvent?.Invoke(e);
    }

    public void ChangeScene(SceneTypes pSceneType)
    {
        _currentScene.Close();

        try
        {
            switch (pSceneType)
            {
                case SceneTypes.SceneMainMenu:
                    _currentScene = new MainMenuScene(this);
                    break;
                case SceneTypes.SceneTerrain:
                    _currentScene = new TerrainScene(this);
                    break;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            _currentScene = new MainMenuScene(this);
        }
    }

    protected override void OnUnload(EventArgs e)
    {
        base.OnUnload(e);
        _currentScene.Close();
    }

}