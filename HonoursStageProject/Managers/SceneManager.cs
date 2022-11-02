using System;
using HonoursStageProject.Scenes;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace HonoursStageProject.Managers;

public class SceneManager : GameWindow
{
    public static int _width = 1280, _height = 720;

    private Scene _currentScene;
    public delegate void SceneDelegate(FrameEventArgs e);
    public SceneDelegate _renderer;
    public SceneDelegate _updater;

    public delegate void MouseDelegate(MouseEventArgs e);

    public MouseDelegate _mouseEvent;

    // Stack of render delegates
    // Update delegate stays the same
    // Input manager for camera movement
    
    public SceneManager() : base(_width, _height, new GraphicsMode(new ColorFormat(8, 8, 8, 8), 16))
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
        _updater(e);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        
        _renderer(e);

        GL.Flush();
        SwapBuffers();
    }

    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
        _mouseEvent(e);
    }

    protected override void OnUnload(EventArgs e)
    {
        base.OnUnload(e);
    }

}