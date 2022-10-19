using HonoursStageProject.Managers;
using OpenTK;

namespace HonoursStageProject.Scenes;

public abstract class Scene : IScene
{
    protected SceneManager _sceneManager;

    public Scene(SceneManager sceneManager)
    {
        this._sceneManager = sceneManager;
    }

    public abstract void Initialize();
    public abstract void Render(FrameEventArgs e);

    public abstract void Update(FrameEventArgs e);

    public abstract void Close();
}