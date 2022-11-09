using System.Runtime.InteropServices.ObjectiveC;
using HonoursStageProject.Managers;
using OpenTK;

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

    public Camera()
    {
        _position = new Vector3(0.0f, 0.5f, 0.5f);
        _target = new Vector3(0.0f, 0.0f, 0.0f);
        _up = new Vector3(0.0f, 1.0f,  0.0f);
        _direction = Vector3.Normalize(_target - _position);
        _right = Vector3.Normalize(Vector3.Cross(_up, _direction));
            
        View = Matrix4.CreateTranslation(0, 0, -2);
        Projection = Matrix4.CreatePerspectiveFieldOfView(1, (float)SceneManager.SWidth / SceneManager.SHeight, 0.5f, 5);
        
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
        UpdateCamera();
    }

    public void UpdateCamera()
    {
        View = Matrix4.LookAt(_position, _position + _direction, _up); 
    }
}