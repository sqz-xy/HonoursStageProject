using HonoursStageProject.Managers;

namespace HonoursStageProject;

/* Use a Terrain Mesh as a chunk essentially
 * Chunk contains a position transform
 * Initialize the transform using double nested loop for the initial map
 * Loop through the chunks and their vertices to add initial height map data
 *
 *
 * 
 */

public static class Program
{
    // Main Method
    public static void Main()
    {
        using var program = new SceneManager();
        program.Run(60.0);
    }
}