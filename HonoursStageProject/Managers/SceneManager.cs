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
    
    protected override void OnLoad(EventArgs pE)
    {
        base.OnLoad(pE);

        //GL.Enable(EnableCap.DepthTest);
        GL.DepthMask(true);
        //GL.Enable(EnableCap.CullFace);
        //GL.CullFace(CullFaceMode.Back);
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        
        _currentScene = new MainMenuScene(this);
    }
    
    protected override void OnResize(EventArgs pE)
    {
        base.OnResize(pE);
        
        GL.Viewport(0, 0, Width, Height);
    }

    protected override void OnUpdateFrame(FrameEventArgs pE)
    {
        base.OnUpdateFrame(pE);
        Updater(pE);
    }

    protected override void OnRenderFrame(FrameEventArgs pE)
    {
        base.OnRenderFrame(pE);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
        Renderer(pE);

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