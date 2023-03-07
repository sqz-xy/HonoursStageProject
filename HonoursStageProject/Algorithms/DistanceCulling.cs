using System.Numerics;
using HonoursStageProject.Objects;

namespace HonoursStageProject.Algorithms;

public class DistanceCulling : ICulling
{
    private float _renderDistance;
    private float _chunkSize;
    
    public DistanceCulling(float pRenderDistance, float pChunkSize)
    {
        _renderDistance = pRenderDistance;
        _chunkSize = pChunkSize;
    }
    
    public bool Cull(Chunk pChunk, Camera pCamera)
    {
        float renderDistance = _chunkSize * _renderDistance; // Multiply so the input is in chunks
        var cam = (float) (Math.Pow((pCamera.Position.X - pChunk.Position.X), 2));
        var chunk = (float) (Math.Pow((pCamera.Position.Z - pChunk.Position.Z), 2));
        var sum = cam + chunk;

        // Multiply renderdistance by itself instead, its much faster
        if (sum < renderDistance * renderDistance)
            return true;

        return false;
    }
}