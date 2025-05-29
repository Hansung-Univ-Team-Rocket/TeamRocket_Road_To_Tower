using UnityEngine;
using System.Collections;
using UnityEngine.XR;

public class ChildMonster1 : MonoBehaviour
{
    public Transform player;                // 플레이어 위치
    public GameObject bulletPrefab;        // 발사할 총알 프리팹
    public Transform shootPoint;           // 총알 발사 위치 (빈 오브젝트)

    public float shootCooldown = 3f;       // 총알 발사 간격

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        StartCoroutine(ShootRoutine());
    }

    IEnumerator ShootRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(shootCooldown);

            if (player == null)
                continue;

            anim.SetTrigger("isShooting");
            yield return new WaitForSeconds(1f); // 애니메이션 발사 준비 시간

            yield return StartCoroutine(FireBulletWithDelay());
        }
    }

    IEnumerator FireBulletWithDelay()
    {

        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
        DelayedBullet sb = bullet.GetComponent<DelayedBullet>();
        if (sb != null)
        {
            sb.target = player;
            sb.StartMovingAfterDelay(2f, shootPoint);
        }
        yield return new WaitForSeconds(2f);
        yield return null;
    }
}
