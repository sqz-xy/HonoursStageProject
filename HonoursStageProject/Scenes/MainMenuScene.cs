using HonoursStageProject.Managers;
using HonoursStageProject.Objects;
using HonoursStageProject.Shaders;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Object = HonoursStageProject.Objects.Object;

// Not using imgui

namespace HonoursStageProject.Scenes;

/// <summary>
/// The main menu scene
/// </summary>
public sealed class MainMenuScene : Scene
{
    private Shader _shader;
    private Quadrilateral _button;
    private Quadrilateral _background;

    public MainMenuScene(SceneManager pSceneManager) : base(pSceneManager)
    {
        pSceneManager.Title = "Welcome";
        
        pSceneManager.MouseMoveEvent += MouseMovement;
        pSceneManager.MouseClickEvent += MouseClick;
        pSceneManager.CursorVisible = true;
        pSceneManager.CursorGrabbed = false;

        Initialize();
    }
    
    public override void Initialize()
    {
        // Initialise variables, Initialize based on number of objects/shaders
        VertexManager.Initialize(2);
        TextureManager.Initialize(2);
        
        _shader = new Shader(@"Shaders/mainmenu.vert", @"Shaders/mainmenu.frag");

        _button = new Quadrilateral(new Vector3(0.0f, 0.0f, 0.0f), 0.2f, 0.1f, new Vector4(0.1f, 0.1f, 0.1f, 0.0f), "Textures/button.png");
        _button.BufferData();
        
        _background = new Quadrilateral(new Vector3(0.0f, 0.0f, 0.0f), 1, 1, new Vector4(0.0f, 0.0f, 0.0f, 0.0f), "Textures/backgroundtex.png"); 
        _background.BufferData();
    }
    
    public override void Render(FrameEventArgs pE)
    {
        //Console.WriteLine("Rendering");
        GL.ClearColor(0.0f, 0.25f, 0.25f, 1.0f);
        _shader.UseShader();
        _background.Render(_shader.Handle);
        _button.Render(_shader.Handle);
    }
    
    public override void Update(FrameEventArgs pE)
    {
        var input = Keyboard.GetState();
        
        if (input.IsKeyDown(Key.Escape))
            _sceneManager.Exit();
    } 
    
    /// <summary>
    /// Logic for the mouse movement, currently checks if  the mouse intersects with a button
    /// </summary>
    /// <param name="pMouseEventArgs">The mouse event arguments</param>
    private void MouseMovement(MouseEventArgs pMouseEventArgs)
    {
        var mousePos = new Vector2((float) (-1.0 + 2.0 * pMouseEventArgs.X / _sceneManager.Width),
            (float) (1.0 - 2.0 * pMouseEventArgs.Y / _sceneManager.Height));

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
        var mousePos = new Vector2((float) (-1.0 + 2.0 * pMouseEventArgs.X / _sceneManager.Width),
            (float) (1.0 - 2.0 * pMouseEventArgs.Y / _sceneManager.Height));

        if (Object.CheckSquareIntersection(_button, mousePos))
            _sceneManager.ChangeScene(SceneTypes.SceneTerrain);
    }
    
    public override void Close()
    {
        VertexManager.ClearData();
        TextureManager.ClearData();
    }
}