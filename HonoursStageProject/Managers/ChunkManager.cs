﻿using System.ComponentModel;
using System.Threading.Tasks.Dataflow;
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
        int centreOffset = pChunkSize / 2; 
        
        for (int i = 0; i < pMapSize; i++)
        for (int j = 0; j < pMapSize; j++)
        {
            // Make sure the chunks are offset correctly so the middle of the chunk map is 0,0
            var xOffset = -(pMapSize * pChunkSize) / 2;
            var yOffset = -(pMapSize * pChunkSize) / 2;
            
            _chunkGrid[i, j] = new Chunk(new Vector3(xOffset + (i * pChunkSize) + centreOffset, -2, yOffset + (j * pChunkSize) + centreOffset), pChunkSize, pMapScale)
            {
                TextureIndex = _textureIndex
            };
        }
        
        // Similar to what I did in advanced programming for the letter grid
        // Construct linked grid
        for (int i = 0; i < pMapSize; i++)
        for (int j = 0; j < pMapSize; j++)
        {
            // Assign current node
            var currentChunk = _chunkGrid[i, j];

            for (int y = 0; y < _directions.Length; y++)
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
        
        var ds = new DiamondSquare(_sourceChunk.Size);
        var heightData = ds.GenerateData(2, _sourceChunk.Scale, 0.5f);
        _sourceChunk.AddHeightData(heightData);
        
    }
    
    
    public void RenderMap(int pShaderHandle)
    {
        foreach (var chunk in _chunkGrid)
        {
            // Can't be multithreaded because the binding indexes increment
            chunk.Render(pShaderHandle);
        }
        
        // Adjacent testing
        
        
        _sourceChunk.Render(pShaderHandle);
        foreach (var chunk in _sourceChunk.Adjacents)
        {
            //if (chunk != null) 
                //chunk.Render(pShaderHandle);
        }
        
        var test2 = _sourceChunk.Adjacents[2];
        foreach (var chunk in test2.Adjacents)
        {
            //if (chunk != null) 
            //chunk.Render(pShaderHandle);
        }

        
    }
}