namespace HonoursStageProject.Objects;

public interface IObject
{
    void Render(int pShaderHandle);

    void Update(int pShaderHandle);
}