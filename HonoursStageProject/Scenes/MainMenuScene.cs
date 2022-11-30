using HonoursStageProject.Managers;
using HonoursStageProject.Objects;
using HonoursStageProject.Shaders;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Object = HonoursStageProject.Objects.Object;

namespace HonoursStageProject.Scenes;

/// <summary>
/// The main menu scene
/// </summary>
public sealed class MainMenuScene : Scene
{
    private Shader _shader;
    private Quadrilateral _button;

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
        // Initialise variables, Initialize based on number of objects/shaders
        VertexManager.Initialize(1);
        TextureManager.Initialize(1);
        
        _shader = new Shader(@"Shaders/mainmenu.vert", @"Shaders/mainmenu.frag");

        _button = new Quadrilateral(new Vector3(0.0f, 0.25f, 0.0f), 0.2f, 0.1f, new Vector4(0.1f, 0.1f, 0.1f, 0.0f));
        _button.BufferData();
    }
    
    public override void Render(FrameEventArgs pE)
    {
        //Console.WriteLine("Rendering");
        
        _shader.UseShader();
        _button.Render(_shader.Handle);
    }
    
    public override void Update(FrameEventArgs pE)
    {
     
    } 
    
    /// <summary>
    /// Logic for the mouse movement, currently checks if  the mouse intersects with a button
    /// </summary>
    /// <param name="pMouseEventArgs">The mouse event arguments</param>
    private void MouseMovement(MouseEventArgs pMouseEventArgs)
    {
        var mousePos = new Vector2((float) (-1.0 + 2.0 * pMouseEventArgs.X / SceneManager.Width),
            (float) (1.0 - 2.0 * pMouseEventArgs.Y / SceneManager.Height));

        if (Object.CheckSquareIntersection(_button, mousePos))
            _button.BaseColour = new Vector4(1.0f, 0.0f, 0.0f, 0.0f);
        else
            _button.BaseColour = new Vector4(0.1f, 0.1f, 0.1f, 0.0f);
    }

    /// <summary>
    /// Logic for the mouse clicking, checks if the button on the screen is clicked
    /// </summary>
    /// <param name="pMouseEventArgs">The mouse event arguments</param>
    private void MouseClick(MouseEventArgs pMouseEventArgs)
    {
        var mousePos = new Vector2((float) (-1.0 + 2.0 * pMouseEventArgs.X / SceneManager.Width),
            (float) (1.0 - 2.0 * pMouseEventArgs.Y / SceneManager.Height));

        if (Object.CheckSquareIntersection(_button, mousePos))
            SceneManager.ChangeScene(SceneTypes.SceneTerrain);
    }
    
    public override void Close()
    {
        VertexManager.ClearData();
        TextureManager.ClearData();
    }
}