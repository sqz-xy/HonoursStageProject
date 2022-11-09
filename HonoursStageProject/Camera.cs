using System.Runtime.InteropServices.ObjectiveC;
using HonoursStageProject.Managers;
using OpenTK;
using OpenTK.Input;

namespace HonoursStageProject;

public enum Direction
{
    FORWARD,
    BACKWARD,
    LEFT,
    RIGHT
}

public class Camera
{
    public Matrix4 View;
    public Matrix4 Projection;

    private Vector3 _position;
    private Vector3 _target;
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
        _position = new Vector3(0.0f, 0.5f, 0.5f);
        _target = new Vector3(0.0f, 0.0f, 0.0f);
        _up = new Vector3(0.0f, 1.0f,  0.0f);
        _direction = Vector3.Normalize(_target - _position);
        _right = Vector3.Normalize(Vector3.Cross(_up, _direction));
            
        View = Matrix4.CreateTranslation(0, 0, -2);
        Projection = Matrix4.CreatePerspectiveFieldOfView(1, (float)SceneManager.SWidth / SceneManager.SHeight, 0.5f, 5);

        _sensitivity = 0.05f;
        _hasMouseMoved = false;
        
        UpdateCamera();
    }

    public void MoveCamera(Direction pDirection, float pDistance)
    {
        switch (pDirection)
        {
            case Direction.FORWARD:
                _position += _direction * pDistance;
                break;
            case Direction.BACKWARD:
                _position -= _direction * pDistance;
                break;
            case Direction.LEFT:
                _position += _right * pDistance;
                break;
            case Direction.RIGHT:
                _position -= _right * pDistance;
                break;
        }
    }

    public void RotateCamera(MouseState pMouseState)
    {
        // https://learnopengl.com/Getting-started/Camera
        
        // Calculate mouse delta, mouse change
        float mouseDeltaX = pMouseState.X - _lastMousePos.X;
        float mouseDeltaY = pMouseState.Y - _lastMousePos.Y;
        
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

    public void UpdateCamera()
    {
        View = Matrix4.LookAt(_position, _position + _direction, _up); 
    }
}