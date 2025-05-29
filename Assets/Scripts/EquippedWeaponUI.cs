using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquippedWeaponUI : MonoBehaviour
{
    public Image weaponIconImage;
    public TMP_Text weaponNameText;

    // 무기 정보 반영 함수
    public void UpdateWeaponUI(WeaponData weapon)
    {
        weaponIconImage.sprite = weapon.icon;
        weaponNameText.text = weapon.name;
    }
}