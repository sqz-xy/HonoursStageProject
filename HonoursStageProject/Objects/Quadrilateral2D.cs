using OpenTK;

namespace HonoursStageProject.Objects;

public class Quadrilateral2D : Shape2D
{
    public float Width;
    public float Height;

    public Quadrilateral2D(Vector2 pPosition, float pWidth, float pHeight, Vector4 pColour)
    {
        Position = pPosition;
        Width = pWidth;
        Height = pHeight;
        Colour = pColour;

        Vertices = new float[]
        {
            pWidth + pPosition.X, pHeight + pPosition.Y, 0.0f,
            pWidth + pPosition.X, -pHeight + pPosition.Y, 0.0f, 
            -pWidth + pPosition.X, -pHeight + pPosition.Y, 0.0f,
            -pWidth + pPosition.X, pHeight + pPosition.Y, 0.0f
        };

        Indices = new uint[]
        {
            0, 1, 3, 
            1, 2, 3
        };
    }
    
}