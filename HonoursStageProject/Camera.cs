using HonoursStageProject.Managers;
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

    private Vector3 _position;
    private Vector3 _up;
    private Vector3 _right;
    private Vector3 _direction;

    private Vector2 _lastMousePos;

    private float _pitch;
    private float _yaw;

    private float _sensitivity;

    private bool _hasMouseMoved;
    
    public Camera()
    {
        // Default camera values
        
        _position = new Vector3(0.0f, 8f, 0.0f);
        var target = new Vector3(0.0f, 0.0f, 0.0f);
        _up = new Vector3(0.0f, 1.0f,  0.0f);
        _direction = Vector3.Normalize(target - _position);
        _right = Vector3.Normalize(Vector3.Cross(_up, _direction));
            
        View = Matrix4.CreateTranslation(0, 0, -2);
        Projection = Matrix4.CreatePerspectiveFieldOfView(1, (float)SceneManager.SWidth / SceneManager.SHeight, 0.1f, 100);

        _sensitivity = 0.05f;
        _hasMouseMoved = false;
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
            case Direction.Forward:
                _position += _direction * pDistance;
                break;
            case Direction.Backward:
                _position -= _direction * pDistance;
                break;
            case Direction.Left:
                _position += _right * pDistance;
                break;
            case Direction.Right:
                _position -= _right * pDistance;
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
        
        _direction.X = (float)Math.Cos(MathHelper.DegreesToRadians(_pitch)) * (float)Math.Cos(MathHelper.DegreesToRadians(_yaw));
        _direction.Y = (float)Math.Sin(MathHelper.DegreesToRadians(_pitch));
        _direction.Z = (float)Math.Cos(MathHelper.DegreesToRadians(_pitch)) * (float)Math.Sin(MathHelper.DegreesToRadians(_yaw));
        
        // Update the right value so side to side movement is relative 
        _right = Vector3.Normalize(Vector3.Cross(_up, _direction));
        _direction = Vector3.Normalize(_direction);
        
    }

    /// <summary>
    /// Updates the view matrix, called after a change to the components is made
    /// </summary>
    public void UpdateCamera(int pShaderHandle)
    {
        View = Matrix4.LookAt(_position, _position + _direction, _up); 
        
        var uView = GL.GetUniformLocation(pShaderHandle, "uView");
        GL.UniformMatrix4(uView, true, ref View);

        var uProjection = GL.GetUniformLocation(pShaderHandle, "uProjection");
        GL.UniformMatrix4(uProjection, true, ref Projection);
    }
}