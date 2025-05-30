using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] WeaponScript[] weapons;
    public int currentWeaponIndex = 0;
    public PlayerAnimation animation;

    PlayerCamera _playerCam;
    public WeaponScript currentWeapon => weapons[currentWeaponIndex]; // 현재 무기는 현재 무기 인덱스의 WeaponScript를 리턴
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 스타트에서 Weapons를 받아와서 배열에 넣어줘야 함.
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

        // 이미 무기전환 중이면 무시
        if (animation.isChangingWeapon)
            return;

        if (currentWeaponIndex == weaponIndex) return; // 같은 무기라도 무시

        ForceStopAttacking(); // 교체 전 무기 사격 강제 캔슬

        StartCoroutine(ChangeWeaponWithAnim(weaponIndex));

        Debug.Log($"Weapon Swap {weapons[currentWeaponIndex].name}");
    }

    IEnumerator ChangeWeaponWithAnim(int newWeaponIndex)
    {
        //animation.isChangingWeapon = true;

        //yield return new WaitForSeconds(1.37f);
        //weapons[currentWeaponIndex].gameObject.SetActive(false);
        //Debug.Log($"Weapon Swap first step {weapons[currentWeaponIndex].name}");

        //yield return new WaitForSeconds(0.15f); // 잠깐 빈손으로 만들어서 자연스럽게 

        //currentWeaponIndex = newWeaponIndex;
        //weapons[currentWeaponIndex].gameObject.SetActive(true);
        //Debug.Log($"Weapon Swap Sec Step {weapons[currentWeaponIndex].name}");

        //yield return StartCoroutine(animation.ChangeWeapon(currentWeaponIndex, newWeaponIndex)); // 애니메이션 시간이 끝날 때 아래 실행으로 코드 수정

        //// 실질적 무기 교체절
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
    /// 현재 안 쓰이는 함수. 무기 추가용
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
