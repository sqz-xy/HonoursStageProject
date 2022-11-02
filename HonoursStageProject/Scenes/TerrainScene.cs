using HonoursStageProject.Managers;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace HonoursStageProject.Scenes;

public class TerrainScene : Scene
{
    public TerrainScene(SceneManager sceneManager) : base(sceneManager)
    {
        sceneManager.Renderer = Render;
        sceneManager.Updater = Update;
        sceneManager.MouseEvent = null;
        
        Initialize();
    }

    public override void Initialize()
    {
        GL.ClearColor(1.0f, 0.0f, 0.0f, 1.0f);
    }

    public override void Render(FrameEventArgs e)
    {
        
    }

    public override void Update(FrameEventArgs e)
    {
        
    }

    public override void Close()
    {
        
    }
}