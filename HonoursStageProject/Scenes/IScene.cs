using OpenTK;

namespace HonoursStageProject.Scenes;

public enum SceneTypes
{
    SCENE_MAIN_MENU,
    SCENE_TERRAIN
}

public interface IScene
{
    void Initialize();
    void Render(FrameEventArgs e);
    void Update(FrameEventArgs e);
    void Close();
}