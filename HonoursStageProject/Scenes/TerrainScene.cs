using HonoursStageProject.Managers;
using HonoursStageProject.Objects;
using HonoursStageProject.Shaders;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace HonoursStageProject.Scenes;

public class TerrainScene : Scene
{
    private Shader _shader;
    
    private Matrix4 _modelMatrix;
    private Matrix4 _viewMatrix;
    private Matrix4 _projectionMatrix;

    private Quadrilateral _square;

    // Camera
    public TerrainScene(SceneManager sceneManager) : base(sceneManager)
    {
        SceneManager.Title = "Terrain Scene";
        
        sceneManager.Renderer = Render;
        sceneManager.Updater = Update;
        sceneManager.MouseMoveEvent = null;
        sceneManager.MouseClickEvent = null;
        sceneManager.KeyPressEvent += KeyPress;
        
        Initialize();
    }

    public override void Initialize()
    {
        GL.ClearColor(0.0f, 1.0f, 0.0f, 1.0f);
        GL.Enable(EnableCap.DepthTest);
        GL.DepthMask(true);
        //GL.Enable(EnableCap.CullFace);
        //GL.CullFace(CullFaceMode.Back);

        VertexManager.Initialize(1, 1, 1);
        
        _shader = new Shader(@"Shaders/terrainscene.vert", @"Shaders/terrainscene.frag");
        _modelMatrix = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(0.0f));
        _viewMatrix = Matrix4.CreateTranslation(0.0f, 0.0f, -3.0f);
        _projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), SceneManager.Width / SceneManager.Height, 0.1f, 100.0f);
        
           
        _square = new Quadrilateral(new Vector2(0.0f, 0.0f),0.2f, 0.1f, Vector4.One)
        {
            Colour = Vector4.One,
        };
        
        _square.Index = VertexManager.BindVertexData(_square.Vertices, _square.Indices, 0, 0, 0);
        
        // Camera
        
        _viewMatrix = Matrix4.CreateTranslation(0, 0, -2); 
        Vector3 eye = new Vector3(0.0f, 0.5f, 0.5f);  
        Vector3 lookAt = new Vector3(0, 0, 0);  
        Vector3 up = new Vector3(0, 1, 0); 
        _viewMatrix = Matrix4.LookAt(eye, lookAt, up); 
        
        int uProjectionLocation = GL.GetUniformLocation(_shader.Handle, "uProjection"); 
        // Add to resize method
        _projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(1, (float)SceneManager.Width / SceneManager.Height, 0.5f, 5); 
        GL.UniformMatrix4(uProjectionLocation, true, ref _projectionMatrix); 
        
        GL.UseProgram(_shader.Handle);
        
        int uModel = GL.GetUniformLocation(_shader.Handle, "uModel");
        GL.UniformMatrix4(uModel, true, ref _modelMatrix);
        
        int uView = GL.GetUniformLocation(_shader.Handle, "uView");
        GL.UniformMatrix4(uView, true, ref _viewMatrix);
        
        int uProjection = GL.GetUniformLocation(_shader.Handle, "uProjection");
        GL.UniformMatrix4(uProjection, true, ref _projectionMatrix);
        
    }

    public override void Render(FrameEventArgs e)
    {
        //Console.WriteLine("Rendering");
        
        _shader.UseShader();
        
        int uModelLocation = GL.GetUniformLocation(_shader.Handle, "uModel"); 
        Matrix4 m1 =  Matrix4.CreateTranslation(0,0,0); 
        GL.UniformMatrix4(uModelLocation, true, ref m1); 

        Matrix4 m2 =  Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(45)); 
        GL.UniformMatrix4(uModelLocation, true, ref m2); 
        
        GL.BindVertexArray(VertexManager.GetVaoAtIndex(_square.Index));
        GL.DrawElements((PrimitiveType) PrimitiveType.Triangles, _square.Indices.Length, DrawElementsType.UnsignedInt, 0);
    }

    public override void Update(FrameEventArgs e)
    {
        int vertexColorLocation = GL.GetUniformLocation(_shader.Handle, "uColour");
        GL.Uniform4(vertexColorLocation, Vector4.One);
    }

    private void KeyPress(KeyPressEventArgs e)
    {
        if (e.KeyChar == 'a') 
        { 
            _viewMatrix = _viewMatrix * Matrix4.CreateTranslation(0.01f, 0, 0); 
            int uViewLocation = GL.GetUniformLocation(_shader.Handle, "uView"); 
            GL.UniformMatrix4(uViewLocation, true, ref _viewMatrix); 
        }
        else if (e.KeyChar == 'd') 
        { 
            _viewMatrix = _viewMatrix * Matrix4.CreateTranslation(-0.01f, 0, 0); 
            int uViewLocation = GL.GetUniformLocation(_shader.Handle, "uView"); 
            GL.UniformMatrix4(uViewLocation, true, ref _viewMatrix); 
        }
        else if (e.KeyChar == 'w') 
        { 
            _viewMatrix = _viewMatrix * Matrix4.CreateTranslation(0.0f, 0.0f, 0.01f); 
            int uViewLocation = GL.GetUniformLocation(_shader.Handle, "uView"); 
            GL.UniformMatrix4(uViewLocation, true, ref _viewMatrix); 
        }
        else if (e.KeyChar == 's') 
        { 
            _viewMatrix = _viewMatrix * Matrix4.CreateTranslation(0.0f, 0.0f, -0.01f); 
            int uViewLocation = GL.GetUniformLocation(_shader.Handle, "uView"); 
            GL.UniformMatrix4(uViewLocation, true, ref _viewMatrix); 
        }
        else if (e.KeyChar == 'q')
        {
            _viewMatrix = _viewMatrix * Matrix4.CreateRotationY(0.1f);
            int uViewLocation = GL.GetUniformLocation(_shader.Handle, "uView"); 
            GL.UniformMatrix4(uViewLocation, true, ref _viewMatrix); 
        }
        else if (e.KeyChar == 'e') 
        { 
            _viewMatrix = _viewMatrix * Matrix4.CreateRotationY(-0.1f);
            int uViewLocation = GL.GetUniformLocation(_shader.Handle, "uView"); 
            GL.UniformMatrix4(uViewLocation, true, ref _viewMatrix); 
        }
    }

    public override void Close()
    {
        
    }
}