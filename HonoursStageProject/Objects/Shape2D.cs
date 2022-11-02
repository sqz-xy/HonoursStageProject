
using OpenTK;

namespace HonoursStageProject.Objects;

public abstract class Shape2D
{
    public float[] Vertices;
    public uint[] Indices;
    public Vector2 Position;
    public Vector4 Colour;
    public int Index;
    
    public static bool CheckSquareIntersection(Quadrilateral2D pQuadrilateral2D, Vector2 pPosition)
    {
        return (pPosition.X >= -pQuadrilateral2D.Width + pQuadrilateral2D.Position.X && pPosition.X <= pQuadrilateral2D.Width + pQuadrilateral2D.Position.X) &&
               (pPosition.Y >= -pQuadrilateral2D.Height + pQuadrilateral2D.Position.Y && pPosition.Y <= pQuadrilateral2D.Height + pQuadrilateral2D.Position.Y);
    }
}

