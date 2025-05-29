using UnityEngine;
using UnityEngine.UIElements;

public class PlayerCamera : MonoBehaviour
{
    // �����̺� ������ ��� �����ϱ� �����ϱ� ���� _�� �տ� �ٿ���.
    [Header("Basic setting")]
    [SerializeField]
    float _defaultDis = 6f,
        _minDis = 3f,
        _maxDis = 10f, // Mathf.Clamp�� �� max, min ������ �������� �ִ�, �ּ� ī�޶� �Ÿ��� ����
        _disMovementSpd = 5f,
        _disMovementSharpness = 10f,
        _rotationSpd = 10f,
        _rotationSharpness = 10000f,
        _followSharpness = 10000f,
        _minVerticalAngle = -90f,
        _maxVerticalAngle = 90f,
        _defaultVerticalAngle = 20f,
        _recoilReturnSpeed = 10f; // ī�޶� ��鸲 ���� ����

    [Header("Sprint Camera Shake")] // ī�޶� ������Ʈ ��鸲
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

    // ī�޶� �ѱ� �ݵ��� ���� ����
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

    // ī�޶� �浹 üũ
    public void CheckBump()
    {
        Vector3 playerPos = playerTr.position;
        Vector3 cameraPos = transform.position;
        Vector3 direction = (cameraPos - playerPos).normalized;

        float distrance = Vector3.Distance(playerPos, cameraPos);

        RaycastHit[] hits = Physics.RaycastAll(playerPos, direction, distrance);

        foreach(var hit in hits)
        {
            //���� �浹 �ݶ��̴��� ���� �ƴ϶� ��? continue�� ����������
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
            // ���� ���� �� ���� �Ÿ� ����
            _preAimDistance = _targetDis;
            _targetDis = _minDis; // ������ ���� (����)
        }
        else if (!aiming && _isAiming)
        {
            // ���� ���� �� ���� �Ÿ� ����
            _targetDis = _preAimDistance;
        }

        _isAiming = aiming;
    }

    /// <summary>
    /// �ӵ���� ��鸲 ���� ���
    /// </summary>
    /// <param name="currentSpeed"> ���� �ӵ� </param>
    /// <param name="maxSpeed"> �ְ� �ӵ� </param>
    public void UpdateSprintState(float currentSpeed, float maxSpeed)
    {
        _currentSpeed = currentSpeed / maxSpeed;
    }

    /// <summary>
    /// �ӵ� ��� ī�޶� ��鸲 ����
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

            // Perlin Noise ��� ��鸲 ���� (�� �ڿ�������)
            float noiseX = Mathf.PerlinNoise(_shakeTimer, 0f);
            float noiseY = Mathf.PerlinNoise(0f, _shakeTimer);

            float shakePitch = (noiseY - 0.5f) * 2f * intensity * 5f;  // ���� ��鸲
            float shakeRoll = (noiseX - 0.5f) * 2f * intensity * 30f;  // �¿� ����

            _targetShakeRotation = Quaternion.Euler(shakePitch, 0f, shakeRoll);
        }

        // ��鸲 ���� (������)
        _currentShakeRotation = Quaternion.Slerp(_currentShakeRotation, _targetShakeRotation, deltaTime * 10f);

        // ī�޶� ȸ���� ����
        transform.rotation *= _currentShakeRotation;
    }


    /// <summary>
    /// ī�޶� ����ٴ� ��� t�� ���� (����Ƽ ���� ���� ��, �÷��̾� �ڽ� ������Ʈ�� CameraPos ������Ʈ�� ��ġ���� ��)
    /// </summary>
    /// <param name="t"> ī�޶� ����ٴ� ������Ʈ t�� Ʈ������ ��. ����� ������ ��</param>
    public void SetFollowTransform(Transform t)
    {
        _followTransform = t;
        _currentFollowPos = t.position;
        _planarDir = t.forward;
    }
    /// <summary>
    /// ī�޶� ��鸲 �� �Լ�
    /// </summary>
    /// <param name="horizontalAmount">�¿� ��鸲</param>
    /// <param name="verticalAmount">���� ��鸲</param>
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
    /// ȸ�� �Է��� ó���Ͽ� ī�޶��� ��ǥȸ�� ���
    /// </summary>
    /// <param name="deltaTime"> Time.deltaTime ��. �÷��̾� ��Ʈ�ѷ��� ��ŸŸ�� ���� ���� ��</param>
    /// <param name="rotationInput"> ���콺 </param>
    /// <param name="targetRotation"> ���� ������ t�� ���� ȸ������ �� out �Ķ����. HandlePosition���� �ش� ���� �ٽ� ����</param>
    void HandleRotationInput(float deltaTime, Vector3 rotationInput, out Quaternion targetRotation)
    {
        // ȭ�� ����ũ ���� ���
        _recoilShake = Vector2.SmoothDamp(_recoilShake, Vector2.zero, ref _recoilVelocity, 1f / _recoilReturnSpeed);

        // ��鸲 ����� �� ���콺 ȸ��
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
    /// ī�޶� ����, �� �ƿ��� ���õ� �Լ�. �������� �� �� ���� ȣ���
    /// </summary>
    /// <param name="deltaTime"> Time.deltaTime ��. �÷��̾� ��Ʈ�ѷ��� ��ŸŸ�� ���� ���� ��</param>
    /// <param name="zoomInput"> ���� ��. �÷��̾� ��Ʈ�ѷ��� Input.GetAxis ���콺 ��ũ�� ���� ���� ��</param>
    /// <param name="targetRotation"> HandleRotationInput�� out �Ķ���� targetRotation�� ��Ȱ���Ͽ� ���� �� ī�޶� ��ġ�� �����</param>
    void HandlePosition(float deltaTime, float zoomInput, Quaternion targetRotation)
    {
        _targetDis += zoomInput * _disMovementSpd;
        _targetDis = Mathf.Clamp(_targetDis, _minDis, _maxDis);

        _currentFollowPos = Vector3.Lerp(_currentFollowPos, _followTransform.position, 1f - Mathf.Exp(-_followSharpness * deltaTime));

        // ���������� �ణ ġ��ġ��?
        Vector3 rightOffset = targetRotation * Vector3.right * 2f;
        

        // ī�޶� ���󰡾� �� ������ ���� ���� ī�޶� ��ġ�� �� ���� �����Ͽ�, ���� ���󰡾� �� ������ �� ������ ������.
        // 1f - Mathf.Exp(-_followSharpness * deltaTime) : ���� ���� Ȱ���Ͽ� �ε巯�� ����, �ܾƿ� ����
        Vector3 tagetPosition = _currentFollowPos - ((targetRotation * Vector3.forward) * _curDIs) + rightOffset;

        _curDIs = Mathf.Lerp(_curDIs, _targetDis, 1-Mathf.Exp(-_disMovementSharpness * deltaTime));
        transform.position = tagetPosition;
    }

    /// <summary>
    /// �ٸ� ��ũ��Ʈ (�÷��̾� ��Ʈ�ѷ�)���� �� Ŭ������ ���������� ������ �� �ֵ��� ���ִ� �Լ�
    /// </summary>
    /// <param name="deltaTime">Time.deltaTime ��. �÷��̾� ��Ʈ�ѷ��� ��ŸŸ�� ���� ���� ��</param>
    /// <param name="zoomInput">���� ��. �÷��̾� ��Ʈ�ѷ��� Input.GetAxis ���콺 ��ũ�� ���� ���� ��</param>
    /// <param name="rotationInput">���콺 �Է� ��. �÷��̾� ��Ʈ�ѷ����� ���콺 ��ġ ���� ���� ��.</param>
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
