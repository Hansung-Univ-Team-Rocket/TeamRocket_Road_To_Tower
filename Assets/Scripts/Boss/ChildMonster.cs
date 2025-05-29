using UnityEngine;
using System.Collections;
using UnityEngine.XR;

public class ChildMonster : MonoBehaviour
{
    public Transform player;                // �÷��̾� ��ġ
    public GameObject bulletPrefab;        // �߻��� �Ѿ� ������
    public Transform shootPoint;           // �Ѿ� �߻� ��ġ (�� ������Ʈ)

    public float shootCooldown = 3.6f;       // �Ѿ� �߻� ����
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

        // �Ѿ� ����, �ڽ� ���� �ٷ� �� (shootPoint ��ġ), ���� �������� ����
        for (int i = 0; i < 3; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
            NBullet sb = bullet.GetComponent<NBullet>();
            if (sb != null)
            {
                sb.target = player;
            }
            yield return new WaitForSeconds(0.2f);
        }

        isCoroutinePlaying = false;

        curTime = 0;
    }
}
