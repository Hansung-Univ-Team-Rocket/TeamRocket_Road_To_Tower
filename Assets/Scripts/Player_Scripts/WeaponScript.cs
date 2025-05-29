using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    public enum WEAPON_TYPE
    {
        PISTOL,
        RIFLE,
        BOW,
        SWORD
    }

    [Header("Weapon's Basic Value | If the weapon is meele, set roundsPerMinute as animation delay")]
    public WEAPON_TYPE weaponType;
    public int weaponDamage = 0;
    public float weaponReroadTime = 0;
    public int nowBullet = 12;
    public int maxBullet = 12;
    public bool nowReroading = false;
    public float roundsPerMinute;


    [Header("Weapon's gun recoil Value | use only for Gun")]
    public float maxFireDistance = 100f;    // 총기 사거리
    public float horizontalAmount = 0.2f; // 화면 수평 반동
    public float verticalAmount = 0.3f; // 화면 수직 반동
    /*public float spreadAmount = 0.1f;         // 현재 화면 애임 벌어짐
    public float maxSpread = 0.3f;            // 최대 에임 벌어짐
    public float spreadPerShot = 0.02f;       // 발당 에임 벌어짐 증가수치
    public float spreadRecoverySpeed = 0.05f; // 에임 회복 속도
    */
    // 지금 와서 생각하니 화면 반동이 있는데 굳이 탄튐도 들어갈 필요가 있나? 하는 생각이 있어 주석처리

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject muzzleEffect;
    public Transform muzzlePos;

    public void InsMuzzleEffet()
    {
        GameObject muzzleFlash = Instantiate(muzzleEffect, muzzlePos.transform.position, Quaternion.identity);
        Destroy(muzzleFlash, 0.1f);

    }

    Transform FindChildWithTag(Transform parent, string tag)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>())
        {
            if (child.CompareTag(tag))
            {
                return child;
            }
        }
        return null; // 없을 경우
    }

    void Start()
    {
        weaponType = WEAPON_TYPE.PISTOL;
        muzzlePos = FindChildWithTag(this.gameObject.transform, "MuzzlePos");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
