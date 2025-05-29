using Unity.VisualScripting;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] WeaponScript[] weapons;
    [SerializeField] int currentWeaponIndex = 0;

    PlayerCamera _playerCam;
    public WeaponScript currentWeapon => weapons[currentWeaponIndex]; // ���� ����� ���� ���� �ε����� WeaponScript�� ����
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // ��ŸƮ���� Weapons�� �޾ƿͼ� �迭�� �־���� ��.
        _playerCam = FindFirstObjectByType<PlayerCamera>();

        weapons = GetComponentsInChildren<WeaponScript>();
        
        if(weapons.Length == 0)
        {
            Debug.LogError("Error, the Children(WeaponScript) is null");
            return;
        }

        Debug.Log($"Pass {weapons.Length}");

        for(int i =0; i<weapons.Length; i++)
        {
            weapons[i].Init(_playerCam);
            weapons[i].gameObject.SetActive(false);
            Debug.Log($"{weapons[i]} Init");
        }

        if (weapons.Length > 0)
        {
            currentWeaponIndex = 0;
            weapons[currentWeaponIndex].gameObject.SetActive(true);
            Debug.Log($"In Suc, the first weapon sel {weapons[currentWeaponIndex].name}");
        }
    }

    public void SwitchWeapon(int weaponIndex)
    {
        if (weapons == null || weaponIndex < 0 || weaponIndex >= weapons.Length)
        {
            return;
        }

        weapons[currentWeaponIndex].gameObject.SetActive(false);

        currentWeaponIndex = weaponIndex;
        weapons[currentWeaponIndex].gameObject.SetActive(true);

        Debug.Log($"Weapon Swap {weapons[currentWeaponIndex].name}");
    }

    public void HandleWeaponSwitchInput()
    {
        if (weapons == null) return;

        if (Input.GetKeyDown(KeyCode.Alpha1) && weapons.Length > 0)
        {
            SwitchWeapon(0);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2) && weapons.Length > 1)
        {
            SwitchWeapon(1);
        }
    }

    /// <summary>
    /// ���� �� ���̴� �Լ�. ���� �߰���
    /// </summary>
    public void AddWeapon(WeaponScript newWeapon)
    {
        WeaponScript[] newWeapons = new WeaponScript[weapons.Length + 1];

        for (int i = 0; i < weapons.Length; i++)
        {
            newWeapons[i] = weapons[i];
        }
        newWeapons[weapons.Length] = newWeapon;
        weapons = newWeapons;

        newWeapon.Init(_playerCam);
        newWeapon.gameObject.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
