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
        if (weaponType == WEAPON_TYPE.PISTOL) return true; // 만약 권총인 경우(딱총 기본 무한 탄창) 조건 없이 장전 가능
        else return nowBullet < maxBullet && remainingAmmo > 0;
    }

    public void Reload()
    {
        if(!CanReload()) return;

        int ammoNeeded = maxBullet - nowBullet; // 최대 탄약이 12인 경우, 현재 탄창에 3발 있는 경우 9발이 해당 값으로 들어감. 즉, 실제 장전이 필요한 탄들
        int ammoToReload = Mathf.Min(ammoNeeded, remainingAmmo); // 만약에 remainingAmmo가 ammoNeeded보다 작은 경우, remainingAmmo 만큼 소모

        nowBullet += ammoToReload;
        remainingAmmo -= ammoToReload;
    }
    /// <summary>
    /// 무기의 잔탄을 추가해주는 함수(탄약 습득). 만약 탄약이 드랍시, 콜라이더에 닿으면서 해당 함수의 값을 불러와 추가해주어야 함.
    /// amount에 적당한 값을 넣어주고 해당 아이템을 없애야 함.
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
        return null; // 없을 경우
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
