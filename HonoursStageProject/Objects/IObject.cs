namespace HonoursStageProject.Objects;

public interface IObject
{
    void BufferData();
    
    void Render(int pShaderHandle);

    void Update(int pShaderHandle);
}