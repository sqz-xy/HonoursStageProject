
using OpenTK;
using OpenTK.Graphics.ES11;

namespace HonoursStageProject.Objects;

public abstract class Object : IObject
{
    public float[] Vertices;
    public uint[] Indices;
    public Vector3 Position;
    public Vector4 BaseColour;
    public int BufferIndex;
    public int TextureIndex;
    public PrimitiveType PrimitiveType;

    public abstract void BufferData();
    
    public abstract void Render(int pShaderHandle);

    public abstract void Update(int pShaderHandle);

    /// <summary>
    /// Checks if a point intersects with a square
    /// </summary>
    /// <param name="pSquare">The square to check</param>
    /// <param name="pPosition">The position to check</param>
    /// <returns>A bool representing if the collision has occured</returns>
    public static bool CheckSquareIntersection(Quadrilateral pSquare, Vector2 pPosition)
    {
        return (pPosition.X >= -pSquare.Width + pSquare.Position.X && pPosition.X <= pSquare.Width + pSquare.Position.X) &&
               (pPosition.Y >= -pSquare.Height + pSquare.Position.Y && pPosition.Y <= pSquare.Height + pSquare.Position.Y);
    }
    
}

