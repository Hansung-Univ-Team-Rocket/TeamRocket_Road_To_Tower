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
    public float maxFireDistance = 100f;    // �ѱ� ��Ÿ�
    public float horizontalAmount = 0.2f; // ȭ�� ���� �ݵ�
    public float verticalAmount = 0.3f; // ȭ�� ���� �ݵ�
    /*public float spreadAmount = 0.1f;         // ���� ȭ�� ���� ������
    public float maxSpread = 0.3f;            // �ִ� ���� ������
    public float spreadPerShot = 0.02f;       // �ߴ� ���� ������ ������ġ
    public float spreadRecoverySpeed = 0.05f; // ���� ȸ�� �ӵ�
    */
    // ���� �ͼ� �����ϴ� ȭ�� �ݵ��� �ִµ� ���� źƦ�� �� �ʿ䰡 �ֳ�? �ϴ� ������ �־� �ּ�ó��

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
        return null; // ���� ���
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
