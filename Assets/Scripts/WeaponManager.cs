using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public EquippedWeaponUI equippedWeaponUI;

    // 예시용 무기
    public Sprite gunSprite;

    void Start()
    {
        // 예: 게임 시작 시 기본 무기 장착
        WeaponData starterWeapon = new WeaponData("Basic gun", gunSprite);
        EquipWeapon(starterWeapon);
    }

    public void EquipWeapon(WeaponData weapon)
    {
        Debug.Log($"장착된 무기: {weapon.name}");
        equippedWeaponUI.UpdateWeaponUI(weapon);
    }
}
