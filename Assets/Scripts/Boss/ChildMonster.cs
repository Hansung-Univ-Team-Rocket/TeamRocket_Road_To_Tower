using UnityEngine;
using System.Collections;
using UnityEngine.XR;

public class ChildMonster : MonoBehaviour
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
        // �Ѿ� ����, �ڽ� ���� �ٷ� �� (shootPoint ��ġ), ���� �������� ����
        for (int i = 0; i < 3; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
            NBullet sb = bullet.GetComponent<NBullet>();
            if (sb != null)
            {
                sb.target = player;
            }
            yield return new WaitForSeconds(1f);
        }

        yield return null;
    }
}
