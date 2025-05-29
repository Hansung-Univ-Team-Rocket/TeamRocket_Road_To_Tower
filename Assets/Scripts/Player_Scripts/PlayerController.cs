using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [SerializeField]    PlayerCamera _playerCam;
    [SerializeField]    Transform _cameraFollowPoint;
    [SerializeField]    PlayerMovementController _characterController;
    [SerializeField]    Transform _WeaponPrefab;
    [SerializeField]    WeaponManager _weaponManager;
    [SerializeField]    float _fireTimer = 0;
    [SerializeField]    float _reRoadTimer = 0;
    [SerializeField]    bool _isAim = false;
    [SerializeField]    bool isChangingWeapon = false;
    [SerializeField]    bool _wasDodging = false;

    Vector3 _lookInputVector;

    [Header("Camera Sensivity")]
    [SerializeField] float _cameraSensivity = 0.5f;
    //private void Awake()
    //{
    //    _WeaponPrefab = FindCHildWithTag(GameObject.Find("Character").transform, "Weapon");
    //}
    private void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        _playerCam.SetFollowTransform(_cameraFollowPoint);
        _weaponManager = GameObject.FindGameObjectWithTag("WeaponPos").GetComponent<WeaponManager>();

        if (_weaponManager == null) Debug.LogError("WeaponManager is null");
    }

    private void FixedUpdate()
    {
        Debug.Log($"{PlayerStatusInfo.playerHP} |||| HP");
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
        float scrollInput = -Input.GetAxis("Mouse ScrollWheel");

        // ���� ���ΰ�?
        if (!_wasDodging && _characterController.isNowDodge)
        {
            // ���� ���� || ����
            _playerCam.SetAimState(true);
            _wasDodging = true;
        }
        else if (_wasDodging && !_characterController.isNowDodge)
        {
            // ���� �� || ���� ���� ���·� ����
            _playerCam.SetAimState(_isAim);
            _wasDodging = false;
        }

        // ���� ���� �ƴ� ���� �����
        if (!_characterController.isNowDodge)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                _playerCam.SetAimState(true);
                _isAim = true;
                _weaponManager.currentWeapon.SetAimingState(true);
            }
            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                _playerCam.SetAimState(false);
                _isAim = false;
                _weaponManager.currentWeapon.SetAimingState(false);
            }
        }

        _lookInputVector = new Vector3(mouseRIght, mouseUp, 0f);

        if (!_isAim)
        {
            _playerCam.UpdateWithInput(Time.deltaTime, scrollInput, _lookInputVector * _cameraSensivity);
        }
        else
        {
            _playerCam.UpdateWithInput(Time.deltaTime, -100f, _lookInputVector * _cameraSensivity);
        }

        // ���� �߿��� CheckBump ��Ȱ��ȭ
        if (!_characterController.isNowDodge)
        {
            _playerCam.CheckBump();
        }
        /*
        float mouseUp = Input.GetAxisRaw("Mouse Y");
        float mouseRIght = Input.GetAxisRaw("Mouse X");
        float scrollInput = -Input.GetAxis("Mouse ScrollWheel");

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            _playerCam.SetAimState(true);
            _isAim = true;
            _weaponManager.currentWeapon.SetAimingState(true);
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            _playerCam.SetAimState(false);
            _isAim = false;
            _weaponManager.currentWeapon.SetAimingState(false);
        }

        _lookInputVector = new Vector3(mouseRIght, mouseUp, 0f);

        // if(Physics.Raycast(_playerCam.gameObject.transform.position, -_playerCam.gameObject.transform.forward, out _playerCam.hit, _playerCam.raycastDis))
        // {
        //     _playerCam.UpdatePlayerStickOnWall(Time.deltaTime, _lookInputVector);
        // }
        // Doesnt works

        if (!_isAim)
        {
            _playerCam.UpdateWithInput(Time.deltaTime, scrollInput, _lookInputVector * _cameraSensivity);
        }
        else
        {
            _playerCam.UpdateWithInput(Time.deltaTime, -100f, _lookInputVector * _cameraSensivity);
        }

        if (!_characterController.isNowDodge)
        {
            _playerCam.CheckBump();
        }
        else
        {
            _playerCam.SetAimState(true);
        }*/
    }
    void HandleCharacterInputs()
    {        
        _fireTimer += Time.deltaTime;
        _weaponManager.HandleWeaponSwitchInput();

        PlayerInput inputs = new PlayerInput();
        if (PlayerStatusInfo.playerHP <= 0) {
            inputs.AxisFwd = 0;
            inputs.AxisRight = 0;
            inputs.CameraRotation = Quaternion.identity;
            inputs.CrouchDown = false;
            inputs.CrouchUp = false;
            inputs.Sprint = false;
            inputs.Non_Sprint = false;
            inputs.Dodge = false;
            inputs.MeleeAttack = false;
            inputs.ShootingAttack = false;
            inputs.Reroading = false;

            _characterController.Dead();
            return;
        }

        isChangingWeapon = _weaponManager.animation.isChangingWeapon;

        inputs.AxisFwd = Input.GetAxisRaw("Vertical");
        inputs.AxisRight = Input.GetAxisRaw("Horizontal");
        inputs.CameraRotation = _playerCam.transform.rotation;
        inputs.CrouchDown = Input.GetKeyDown(KeyCode.LeftControl);
        inputs.CrouchUp = Input.GetKeyUp(KeyCode.LeftControl);
        inputs.Sprint = Input.GetKey(KeyCode.LeftShift);
        inputs.Non_Sprint = !inputs.Sprint;
        //inputs.Reroading = Input.GetKeyDown(KeyCode.R);

        if (inputs.Sprint) Debug.Log("�޸��� ��");
        if (inputs.Non_Sprint) Debug.Log("�޸��� �ƴ�");

        // ���� ��ü �ƴ� ���� ���� �ǰ� ����
        if (!isChangingWeapon)
        {
            HandleReloading();
            HandleShooting();
        }
        else // ������ġ �ϳ� �� �߰�, Ȥ�� ���� ���⿡�� �ѹ� �� �Ÿ���
        {
            _characterController.isFire = false;
            if(_characterController.upperPlayerState == UpperPlayerState.SHOOTINGATTACK)
            {
                _characterController.upperPlayerState = UpperPlayerState.IDLE;
            }
        }
        //if (Input.GetKey(KeyCode.Mouse0) && !_weaponScript.isMeele
        //    && !_weaponScript.nowReroading && _weaponScript.nowBullet > 0)
        //{
        // �׽�Ʈ ��ũ��Ʈ. ���� �۵���
        //if(_fireTimer >= _WeaponPrefab.GetComponent<WeaponScript>().roundsPerMinute)
        //{
        //    _fireTimer = 0;
        //    inputs.MeleeAttack = true;
        //    _WeaponPrefab.GetComponent<WeaponScript>().nowBullet--;

        //    Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

        //    Ray ray = _playerCam.gameObject.GetComponent<Camera>().ScreenPointToRay(screenCenter);
        //    if (Physics.Raycast(ray, out hit, 100f))
        //    {
        //        Debug.Log($"Hit {hit.collider.name} ��� �Ϸ�");
        //    }
        //}
        //}
        //if (Input.GetKey(KeyCode.Mouse0) && _WeaponPrefab.GetComponent<WeaponScript>().isMeele)
        //{
        //    if (_fireTimer >= _WeaponPrefab.GetComponent<WeaponScript>().roundsPerMinute)
        //    {
        //        _fireTimer = 0;
        //        inputs.ShootingAttack = true;
        //    }
        //}
        //inputs.Sprint = Input.GetKeyDown(KeyCode.LeftShift);
        //inputs.Non_Sprint = Input.GetKeyUp(KeyCode.LeftShift);
        inputs.Dodge = Input.GetKeyDown(KeyCode.Space);
        

        _characterController.SetInputs(ref inputs);
    }

    void HandleReloading()
    {
        WeaponScript currentWeapon = _weaponManager.currentWeapon;

        if (_characterController.upperPlayerState == UpperPlayerState.DAMAGED) return;

        if (Input.GetKeyDown(KeyCode.R) && !_characterController.isReroading && currentWeapon.CanReload())
        {
            _characterController.isReroading = true;
            _characterController.upperPlayerState = UpperPlayerState.REROADING;
        }

        if (_characterController.isReroading)
        {
            _reRoadTimer += Time.deltaTime;

            if (_reRoadTimer >= currentWeapon.weaponReroadTime)
            {
                _characterController.isReroading = false;
                currentWeapon.Reload();
                _characterController.upperPlayerState = UpperPlayerState.IDLE;
                _reRoadTimer = 0;
            }
        }
    }

    public void ForceStopFiring()
    {
        _characterController.isFire = false;

        if (_characterController.upperPlayerState == UpperPlayerState.SHOOTINGATTACK)
        {
            _characterController.upperPlayerState = UpperPlayerState.IDLE;
        }
        Debug.Log("���� ��ȯ �����?");
    }

    void HandleShooting()
    {
        WeaponScript currentWeapon = _weaponManager.currentWeapon;

        if (_weaponManager.animation.isChangingWeapon || _characterController.upperPlayerState == UpperPlayerState.DAMAGED) return;

        if(Input.GetKey(KeyCode.Mouse0) && !_characterController.isReroading && !_characterController.isNowDodge)
        {
            if(_fireTimer >= currentWeapon.roundsPerMinute && currentWeapon.CanFIre())
            {
                _characterController.isFire = true;
                _characterController.upperPlayerState = UpperPlayerState.SHOOTINGATTACK;
                _fireTimer = 0;
                currentWeapon.FIre();
            }
        }
        if(Input.GetKeyUp(KeyCode.Mouse0))
        {
            _characterController.isFire = false;
            _characterController.upperPlayerState = UpperPlayerState.IDLE;
        }
    }

    /*
    Vector3 GetSpreadDirection()
    {
        // ī�޶� ���� ���� ���� (�¿� ���� ����)
        float spreadX = Random.Range(-_weaponScript.spreadAmount, _weaponScript.spreadAmount);
        float spreadY = Random.Range(-_weaponScript.spreadAmount, _weaponScript.spreadAmount);

        Vector3 right = _playerCam.transform.right;
        Vector3 up = _playerCam.transform.up;

        return (right * spreadX + up * spreadY);
    }*/
    private void Update()
    {
        HandleCharacterInputs();
    }

    private void LateUpdate()
    {
        HandledCameraInput();
    }
}
    