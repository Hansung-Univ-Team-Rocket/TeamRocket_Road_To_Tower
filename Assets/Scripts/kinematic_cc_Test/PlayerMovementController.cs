using KinematicCharacterController;
using UnityEngine;

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
    CROUNCH_MOVE,
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
    [SerializeField] float _stableMovementSharpness = 15f;
    [SerializeField] private float _orientationSharpness = 10f;
    [SerializeField] Vector3 _gravity = new Vector3(0, -60f, 0);

    [Header("Flag value")]
    [SerializeField] bool _isSprinting = false;
    [SerializeField] bool _isCrouching = false; // 캐릭터가 일어설 수 있는 상태인가? 앉아 있어야 하는 상태인가? 충돌 검사와 관련된 플레그 값
    [SerializeField] bool _secondCrouchingChecker = false; // 플레이어가 일어서고 싶어 하는가? 즉, 컨트롤 키를 땠는가?
    [SerializeField] bool _isDodge = false;

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

    public void SetInputs(ref PlayerInput inputs)
    {
        if (playerState == PlayerState.DEAD) return;

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
            if(inputs.CrouchDown)
            {
                //if(inputs.Sprint)
                //{
                //    _secondCrouchingChecker = false;
                //    _motor.SetCapsuleDimensions(0.5f, _playerNonCrouhedCapsuleHieght, 1f);
                //    if (_motor.CharacterOverlap(
                //        _motor.TransientPosition,
                //        _motor.TransientRotation,
                //        _probedColliders,
                //        _motor.CollidableLayers,
                //        QueryTriggerInteraction.Ignore) > 0) return;
                //    else
                //        playerState = PlayerState.SPRINT;
                //}
                // 필요 없을 듯? 일단 빼보기
                _isSprinting = false;
                _secondCrouchingChecker = true;

                if (!_isCrouching)
                {
                    _isCrouching = true;
                    _motor.SetCapsuleDimensions(0.5f, _playerCrouchedCapsuleHieght, _playerCrouchedCapsuleHieght * .5f);
                    playerState = PlayerState.CROUNCH_MOVE;
                }

                //if (inputs.Non_Sprint)
                //{
                //    _isSprinting= false;
                //    _secondCrouchingChecker = true;

                //    if(!_isCrouching)
                //    {
                //        _isCrouching = true;
                //        _motor.SetCapsuleDimensions(0.5f, _playerCrouchedCapsuleHieght, _playerCrouchedCapsuleHieght * .5f);
                //        playerState = PlayerState.CROUNCH_MOVE;
                //    }
                //}
                // 여기도
            }
            else if(inputs.CrouchUp)
            {
                _secondCrouchingChecker = false;
                //if(inputs.Sprint)
                //{
                //    _isSprinting = true;
                //    playerState = PlayerState.SPRINT;
                //}
                // 여기도
                // _isSprinting = false;

                _motor.SetCapsuleDimensions(0.5f, _playerNonCrouhedCapsuleHieght, 1f);
                if (_motor.CharacterOverlap(
                    _motor.TransientPosition,
                    _motor.TransientRotation,
                    _probedColliders,
                    _motor.CollidableLayers,
                    QueryTriggerInteraction.Ignore) > 0) return;
                else
                {
                    if (inputs.Sprint)
                    {
                        _isSprinting = true;
                        playerState = PlayerState.SPRINT;
                    }
                    else if(inputs.Non_Sprint)
                    {
                        _isSprinting = false;
                         playerState = PlayerState.MOVE;
                    }
                }

                //else if(inputs.Non_Sprint)
                //{
                //    _isSprinting = false;
                //    _motor.SetCapsuleDimensions(0.5f, _playerNonCrouhedCapsuleHieght, 1f);
                //    if (_motor.CharacterOverlap(
                //        _motor.TransientPosition,
                //        _motor.TransientRotation,
                //        _probedColliders,
                //        _motor.CollidableLayers,
                //        QueryTriggerInteraction.Ignore) > 0) return;
                //    else
                //        playerState = PlayerState.MOVE;
                //}
                // 여기도
            }
            else
            {
                if(!_isCrouching && !_isSprinting)
                {
                    playerState = PlayerState.MOVE;
                }
                if(!_isCrouching && _isSprinting)
                {
                    playerState = PlayerState.SPRINT;
                }
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
            }
        }

    }
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

            Vector3 inputRight = Vector3.Cross(_moveInputVector, _motor.CharacterUp);
            Vector3 reorientedInput = Vector3.Cross(effectiveGroundNorm, inputRight).normalized * _moveInputVector.magnitude;

            Vector3 targetMovementVelocity;

            if (playerState == PlayerState.SPRINT)
                targetMovementVelocity = reorientedInput * _maxSprintMoveSpeed;
            else if(playerState == PlayerState.CROUCH || playerState == PlayerState.CROUNCH_MOVE)
                targetMovementVelocity = reorientedInput * _maxStableMoveSpeed * 2/3;
            else
                targetMovementVelocity = reorientedInput * _maxStableMoveSpeed;

            currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1f - Mathf.Exp(-_stableMovementSharpness * deltaTime));
        }
        else
        {
            currentVelocity.y += _gravity.y * deltaTime;
            //currentVelocity += _gravity * deltaTime;
        }
    }
}
