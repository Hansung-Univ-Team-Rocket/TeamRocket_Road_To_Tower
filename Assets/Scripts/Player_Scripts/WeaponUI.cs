using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] private Slider ammoSlider;
    private WeaponManager weaponManager;

    void Start()
    {
        weaponManager = FindObjectOfType<WeaponManager>();
        if (weaponManager == null)
        {
            Debug.LogError("WeaponUI: WeaponManager를 찾을 수 없습니다!");
            enabled = false;
            return;
        }

        // 슬라이더는 총알 개수 처럼 정수 단위로만 움직이게
        ammoSlider.wholeNumbers = true;
    }

    void Update()
    {
        // 1) WeaponManager에 붙은 WeaponScript 컴포넌트들을 매 프레임 가져오기
        var weapons = weaponManager.GetComponentsInChildren<WeaponScript>();

        // 2) 배열이 비었거나, currentWeaponIndex가 범위를 벗어나면 그냥 리턴
        int idx = weaponManager.currentWeaponIndex;
        if (weapons.Length == 0 || idx < 0 || idx >= weapons.Length)
            return;

        // 3) 유효한 무기를 꺼내서 슬라이더에 반영
        var w = weapons[idx];
        ammoSlider.maxValue = w.maxBullet;
        ammoSlider.value = w.nowBullet;
    }
}
