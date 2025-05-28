using UnityEngine;
using System.Collections;

public class ChildMonsterShoot : MonoBehaviour
{
    public Transform player;
    public GameObject bulletPrefab;
    public Transform shootPoint;

    public float shootCooldown = 3f; 

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

            anim.SetBool("isShooting", true);
            yield return new WaitForSeconds(0.5f); // 애니메이션 발사 준비 시간

            yield return StartCoroutine(FireBulletWithDelay());

            anim.SetBool("isShooting", false);
        }
    }

    IEnumerator FireBulletWithDelay()
    {
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
        DelayedBullet delayedBullet = bullet.GetComponent<DelayedBullet>();
        if (delayedBullet != null)
        {
            delayedBullet.Init(shootPoint, player);
        }

        yield return null;
    }
}