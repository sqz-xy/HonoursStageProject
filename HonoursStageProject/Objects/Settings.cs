﻿using HonoursStageProject.Algorithms;

namespace HonoursStageProject.Objects;

public struct Settings
{
    public int MapSize;
    public int ChunkSize;
    public float MapScale;
    public float Roughness;
    public int Seed;
    public String FileName;
    public float RenderDistance;
    public IAlgorithm TerrainAlgorithm;
    public List<ICulling> CullingAlgorithms;
}