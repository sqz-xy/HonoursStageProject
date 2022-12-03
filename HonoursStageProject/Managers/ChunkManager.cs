using HonoursStageProject.Objects;
using OpenTK;

namespace HonoursStageProject.Managers;

public class ChunkManager
{
    private List<TerrainMesh> _renderableChunks;
    private List<TerrainMesh> _unRenderableChunks;
    private int _textureIndex;
    
    // Texture Initialization
    

    public ChunkManager()
    {
        _renderableChunks = new();
        _unRenderableChunks = new();
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
            
            TerrainMesh chunk = new TerrainMesh(new Vector3(xPos, -2, zPos), pChunkSize, pMapScale);
            chunk.TextureIndex = _textureIndex;
            _renderableChunks.Add(chunk);
            xPos += pChunkSize * pMapScale;
        }

        foreach (var chunk in _renderableChunks)
        {
           chunk.BufferData();
        }
    }

    public void RenderMap(int pShaderHandle)
    {
        foreach (var chunk in _renderableChunks)
            chunk.Render(pShaderHandle);
    }
}