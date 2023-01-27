using HonoursStageProject.Algorithms;
using HonoursStageProject.Objects;
using OpenTK;

namespace HonoursStageProject.Managers;

public class ChunkManager
{
    private List<Chunk> _renderableChunks;
    private List<Chunk> _unRenderableChunks;
    private Chunk _sourceChunk;
    private int _textureIndex;
    private Chunk[,] _chunkGrid;
    private Vector2[] _directions = 
    {
        // Clockwise, Can add diagonals if needed
        new Vector2(0, -1), // UP
        new Vector2(1, 0),  // RIGHT
        new Vector2(0, 1),  // DOWN
        new Vector2(-1, 0)  // LEFT
    };
    
    // Texture Initialization
    

    public ChunkManager()
    {
        _renderableChunks = new();
        _unRenderableChunks = new();
        _textureIndex = TextureManager.BindTextureData("Textures/button.png");
    }

    public void GenerateMap(int pMapSize, int pChunkSize, float pMapScale)
    {
        // Populate the grid of chunks
        _chunkGrid = new Chunk[pMapSize, pMapSize];
        
        // is used to make sure the chunks are centred correctly
        var centreOffset = (pChunkSize / 2); 
        
        for (var i = 0; i < pMapSize; i++)
        for (var j = 0; j < pMapSize; j++)
        {
            // Make sure the chunks are offset correctly so the middle of the chunk map is 0,0
            var xOffset = -(pMapSize * pChunkSize) / 2;
            var yOffset = -(pMapSize * pChunkSize) / 2;

            _chunkGrid[i, j] =
                new Chunk(
                    new Vector3(xOffset + (i * pChunkSize) + centreOffset, -2, yOffset + (j * pChunkSize) + centreOffset),
                    pChunkSize, pMapScale, 
                    new Vector2(i, j), _textureIndex);
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
        float[,] heightData = ds.GenerateData(2, _sourceChunk.Scale, 0.5f);
        _sourceChunk.AddHeightData(heightData);
        
        // Traverse the linked grid
        var downNode = _sourceChunk;

        while (downNode != null)
        {
            var rightNode = downNode;

            while (rightNode != null)
            {
                // Loop through adjacents, create an empty 2d array of chunksize and populate the sides with edge values of adjacents (Clockwise) 
                // Fix Corners by taking an avg and make sure the terrain stitches together properly.
                // Fix terrain stitching not working correctly
                float[,] heightValues = new float[rightNode.HeightData.GetLength(0), rightNode.HeightData.GetLength(1)];
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
                    heightData = ds.GenerateData(rnd.Next(), rightNode.Scale, 0.5f, heightValues);
                    rightNode.AddHeightData(heightData);
                }
                rightNode = rightNode.Adjacents[1];
            }
            downNode = downNode.Adjacents[2];
        }
    }
    
    
    public void RenderMap(int pShaderHandle)
    {
        foreach (var chunk in _chunkGrid)
        {
            // Can't be multithreaded because the binding indexes increment
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