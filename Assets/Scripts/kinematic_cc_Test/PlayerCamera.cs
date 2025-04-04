using UnityEngine;
using UnityEngine.UIElements;

public class PlayerCamera : MonoBehaviour
{
    [Header("Basic setting")]
    [SerializeField]
    float _defaultDis = 6f,
        _minDis = 3f,
        _maxDis = 10f,
        _disMovementSpd = 5f,
        _disMovementSharpness = 10f,
        _rotationSpd = 10f,
        _rotationSharpness = 10000f,
        _followSharpness = 10000f,
        _minVerticalAngle = -90f,
        _maxVerticalAngle = 90f,
        _defaultVerticalAngle = 20f;

    public float raycastDis = 10f;
    public RaycastHit hit;
    Transform _followTransform;
    Vector3 _currentFollowPos, _planarDir;
    float _targetVerticalAngle;

    float _curDIs, _targetDis;


    private void Awake()
    {
        _curDIs = _defaultDis;
        _targetDis = _curDIs;
        _targetVerticalAngle = 0f;
        _planarDir = Vector3.forward;
    }

    public void SetFollowTransform(Transform t)
    {
        _followTransform = t;
        _currentFollowPos = t.position;
        _planarDir = t.forward;
    }

    private void OnValidate()
    {
        _defaultDis = Mathf.Clamp(_defaultDis, _minDis, _maxDis);
        _defaultVerticalAngle = Mathf.Clamp(_defaultVerticalAngle, _minVerticalAngle, _maxVerticalAngle);
    }
    void HandleRotationInput(float deltaTime, Vector3 rotationInput, out Quaternion targetRotation)
    {
        Quaternion rotationFromInput = Quaternion.Euler(_followTransform.up * (rotationInput.x * _rotationSpd));
        _planarDir = rotationFromInput * _planarDir;
        Quaternion planarRotation = Quaternion.LookRotation(_planarDir, _followTransform.up);

        _targetVerticalAngle -= (rotationInput.y * _rotationSpd);
        _targetVerticalAngle = Mathf.Clamp(_targetVerticalAngle, _minVerticalAngle, _maxVerticalAngle);
        Quaternion verticalRotation = Quaternion.Euler(_targetVerticalAngle, 0, 0);

        targetRotation = Quaternion.Slerp(transform.rotation, planarRotation * verticalRotation, _rotationSharpness * deltaTime);

        transform.rotation = targetRotation;
    }

    void HandlePosition(float deltaTime, float zoomInput, Quaternion targetRotation)
    {
        _targetDis += zoomInput * _disMovementSpd;
        _targetDis = Mathf.Clamp(_targetDis, _minDis, _maxDis);

        _currentFollowPos = Vector3.Lerp(_currentFollowPos, _followTransform.position, 1f - Mathf.Exp(-_followSharpness * deltaTime));
        Vector3 tagetPosition = _currentFollowPos - ((targetRotation * Vector3.forward) * _curDIs);

        _curDIs = Mathf.Lerp(_curDIs, _targetDis, 1-Mathf.Exp(-_disMovementSharpness * deltaTime));
        transform.position = tagetPosition;
    }

    public void UpdateWithInput(float deltaTime, float zoomInput, Vector3 rotationInput)
    {
        if(_followTransform)
        {
            HandleRotationInput(deltaTime, rotationInput, out Quaternion targetRotation);
            HandlePosition(deltaTime, zoomInput, targetRotation);
        }
    }

    public void UpdatePlayerStickOnWall(float deltaTime, Vector3 rotationInput)
    {
        // if(_followTransform)
        // {
        //     HandleRotationInput(deltaTime, rotationInput, out Quaternion targetRotation);
        //     HandlePosition(deltaTime, -10f, targetRotation);
        // }
        // Doesnt works
    }
    
}
