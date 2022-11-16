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

    /// <summary>
    /// Calls the stored delegate on a mouse move event
    /// </summary>
    /// <param name="pMouseMoveEventArgs">The mouse movement event arguments</param>
    protected override void OnMouseMove(MouseMoveEventArgs pMouseMoveEventArgs)
    {
        MouseMoveEvent?.Invoke(pMouseMoveEventArgs);
    }

    /// <summary>
    /// Calls the stored delegate on a mouse down event 
    /// </summary>
    /// <param name="pMouseButtonEventArgs">The mouse button event arguments</param>
    protected override void OnMouseDown(MouseButtonEventArgs pMouseButtonEventArgs)
    {
        MouseClickEvent?.Invoke(pMouseButtonEventArgs);
    }

    /// <summary>
    /// Calls the stored delegate on a key press event
    /// </summary>
    /// <param name="pKeyPressEventArgs">The key press event arguments</param>
    protected override void OnKeyPress(KeyPressEventArgs pKeyPressEventArgs)
    {
        KeyPressEvent?.Invoke(pKeyPressEventArgs);
    }

    /// <summary>
    /// Changes the current scene to a different scene
    /// </summary>
    /// <param name="pSceneType">The scene type you want to change to</param>
    public void ChangeScene(SceneTypes pSceneType)
    {
        _currentScene.Close();

        //try
        //{
            switch (pSceneType)
            {
                case SceneTypes.SceneMainMenu:
                    _currentScene = new MainMenuScene(this);
                    break;
                case SceneTypes.SceneTerrain:
                    _currentScene = new TerrainScene(this);
                    break;
            }
        //}
        /*catch (Exception e)
        {
            Console.WriteLine(e.Message);
            _currentScene = new MainMenuScene(this);
        }*/
    }

    /// <summary>
    /// The unload event, triggers when the window is closed
    /// </summary>
    /// <param name="pEventArgs">The event arguments</param>
    protected override void OnUnload(EventArgs pEventArgs)
    {
        base.OnUnload(pEventArgs);
        _currentScene.Close();
    }

}