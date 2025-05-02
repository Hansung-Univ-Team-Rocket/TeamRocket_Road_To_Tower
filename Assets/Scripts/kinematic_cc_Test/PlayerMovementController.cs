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
}

public enum PlayerState
{
    IDLE = 0,
    MOVE,
    SPRINT,
    CROUCH,
    CROUCH_MOVE,
    DODGE,
    ATTACK,
    DAMAGED,
    DEAD
}


public class PlayerMovementController : MonoBehaviour, ICharacterController
{
    [SerializeField] KinematicCharacterMotor _motor;
    [SerializeField] float _maxStableMoveSpeed = 10f;
    [SerializeField] float _maxSprintMoveSpeed = 18f;
    [SerializeField] float _maxDodgeMoveSpeed = 33f;
    [SerializeField] float _stableMovementSharpness = 15f;
    [SerializeField] private float _orientationSharpness = 10f;
    [SerializeField] Vector3 _gravity = new Vector3(0, -60f, 0);

    [Header("CoolTimes")]
    [SerializeField] float _dodgeTime = .5f;
    [SerializeField] float _dodgeTimeChecker = 0f;

    [Header("Flag value")]
    [SerializeField] bool _isSprinting = false;
    [SerializeField] bool _isCrouching = false; // 캐릭터가 일어설 수 있는 상태인가? 앉아 있어야 하는 상태인가? 충돌 검사와 관련된 플레그 값
    [SerializeField] bool _secondCrouchingChecker = false; // 플레이어가 일어서고 싶어 하는가? 즉, 컨트롤 키를 땠는가?
    [SerializeField] bool _isDodge = false;
    [SerializeField] bool _isNowDodge = false;

    [Header("Player Size")]
    [SerializeField] float _playerCrouchedCapsuleHieght = 1f;
    [SerializeField] float _playerNonCrouhedCapsuleHieght = 2f;

    [Header("Player state")]
    public PlayerState playerState;

    private Collider[] _probedColliders = new Collider[8];
    Vector3 _moveInputVector;
    Vector3 _lookInputVector;

    private void Start()
    {
        playerState = PlayerState.IDLE;
        _motor.CharacterController = this;
    }


    // 정지 상태인 경우에 여기서 플레이어 스테이트 머신 해결
    public void SetInputs(ref PlayerInput inputs)
    {
        if (playerState == PlayerState.DEAD) return;

        if (_dodgeTimeChecker >= _dodgeTime)
        {
            _isNowDodge = false;
            _isDodge = false;
            playerState = PlayerState.IDLE;
            _dodgeTimeChecker = 0f;
        }
        if (inputs.Dodge && !_isNowDodge && !_isCrouching)
        {
            _isDodge = true;
            _isNowDodge = true;
            _isCrouching = false;
            _isSprinting = false;
        }

        if(_isNowDodge)
        {
            _dodgeTimeChecker += Time.deltaTime;

            playerState = PlayerState.DODGE;
            
            return;
        }

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
            //Debug.Log("들어 옴?");
            if (inputs.CrouchDown)
            {
                _isSprinting = false;
                _secondCrouchingChecker = true;

                if (!_isCrouching)
                {
                    _isCrouching = true;
                    _motor.SetCapsuleDimensions(0.5f, _playerCrouchedCapsuleHieght, _playerCrouchedCapsuleHieght * .5f);
                    playerState = PlayerState.CROUCH_MOVE;
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
            else if (inputs.Non_Sprint) // 사실상의 달리기를 안 했을 때의 모든 상태가 들어가는 else 구문
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
                    playerState = PlayerState.CROUCH;
                }
                else
                {
                    playerState = PlayerState.CROUCH;
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
                    playerState = PlayerState.IDLE;
                }
                else
                {
                    playerState = PlayerState.CROUCH;
                }
            }
        }

    }
    // 최대한 이동 상태와 관련된 플레이어 스테이트는 이 콜백에서 해결
    public void AfterCharacterUpdate(float deltaTime)
    {
        if (_isCrouching && !_secondCrouchingChecker)
        {
            // 앉은 상태에서 머리 위에 콜라이더가 있는 오브젝트가 있는가?
            _motor.SetCapsuleDimensions(0.5f, _playerNonCrouhedCapsuleHieght, 1f);
            if(_motor.CharacterOverlap(
                _motor.TransientPosition,
                _motor.TransientRotation,
                _probedColliders,
                _motor.CollidableLayers,
                QueryTriggerInteraction.Ignore) > 0)
            {
                // 그렇다면 계속 앉은 상태
                _motor.SetCapsuleDimensions(0.5f, _playerCrouchedCapsuleHieght, _playerCrouchedCapsuleHieght * .5f);
            }
            else
            {
                _isCrouching = false;
                playerState = PlayerState.IDLE;
            }
        }
        if(_isSprinting)
        {
            playerState = PlayerState.SPRINT;
        }
        else
        {
            if (_isCrouching)
            {
                playerState = PlayerState.CROUCH_MOVE;
            }
            if (!_isCrouching && !_isSprinting)
            {
                playerState = PlayerState.MOVE;
            }
        }
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
        if(_lookInputVector.sqrMagnitude > 0f && _orientationSharpness > 0f)
        {
            Vector3 smoothedLookInputDir = Vector3.Slerp(_motor.CharacterForward, _lookInputVector, 1 - Mathf.Exp(-_orientationSharpness * deltaTime)).normalized;

            currentRotation = Quaternion.LookRotation(smoothedLookInputDir, _motor.CharacterUp);
        }
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {

        if (_motor.GroundingStatus.IsStableOnGround)
        {
            float curVelocityMagnitude = currentVelocity.magnitude;
            Vector3 effectiveGroundNorm = _motor.GroundingStatus.GroundNormal;

            currentVelocity = _motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNorm) * curVelocityMagnitude;

            // 외적으로 방향찾기
            Vector3 inputRight = Vector3.Cross(_moveInputVector, _motor.CharacterUp);
            // 방향 벡터와 카메라 이동 방향과 키보드 인풋을 곱한 벡터 값의 속도(벡터의 크기)를 곱한 벡터값
            Vector3 reorientedInput = Vector3.Cross(effectiveGroundNorm, inputRight).normalized * _moveInputVector.magnitude;
            // 여기서 방향을 구하고 있다면, 위에서 스프린트 상태 관리만 하면 되는게 아닐까?


            Vector3 targetMovementVelocity;

            // 3개의 상태별로 나눠서 구한 해당 방향으로의 벡터 이동 및 속도값을 변수로 지정한 최대 수치 값과 곱함
            // 즉, 3개의 상태 당, 각 유저 인풋에 대한 방향별 최대 이동속도 값
            if (playerState == PlayerState.SPRINT)
                targetMovementVelocity = reorientedInput * _maxSprintMoveSpeed;
            else if(playerState == PlayerState.CROUCH || playerState == PlayerState.CROUCH_MOVE)
                targetMovementVelocity = reorientedInput * _maxStableMoveSpeed * 2/3;
            else if(playerState == PlayerState.DODGE || _isNowDodge)
                targetMovementVelocity = reorientedInput * _maxDodgeMoveSpeed;
            else
                targetMovementVelocity = reorientedInput * _maxStableMoveSpeed;

            
            // 현재 이동한 ref 파라메터인 currentVelocity에 targetMovementVelocity과 지수 감쇠 보간처리 하여 
            // 점차 가속되게 함
            if(playerState == PlayerState.DODGE)
                currentVelocity = Vector3.Lerp(targetMovementVelocity, currentVelocity, 1f - Mathf.Exp(-_stableMovementSharpness * deltaTime));
            else
                currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1f - Mathf.Exp(-_stableMovementSharpness * deltaTime));
        }
        else
        {
            currentVelocity.y += _gravity.y * deltaTime;
            //currentVelocity += _gravity * deltaTime;
        }
    }
}
