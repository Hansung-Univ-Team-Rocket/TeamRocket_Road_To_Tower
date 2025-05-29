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
        _defaultVerticalAngle = 20f,
        _recoilReturnSpeed = 10f; // 카메라 흔들림 복구 변수

    [Header("Sprint Camera Shake")] // 카메라 스프린트 흔들림
    [SerializeField] float _maxShakeIntensity = 0.15f;
    [SerializeField] float _shakeFrequency = 10f;
    [SerializeField] Quaternion _targetShakeRotation = Quaternion.identity;
    [SerializeField] Quaternion _currentShakeRotation = Quaternion.identity;

    [Header("Camera bump Values")]
    [SerializeField] float collisionChecker = .3f;
    [SerializeField] float collisionOffset = .5f;
    [SerializeField] Transform playerTr;

    float _currentSpeed = 0f;
    float _shakeTimer = 0f;
    float _preAimDistance;
    bool _isAiming = false;

    // 카메라 총기 반동에 따른 변수
    Vector2 _recoilShake = Vector2.zero;
    Vector2 _recoilVelocity = Vector2.zero;

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

    private void Start()
    {
        playerTr = GameObject.FindGameObjectWithTag("CameraPos").GetComponent<Transform>();
    }

    // 카메라 충돌 체크
    public void CheckBump()
    {
        Vector3 playerPos = playerTr.position;
        Vector3 cameraPos = transform.position;
        Vector3 direction = (cameraPos - playerPos).normalized;

        float distrance = Vector3.Distance(playerPos, cameraPos);

        RaycastHit[] hits = Physics.RaycastAll(playerPos, direction, distrance);

        foreach(var hit in hits)
        {
            //만약 충돌 콜라이더가 벽이 아니라 적? continue로 빠져나오기
            if (hit.collider.tag == "Enemy") continue;

            if(hit.transform != playerTr.transform && hit.transform != transform)
            {
                transform.position = hit.point - direction * collisionOffset;
                break;
            }
        }
    }
    public void SetAimState(bool aiming)
    {
        if (aiming && !_isAiming)
        {
            // 에임 시작 시 기존 거리 저장
            _preAimDistance = _targetDis;
            _targetDis = _minDis; // 가까이 줌인 (에임)
        }
        else if (!aiming && _isAiming)
        {
            // 에임 종료 시 원래 거리 복원
            _targetDis = _preAimDistance;
        }

        _isAiming = aiming;
    }

    /// <summary>
    /// 속도비례 흔들림 강도 계산
    /// </summary>
    /// <param name="currentSpeed"> 현재 속도 </param>
    /// <param name="maxSpeed"> 최고 속도 </param>
    public void UpdateSprintState(float currentSpeed, float maxSpeed)
    {
        _currentSpeed = currentSpeed / maxSpeed;
    }

    /// <summary>
    /// 속도 비례 카메라 흔들림 적용
    /// </summary>
    /// <param name="deltaTime"></param>
    private void ApplySpeedBasedShake(float deltaTime)
    {
        if (_currentSpeed < 0.5f)
        {
            _targetShakeRotation = Quaternion.identity;
        }
        else
        {
            _shakeTimer += deltaTime * _shakeFrequency;
            float intensity = _currentSpeed * _maxShakeIntensity;

            // Perlin Noise 기반 흔들림 생성 (더 자연스러움)
            float noiseX = Mathf.PerlinNoise(_shakeTimer, 0f);
            float noiseY = Mathf.PerlinNoise(0f, _shakeTimer);

            float shakePitch = (noiseY - 0.5f) * 2f * intensity * 5f;  // 상하 흔들림
            float shakeRoll = (noiseX - 0.5f) * 2f * intensity * 30f;  // 좌우 기울기

            _targetShakeRotation = Quaternion.Euler(shakePitch, 0f, shakeRoll);
        }

        // 흔들림 감쇠 (스무딩)
        _currentShakeRotation = Quaternion.Slerp(_currentShakeRotation, _targetShakeRotation, deltaTime * 10f);

        // 카메라 회전에 적용
        transform.rotation *= _currentShakeRotation;
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
    /// <summary>
    /// 카메라 흔들림 콜 함수
    /// </summary>
    /// <param name="horizontalAmount">좌우 흔들림</param>
    /// <param name="verticalAmount">상하 흔들림</param>
    public void ApplyRecoilShake(float horizontalAmount, float verticalAmount)
    {
        _recoilShake.x += horizontalAmount; 
        _recoilShake.y += verticalAmount;  
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
        // 화면 쉐이크 감쇠 계산
        _recoilShake = Vector2.SmoothDamp(_recoilShake, Vector2.zero, ref _recoilVelocity, 1f / _recoilReturnSpeed);

        // 흔들림 계산이 들어간 마우스 회전
        float totalYaw = rotationInput.x * _rotationSpd + _recoilShake.x;
        float totalPitch = rotationInput.y * _rotationSpd + _recoilShake.y;

        Quaternion rotationFromInput = Quaternion.Euler(_followTransform.up * totalYaw);
        _planarDir = rotationFromInput * _planarDir;
        Quaternion planarRotation = Quaternion.LookRotation(_planarDir, _followTransform.up);

        _targetVerticalAngle -= totalPitch;
        _targetVerticalAngle = Mathf.Clamp(_targetVerticalAngle, _minVerticalAngle, _maxVerticalAngle);

        Quaternion verticalRotation = Quaternion.Euler(_targetVerticalAngle, 0, 0);
        targetRotation = Quaternion.Slerp(transform.rotation, planarRotation * verticalRotation, _rotationSharpness * deltaTime);

        transform.rotation = targetRotation;

        //Quaternion rotationFromInput = Quaternion.Euler(_followTransform.up * (rotationInput.x * _rotationSpd));
        //_planarDir = rotationFromInput * _planarDir;
        //Quaternion planarRotation = Quaternion.LookRotation(_planarDir, _followTransform.up);

        //_targetVerticalAngle -= (rotationInput.y * _rotationSpd);
        //_targetVerticalAngle = Mathf.Clamp(_targetVerticalAngle, _minVerticalAngle, _maxVerticalAngle);
        //Quaternion verticalRotation = Quaternion.Euler(_targetVerticalAngle, 0, 0);

        //targetRotation = Quaternion.Slerp(transform.rotation, planarRotation * verticalRotation, _rotationSharpness * deltaTime);

        //transform.rotation = targetRotation;
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

        // 오른쪽으로 약간 치우치게?
        Vector3 rightOffset = targetRotation * Vector3.right * 2f;
        

        // 카메라가 따라가야 할 포지션 값과 현재 카메라가 위치한 두 값을 보간하여, 현재 따라가야 할 포지션 값 변수를 갱신함.
        // 1f - Mathf.Exp(-_followSharpness * deltaTime) : 지수 감쇠 활용하여 부드러운 줌인, 줌아웃 구현
        Vector3 tagetPosition = _currentFollowPos - ((targetRotation * Vector3.forward) * _curDIs) + rightOffset;

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
            ApplySpeedBasedShake(deltaTime);
        }
    }    
}
