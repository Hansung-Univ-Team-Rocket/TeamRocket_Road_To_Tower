using Unity.VisualScripting;
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
    public float roundsPerMinute;

    [Header("Weapon's Ammo Value")]
    public int nowBullet = 12;
    public int maxBullet = 12;
    public int remainingAmmo = 12;
    public bool nowReroading = false;

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

    [Header("Effect | Pos")]
    public GameObject muzzleEffect;
    public Transform muzzlePos;
    public GameObject bulletHolePrefab;
    public GameObject enemyHitImpactVFX;
    public GameObject enemyHitVFX;

    PlayerCamera _playerCamera;


    [Header("Flag Value")]
    [SerializeField] bool _isAiming;

    void Start()
    {
        weaponType = WEAPON_TYPE.PISTOL;
        muzzlePos = FindChildWithTag(this.gameObject.transform, "MuzzlePos");
    }

    public void Init(PlayerCamera playerCamera)
    {
        _playerCamera = playerCamera;
    }

    public void SetAimingState(bool isAiming)
    {
        _isAiming = isAiming;
    }

    public bool CanFIre()
    {
        return !nowReroading && nowBullet > 0 && weaponType != WEAPON_TYPE.SWORD;
    }

    public bool CanReload()
    {
        if (weaponType == WEAPON_TYPE.PISTOL) return true; // ���� ������ ���(���� �⺻ ���� źâ) ���� ���� ���� ����
        else return nowBullet < maxBullet && remainingAmmo > 0;
    }

    public void Reload()
    {
        if(!CanReload()) return;

        int ammoNeeded = maxBullet - nowBullet; // �ִ� ź���� 12�� ���, ���� źâ�� 3�� �ִ� ��� 9���� �ش� ������ ��. ��, ���� ������ �ʿ��� ź��
        int ammoToReload = Mathf.Min(ammoNeeded, remainingAmmo); // ���࿡ remainingAmmo�� ammoNeeded���� ���� ���, remainingAmmo ��ŭ �Ҹ�

        nowBullet += ammoToReload;
        remainingAmmo -= ammoToReload;
    }
    /// <summary>
    /// ������ ��ź�� �߰����ִ� �Լ�(ź�� ����). ���� ź���� �����, �ݶ��̴��� �����鼭 �ش� �Լ��� ���� �ҷ��� �߰����־�� ��.
    /// amount�� ������ ���� �־��ְ� �ش� �������� ���־� ��.
    /// </summary>
    /// <param name="amount"></param>
    public void AddAmmo(int amount)
    {
        remainingAmmo += amount;
    }
    void ApplyRecoil()
    {
        if (_playerCamera == null) return;

        float recoilMultiplier;
        if (_isAiming) recoilMultiplier = 2 / 3f;
            else recoilMultiplier = 1f;

        float horizontalRecoil = Random.Range(-horizontalAmount * recoilMultiplier, horizontalAmount * recoilMultiplier);
        float verticalRecoil = verticalAmount * recoilMultiplier;

        _playerCamera.ApplyRecoilShake(horizontalRecoil, verticalRecoil);
    }

    void HandleHit(RaycastHit hit)
    {
        if (hit.collider.tag == "Enemy")
        {
            var enemy = hit.collider.GetComponent<Enemy_FSM>();

            if (enemy != null)
            {
                enemy.Damaged(weaponDamage);
            }

            GameObject enemyHitImpact = Instantiate(enemyHitImpactVFX, hit.point + hit.normal * 0.01f, Quaternion.LookRotation(-hit.normal));
            GameObject enemyHit = Instantiate(enemyHitVFX, hit.point + hit.normal * 0.01f, Quaternion.LookRotation(-hit.normal));
            Destroy(enemyHit, 3.3f);
        }
        else
        {
            GameObject bulletHole = Instantiate(bulletHolePrefab, hit.point + hit.normal * 0.01f, Quaternion.LookRotation(-hit.normal));
        }
    }
    
    void RaycastCal()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        //Vector3 spreadDir = GetSpreadDirection();
        Ray ray = _playerCamera.GetComponent<Camera>().ScreenPointToRay(screenCenter);

        if (Physics.Raycast(ray, out RaycastHit hit, maxFireDistance))
        {
            Debug.Log($"Hit {hit.collider.name} ||||||||| {hit.point}");
            HandleHit(hit);
        }
    }
    public void FIre()
    {
        if (!CanFIre()) return;

        nowBullet--;
        InsMuzzleEffet();
        ApplyRecoil();
        RaycastCal();
    }
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


    // Update is called once per frame
    void Update()
    {
        
    }
}
