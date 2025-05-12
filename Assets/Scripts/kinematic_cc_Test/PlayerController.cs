using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerController : MonoBehaviour
{
    [SerializeField]    PlayerCamera _playerCam;
    [SerializeField]    Transform _cameraFollowPoint;
    [SerializeField]    PlayerMovementController _characterController;
    [SerializeField]    Transform _WeaponPrefab;
    [SerializeField]    float _fireTimer = 0;
    RaycastHit hit;

    Vector3 _lookInputVector;

    //private void Awake()
    //{
    //    _WeaponPrefab = FindCHildWithTag(GameObject.Find("Character").transform, "Weapon");
    //}
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _playerCam.SetFollowTransform(_cameraFollowPoint);
        _WeaponPrefab = FindChildWithTag(_characterController.gameObject.transform, "Weapon");

    }

    Transform FindChildWithTag(Transform character, string tag)
    {
        foreach(Transform childs in character.transform.GetComponentInChildren<Transform>())
        {
            if(childs.CompareTag(tag))
            {
                return childs;
            }
        }

        return null;
    }

    void HandledCameraInput()
    {
        float mouseUp = Input.GetAxisRaw("Mouse Y");
        float mouseRIght = Input.GetAxisRaw("Mouse X");

        _lookInputVector = new Vector3(mouseRIght, mouseUp, 0f);

        // if(Physics.Raycast(_playerCam.gameObject.transform.position, -_playerCam.gameObject.transform.forward, out _playerCam.hit, _playerCam.raycastDis))
        // {
        //     _playerCam.UpdatePlayerStickOnWall(Time.deltaTime, _lookInputVector);
        // }
        // Doesnt works

        float scrollInput = -Input.GetAxis("Mouse ScrollWheel");
        _playerCam.UpdateWithInput(Time.deltaTime, scrollInput, _lookInputVector);
    }
    void HandleCharacterInputs()
    {
        _fireTimer += Time.deltaTime;
        PlayerInput inputs = new PlayerInput();
        inputs.AxisFwd = Input.GetAxisRaw("Vertical");
        inputs.AxisRight = Input.GetAxisRaw("Horizontal");
        inputs.CameraRotation = _playerCam.transform.rotation;
        inputs.CrouchDown = Input.GetKeyDown(KeyCode.LeftControl);
        inputs.CrouchUp = Input.GetKeyUp(KeyCode.LeftControl);
        inputs.Sprint = Input.GetKey(KeyCode.LeftShift);
        inputs.Non_Sprint = !inputs.Sprint;
        if (inputs.Sprint) Debug.Log("달리기 온");
        if (inputs.Non_Sprint) Debug.Log("달리기 아님");

        if (Input.GetKey(KeyCode.Mouse0) && !_WeaponPrefab.GetComponent<WeaponScript>().isMeele
            && !_WeaponPrefab.GetComponent<WeaponScript>().nowReroading && _WeaponPrefab.GetComponent<WeaponScript>().nowBullet > 0)
        {
            // 테스트 스크립트. 정상 작동함
            //if(_fireTimer >= _WeaponPrefab.GetComponent<WeaponScript>().roundsPerMinute)
            //{
            //    _fireTimer = 0;
            //    inputs.MeleeAttack = true;
            //    _WeaponPrefab.GetComponent<WeaponScript>().nowBullet--;

            //    Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

            //    Ray ray = _playerCam.gameObject.GetComponent<Camera>().ScreenPointToRay(screenCenter);
            //    if (Physics.Raycast(ray, out hit, 100f))
            //    {
            //        Debug.Log($"Hit {hit.collider.name} 사격 완료");
            //    }
            //}
        }
        if (Input.GetKey(KeyCode.Mouse0) && _WeaponPrefab.GetComponent<WeaponScript>().isMeele)
        {
            if (_fireTimer >= _WeaponPrefab.GetComponent<WeaponScript>().roundsPerMinute)
            {
                _fireTimer = 0;
                inputs.ShootingAttack = true;
            }
        }
        //inputs.Sprint = Input.GetKeyDown(KeyCode.LeftShift);
        //inputs.Non_Sprint = Input.GetKeyUp(KeyCode.LeftShift);
        inputs.Dodge = Input.GetKeyDown(KeyCode.Space);
        

        _characterController.SetInputs(ref inputs);
    }

    private void Update()
    {
        HandleCharacterInputs();
    }

    private void LateUpdate()
    {
        HandledCameraInput();
    }
}
    