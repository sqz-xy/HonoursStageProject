using HonoursStageProject.Objects;
using OpenTK;

namespace HonoursStageProject.Managers;

public class AscFileManager : FileManager
{
    
    //TODO: Remove nodata vars and use settings instead, fix algorithm loading
    //TODO: Next Week: Texturing, testing

    public override bool ReadHeightData(string pFileName, int pTextureIndex, out Chunk[,] pChunkGrid)
    {
        int mapSize, cellSize;
        float mapScale;
        float[,] chunkedData;

        try
        {
            var headerData = File.ReadLines(pFileName).Take(6).ToList();
        
            // Need to add defaults for event of no header data
            cellSize = int.Parse(headerData[4].Split(' ')[1]);
            mapScale = int.Parse(headerData[5].Split(' ')[4]);
            mapSize = int.Parse(headerData[5].Split(' ')[6]);

            var heightData = File.ReadLines(pFileName).Skip(6).ToList();
            chunkedData = new float[cellSize * mapSize, cellSize * mapSize];

            for (int i = 0; i < heightData.Count; ++i)
            {
                var line = heightData[i];
                for (int j = 0; j < chunkedData.GetLength(1); ++j)
                {
                    var lineData = line.Split(' ');
                    chunkedData[i, j] = float.Parse(lineData[j]);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            pChunkGrid = new Chunk[0, 0];
            return false;
        }
        
        var chunkGrid = new Chunk[mapSize, mapSize];

        var centreOffset = cellSize / 2;
        
        // Create Chunks
        for (var i = 0; i < mapSize; i++)
        for (var j = 0; j < mapSize; j++)
        {
            var xOffset = -(mapSize * cellSize) / 2;
            var yOffset = -(mapSize * cellSize) / 2;
            
            var xDataPointer = 0;
            var yDataPointer = 0;

            chunkGrid[i, j] =
                new Chunk(
                    new Vector3(xOffset + (i * cellSize) + centreOffset, -2, yOffset + (j * cellSize) + centreOffset),
                    cellSize, mapScale, 
                    new Vector2(i, j), pTextureIndex);
            
            for (int k = xDataPointer; k < cellSize; k++)
            for (int l = yDataPointer; l < cellSize; l++)
            {
                chunkGrid[i, j].HeightData[k, l] = chunkedData[k, l];
            }
            chunkGrid[i, j].AddHeightData(chunkGrid[i, j].HeightData);
        }
        
        pChunkGrid = chunkGrid;
        return true;
    }

    public override void SaveHeightData(string pFileName, int pMapSize, float pMapScale, int pSeed, Chunk pSourceChunk)
    {
        // Add header info
        using StreamWriter sw = new StreamWriter(pFileName);
        
        sw.WriteLine($"ncols {pSourceChunk.Size * pMapSize}");
        sw.WriteLine($"nrows {pSourceChunk.Size * pMapSize}");
        sw.WriteLine($"xllcorner {pSourceChunk.Size * pMapSize}");
        sw.WriteLine($"yllcorner {pSourceChunk.Size * pMapSize}");
        sw.WriteLine($"cellsize {pSourceChunk.Size}");
        sw.WriteLine($"nodata_value sd: {pSeed} mscale: {pMapScale} msize: {pMapSize}");
        
        // Construct a large 2d array of all the data and then write it
        float[,] cumulativeData = new float[pSourceChunk.Size * (pMapSize),
            pSourceChunk.Size * (pMapSize)];

        // Traverse the linked grid
        var downNode = pSourceChunk;

        var xPointer = 0;
        var yPointer = 0;

        while (downNode != null)
        {
            var rightNode = downNode;

            while (rightNode != null)
            {
                for (int i = 0; i < rightNode.HeightData.GetLength(0); i++)
                for (int j = 0; j < rightNode.HeightData.GetLength(1); j++)
                    cumulativeData[i + xPointer, j + yPointer] = rightNode.HeightData[i, j];

                rightNode = rightNode.Adjacents[1];
                xPointer += pSourceChunk.Size;
            }

            downNode = downNode.Adjacents[2];
            yPointer += pSourceChunk.Size;
            xPointer = 0;
        }

        // Save cumulative data

        for (int i = 0; i < cumulativeData.GetLength(0); i++)
        {
            if (i != 0)
                sw.WriteLine();
            
            for (int j = 0; j < cumulativeData.GetLength(1); j++)
                sw.Write($"{cumulativeData[i, j]}");
        }
    }
}