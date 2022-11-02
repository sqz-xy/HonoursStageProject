using HonoursStageProject.Managers;
using OpenTK;

namespace HonoursStageProject.Scenes;

public abstract class Scene : IScene
{
    protected SceneManager SceneManager;

    public Scene(SceneManager sceneManager)
    {
        this.SceneManager = sceneManager;
    }

    public abstract void Initialize();
    public abstract void Render(FrameEventArgs e);

    public abstract void Update(FrameEventArgs e);

    public abstract void Close();
}