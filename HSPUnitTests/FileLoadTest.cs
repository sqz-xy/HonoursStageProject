
using System.ComponentModel.Design;
using HonoursStageProject.Algorithms;
using HonoursStageProject.Managers;
using HonoursStageProject.Objects;

namespace HSPUnitTests;

public class FileLoadTest
{
    [Fact]
    public void LoadFileSuccess()
    {
        AscFileManager manager = new AscFileManager();
        var settings = manager.LoadSettings("Resources/settings_test.txt");
        
        // Check settings
        Assert.Equal(5, settings.MapSize);
        Assert.Equal(17, settings.ChunkSize);
        Assert.Equal(1.0f, settings.MapScale);
        Assert.Equal(5.0f, settings.RenderDistance);
        Assert.Equal(10, settings.Seed);
        Assert.Equal("Resources/TA12NE.asc",settings.FileName);
        
        Assert.True(settings.TerrainAlgorithm.GetType() == typeof(DiamondSquare));
        
        Assert.True(settings.CullingAlgorithms.Count == 2);
        Assert.True(settings.CullingAlgorithms[0].GetType() == typeof(DistanceCulling));
        Assert.True(settings.CullingAlgorithms[1].GetType() == typeof(FrustumCulling));
        
        // Write test to test if adjacent chunks mesh together
    }
}