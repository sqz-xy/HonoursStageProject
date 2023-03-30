using HonoursStageProject.Algorithms;
using HonoursStageProject.Objects;
using OpenTK;

namespace HonoursStageProject.Managers;

public class ChunkManager
{
    private Chunk _sourceChunk;
    private readonly int _textureIndex;
    private Chunk[,] _chunkGrid;
    private Settings _settings;
    
    private readonly FileManager _fileManager;
    
    private readonly Vector2[] _directions = 
    {
        // Clockwise, Can add diagonals if needed
        new(0, -1), // UP
        new(1, 0),  // RIGHT
        new(0, 1),  // DOWN
        new(-1, 0)  // LEFT
    };
    
    

    public ChunkManager(bool pBindTexture)
    {
        if (pBindTexture)
            _textureIndex = TextureManager.BindTextureData("Textures/button.png");
        
        _fileManager = new AscFileManager();
    }

    public void GenerateMap(Settings pSettings)
    {
        // Populate the grid of chunks
        _chunkGrid = new Chunk[pSettings.MapSize, pSettings.MapSize];
        _settings = pSettings;

        // * 0.5f
        
        // is used to make sure the chunks are centred correctly
        var centreOffset = (_settings.ChunkSize / 2) * _settings.MapScale;

        var chunkID = 0;
        if (pSettings.FileName == string.Empty)
        {
            for (var i = 0; i < _settings.MapSize; i++)
            for (var j = 0; j < _settings.MapSize; j++)
            {
                var t = new Thread(() =>
                {
                    // Make sure the chunks are offset correctly so the middle of the chunk map is 0,0
                    var xOffset = -((_settings.MapSize * _settings.ChunkSize) / 2) * _settings.MapScale;
                    var yOffset = -((_settings.MapSize * _settings.ChunkSize) / 2) * _settings.MapScale;

                    _chunkGrid[i, j] =
                        new Chunk(
                            new Vector3(xOffset + ((j * ((_settings.ChunkSize * _settings.MapScale) - 1)) + centreOffset), 0, yOffset + (i * ((_settings.ChunkSize * _settings.MapScale) - 1)) + centreOffset),
                            _settings.ChunkSize, _settings.MapScale, 
                            new Vector2(j, i), _textureIndex);
                    _chunkGrid[i, j].ID = chunkID++;
                });
                t.Start();
                t.Join();
            } 
        }
        else
        {
            var success = _fileManager.ReadHeightData(pSettings.FileName, _textureIndex, out _chunkGrid, ref _settings);
            
            // Set the data again if it has been changed by the loaded file

            if (!success)
            {
                // Recursive call if file not read successfully
                Console.WriteLine("File Not Read!");
                pSettings.FileName = "";
                GenerateMap(pSettings);
            }
        }
        
        // Construct linked grid
        for (var i = 0; i < _settings.MapSize; i++)
        for (var j = 0; j < _settings.MapSize; j++)
        {
            // Assign current node
            var currentChunk = _chunkGrid[i, j];

            for (var y = 0; y < _directions.Length; y++)
            {
                // Add the direction offset to the current array position
                 var xOffset = i + (int)_directions[y].X;
                 var yOffset = j + (int)_directions[y].Y;

                // Bounds checking
                if (xOffset >= _settings.MapSize || yOffset >= _settings.MapSize || xOffset < 0 || yOffset < 0)
                {
                    currentChunk.Adjacents[y] = null;
                    continue;
                }

                currentChunk.Adjacents[y] = _chunkGrid[xOffset, yOffset];
            }
        }
        
        _sourceChunk = _chunkGrid[0, 0];

        // This now needs to use the adjacents to populate the height data
        if (pSettings.FileName == string.Empty)
            GenChunkHeightData(_settings.MapSize);
        
        
    }

    public void BufferMap()
    {
        // Buffer the chunk data
        foreach (var chunk in _chunkGrid)
            chunk.BufferData();
    }

    public void RegenerateMap()
    {
        GenChunkHeightData(_settings.MapSize);
        
        // Buffer the chunk data
        foreach (var chunk in _chunkGrid)
        {
            // Can't be multithreaded because the binding indexes increment
            //chunk.PrintHeightData();
            chunk.BufferData(chunk.BufferIndex);
        }
    }

    private void GenChunkHeightData(int pMapSize)
    {
        /*
         * Chunk hold reference to adjacents
         * Populate data array with the edges of adjacents
         * Then run the algorithm
         */
        
        // Seed source chunk
        //var ds = new DiamondSquare(_sourceChunk.Size);

        // Traverse the linked grid
        var downNode = _sourceChunk;

        while (downNode != null)
        {
            var rightNode = downNode;

            while (rightNode != null)
            {
                var t = new Thread(() =>
                {
                    // Loop through adjacents, create an empty 2d array of chunksize and populate the sides with edge values of adjacents (Clockwise) 
                    // Fix Corners by taking an avg and make sure the terrain stitches together properly.
                
                    float[,] heightValues = new float[rightNode.HeightData.GetLength(0), rightNode.HeightData.GetLength(1)];

                    // Initial pass for seeding data
                    heightValues = MatchSides(rightNode, heightValues);
                    Random rnd = new Random();

                    float[,] heightData;
                
                    if (rightNode == _sourceChunk)
                        heightData = _settings.TerrainAlgorithm.GenerateData(rnd.Next(), rightNode.Scale, 0.5f, heightValues, true);
                    else
                        heightData = _settings.TerrainAlgorithm.GenerateData(rnd.Next(), rightNode.Scale, 0.5f, heightValues, false);
                
                    rightNode.AddHeightData(heightData);
                });
                t.Start();
                t.Join();
                rightNode = rightNode.Adjacents[1];
            }
            downNode = downNode.Adjacents[2];
        }
        // Second size matching pass
        
        MatchSides();
        
        //TODO: Chunks arent placed correctly im pretty sure causing the matching issue
    }

    private void MatchSides()
    {
        var downNode = _sourceChunk;

        while (downNode != null)
        {
            var rightNode = downNode;

            while (rightNode != null)
            {
                var t = new Thread(() =>
                {
                    rightNode.AddHeightData(MatchSides(rightNode, rightNode.HeightData));
                });
                t.Start();
                t.Join();
                
                rightNode = rightNode.Adjacents[1];
            }
            downNode = downNode.Adjacents[2];
        }
    }

    private float[,] MatchSides(Chunk rightNode, float[,] heightValues)
    {
        for (int i = 0; i < rightNode.Adjacents.Length; i++)
        {
            // The bug is related to this, the 0s dont appear if my diamond square runs without pre seed, next thing to fix is other 0s
            float[] row, col;

            switch (i)
            {
                case 0: // UP
                    if (rightNode.Adjacents[0] == null)
                    {
                        break;
                    }

                    row = GetRow(rightNode.Adjacents[0].HeightData, rightNode.Adjacents[0].HeightData.GetLength(0) - 1);
                    SetRow(heightValues, 0, row);
                    break;
                case 1: // RIGHT
                    if (rightNode.Adjacents[1] == null)
                    {
                        break;
                    }

                    col = GetCol(rightNode.Adjacents[1].HeightData, 0);
                    SetCol(heightValues, heightValues.GetLength(1) - 1, col);
                    break;
                case 2: // DOWN
                    if (rightNode.Adjacents[2] == null)
                    {
                        break;
                    }

                    row = GetRow(rightNode.Adjacents[2].HeightData, 0);
                    SetRow(heightValues, heightValues.GetLength(0) - 1, row);
                    break;
                case 3: // LEFT
                    if (rightNode.Adjacents[3] == null)
                    {
                        break;
                    }

                    col = GetCol(rightNode.Adjacents[3].HeightData, rightNode.Adjacents[3].HeightData.GetLength(1) - 1);
                    SetCol(heightValues, 0, col);
                    break;
            }
        }

        return heightValues;
    }

    public void SaveData(string pFileName)
    {
        new Thread(() =>
        {
            _fileManager.SaveHeightData(pFileName, _settings.MapSize, _settings.MapScale, _settings.Seed, _sourceChunk);
        }).Start();
    }
    
    
    public void RenderMap(int pShaderHandle, Camera pCamera)
    {
        foreach (var chunk in _chunkGrid)
        {
            if (_settings.CullingAlgorithms.Count > 0)
            {
                // true means in the view
                bool renderChunk = true;
                foreach (var culling in _settings.CullingAlgorithms)
                    renderChunk &= culling.Cull(chunk, pCamera, _settings);

                if (renderChunk)
                    chunk.Render(pShaderHandle);
            }
            else
                chunk.Render(pShaderHandle);
        }
    }

    public void ScaleChunkHeight(float pScale)
    {
        var downNode = _sourceChunk;
        var threads = new List<Thread>();
        
        while (downNode != null)
        {
            var rightNode = downNode;

            while (rightNode != null)
            {
                var node = rightNode;
                var t = new Thread(() =>
                {
                    for (var i = 0;  i < node.HeightData.GetLength(0); i++)
                    for (int j = 0;  j < node.HeightData.GetLength(1); j++)
                    {
                        node.HeightData[i, j] *= pScale;
                        node.AddHeightData(node.HeightData);
                        
                    }
                });
                t.Start();
                threads.Add(t);
                
                rightNode.BufferData(rightNode.BufferIndex);
                rightNode = rightNode.Adjacents[1];
            }
            downNode = downNode.Adjacents[2];

            foreach (var thread in threads)
                thread.Join();
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

    public Chunk GetSourceChunk()
    {
        return _sourceChunk;
    }
}