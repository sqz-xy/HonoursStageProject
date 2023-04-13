using Xunit;
using System.ComponentModel.Design;
using HonoursStageProject.Algorithms;
using HonoursStageProject.Managers;
using HonoursStageProject.Objects;
using OpenTK;

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
        Assert.Equal(2.5f, settings.Roughness);
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
        
        var chunkManager = new ChunkManager(false, null);
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

    [Fact]
    public void DataGenSuccess()
    {
        // Write test to test if adjacent chunks mesh together
        AscFileManager manager = new AscFileManager();
        var settings = manager.LoadSettings("Resources/settings_chunk_test.txt");
        
        var chunkManager = new ChunkManager(false, null);
        chunkManager.GenerateMap(settings);

        var sourceChunk = chunkManager.GetSourceChunk();

        // Loop through chunks
        var downNode = sourceChunk;

        while (downNode != null)
        {
            var rightNode = downNode;

            while (rightNode != null)
            {
                // test all chunks for adjacent 0s
                CheckZeros(rightNode);
                rightNode = rightNode.Adjacents[1];
            }
            downNode = downNode.Adjacents[2];
        }
    }

    private void CheckZeros(Chunk pChunk)
    {
        Vector2[] _directions = 
        {
            new(-1, 0),
            new(-1, 1),
            new(0, 1),
            new(1, 1),
            new(1, 0),
            new(1, -1),
            new(0, -1),
            new(-1, -1)
        };
        
        // Loop through all values
        for (var i = 0; i < pChunk.HeightData.GetLength(0); i++)
        for (var j = 0; j < pChunk.HeightData.GetLength(1); j++)
        {
            // If 0, check for adjacent 0s
            if (pChunk.HeightData[i, j] != 0) continue;
            
            // Multiple adjacent 0s = fail
            for (var k = 0; k < _directions.Length; k++)
            {
                var x = i + (int) _directions[k].X;
                var y = j + (int) _directions[k].Y;

                // Bounds check
                if (x > pChunk.HeightData.GetLength(0) - 1 || y > pChunk.HeightData.GetLength(0) - 1 || y < 0 || x < 0)
                    continue;

                if (pChunk.HeightData[x, y] == 0)
                    Assert.Fail("Multiple adjacent 0s");
            }
        }
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
    
    private float[] GetCol(float[,] pMatrix, int pCol)
    {
        var colLength = pMatrix.GetLength(0);
        var colVector = new float[colLength];

        for (var i = 0; i < colLength; i++)
            colVector[i] = pMatrix[i, pCol];

        return colVector;
    }
}