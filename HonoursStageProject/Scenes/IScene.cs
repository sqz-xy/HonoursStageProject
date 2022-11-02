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
    void Render(FrameEventArgs e);
    void Update(FrameEventArgs e);
    void Close();
}