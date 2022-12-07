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
        float xPos = -pMapSize / 2, zPos = pMapSize;

        for (int i = 0; i < pMapSize * pMapSize; i++)
        {
            if (i % pMapSize == 0 && i != 0)
            {
                zPos += pChunkSize * pMapScale;
                xPos = -pMapSize / 2;
            }
            
            var chunk = new Chunk(new Vector3(xPos, -2, zPos), pChunkSize, pMapScale)
            {
                TextureIndex = _textureIndex
            };
            _chunks.Add(chunk);
            xPos += pChunkSize * pMapScale;
        }

        _sourceChunk = _chunks[0];

        GenChunkHeightData(pMapSize);
        
        foreach (var chunk in _chunks)
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
        foreach (var chunk in _chunks)
        {
            chunk.Render(pShaderHandle);
        }
    }
}