using UnityEngine;
using System.Collections;
using UnityEngine.XR;

public class ChildMonster1 : MonoBehaviour
{
    public Transform player;                // �÷��̾� ��ġ
    public GameObject bulletPrefab;        // �߻��� �Ѿ� ������
    public Transform shootPoint;           // �Ѿ� �߻� ��ġ (�� ������Ʈ)

    public float shootCooldown = 3f;       // �Ѿ� �߻� ����

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
            yield return new WaitForSeconds(1f); // �ִϸ��̼� �߻� �غ� �ð�

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
