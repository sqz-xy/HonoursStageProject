using HonoursStageProject.Algorithms;
using HonoursStageProject.Objects;
using OpenTK;

namespace HonoursStageProject.Managers;

public class ChunkManager
{
    private Chunk _sourceChunk;
    private int _textureIndex;
    private Chunk[,] _chunkGrid;
    private int _mapSize;
    private float _mapScale;
    private int _chunkSize;
    private int _seed;

    private FileManager _fileManager;
    
    private Vector2[] _directions = 
    {
        // Clockwise, Can add diagonals if needed
        new(0, -1), // UP
        new(1, 0),  // RIGHT
        new(0, 1),  // DOWN
        new(-1, 0)  // LEFT
    };
    
    

    public ChunkManager()
    {
        _textureIndex = TextureManager.BindTextureData("Textures/button.png");
        _fileManager = new AscFileManager();
    }

    public void GenerateMap(int pMapSize, int pChunkSize, float pMapScale, int pSeed, string pFileName)
    {
        // Populate the grid of chunks
        _chunkGrid = new Chunk[pMapSize, pMapSize];
        _mapScale = pMapScale;
        _mapSize = pMapSize;
        _chunkSize = pChunkSize;
        _seed = pSeed;
        
        // For diamond square
        if (_chunkSize % 2 == 0)
            _chunkSize++;
        
        // is used to make sure the chunks are centred correctly
        var centreOffset = (_chunkSize / 2);

        if (pFileName == string.Empty)
        {
            for (var i = 0; i < _mapSize; i++)
            for (var j = 0; j < _mapSize; j++)
            {
                // Make sure the chunks are offset correctly so the middle of the chunk map is 0,0
                var xOffset = -(_mapSize * _chunkSize) / 2;
                var yOffset = -(_mapSize * _chunkSize) / 2;

                _chunkGrid[i, j] =
                    new Chunk(
                        new Vector3(xOffset + (i * _chunkSize) + centreOffset, -2, yOffset + (j * _chunkSize) + centreOffset),
                        _chunkSize, _mapScale, 
                        new Vector2(i, j), _textureIndex);
            } 
        }
        else
        {
            var success = _fileManager.ReadHeightData(pFileName, _textureIndex, out _chunkGrid);
            if (!success)
            {
                // Recursive call if file not read successfully
                Console.WriteLine("File Not Read!");
                //GenerateMap(pMapSize, pChunkSize, pMapScale, pSeed, "");
            }
        }
        
        // Construct linked grid
        for (var i = 0; i < pMapSize; i++)
        for (var j = 0; j < pMapSize; j++)
        {
            // Assign current node
            var currentChunk = _chunkGrid[i, j];

            for (var y = 0; y < _directions.Length; y++)
            {
                // Add the direction offset to the current array position
                 var xOffset = i + (int)_directions[y].X;
                 var yOffset = j + (int)_directions[y].Y;

                // Bounds checking
                if (xOffset >= pMapSize || yOffset >= pMapSize || xOffset < 0 || yOffset < 0)
                {
                    currentChunk.Adjacents[y] = null;
                    continue;
                }

                currentChunk.Adjacents[y] = _chunkGrid[xOffset, yOffset];
            }
        }
        
        _sourceChunk = _chunkGrid[0, 0];

        // This now needs to use the adjacents to populate the height data
        if (pFileName == string.Empty)
            GenChunkHeightData(pMapSize);
        
        // Buffer the chunk data
        foreach (var chunk in _chunkGrid)
        {
            // Can't be multithreaded because the binding indexes increment
            chunk.BufferData();
        }
    }

    private void GenChunkHeightData(int pMapSize)
    {
        /* Chunk hold reference to adjacents
         * Populate data array with the edges of adjacents
         * Then run the algorithm
         */
        
        // Seed source chunk
        var ds = new DiamondSquare(_sourceChunk.Size);

        // Traverse the linked grid
        var downNode = _sourceChunk;

        while (downNode != null)
        {
            var rightNode = downNode;

            while (rightNode != null)
            {
                // Loop through adjacents, create an empty 2d array of chunksize and populate the sides with edge values of adjacents (Clockwise) 
                // Fix Corners by taking an avg and make sure the terrain stitches together properly.
                // Fix terrain stitching not working correctly, due to the fact that the adjacent height data is sometimes all 0s which means and those 0s will be acted on by the ds 
                float[,] heightValues = new float[rightNode.HeightData.GetLength(0), rightNode.HeightData.GetLength(1)];
                float[,] nullValues = new float[rightNode.HeightData.GetLength(0), rightNode.HeightData.GetLength(1)];
                
                float[] row, col;
                
                for (int i = 0; i < rightNode.Adjacents.Length; i++)
                {
                    switch (i)
                    {
                        case 0: // UP
                            if (rightNode.Adjacents[0] == null) {break;}
                            row = GetRow(rightNode.Adjacents[0].HeightData, rightNode.Adjacents[0].HeightData.GetLength(0) - 1);
                            SetRow(heightValues, 0, row);
                            break;
                        case 1: // RIGHT
                            if (rightNode.Adjacents[1] == null) {break;}
                            col = GetCol(rightNode.Adjacents[1].HeightData, 0);
                            SetCol(heightValues, heightValues.GetLength(1) - 1, col);
                            break;
                        case 2: // DOWN
                            if (rightNode.Adjacents[2] == null) {break;}
                            row = GetRow(rightNode.Adjacents[2].HeightData, 0);
                            SetRow(heightValues, heightValues.GetLength(0) - 1, row);
                            break;
                        case 3: // LEFT
                            if (rightNode.Adjacents[3] == null) {break;}
                            col = GetCol(rightNode.Adjacents[3].HeightData, rightNode.Adjacents[3].HeightData.GetLength(1) - 1);
                            SetCol(heightValues, 0, col);
                            break;
                    }
                    
                    Random rnd = new Random();
                    //heightData = ds.GenerateData(2, rightNode.Scale, 0.5f, heightValues);
                    var heightData = ds.GenerateData(rnd.Next(), rightNode.Scale, 0.5f, heightValues);
                    rightNode.AddHeightData(heightData);  
                    
                }
                rightNode = rightNode.Adjacents[1];
            }
            downNode = downNode.Adjacents[2];
        }
    }

    public void SaveData(string pFileName)
    {
        new Thread(() =>
        {
            _fileManager.SaveHeightData(pFileName, _mapSize, _mapScale, _seed, _sourceChunk);
        }).Start();
    }

    public bool DistanceCulling(Camera pCamera, Chunk pChunk, float pRenderDistance)
    {
        pRenderDistance *= _chunkSize; // Multiply so the input is in chunks
        var cam = (float) (Math.Pow((pCamera.Position.X - pChunk.Position.X), 2));
        var chunk = (float) (Math.Pow((pCamera.Position.Z - pChunk.Position.Z), 2));
        var sum = cam + chunk;

        if (Math.Sqrt(sum) < pRenderDistance)
            return true;

        return false;
    }
    
    
    public void RenderMap(int pShaderHandle, Camera pCamera, float pRenderDistance)
    {
        foreach (var chunk in _chunkGrid)
        {
           //if (DistanceCulling(pCamera, chunk, pRenderDistance))
                if (pCamera.ViewFrustum.IsPointIntersecting(chunk.Position))
                    chunk.Render(pShaderHandle);
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