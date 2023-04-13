using HonoursStageProject.Managers;

namespace HonoursStageProject;

/* TODO:
 * Add multiple textures for different reliefs
 * Fix Lighting
 * Fix chunk joining
 * 
 */

public static class Program
{
    // Main Method
    public static void Main()
    {
        using var program = new SceneManager();
        program.Run();
    }
}