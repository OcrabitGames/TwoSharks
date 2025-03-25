using UnityEngine;
using System;


public class MovementManager : MonoBehaviour
{
    [SerializeField] private bool isActive = true;
    
    public GameObject[] movementMarkers;
    private Transform[] _leftMarkers;
    private Transform[] _rightMarkers;
    private Vector3 _leftDestination;
    private Vector3 _rightDestination;
    
    private float _maxDistance;

    public GameObject leftCar;
    public GameObject rightCar;
    public int speed;
    
    private bool _leftCarIsMoving;
    private bool _rightCarIsMoving;

    public bool useLerp;
    private Func<Vector3, Vector3, float, Vector3> _movementFunction;
    
    // Check on left
    private bool _leftCarOnLeft = true;
    private bool _rightCarOnLeft = true;
    
    // Check heading left
    private bool _leftCarHeadingLeft = true;
    private bool _rightCarHeadingLeft = true;
    
    // Particle System
    private ParticleSystem _leftPS;
    private ParticleSystem _rightPS;
    private static float _normEmissionRate;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (movementMarkers[0] != null && movementMarkers[1] != null)
        {
            _leftMarkers = new[] { movementMarkers[0].transform, movementMarkers[1].transform };
        }

        if (movementMarkers[2] != null && movementMarkers[3] != null)
        {
            _rightMarkers = new [] { movementMarkers[2].transform, movementMarkers[3].transform }; 
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
        
        // Set Particle System Information
        _leftPS = leftCar.GetComponent<ParticleSystem>();
        _rightPS = rightCar.GetComponent<ParticleSystem>();
        _normEmissionRate = _leftPS.emission.rateOverTime.constantMax;
        print($"Set emission rate {_normEmissionRate}");
    }

    // Update is called once per frame
    private void Update()
    {
        if (!isActive) return;
        
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

        SlowEmissionRate(_leftPS);
    }

    private void CheckRight()
    {
        var rightCarKey = VirtualInput.GetRightPressed();
        if (!rightCarKey) return;
        
        _rightDestination = _rightCarHeadingLeft ? _rightMarkers[1].position : _rightMarkers[0].position;
        _rightCarHeadingLeft = !_rightCarHeadingLeft;
        _rightCarIsMoving = true;
        
        SlowEmissionRate(_rightPS);
    }

    private void CheckLeftReached()
    {
        if (_leftCarIsMoving)
        {
            if (Vector3.Distance(leftCar.transform.position, _leftDestination) < 0.01f)
            {
                // Fully Set Position
                leftCar.transform.position = _leftDestination;
                
                // Set new direction.
                _leftCarIsMoving = false;
                _leftCarOnLeft = !_leftCarOnLeft;
                
                ResetEmissionRate(_leftPS);
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
                
                ResetEmissionRate(_rightPS);
            }
        }
    }

    public void Activate()
    {
        isActive = true;
    }

    public void Deactivate()
    {
        isActive = false;
    }

    public void Reset()
    {
        isActive = false;
        leftCar.transform.position = _leftMarkers[0].position;
        leftCar.transform.rotation = Quaternion.identity;
        rightCar.transform.position = _rightMarkers[0].position;
        rightCar.transform.rotation = Quaternion.identity;
    }

    private void SlowEmissionRate(ParticleSystem ps)
    {
        var emissionModule = ps.emission;
        emissionModule.rateOverTime = 3f;
    }

    private static void ResetEmissionRate(ParticleSystem ps)
    {
        var emissionModule = ps.emission;
        emissionModule.rateOverTime = _normEmissionRate;
    }
}
