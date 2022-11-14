using HonoursStageProject.Managers;
using HonoursStageProject.Objects;
using HonoursStageProject.Shaders;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace HonoursStageProject.Scenes;

/// <summary>
/// The main menu scene
/// </summary>
public sealed class MainMenuScene : Scene
{
    private Shader _shader;
    
    private Quadrilateral _button;
    private int _buttonTextureIndex;

    public MainMenuScene(SceneManager pSceneManager) : base(pSceneManager)
    {
        pSceneManager.Title = "Main Menu";

        pSceneManager.Renderer = Render;
        pSceneManager.Updater = Update;
        pSceneManager.MouseMoveEvent += MouseMovement;
        pSceneManager.MouseClickEvent += MouseClick;
        
        Initialize();
    }
    
    public override void Initialize()
    {
        // Initialise variables
        VertexManager.Initialize(1, 1, 1);
        TextureManager.Initialize(1);
        
        _shader = new Shader(@"Shaders/mainmenu.vert", @"Shaders/mainmenu.frag");

        _button = new Quadrilateral(new Vector2(0.0f, 0.0f), 0.2f, 0.1f, new Vector4(0.1f, 0.1f, 0.1f, 0.0f));

        _button.Index = VertexManager.BindVertexData(_button.Vertices, _button.Indices, true);
        _buttonTextureIndex = TextureManager.BindTextureData("Textures/button.png");
        
        // Shader stuff
        _shader.UseShader();
        
        UpdateValues();
    }

    /// <summary>
    /// Updates shader values
    /// </summary>
    private void UpdateValues()
    {
        var vertexColorLocation = GL.GetUniformLocation(_shader.Handle, "uColour");
        GL.Uniform4(vertexColorLocation, _button.Colour);

        var uTexLocation1 = GL.GetUniformLocation(_shader.Handle, "uTextureSampler1");
        GL.Uniform1(uTexLocation1, _buttonTextureIndex);
    }

    public override void Render(FrameEventArgs pE)
    {
        //Console.WriteLine("Rendering");
        
        _shader.UseShader();
        
        GL.BindVertexArray(VertexManager.GetVaoAtIndex(_button.Index));
        GL.DrawElements((PrimitiveType) _button.PrimitiveType, _button.Indices.Length, DrawElementsType.UnsignedInt, 0);
    }

    /// <summary>
    /// Logic for the mouse movement, currently checks if  the mouse intersects with a button
    /// </summary>
    /// <param name="pMouseEventArgs">The mouse event arguments</param>
    private void MouseMovement(MouseEventArgs pMouseEventArgs)
    {
        var mousePos = new Vector2((float) (-1.0 + 2.0 * pMouseEventArgs.X / SceneManager.Width),
            (float) (1.0 - 2.0 * pMouseEventArgs.Y / SceneManager.Height));

        if (Shape.CheckSquareIntersection(_button, mousePos))
            _button.Colour = new Vector4(1.0f, 0.0f, 0.0f, 0.0f);
        else
            _button.Colour = new Vector4(0.1f, 0.1f, 0.1f, 0.0f);
    }

    /// <summary>
    /// Logic for the mouse clicking, checks if the button on the screen is clicked
    /// </summary>
    /// <param name="pMouseEventArgs">The mouse event arguments</param>
    private void MouseClick(MouseEventArgs pMouseEventArgs)
    {
        var mousePos = new Vector2((float) (-1.0 + 2.0 * pMouseEventArgs.X / SceneManager.Width),
            (float) (1.0 - 2.0 * pMouseEventArgs.Y / SceneManager.Height));

        if (Shape.CheckSquareIntersection(_button, mousePos))
            SceneManager.ChangeScene(SceneTypes.SceneTerrain);
    }
    
    public override void Update(FrameEventArgs pE)
    {
        UpdateValues();
    } 
    
    public override void Close()
    {
        VertexManager.ClearData();
        TextureManager.ClearData();
    }
}