using HonoursStageProject.Managers;

namespace HonoursStageProject;

//TODO: Refactor shader updating to the shader class

public static class Program
{
    // Main Method
    public static void Main()
    {
        using SceneManager program = new SceneManager();
        program.Run(60.0);
    }
}