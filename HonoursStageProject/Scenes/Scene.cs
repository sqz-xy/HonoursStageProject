using HonoursStageProject.Managers;
using OpenTK;

namespace HonoursStageProject.Scenes;

public abstract class Scene : IScene
{
    protected readonly SceneManager _sceneManager;

    protected Scene(SceneManager pSceneManager)
    {
        this._sceneManager = pSceneManager;
    }

    public abstract void Initialize();
    public abstract void Render(FrameEventArgs pE);
    public abstract void Update(FrameEventArgs pE);
    public abstract void Close();
    public void ChangeScene(SceneTypes pSceneType)
    {
        _sceneManager.ChangeScene(pSceneType);
    }
}