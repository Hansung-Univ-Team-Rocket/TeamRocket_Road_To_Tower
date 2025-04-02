using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]    PlayerCamera _playerCam;
    [SerializeField]    Transform _cameraFollowPoint;

    Vector3 _lookInputVector;

    private void Start()
    {
        _playerCam.SetFollowTransform(_cameraFollowPoint);

    }
    
    void HandledCameraInput()
    {
        float mouseUp = Input.GetAxisRaw("Mouse Y");
        float mouseRIght = Input.GetAxisRaw("Mouse X");

        _lookInputVector = new Vector3(mouseRIght, mouseUp, 0f);

        float scrollInput = -Input.GetAxis("Mouse ScrollWheel");
        _playerCam.UpdateWithInput(Time.deltaTime, scrollInput, _lookInputVector);
    }

    private void LateUpdate()
    {
        HandledCameraInput();
    }
}
    