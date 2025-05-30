using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] WeaponScript[] weapons;
    public int currentWeaponIndex = 0;
    public PlayerAnimation animation;

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

            if(animation != null)
            {
                animation.SetWeaponIndex(currentWeaponIndex);
            }
            Debug.Log($"In Suc, the first weapon sel {weapons[currentWeaponIndex].name}");
        }
    }

    public void SwitchWeapon(int weaponIndex)
    {
        if (weapons == null || weaponIndex < 0 || weaponIndex >= weapons.Length)
        {
            return;
        }

        // �̹� ������ȯ ���̸� ����
        if (animation.isChangingWeapon)
            return;

        if (currentWeaponIndex == weaponIndex) return; // ���� ����� ����

        ForceStopAttacking(); // ��ü �� ���� ��� ���� ĵ��

        StartCoroutine(ChangeWeaponWithAnim(weaponIndex));

        Debug.Log($"Weapon Swap {weapons[currentWeaponIndex].name}");
    }

    IEnumerator ChangeWeaponWithAnim(int newWeaponIndex)
    {
        //animation.isChangingWeapon = true;

        //yield return new WaitForSeconds(1.37f);
        //weapons[currentWeaponIndex].gameObject.SetActive(false);
        //Debug.Log($"Weapon Swap first step {weapons[currentWeaponIndex].name}");

        //yield return new WaitForSeconds(0.15f); // ��� ������� ���� �ڿ������� 

        //currentWeaponIndex = newWeaponIndex;
        //weapons[currentWeaponIndex].gameObject.SetActive(true);
        //Debug.Log($"Weapon Swap Sec Step {weapons[currentWeaponIndex].name}");

        //yield return StartCoroutine(animation.ChangeWeapon(currentWeaponIndex, newWeaponIndex)); // �ִϸ��̼� �ð��� ���� �� �Ʒ� �������� �ڵ� ����

        //// ������ ���� ��ü��
        //weapons[currentWeaponIndex].gameObject.SetActive(false);
        //currentWeaponIndex = newWeaponIndex;
        //weapons[currentWeaponIndex].gameObject.SetActive(true);

        //Debug.Log($"Weapon Swap {weapons[currentWeaponIndex].name}");

        yield return StartCoroutine(animation.ChangeWeapon(currentWeaponIndex, newWeaponIndex));

        currentWeaponIndex = newWeaponIndex;

        Debug.Log($"Weapon Swap |||| {weapons[currentWeaponIndex].name}");
    }

    public void HideWeapon(int weaponIndex)
    {
        weapons[weaponIndex].gameObject.SetActive(false);
        Debug.Log($"Hide ||| {weapons[weaponIndex].name}");
    }

    public void ShowWeapon(int weaponIndex)
    {
        weapons[weaponIndex].gameObject.SetActive(true);
        Debug.Log($"Show ||| {weapons[weaponIndex].name}");
    }

    void ForceStopAttacking()
    {
        PlayerController playerController = FindFirstObjectByType<PlayerController>();

        if (playerController != null)
        {
            playerController.ForceStopFiring();
        }
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
