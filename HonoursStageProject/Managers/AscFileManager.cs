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
            
            for (var i = 0; i < heightData.Count; ++i)
            {
                var line = heightData[i];
                for (var j = 0; j < chunkedData.GetLength(1); ++j)
                {
                    string[] split = line.Split(' ');
                    chunkedData[i, j] = float.Parse(split[j]);
                }
            }
        }
        catch (Exception e)
        {
            FileManager.LogMessage($"Error loading external height map {e.Message}");
            return false;
        }

        var chunkGrid = new Chunk[mapSize, mapSize];

        var centreOffset = cellSize / 2;
        var threads = new List<Thread>();
        
        // Create Chunks
        for (var i = 0; i < mapSize; i++)
        {
            for (var j = 0; j < mapSize; j++)
            {
                var xOffset = -(mapSize * cellSize) / 2;
                var yOffset = -(mapSize * cellSize) / 2;

                var i1 = i;
                var j1 = j;
                var t = new Thread(() =>
                {
                    // Make sure the chunks are offset correctly so the middle of the chunk map is 0,0
                    var xOffset = -((mapSize * cellSize) / 2) * mapScale;
                    var yOffset = -((mapSize * cellSize) / 2) * mapScale;

                    chunkGrid[i1, j1] =
                        new Chunk(
                            new Vector3(xOffset + ((i1 * ((cellSize * mapScale) - 1 * mapScale)) + centreOffset), 0, yOffset + (j1 * ((cellSize * mapScale) - 1 * mapScale)) + centreOffset),
                            cellSize, mapScale, 
                            new Vector2(i1, j1), pTextureIndex);
                });
                threads.Add(t);
                t.Start();
            }
        }
        
        foreach (var thread in threads)
            thread.Join();
        
        var chunkPointerX = 0;
        var chunkPointerY = 0;

        // Add height data
        for (int chunkIndexX = 0; chunkIndexX < chunkGrid.GetLength(0); chunkIndexX++)
        {
            for (int chunkIndexY = 0; chunkIndexY < chunkGrid.GetLength(1); chunkIndexY++)
            {
                for (int i = 0; i < cellSize; i++)
                {
                    for (int j = 0; j < cellSize; j++)
                    {
                        chunkGrid[chunkIndexX, chunkIndexY].HeightData[i, j] = chunkedData[i + chunkPointerX, j + chunkPointerY];
                    }
                }
                chunkGrid[chunkIndexX, chunkIndexY].AddHeightData(chunkGrid[chunkIndexX, chunkIndexY].HeightData);
                chunkPointerY += cellSize;
            }
            chunkPointerY = 0;
            chunkPointerX += cellSize;
        }
        
        pChunkGrid = chunkGrid;
        pSettings.MapSize = mapSize;
        pSettings.ChunkSize = cellSize;
        FileManager.LogMessage($"Data Loaded from: {pSettings.FileName}");
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

        FileManager.LogMessage($"Data saved at: {pFileName}");
    }
}