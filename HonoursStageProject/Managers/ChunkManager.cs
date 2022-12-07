using System.ComponentModel;
using System.Threading.Tasks.Dataflow;
using HonoursStageProject.Algorithms;
using HonoursStageProject.Objects;
using OpenTK;

namespace HonoursStageProject.Managers;

public class ChunkManager
{
    private List<Chunk> _renderableChunks;
    private List<Chunk> _unRenderableChunks;
    private List<Chunk> _chunks;
    private Chunk _sourceChunk;
    private int _textureIndex;
    private Chunk[,] _chunkGrid;
    
    // Texture Initialization
    

    public ChunkManager()
    {
        _renderableChunks = new();
        _unRenderableChunks = new();
        _chunks = new();
        _textureIndex = TextureManager.BindTextureData("Textures/button.png");
    }

    public void GenerateMap(int pMapSize, int pChunkSize, float pMapScale)
    {
        _chunkGrid = new Chunk[pMapSize, pMapSize];
        
        for (int i = 0; i < pMapSize; i++)
        for (int j = 0; j < pMapSize; j++)
        {
            _chunkGrid[i, j] = new Chunk(new Vector3(pChunkSize * i, -2, pChunkSize * j), pChunkSize, pMapScale);
        }
        
        _sourceChunk = _chunkGrid[0, 0];

        GenChunkHeightData(pMapSize);
        
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
         * 2D array of chunks, linked grid, currentNode, head, loop through directions, if the index is outside of the 2d array set it to null, don't need to do diagonals, yet??
         */
        
        var ds = new DiamondSquare(_sourceChunk.Size);
        var heightData = ds.GenerateData(1, _sourceChunk.Scale, 0.5f);
        _sourceChunk.AddHeightData(heightData);

        for (int i = 1; i < (pMapSize * pMapSize) - 1; i++)
        {
            // Previous method wont work because ill have to populate it with multiple sides
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
}