using HonoursStageProject.Objects;

namespace HonoursStageProject.Managers;

public class AsciiFileManager : FileManager
{
    public override float[,] ReadHeightData(string pFileName)
    {
        throw new NotImplementedException();
        // Double nested loop with a chunkSize pointer
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
        sw.WriteLine($"nodata_value sd:{pSeed} mscale:{pMapScale} msize:{pMapSize}");
        
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