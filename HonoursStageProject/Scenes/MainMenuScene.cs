﻿using HonoursStageProject.Managers;
using HonoursStageProject.Objects;
using HonoursStageProject.Shaders;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace HonoursStageProject.Scenes;

public class MainMenuScene : Scene
{
    private readonly float[] _triangleVertices =
    {
        0.5f,  0.5f, 0.0f, // top right
        0.5f, -0.5f, 0.0f, // bottom right
        -0.5f, -0.5f, 0.0f, // bottom left
        -0.5f,  0.5f, 0.0f, // top left
    };
    
    private readonly uint[] _triangleIndices =
    {
        // Note that indices start at 0!
        0, 1, 3, // The first triangle will be the top-right half of the triangle
        1, 2, 3  // Then the second will be the bottom-left half of the triangle
    };
    
    private int _vbo;
    private int _vao;
    private int _ebo;
    private Shader _shader;

    private Quadrilateral _button;

    public MainMenuScene(SceneManager sceneManager) : base(sceneManager)
    {
        sceneManager.Title = "Main Menu";

        sceneManager.Renderer = Render;
        sceneManager.Updater = Update;
        sceneManager.MouseEvent += MouseMovement;
        Initialize();
    }

    public override void Initialize()
    {
        // Initialise variables
        _shader = new Shader(@"Shaders/vs.vert", @"Shaders/fs.frag");
        VertexManager.Initialize(1, 1, 1);
        
        _button = new Quadrilateral(0.5f, 0.9f, Vector4.One);
        _button.Colour = Vector4.One;
        _button.Index = VertexManager.BindVertexData(_button.Vertices, _button.Indices, 0, 0, 0);
        
        // Shader stuff
        _shader.UseShader();
        
    }

    public override void Render(FrameEventArgs e)
    {
        //Console.WriteLine("Rendering");
        
        _shader.UseShader();
        
        GL.BindVertexArray(VertexManager.GetVaoAtIndex(_button.Index));
        GL.DrawElements(PrimitiveType.Triangles, _button.Indices.Length, DrawElementsType.UnsignedInt, 0);
    }

    private void MouseMovement(MouseEventArgs e)
    {
        double normalizedX = -1.0 + 2.0 * (double)e.X / SceneManager.Width; 
        double normalizedY = 1.0 - 2.0 * (double)e.Y / SceneManager.Height; 
        Console.WriteLine($"{normalizedX} {normalizedY}");

        if ((normalizedX >= -_button.Width && normalizedX <= _button.Width) && (normalizedY >= -_button.Height && normalizedY <= _button.Height))
            _button.Colour = new Vector4(1.0f, 0.0f, 0.0f, 0.0f);
            //_sceneManager.ChangeScene(SceneTypes.SCENE_TERRAIN);
        else
            _button.Colour = Vector4.One;
    }
    
    public override void Update(FrameEventArgs e)
    {
        int vertexColorLocation = GL.GetUniformLocation(_shader.Handle, "uColour");
        GL.Uniform4(vertexColorLocation, _button.Colour);
    } 

    public override void Close()
    {
        VertexManager.ClearData();
    }
}