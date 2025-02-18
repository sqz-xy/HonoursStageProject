﻿using HonoursStageProject.Algorithms;
using HonoursStageProject.Objects;

namespace HonoursStageProject.Managers;

public abstract class FileManager
{
    public abstract bool ReadHeightData(string pFileName, int pTextureIndex, out Chunk[,] pChunkGrid, ref Settings pSettings);

    public abstract void SaveHeightData(string pFileName, int pMapSize, float pF, int pSeed, Chunk pSourceChunk);

    public static void LogMessage(string pMessage)
    {
        var sw = new StreamWriter("Log.txt", true); 
        sw.WriteLine($"{pMessage} - Event occured at: {DateTime.Now:HH:mm:ss tt}");
        Console.WriteLine($"Event Logged to logfile in output directory! - {pMessage}");
        sw.Close();
    }

    public Settings LoadSettings(string pFileName)
    {
        // parse settings file into settings obj
        Settings settings = new Settings();
        Random rnd = new Random();

        string[] lines = File.ReadAllLines(pFileName);
        const string namespaceName = "HonoursStageProject.Algorithms.";
        
        settings.CullingAlgorithms = new List<ICulling>();

        foreach (var line in lines)
        {
            // Handle comments and gaps
            if (line == "")
                continue;
            
            if (line[0] == '/')
                continue;
            
            var lineData = line.Split(':');
            var setting = lineData[0];
            var value = lineData[1];
            
            switch (setting)
            {
                case "map_size*":
                    settings.MapSize = int.Parse(value);
                    break;
                case "chunk_size*":
                    settings.ChunkSize = int.Parse(value);
                    break;
                case  "map_scale*":
                    settings.MapScale = float.Parse(value);
                    break;
                case "roughness":
                    settings.Roughness = float.Parse(value);
                    break;
                case  "render_distance":
                     settings.RenderDistance = float.Parse(value);
                    break;
                case  "seed":
                    
                    // Deal with null seed
                    if (value == "")
                    {
                        settings.Seed = rnd.Next();
                        break;
                    }
   
                    settings.Seed = int.Parse(value);
                    break;
                case "filename":
                    settings.FileName = value;
                    break;
                case "terrain_algorithm":
                    // Parse terrain algorithms
                    {
                        
                        try
                        {
                            var type = Type.GetType(namespaceName + value);
                            var alg = (Algorithm) Activator.CreateInstance(type);
                            alg.Size = settings.ChunkSize;
                            settings.TerrainAlgorithm = alg;
                        }
                        catch (Exception e)
                        {
                            FileManager.LogMessage("Valid terrain algorithm not present!");
                        }

                    }
                    break;
                case "culling_algorithms" :
                    // Parse terrain algorithms
                    var cullingAlgorithms = value.Split(' ');  
                    
                    if (value == String.Empty)
                        break;
                    
                    foreach (var algorithm in cullingAlgorithms)
                    {
                        var type = Type.GetType(namespaceName + algorithm);
                        var alg = Activator.CreateInstance(type);
                        settings.CullingAlgorithms.Add(alg as ICulling);
                    }
                    break;
                default:
                    break;
            }
        }
        
        return settings;
    }
}