using UnityEngine;
using System;

public class MovementManager : MonoBehaviour
{
    private bool _isActive = true;
    
    public GameObject[] movementMarkers;
    private Transform[] _leftMarkers;
    private Transform[] _rightMarkers;
    private Vector3 _leftDestination;
    private Vector3 _rightDestination;
    
    private float _maxDistance;

    public GameObject leftCar;
    public GameObject rightCar;
    public int speed;
    
    private bool _leftCarIsMoving = false;
    private bool _rightCarIsMoving = false;

    public bool useLerp = false;
    private Func<Vector3, Vector3, float, Vector3> _movementFunction;
    
    // Check on left
    private bool _leftCarOnLeft = true;
    private bool _rightCarOnLeft = true;
    
    // Check heading left
    private bool _leftCarHeadingLeft = true;
    private bool _rightCarHeadingLeft = true;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (movementMarkers[0] != null && movementMarkers[1] != null)
        {
            _leftMarkers = new Transform[] { movementMarkers[0].transform, movementMarkers[1].transform };
        }

        if (movementMarkers[2] != null && movementMarkers[3] != null)
        {
            _rightMarkers = new Transform[] { movementMarkers[2].transform, movementMarkers[3].transform }; 
        }

        if (useLerp)
        {
            _movementFunction = Vector3.Lerp;
        }
        else
        {
            _movementFunction = Vector3.MoveTowards;
        }
        
        // Max Distance Set
        _maxDistance = Vector3.Distance(_leftMarkers[0].position, _leftMarkers[1].position);
    }

    // Update is called once per frame
    private void Update()
    {
        if (!_isActive) return;
        
        CheckLeft();
        CheckRight();
        
        if (_leftCarIsMoving)
        {
            MoveLeft();
        }

        if (_rightCarIsMoving)
        {
            MoveRight();
        }

        CheckLeftReached();
        CheckRightReached();
    }
    
    private void MoveLeft()
    {
        leftCar.transform.position = _movementFunction(leftCar.transform.position, 
            _leftDestination, 
            speed * Time.deltaTime);
        
        float distance = Vector3.Distance(leftCar.transform.position, _leftDestination);
        float t = Mathf.Clamp01(distance / _maxDistance);
        float angle = _leftCarHeadingLeft ? 30f : -30f;
        Quaternion veerRotation = Quaternion.Euler(0, 0, angle);
        
        leftCar.transform.rotation = Quaternion.Lerp(Quaternion.identity, veerRotation, t);
    }

    private void MoveRight()
    {
        rightCar.transform.position = _movementFunction(
            rightCar.transform.position,
            _rightDestination,
            Time.deltaTime * speed);
        
        float distance = Vector3.Distance(rightCar.transform.position, _rightDestination);
        float t = Mathf.Clamp01(distance / _maxDistance);
        float angle = _rightCarHeadingLeft ? 30f : -30f;
        Quaternion veerRotation = Quaternion.Euler(0, 0, angle);
        
        rightCar.transform.rotation = Quaternion.Lerp(Quaternion.identity, veerRotation, t);
    }
    private void CheckLeft()
    {
        var leftCarKey = VirtualInput.GetLeftPressed();
        if (!leftCarKey) return;
        _leftDestination = _leftCarHeadingLeft ? _leftMarkers[1].position : _leftMarkers[0].position;
        _leftCarHeadingLeft = !_leftCarHeadingLeft;
        _leftCarIsMoving = true;
    }

    private void CheckRight()
    {
        var rightCarKey = VirtualInput.GetRightPressed();
        if (!rightCarKey) return;
        
        _rightDestination = _rightCarHeadingLeft ? _rightMarkers[1].position : _rightMarkers[0].position;
        _rightCarHeadingLeft = !_rightCarHeadingLeft;
        _rightCarIsMoving = true;
    }

    private void CheckLeftReached()
    {
        if (_leftCarIsMoving)
        {
            if (Vector3.Distance(leftCar.transform.position, _leftDestination) < 0.01f)
            {
                _leftCarIsMoving = false;
                _leftCarOnLeft = !_leftCarOnLeft;
            }
        }
    }
    
    private void CheckRightReached()
    {
        if (_rightCarIsMoving)
        {
            if (Vector3.Distance(rightCar.transform.position, _rightDestination) < 0.01f)
            {
                // Fully Set Position
                rightCar.transform.position = _rightDestination;
                
                // Set new direction.
                _rightCarIsMoving = false;
                _rightCarOnLeft = !_rightCarOnLeft;
            }
        }
    }

    public void Activate()
    {
        _isActive = true;
    }

    public void Deactivate()
    {
        _isActive = false;
    }
}
