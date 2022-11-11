using HonoursStageProject.Managers;

namespace HonoursStageProject;

//TODO: Refactor shader updating to the shader class

public static class Program
{
    // Main Method
    public static void Main()
    {
        using var program = new SceneManager();
        program.Run(60.0);
    }
}