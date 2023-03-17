using HonoursStageProject.Objects;
using OpenTK;

namespace HonoursStageProject.Managers;

public class AscFileManager : FileManager
{
    //TODO: Next Week: Texturing, testing, REFACTOR REFACTOR REFACTOR
    //TODO: Maybe store map data in a struct so i can re load them at runtime?
    
    //TODO: CellSize Limitation https://www.loc.gov/preservation/digital/formats/fdd/fdd000421.shtml, ive been using cellsize for the chunksize in line coordinate system
    

    public override bool ReadHeightData(string pFileName, int pTextureIndex, out Chunk[,] pChunkGrid, ref Settings pSettings)
    {
        pChunkGrid = new Chunk[0, 0];

        int numRowsCols;
        int mapSize, cellSize;
        float mapScale;
        float[,] chunkedData;

        try
        {
            var nrows = int.Parse(File.ReadLines(pFileName).Take(1).ToList()[0].Split(' ')[1]);
            var ncols = int.Parse(File.ReadLines(pFileName).Skip(1).Take(1).ToList()[0].Split(' ')[1]);

            // Square the map if uneven
            if (nrows != ncols)
                numRowsCols = nrows < ncols ? nrows : ncols;
            else
                numRowsCols = nrows;

            // Need to add defaults for event of no header data
            cellSize = int.Parse(File.ReadLines(pFileName).Skip(4).Take(1).ToList()[0].Split(' ')[1]);
            mapScale = pSettings.MapScale;
            mapSize = numRowsCols / cellSize;

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
        pSettings.MapSize = mapSize;
        pSettings.ChunkSize = cellSize;
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
        sw.WriteLine($"nodata_value {pSeed}");
        
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
                sw.Write($"{cumulativeData[i, j]} ");
        }
    }
}