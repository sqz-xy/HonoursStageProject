using HonoursStageProject.Managers;
using HonoursStageProject.Objects;
using HonoursStageProject.Shaders;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace HonoursStageProject.Objects;

public class Quadrilateral : Object
{
    public float Width;
    public float Height;

    public Quadrilateral(Vector3 pPosition, float pWidth, float pHeight, Vector4 pBaseColour, string pTexturePath)
    {
        PrimitiveType = OpenTK.Graphics.ES11.PrimitiveType.Triangles;
            
        Position = pPosition;
        Width = pWidth;
        Height = pHeight;
        BaseColour = pBaseColour;
        TexturePath = pTexturePath;

        Vertices = new float[]
        {
            pWidth + pPosition.X, pHeight + pPosition.Y, 0.0f, 0, 1, 0, 1.0f, 0.0f,
            pWidth + pPosition.X, -pHeight + pPosition.Y, 0.0f, 0, 1, 0, 1.0f, 1.0f, 
            -pWidth + pPosition.X, -pHeight + pPosition.Y, 0.0f, 0, 1 ,0, 0.0f, 1.0f,
            -pWidth + pPosition.X, pHeight + pPosition.Y, 0.0f, 0, 1, 0, 0.0f, 0.0f
        };

        Indices = new uint[]
        {
            0, 1, 3, 
            1, 2, 3
        };
    }

    public override void BufferData()
    {
        BufferIndex = VertexManager.BindVertexData(Vertices, Indices, true);
        TextureIndex = TextureManager.BindTextureData(TexturePath);
    }
    
    public override void BufferData(int pBufferTarget)
    {
        BufferIndex = VertexManager.BindVertexData(Vertices, Indices, true, pBufferTarget);
    }

    public sealed override void Render(int pShaderHandle)
    {
        Update(pShaderHandle);

        GL.BindVertexArray(VertexManager.GetVaoAtIndex(BufferIndex));
        GL.DrawElements((PrimitiveType) PrimitiveType, Indices.Length, DrawElementsType.UnsignedInt, 0);
    }

    public override void Update(int pShaderHandle)
    {
        var uModel = GL.GetUniformLocation(pShaderHandle, "uModel");
        var translation = Matrix4.CreateTranslation(Position);
        GL.UniformMatrix4(uModel, true, ref translation);
        
        var vertexColorLocation = GL.GetUniformLocation(pShaderHandle, "uColour");
        GL.Uniform4(vertexColorLocation, BaseColour);

        var uTextureSamplerLocation = GL.GetUniformLocation(pShaderHandle, "uTextureSampler1");
        GL.Uniform1(uTextureSamplerLocation, TextureIndex);

    }
}