using UnityEngine;
using UnityEngine.UIElements;

public class PlayerCamera : MonoBehaviour
{
    // 프라이빗 변수의 경우 구별하기 쉽게하기 위해 _을 앞에 붙였음.
    [Header("Basic setting")]
    [SerializeField]
    float _defaultDis = 6f,
        _minDis = 3f,
        _maxDis = 10f, // Mathf.Clamp로 두 max, min 변수를 기준으로 최대, 최소 카메라 거리를 고정
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

    /// <summary>
    /// 카메라가 따라다닐 대상 t를 설정 (유니티 계층 구조 상, 플레이어 자식 오브젝트인 CameraPos 오브젝트의 위치값이 들어감)
    /// </summary>
    /// <param name="t"> 카메라가 따라다닐 오브젝트 t의 트랜스폼 값. 계산의 기준이 됨</param>
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

    /// <summary>
    /// 회전 입력을 처리하여 카메라의 목표회전 계산
    /// </summary>
    /// <param name="deltaTime"> Time.deltaTime 값. 플레이어 컨트롤러의 델타타임 값이 들어가야 함</param>
    /// <param name="rotationInput"> 마우스 </param>
    /// <param name="targetRotation"> 계산된 기준점 t에 대한 회전값이 들어간 out 파라미터. HandlePosition에서 해당 값이 다시 사용됨</param>
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

    /// <summary>
    /// 카메라 줌인, 줌 아웃과 관련된 함수. 프레임이 돌 때 마다 호출됨
    /// </summary>
    /// <param name="deltaTime"> Time.deltaTime 값. 플레이어 컨트롤러의 델타타임 값이 들어가야 함</param>
    /// <param name="zoomInput"> 줌인 값. 플레이어 컨트롤러의 Input.GetAxis 마우스 스크롤 값이 들어가야 함</param>
    /// <param name="targetRotation"> HandleRotationInput의 out 파라미터 targetRotation을 재활용하여 줌인 시 카메라 위치를 계산함</param>
    void HandlePosition(float deltaTime, float zoomInput, Quaternion targetRotation)
    {
        _targetDis += zoomInput * _disMovementSpd;
        _targetDis = Mathf.Clamp(_targetDis, _minDis, _maxDis);

        _currentFollowPos = Vector3.Lerp(_currentFollowPos, _followTransform.position, 1f - Mathf.Exp(-_followSharpness * deltaTime));
        // 카메라가 따라가야 할 포지션 값과 현재 카메라가 위치한 두 값을 보간하여, 현재 따라가야 할 포지션 값 변수를 갱신함.
        // 1f - Mathf.Exp(-_followSharpness * deltaTime) : 지수 감쇠 활용하여 부드러운 줌인, 줌아웃 구현
        Vector3 tagetPosition = _currentFollowPos - ((targetRotation * Vector3.forward) * _curDIs);

        _curDIs = Mathf.Lerp(_curDIs, _targetDis, 1-Mathf.Exp(-_disMovementSharpness * deltaTime));
        transform.position = tagetPosition;
    }

    /// <summary>
    /// 다른 스크립트 (플레이어 컨트롤러)에서 이 클래스에 간접적으로 접근할 수 있도록 해주는 함수
    /// </summary>
    /// <param name="deltaTime">Time.deltaTime 값. 플레이어 컨트롤러의 델타타임 값이 들어가야 함</param>
    /// <param name="zoomInput">줌인 값. 플레이어 컨트롤러의 Input.GetAxis 마우스 스크롤 값이 들어가야 함</param>
    /// <param name="rotationInput">마우스 입력 값. 플레이어 컨트롤러에서 마우스 위치 값이 들어가야 함.</param>
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
