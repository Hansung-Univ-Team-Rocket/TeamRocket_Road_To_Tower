using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] WeaponScript[] weapons;
    [SerializeField] int currentWeaponIndext = 0;

    PlayerCamera _playerCam;
    public WeaponScript currentWeapon => weapons[currentWeaponIndext]; // ���� ����� ���� ���� �ε����� WeaponScript�� ����
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // ��ŸƮ���� Weapons�� �޾ƿͼ� �迭�� �־���� ��.
        _playerCam = FindFirstObjectByType<PlayerCamera>();

        foreach (var weapon in weapons)
        {
            weapon.Init(_playerCam);
            weapon.gameObject.SetActive(false);
        }

        if (weapons.Length > 0)
        {
            weapons[currentWeaponIndext].gameObject.SetActive(true);
        }
    }

    public void SwitchWeapon(int weaponIndex)
    {
        if (weaponIndex < 0 || weaponIndex >= weapons.Length)
        {
            return;
        }

        weapons[currentWeaponIndext].gameObject.SetActive(false);

        currentWeaponIndext = weaponIndex;
        weapons[currentWeaponIndext].gameObject.SetActive(true);
    }

    public void HandleWeaponSwitchInput()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchWeapon(0);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchWeapon(1);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
