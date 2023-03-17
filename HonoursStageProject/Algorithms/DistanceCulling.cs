using System.Numerics;
using HonoursStageProject.Objects;

namespace HonoursStageProject.Algorithms;

public class DistanceCulling : ICulling
{
    public float RenderDistance { get; set; }
    public float ChunkSize { get; set; }
    
    public DistanceCulling(float pRenderDistance, float pChunkSize)
    {
        RenderDistance = pRenderDistance;
        ChunkSize = pChunkSize;
    }

    public DistanceCulling()
    {
        
    }
    
    public bool Cull(Chunk pChunk, Camera pCamera, Settings pSettings)
    {
        float renderDistance = pSettings.ChunkSize * pSettings.RenderDistance; // Multiply so the input is in chunks
        var cam = (float) (Math.Pow((pCamera.Position.X - pChunk.Position.X), 2));
        var chunk = (float) (Math.Pow((pCamera.Position.Z - pChunk.Position.Z), 2));
        var sum = cam + chunk;

        // Multiply renderdistance by itself instead, its much faster
        return sum < renderDistance * renderDistance;
    }
}