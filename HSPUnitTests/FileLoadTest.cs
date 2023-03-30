using Xunit;
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
    }

    [Fact]
    public void ChunkTests()
    {
        // Write test to test if adjacent chunks mesh together
        AscFileManager manager = new AscFileManager();
        var settings = manager.LoadSettings("Resources/settings_chunk_test.txt");
        
        var chunkManager = new ChunkManager(false);
        chunkManager.GenerateMap(settings);

        var sourceChunk = chunkManager.GetSourceChunk();
        
        // Chunk grid built clockwise
        // Top Chunk should be null
        Assert.Null(sourceChunk.Adjacents[0]);
        
        // Right Chunk
        var sourceChunkRight = GetCol(sourceChunk.HeightData, sourceChunk.HeightData.GetLength(0) - 1);
        var adjLeft = GetCol(sourceChunk.Adjacents[1].HeightData, 0);
        Assert.True(sourceChunkRight.SequenceEqual(adjLeft));
        
        // Bottom Chunk
        var sourceChunkBottom = GetRow(sourceChunk.HeightData, sourceChunk.HeightData.GetLength(0) - 1);
        var adjTop = GetRow(sourceChunk.Adjacents[2].HeightData, 0);
        Assert.True(sourceChunkBottom.SequenceEqual(adjTop));
        
        // Left Chunk should be null
        Assert.Null(sourceChunk.Adjacents[3]);
    }
    
    private float[] GetRow(float[,] pMatrix, int pRow)
    {
        var rowLength = pMatrix.GetLength(1);
        var rowVector = new float[rowLength];

        for (var i = 0; i < rowLength; i++)
        {
            rowVector[i] = pMatrix[pRow, i];
        }
        return rowVector;
    }
    
    private void SetRow(float[,] pMatrix, int pRow, float[] pRowValues)
    {
        var rowLength = pMatrix.GetLength(1);

        for (var i = 0; i < rowLength; i++)
            pMatrix[pRow, i] = pRowValues[i];
    }
    
    private float[] GetCol(float[,] pMatrix, int pCol)
    {
        var colLength = pMatrix.GetLength(0);
        var colVector = new float[colLength];

        for (var i = 0; i < colLength; i++)
            colVector[i] = pMatrix[i, pCol];

        return colVector;
    }
    
    private void SetCol(float[,] pMatrix, int pCol, float[] pColValues)
    {
        var colLength = pMatrix.GetLength(0);

        for (var i = 0; i < colLength; i++)
            pMatrix[i, pCol] = pColValues[i];
    }
}