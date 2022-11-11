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
    public delegate void SceneDelegate(FrameEventArgs pE);
    public SceneDelegate Renderer;
    public SceneDelegate Updater;

    public delegate void MouseMoveDelegate(MouseEventArgs pE);
    public MouseMoveDelegate? MouseMoveEvent;

    public delegate void MouseClickDelegate(MouseButtonEventArgs pE);
    public MouseClickDelegate? MouseClickEvent;

    public delegate void KeyPressDelegate(KeyPressEventArgs pE);
    public KeyPressDelegate? KeyPressEvent;

    // Stack of render delegates
    // Update delegate stays the same
    // Input manager for camera movement
    
    public SceneManager() : base(SWidth, SHeight, new GraphicsMode(new ColorFormat(8, 8, 8, 8), 16))
    {

    }
    
    /// <summary>
    /// Called when window loads
    /// </summary>
    /// <param name="pEventArgs">The event arguments for the OnLoad event</param>
    protected override void OnLoad(EventArgs pEventArgs)
    {
        base.OnLoad(pEventArgs);
        
        GL.DepthMask(true);
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        
        _currentScene = new MainMenuScene(this);
    }
    
    /// <summary>
    /// Called when the window is resized
    /// </summary>
    /// <param name="pEventArgs">Event arguments for the OnResize event</param>
    protected override void OnResize(EventArgs pEventArgs)
    {
        base.OnResize(pEventArgs);
        
        GL.Viewport(0, 0, Width, Height);
    }

    /// <summary>
    /// Called each frame to update logic within a scene
    /// </summary>
    /// <param name="pFrameEventArgs">The frame event arguments for OnUpdateFrame</param>
    protected override void OnUpdateFrame(FrameEventArgs pFrameEventArgs)
    {
        base.OnUpdateFrame(pFrameEventArgs);
        Updater(pFrameEventArgs);
    }

    /// <summary>
    /// Called each frame to update Rendering within a scene
    /// </summary>
    /// <param name="pFrameEventArgs">The frame event arguments for OnUpdateFrame</param>
    protected override void OnRenderFrame(FrameEventArgs pFrameEventArgs)
    {
        base.OnRenderFrame(pFrameEventArgs);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
        Renderer(pFrameEventArgs);

        GL.Flush();
        SwapBuffers();
    }

    protected override void OnMouseMove(MouseMoveEventArgs pE)
    {
        MouseMoveEvent?.Invoke(pE);
    }

    protected override void OnMouseDown(MouseButtonEventArgs pE)
    {
        MouseClickEvent?.Invoke(pE);
    }

    protected override void OnKeyPress(KeyPressEventArgs pE)
    {
        KeyPressEvent?.Invoke(pE);
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

    protected override void OnUnload(EventArgs pE)
    {
        base.OnUnload(pE);
        _currentScene.Close();
    }

}