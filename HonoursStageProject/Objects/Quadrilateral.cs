using OpenTK;

namespace HonoursStageProject.Objects;

public class Quadrilateral : Shape
{
    public float Width;
    public float Height;

    public Quadrilateral(float pWidth, float pHeight, Vector4 pColour)
    {
        Width = pWidth;
        Height = pHeight;
        Colour = pColour;
        
        Vertices = new float[]
        {
            pWidth, pHeight, 0.0f,
            pWidth, -pHeight, 0.0f, 
            -pWidth, -pHeight, 0.0f,
            -pWidth, pHeight, 0.0f
        };

        Indices = new uint[]
        {
            0, 1, 3, 
            1, 2, 3
        };
    }
}