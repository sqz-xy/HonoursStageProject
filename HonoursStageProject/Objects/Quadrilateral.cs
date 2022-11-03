using OpenTK;
using OpenTK.Graphics.ES11;

namespace HonoursStageProject.Objects;

public class Quadrilateral : Shape
{
    public float Width;
    public float Height;

    public Quadrilateral(Vector2 pPosition, float pWidth, float pHeight, Vector4 pColour)
    {
        PrimitiveType = PrimitiveType.Triangles;
            
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