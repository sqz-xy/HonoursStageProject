using OpenTK;

namespace HonoursStageProject.Scenes;

enum SceneTypes
{
    SCENE_MAIN_MENU,
}

public interface IScene
{
    void Initialize();
    void Render(FrameEventArgs e);
    void Update(FrameEventArgs e);
    void Close();
}