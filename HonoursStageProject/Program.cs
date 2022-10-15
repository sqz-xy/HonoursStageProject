using HonoursStageProject.Managers;

namespace HonoursStageProject;

public static class Program
{
    // Main Method
    public static void Main()
    {
        using SceneManager program = new SceneManager();
        program.Run(60.0);
    }
}