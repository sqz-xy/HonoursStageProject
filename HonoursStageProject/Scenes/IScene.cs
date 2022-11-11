using OpenTK;

namespace HonoursStageProject.Scenes;

public enum SceneTypes
{
    SceneMainMenu,
    SceneTerrain
}

public interface IScene
{
    void Initialize();
    void Render(FrameEventArgs pE);
    void Update(FrameEventArgs pE);
    void Close();
}