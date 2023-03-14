using HonoursStageProject.Algorithms;
using HonoursStageProject.Objects;

namespace HonoursStageProject.Managers;

public abstract class FileManager
{
    public abstract bool ReadHeightData(string pFileName, int pTextureIndex, out Chunk[,] pChunkGrid);

    public abstract void SaveHeightData(string pFileName, int pMapSize, float pF, int pSeed, Chunk pSourceChunk);

    public Settings LoadSettings()
    {
        //TODO: Load in algorithm types, move file manager into scene manager as singleton
        Settings settings = new Settings();
        Random rnd = new Random();

        string[] lines = File.ReadAllLines("Resources/Settings.txt");
        const string namespaceName = "HonoursStageProject.Algorithms.";

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
                case "terrain_algorithms":
                    
                    // Parse terrain algorithms
                    var terrainAlgorithms = value.Split();
                    foreach (var algorithm in terrainAlgorithms)
                    {
                        //var alg = Activator.CreateInstance(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, namespaceName + algorithm) as IAlgorithm;
                        //var algAsClass = alg as Algorithm;
                        //algAsClass.Size = settings.ChunkSize;
                        //settings.TerrainAlgorithms.Add(algAsClass);
                    }
                       
                    break;
                case "culling_algorithms" :
                    
                    // Parse terrain algorithms
                    var cullingAlgorithms = value.Split();  
                    foreach (var algorithm in cullingAlgorithms)
                    {
                        
                    }
                    
                    break;
                default:
                    break;
            }
        }
        
        return settings;
    }
}