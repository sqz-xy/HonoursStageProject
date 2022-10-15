using OpenTK;
using OpenTK.Graphics;

namespace HonoursStageProject.Managers;

public class SceneManager : GameWindow
{
    public static int _width = 1280, _height = 720;

    public SceneManager() : base(_width, _height, new GraphicsMode(new ColorFormat(8, 8, 8, 8), 16))
    {
    }

}