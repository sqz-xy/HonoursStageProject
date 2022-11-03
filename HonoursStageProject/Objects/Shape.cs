
using OpenTK;
using OpenTK.Graphics.ES11;

namespace HonoursStageProject.Objects;

public abstract class Shape
{
    public float[] Vertices;
    public uint[] Indices;
    public Vector2 Position;
    public Vector4 Colour;
    public int Index;
    public PrimitiveType PrimitiveType;
    
    public static bool CheckSquareIntersection(Quadrilateral pQuadrilateral, Vector2 pPosition)
    {
        return (pPosition.X >= -pQuadrilateral.Width + pQuadrilateral.Position.X && pPosition.X <= pQuadrilateral.Width + pQuadrilateral.Position.X) &&
               (pPosition.Y >= -pQuadrilateral.Height + pQuadrilateral.Position.Y && pPosition.Y <= pQuadrilateral.Height + pQuadrilateral.Position.Y);
    }
}

