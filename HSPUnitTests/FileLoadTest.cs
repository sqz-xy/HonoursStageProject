
using HonoursStageProject.Managers;
using HonoursStageProject.Objects;

namespace HSPUnitTests;

public class FileLoadTest
{
    [Fact]
    public void LoadFileSuccess()
    {
        AscFileManager manager = new AscFileManager();
        var chunkGrid = new Chunk[0, 0];
        //var output = manager.ReadHeightData("Resources/TestInput.txt", 0, out chunkGrid);
        //Assert.True(output);

    }
}