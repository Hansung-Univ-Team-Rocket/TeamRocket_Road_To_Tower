using KinematicCharacterController;
using UnityEngine;
using UnityEngine.Rendering;

public struct PlayerInput
{
    public float AxisFwd; 
    public float AxisRight;
    public Quaternion CameraRotation;
    public bool CrouchDown;
    public bool CrouchUp;
    public bool Non_Sprint;
    public bool Sprint;
    public bool Dodge;
    public bool MeleeAttack;
    public bool ShootingAttack;
    public bool Reroading;
}

public enum UpperPlayerState
{
    IDLE = 0,
    MELEEATTACK,
    SHOOTINGATTACK,
    REROADING,
    SPRINT,
    DODGE,
    DAMAGED,
    DEAD
}

public enum LowerPlayerState
{
    IDLE = 0,
    MOVE,
    SPRINT,
    CROUCH,
    CROUCH_MOVE,
    DODGE,
    DAMAGED,
    DEAD
}

// PlayerState�� UpperPlayerState, LowerPlayerState �ѷ� ������ ��.

public class PlayerMovementController : MonoBehaviour, ICharacterController
{
    [SerializeField] KinematicCharacterMotor _motor;
    [SerializeField] float _maxStableMoveSpeed = 10f;
    [SerializeField] float _maxSprintMoveSpeed = 18f;
    [SerializeField] float _maxDodgeMoveSpeed = 33f;
    [SerializeField] float _stableMovementSharpness = 15f;
    [SerializeField] public float _orientationSharpness = 10f;
    [SerializeField] Vector3 _gravity = new Vector3(0, -60f, 0);
    [SerializeField] CapsuleCollider _capsuleCollider;

    [Header("CoolTimes")]
    [SerializeField] float _dodgeTime = .5f;
    [SerializeField] float _dodgeTimeChecker = 0f;

    [Header("Flag value")]
    [SerializeField] bool _isSprinting = false;
    [SerializeField] bool _isCrouching = false; // ĳ���Ͱ� �Ͼ �� �ִ� �����ΰ�? �ɾ� �־�� �ϴ� �����ΰ�? �浹 �˻�� ���õ� �÷��� ��
    [SerializeField] bool _secondCrouchingChecker = false; // �÷��̾ �Ͼ�� �;� �ϴ°�? ��, ��Ʈ�� Ű�� ���°�?
    [SerializeField] bool _isDodge = false;
    public           bool isNowDodge = false;
    public           bool isReroading = false;
    public           bool isFire = false;

    [Header("Player Size")]
    [SerializeField] float _playerCrouchedCapsuleHieght = 1f;
    [SerializeField] float _playerNonCrouhedCapsuleHieght = 2f;

    [Header("Player state")]
    public UpperPlayerState upperPlayerState;
    public LowerPlayerState lowerPlayerState;

    float _movementCheckValue = 0f;


    private Collider[] _probedColliders = new Collider[8];
    Vector3 _moveInputVector;
    Vector3 _lookInputVector;

    private void Start()
    {
        upperPlayerState = UpperPlayerState.IDLE;
        lowerPlayerState = LowerPlayerState.IDLE;
        _motor.CharacterController = this;
        _capsuleCollider = GetComponent<CapsuleCollider>();
        _capsuleCollider.isTrigger = true;
    }

    public void Dead()
    {
        // �ִϸ��̼� ��� �ڵ���� �߰� �ʿ�
        if (upperPlayerState == UpperPlayerState.DEAD || lowerPlayerState == LowerPlayerState.DEAD) return;

        upperPlayerState = UpperPlayerState.DEAD;
        lowerPlayerState = LowerPlayerState.DEAD;
        

    }
    void ReturnMoveInput(PlayerInput inputs)
    {
        _movementCheckValue = Mathf.Abs(inputs.AxisFwd) + Mathf.Abs(inputs.AxisRight);
    }
    // ���� ������ ��쿡 ���⼭ �÷��̾� ������Ʈ �ӽ� �ذ�
    public void SetInputs(ref PlayerInput inputs)
    {
        if (upperPlayerState == UpperPlayerState.DEAD || lowerPlayerState == LowerPlayerState.DEAD)
        {
            _moveInputVector = Vector3.zero;
            _lookInputVector = Vector3.zero;

            return;
        }
        ReturnMoveInput(inputs);
        if (_dodgeTimeChecker >= _dodgeTime && isNowDodge)
        {
            isNowDodge = false;
            _isDodge = false;
            upperPlayerState = UpperPlayerState.IDLE;
            lowerPlayerState = LowerPlayerState.IDLE;
            _dodgeTimeChecker = 0f;
        }
        if (inputs.Dodge && !isNowDodge && !_isCrouching)
        {
            _isDodge = true;
            isNowDodge = true;
            _isCrouching = false;
            _isSprinting = false;
        }

        if(isNowDodge)
        {
            _dodgeTimeChecker += Time.deltaTime;

            upperPlayerState = UpperPlayerState.DODGE;
            lowerPlayerState = LowerPlayerState.DODGE;
            
            return;
        }

        // ������ �ƾ�. �׷� ������ ����.
        // ���� �߿����� ���� ���ݰ� ���Ÿ� ����
        // ��ü, ��ü�� ���� ���� ������, ȸ�Ǹ� �����ϸ�, ������ ������ �Ǿ�� ��.
        // ������ ������ �����ͼ� ������ �ڽ� ��ü�� ���Ÿ��ΰ�? �ٰŸ��ΰ��� ���� flag ���� �������� ������.
        // ���Ÿ� ����� ����� �ΰ����� ���� ī�޶� �߾ӿ��� ������ ��
        // �ѱ����� ������ ��. ���ڰ� ���� ���ƺ��̴µ�...

        Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(inputs.AxisRight, 0f, inputs.AxisFwd), 1f);
        Vector3 cameraPlanarDir = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.forward, _motor.CharacterUp).normalized;
  
        if(cameraPlanarDir.sqrMagnitude == 0f)
        {
            cameraPlanarDir = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.up, _motor.CharacterUp).normalized;
        }

        Quaternion cameraPlanerRotation = Quaternion.LookRotation(cameraPlanarDir, _motor.CharacterUp);

        _moveInputVector = cameraPlanerRotation * moveInputVector;
        _lookInputVector = _moveInputVector.normalized;

        if (inputs.AxisFwd != 0 || inputs.AxisRight != 0)
        {
            //Debug.Log("��� ��?");
            if (inputs.CrouchDown)
            {
                _isSprinting = false;
                _secondCrouchingChecker = true;

                if (!_isCrouching)
                {
                    _isCrouching = true;
                    _motor.SetCapsuleDimensions(0.5f, _playerCrouchedCapsuleHieght, _playerCrouchedCapsuleHieght * .5f);

                    lowerPlayerState = LowerPlayerState.CROUCH_MOVE;
                }
            }
            else if (inputs.CrouchUp)
            {
                _secondCrouchingChecker = false;
            }
            else if (inputs.Sprint)
            {
                if (!_isCrouching)
                {
                    _isSprinting = true;
                }
                else _isSprinting = false;
            }
            else if (inputs.Non_Sprint) // ��ǻ��� �޸��⸦ �� ���� ���� ��� ���°� ���� else ����
            {
                _isSprinting = false;
            }
        }
        if(inputs.AxisFwd == 0 && inputs.AxisRight == 0)
        {

            if(inputs.CrouchDown)
            {
                _secondCrouchingChecker = true;

                if (!_isCrouching)
                {
                    _isCrouching = true;
                    _motor.SetCapsuleDimensions(0.5f, _playerCrouchedCapsuleHieght, _playerCrouchedCapsuleHieght * .5f);
                    lowerPlayerState = LowerPlayerState.CROUCH;
                }
                else
                {
                    lowerPlayerState = LowerPlayerState.CROUCH;
                }
            }
            if(inputs.CrouchUp)
            {
                _secondCrouchingChecker = false;
            }
            else
            {
                if(!_isCrouching)
                {
                    lowerPlayerState = LowerPlayerState.IDLE;
                }
                else
                {
                    lowerPlayerState = LowerPlayerState.CROUCH;
                }
            }
        }

    }
    // �ִ��� �̵� ���¿� ���õ� �÷��̾� ������Ʈ�� �� �ݹ鿡�� �ذ�
    public void AfterCharacterUpdate(float deltaTime)
    {
        // Dodge �߿��� �ٸ� ���� �������� ���� �ٷ� ����
        if (isNowDodge)
        {
            upperPlayerState = UpperPlayerState.DODGE;
            lowerPlayerState = LowerPlayerState.DODGE;
            return;
        }

        // �ϵ��ڵ�, ������Ʈ�� �ϴ�, �ɱ� Ű�� ������ ���ÿ� ���� ���� ��ü�� ������Ʈ��, ��ü�� ���̵��� ���°� �Ǵ� ��찡 ����
        if (upperPlayerState == UpperPlayerState.SPRINT && lowerPlayerState == LowerPlayerState.IDLE)
        {
            upperPlayerState = UpperPlayerState.IDLE;
        }
        if (_isCrouching && !_secondCrouchingChecker)
        {
            // ���� ���¿��� �Ӹ� ���� �ݶ��̴��� �ִ� ������Ʈ�� �ִ°�?
            _motor.SetCapsuleDimensions(0.5f, _playerNonCrouhedCapsuleHieght, 1f);
            if(_motor.CharacterOverlap(
                _motor.TransientPosition,
                _motor.TransientRotation,
                _probedColliders,
                _motor.CollidableLayers,
                QueryTriggerInteraction.Ignore) > 0)
            {
                // �׷��ٸ� ��� ���� ����
                _motor.SetCapsuleDimensions(0.5f, _playerCrouchedCapsuleHieght, _playerCrouchedCapsuleHieght * .5f);
            }
            else
            {
                _isCrouching = false;
                //upperPlayerState = UpperPlayerState.IDLE;
                lowerPlayerState = LowerPlayerState.IDLE;
            }
        }
        if(_isSprinting)
        {
            if (isFire || isReroading)
            {
                lowerPlayerState = LowerPlayerState.SPRINT;
            }
            else
            {
                upperPlayerState = UpperPlayerState.SPRINT;
                lowerPlayerState = LowerPlayerState.SPRINT;
            }
        }
        else
        {
            if (_isCrouching)
            {
                if (isFire || isReroading)
                {
                    lowerPlayerState = LowerPlayerState.CROUCH_MOVE;
                }
                else
                {
                    upperPlayerState = UpperPlayerState.IDLE;
                    lowerPlayerState = LowerPlayerState.CROUCH_MOVE;
                }
            }
            if (!_isCrouching && !_isSprinting)
            {
                if (isFire || isReroading)
                {
                    lowerPlayerState = LowerPlayerState.MOVE;
                }
                if (!isFire && !isReroading && _movementCheckValue != 0)
                {
                    upperPlayerState = UpperPlayerState.IDLE;
                    lowerPlayerState = LowerPlayerState.MOVE;
                }
                if (!isFire && !isReroading && _movementCheckValue == 0)
                {
                    upperPlayerState = UpperPlayerState.IDLE;
                    lowerPlayerState = LowerPlayerState.IDLE;
                }
            }
            else if (_isCrouching && !_isSprinting)  // ���� ����. -> ���� �߰� else if�� ����
            {
                if (isFire || isReroading)
                {
                    if (_movementCheckValue > 0)
                        lowerPlayerState = LowerPlayerState.CROUCH_MOVE;
                    else
                        lowerPlayerState = LowerPlayerState.CROUCH;
                }
                else
                {
                    upperPlayerState = UpperPlayerState.IDLE;
                    if (_movementCheckValue > 0)                    // �������� �ִ°�?
                        lowerPlayerState = LowerPlayerState.CROUCH_MOVE;
                    else
                        lowerPlayerState = LowerPlayerState.CROUCH;
                }
            }
        }

        Debug.Log($"lowerPlayerState: {lowerPlayerState}, upperPlayerState: {upperPlayerState}");
    }

    public void BeforeCharacterUpdate(float deltaTime)
    {
    }

    public bool IsColliderValidForCollisions(Collider coll)
    {
        return true;
    }

    public void OnDiscreteCollisionDetected(Collider hitCollider)
    {
    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
    }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
    }

    public void PostGroundingUpdate(float deltaTime)
    {
    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {
    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        // ���� enum�� ��� �� �ƴ� ��, ĳ���ʹ� ī�޶� �������� ȸ�� X
        if (upperPlayerState == UpperPlayerState.SHOOTINGATTACK)
        {
            _orientationSharpness = 150f;
            // ī�޶� �ٶ󺸴� ��� ���� (Y�� ����)
            Vector3 cameraForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized;

            if (cameraForward.sqrMagnitude > 0f)
            {
                // �ε巯�� ȸ���� ���� ����
                Vector3 smoothedLookDir = Vector3.Slerp(_motor.CharacterForward, cameraForward,
                    1 - Mathf.Exp(-_orientationSharpness * deltaTime)).normalized;

                // �÷��̾ ī�޶� ������ �ٶ󺸵��� ȸ��
                currentRotation = Quaternion.LookRotation(smoothedLookDir, _motor.CharacterUp);
            }
        }
        else // �� �ܿ���
        {
            _orientationSharpness = 10f;
            if (_lookInputVector.sqrMagnitude > 0f && _orientationSharpness > 0f)
            {
                Vector3 smoothedLookInputDir = Vector3.Slerp(_motor.CharacterForward, _lookInputVector, 1 - Mathf.Exp(-_orientationSharpness * deltaTime)).normalized;

                currentRotation = Quaternion.LookRotation(smoothedLookInputDir, _motor.CharacterUp);
            }
        }
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        if (upperPlayerState == UpperPlayerState.DEAD || lowerPlayerState == LowerPlayerState.DEAD)
        {
            currentVelocity = Vector3.zero;
            return;
        }

        if (_motor.GroundingStatus.IsStableOnGround)
        {
            float curVelocityMagnitude = currentVelocity.magnitude;
            Vector3 effectiveGroundNorm = _motor.GroundingStatus.GroundNormal;

            currentVelocity = _motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNorm) * curVelocityMagnitude;

            // �������� ����ã��
            Vector3 inputRight = Vector3.Cross(_moveInputVector, _motor.CharacterUp);
            // ���� ���Ϳ� ī�޶� �̵� ����� Ű���� ��ǲ�� ���� ���� ���� �ӵ�(������ ũ��)�� ���� ���Ͱ�
            Vector3 reorientedInput = Vector3.Cross(effectiveGroundNorm, inputRight).normalized * _moveInputVector.magnitude;
            // ���⼭ ������ ���ϰ� �ִٸ�, ������ ������Ʈ ���� ������ �ϸ� �Ǵ°� �ƴұ�?

            Vector3 targetMovementVelocity;

            // 3���� ���º��� ������ ���� �ش� ���������� ���� �̵� �� �ӵ����� ������ ������ �ִ� ��ġ ���� ����
            // ��, 3���� ���� ��, �� ���� ��ǲ�� ���� ���⺰ �ִ� �̵��ӵ� ��
            if (lowerPlayerState == LowerPlayerState.SPRINT || upperPlayerState == UpperPlayerState.SPRINT)
                targetMovementVelocity = reorientedInput * _maxSprintMoveSpeed;
            else if(lowerPlayerState == LowerPlayerState.CROUCH || lowerPlayerState == LowerPlayerState.CROUCH_MOVE)
                targetMovementVelocity = reorientedInput * _maxStableMoveSpeed * 2/3;
            else if(lowerPlayerState == LowerPlayerState.DODGE || isNowDodge)
                targetMovementVelocity = reorientedInput * _maxDodgeMoveSpeed;
            else
                targetMovementVelocity = reorientedInput * _maxStableMoveSpeed;

            
            // ���� �̵��� ref �Ķ������ currentVelocity�� targetMovementVelocity�� ���� ���� ����ó�� �Ͽ� 
            // ���� ���ӵǰ� ��
            if(lowerPlayerState == LowerPlayerState.DODGE)
                currentVelocity = Vector3.Lerp(targetMovementVelocity, currentVelocity, 1f - Mathf.Exp(-_stableMovementSharpness * deltaTime));
            else
                currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1f - Mathf.Exp(-_stableMovementSharpness * deltaTime));

            float currentSpeedMagnitude = currentVelocity.magnitude;
            float maxAllowedSpeed = _isSprinting ? _maxSprintMoveSpeed : _maxStableMoveSpeed;

            // ī�޶� �ӵ� ���� ����
            FindObjectOfType<PlayerCamera>().UpdateSprintState(currentSpeedMagnitude, maxAllowedSpeed);
        }
        else
        {
            currentVelocity.y += _gravity.y * deltaTime;
            //currentVelocity += _gravity * deltaTime;
        }
    }
}
