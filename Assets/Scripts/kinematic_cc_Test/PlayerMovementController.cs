using KinematicCharacterController;
using UnityEngine;

public struct PlayerInput
{
    public float AxisFwd; 
    public float AxisRight;
    public Quaternion CameraRotation;
}

public class PlayerMovementController : MonoBehaviour, ICharacterController
{
    [SerializeField] KinematicCharacterMotor _motor;
    [SerializeField] float _maxStableMoveSpeed = 10f;
    [SerializeField] float _stableMovementSharpness = 15f;
    [SerializeField] private float _orientationSharpness = 10f;
    Vector3 _moveInputVector;
    Vector3 _lookInputVector;

    private void Start()
    {
        _motor.CharacterController = this;
    }

    public void SetInputs(ref PlayerInput inputs)
    {
        Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(inputs.AxisRight, 0f, inputs.AxisFwd), 1f);
        Vector3 cameraPlanarDir = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.forward, _motor.CharacterUp).normalized;
  
        if(cameraPlanarDir.sqrMagnitude == 0f)
        {
            cameraPlanarDir = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.up, _motor.CharacterUp).normalized;
        }

        Quaternion cameraPlanerRotation = Quaternion.LookRotation(cameraPlanarDir, _motor.CharacterUp);

        _moveInputVector = cameraPlanerRotation * moveInputVector;
        _lookInputVector = _moveInputVector.normalized;
    }
    public void AfterCharacterUpdate(float deltaTime)
    {
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
        float curVelocityMagnitude = currentVelocity.magnitude;
        Vector3 effectiveGroundNorm = _motor.GroundingStatus.GroundNormal;

        currentVelocity = _motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNorm) * curVelocityMagnitude;

        Vector3 inputRight = Vector3.Cross(_moveInputVector, _motor.CharacterUp);
        Vector3 reorientedInput = Vector3.Cross(effectiveGroundNorm, inputRight).normalized * _moveInputVector.magnitude;

        Vector3 targetMovementVelocity = reorientedInput * _maxStableMoveSpeed;

        currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1f - Mathf.Exp(-_stableMovementSharpness * deltaTime));
    }
}
