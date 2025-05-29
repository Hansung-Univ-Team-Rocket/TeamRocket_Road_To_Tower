using UnityEngine;
using UnityEngine.UI;

public class AmmoManager : MonoBehaviour
{
    [Header("UI")]
    public Slider ammoSlider;        // 장착된 슬라이더
    [Header("Ammo Settings")]
    public int maxAmmo = 30;         // 최대 탄약 수
    private int currentAmmo;         // 현재 남은 탄약

    [Header("Shooting")]
    public GameObject bulletPrefab;  // 발사할 총알 프리팹
    public Transform firePoint;      // 총알이 나올 위치
    public float bulletSpeed = 20f;  // 총알 발사 속도

    void Start()
    {
        // 초기화
        currentAmmo = maxAmmo;
        ammoSlider.maxValue = maxAmmo;
        ammoSlider.value = currentAmmo;
    }

    void Update()
    {
        // 스페이스바 눌러 발사
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (currentAmmo <= 0)
        {
            Debug.Log("탄약이 없습니다!");
            return;
        }

        // 총알 생성
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = firePoint.right * bulletSpeed;

        // 탄약 감소 및 UI 업데이트
        currentAmmo--;
        ammoSlider.value = currentAmmo;
    }

    // 외부에서 탄약 보충할 때 호출
    public void Reload(int amount)
    {
        currentAmmo = Mathf.Clamp(currentAmmo + amount, 0, maxAmmo);
        ammoSlider.value = currentAmmo;
    }
}

