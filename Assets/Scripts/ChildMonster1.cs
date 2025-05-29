using UnityEngine;
using System.Collections;
using UnityEngine.XR;

public class ChildMonster1 : MonoBehaviour
{
    public Transform player;                // 플레이어 위치
    public GameObject bulletPrefab;        // 발사할 총알 프리팹
    public Transform shootPoint;           // 총알 발사 위치 (빈 오브젝트)

    public float shootCooldown = 3.6f;       // 총알 발사 간격
    float curTime = 0;
    bool isCoroutinePlaying = false;

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

    }

    private void Update()
    {
        curTime += Time.deltaTime;

        if (curTime >= shootCooldown && !isCoroutinePlaying)
        {
            isCoroutinePlaying = true;
            anim.SetTrigger("isShooting");

            StartCoroutine(Shooting());
        }
    }

    IEnumerator Shooting()
    {
        yield return new WaitForSeconds(1f);
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
        DelayedBullet sb = bullet.GetComponent<DelayedBullet>();

        if (sb != null)
        {
            sb.target = player;
            sb.StartMovingAfterDelay(0.5f, shootPoint);
        }

        isCoroutinePlaying = false;
        
            curTime = 0;
    }

}
