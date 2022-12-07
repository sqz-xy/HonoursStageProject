using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace HonoursStageProject.Objects;

public interface IObject
{
    void BufferData();

    void BufferData(int pBufferTarget);
    
    void Render(int pShaderHandle);

    void Update(int pShaderHandle);
}