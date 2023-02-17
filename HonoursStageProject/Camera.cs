using HonoursStageProject.Managers;
using HonoursStageProject.Objects;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace HonoursStageProject;

public enum Direction
{
    Forward,
    Backward,
    Left,
    Right
}

public class Camera
{
    public Matrix4 View;
    public Matrix4 Projection;
    
    public Matrix4 CullingProj;
    
    public Vector3 Position;
    public Vector3 Up;
    public Vector3 Right;
    public Vector3 Direction;
    public Frustum ViewFrustum;
    
    private Vector2 _lastMousePos;
    private float _pitch;
    private float _yaw;
    private float _sensitivity;
    private bool _hasMouseMoved;

    
    public Camera()
    {
        // Default camera values
        
        Position = new Vector3(0.0f, 8f, 0.0f);
        var target = new Vector3(0.0f, 0.0f, 0.0f);
        Up = new Vector3(0.0f, 1.0f,  0.0f);
        Direction = Vector3.Normalize(target - Position);
        Right = Vector3.Normalize(Vector3.Cross(Up, Direction));
            
        View = Matrix4.CreateTranslation(0, 0, -2);
        Projection = Matrix4.CreatePerspectiveFieldOfView(1, (float)SceneManager.SWidth / SceneManager.SHeight, 0.1f, 100);
        CullingProj = Matrix4.CreatePerspectiveFieldOfView(1.75f, (float)SceneManager.SWidth / SceneManager.SHeight, 0.05f, 100);
        
        // Make a slightly bigger matrices for the culling

        _sensitivity = 0.05f;
        _hasMouseMoved = false;

        ViewFrustum = new Frustum();
        ViewFrustum.GenerateViewFrustum(Projection, View);
    }

    /// <summary>
    /// Handles camera movement
    /// </summary>
    /// <param name="pDirection">The direction to move</param>
    /// <param name="pDistance">The distance to move</param>
    public void MoveCamera(Direction pDirection, float pDistance)
    {
        switch (pDirection)
        {
            case HonoursStageProject.Direction.Forward:
                Position += Direction * pDistance;
                break;
            case HonoursStageProject.Direction.Backward:
                Position -= Direction * pDistance;
                break;
            case HonoursStageProject.Direction.Left:
                Position += Right * pDistance;
                break;
            case HonoursStageProject.Direction.Right:
                Position -= Right * pDistance;
                break;
        }
    }

    /// <summary>
    /// Handles camera rotation using mouse control
    /// </summary>
    /// <param name="pMouseState">The mouse state</param>
    public void RotateCamera(MouseState pMouseState)
    {
        // https://learnopengl.com/Getting-started/Camera
        
        // Calculate mouse delta, mouse change
        var mouseDeltaX = pMouseState.X - _lastMousePos.X;
        var mouseDeltaY = pMouseState.Y - _lastMousePos.Y;
        
        // If mouse hasn't moved yet, grab the position
        if (!_hasMouseMoved)
        {
            _lastMousePos = new Vector2(pMouseState.X, pMouseState.Y);
            _hasMouseMoved = true;
        }
        // If mouse has moved, calculate the mouse delta to calculate pitch and yaw
        else
        {
            _lastMousePos = new Vector2(pMouseState.X, pMouseState.Y);
            
            _yaw += mouseDeltaX * _sensitivity; // Horizontal Movement
            _pitch -= mouseDeltaY * _sensitivity; // Vertical Movement
            
            // Limit the pitch so the camera can't wrap around, +-90 degrees is directly up/down
            switch (_pitch)
            {
                case > 90.0f:
                    _pitch = 90.0f;
                    break;
                case < -90.0f:
                    _pitch = -90.0f;
                    break;
                default:
                    _pitch -= mouseDeltaY * _sensitivity; // Vertical Movement
                    break;
            }
        }
        
        Direction.X = (float)Math.Cos(MathHelper.DegreesToRadians(_pitch)) * (float)Math.Cos(MathHelper.DegreesToRadians(_yaw));
        Direction.Y = (float)Math.Sin(MathHelper.DegreesToRadians(_pitch));
        Direction.Z = (float)Math.Cos(MathHelper.DegreesToRadians(_pitch)) * (float)Math.Sin(MathHelper.DegreesToRadians(_yaw));
        
        // Update the right value so side to side movement is relative 
        Right = Vector3.Normalize(Vector3.Cross(Up, Direction));
        Direction = Vector3.Normalize(Direction);
    }

    /// <summary>
    /// Updates the view matrix, called after a change to the components is made
    /// </summary>
    public void UpdateCamera(int pShaderHandle)
    {
        View = Matrix4.LookAt(Position, Position + Direction, Up);
        ViewFrustum.GenerateViewFrustum(CullingProj, View);
        
        var uView = GL.GetUniformLocation(pShaderHandle, "uView");
        GL.UniformMatrix4(uView, true, ref View);

        var uProjection = GL.GetUniformLocation(pShaderHandle, "uProjection");
        GL.UniformMatrix4(uProjection, true, ref Projection);
    }
}