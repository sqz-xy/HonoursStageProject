using HonoursStageProject.Managers;
using OpenTK;

namespace HonoursStageProject.Scenes;

public abstract class Scene : IScene
{
    protected readonly SceneManager SceneManager;

    protected Scene(SceneManager pSceneManager)
    {
        this.SceneManager = pSceneManager;
    }

    public abstract void Initialize();
    public abstract void Render(FrameEventArgs pE);

    public abstract void Update(FrameEventArgs pE);

    public abstract void Close();
}