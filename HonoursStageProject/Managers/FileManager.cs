using HonoursStageProject.Objects;

namespace HonoursStageProject.Managers;

public abstract class FileManager
{
    public abstract bool ReadHeightData(string pFileName, int pTextureIndex, out Chunk[,] pChunkGrid);

    public abstract void SaveHeightData(string pFileName, int pMapSize, float pF, int pSeed, Chunk pSourceChunk);

    public Settings LoadSettings()
    {
        Settings settings = new Settings();
        Random rnd = new Random();

        string[] lines = File.ReadAllLines("Resources/Settings.txt");

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
            }
        }
        
        return settings;
    }
}