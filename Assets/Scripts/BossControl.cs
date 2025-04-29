using UnityEngine;
using System.Collections;

public class BossControl : MonoBehaviour
{
    public Transform player;
    MeshRenderer[] meshs;

    public Transform LHand;
    public Transform RHand;

    public GameObject straightBulletPrefab;
    public GameObject homingBulletPrefab;

    public float dashSpeed = 20f;
    public float patternCooldown = 2f;

    private bool isAttacking = false;

    void Start()
    {
        meshs = GetComponentsInChildren<MeshRenderer>();
        StartCoroutine(PatternLoop());
    }

    IEnumerator PatternLoop()
    {
        while (true)
        {
            if (!isAttacking)
            {
                isAttacking = true;

                int pattern = Random.Range(0, 3);
                switch (pattern)
                {
                    case 0:
                        yield return StartCoroutine(DashPattern());
                        break;
                    case 1:
                        yield return StartCoroutine(StraightShotPattern());
                        break;
                    case 2:
                        yield return StartCoroutine(HomingShotPattern());
                        break;
                }

                yield return new WaitForSeconds(patternCooldown);
                isAttacking = false;
            }

            yield return null;
        }
    }

    // 패턴 1: 플레이어를 향한 돌진
    IEnumerator DashPattern()
    {
        // 플레이어 위치 인식
        Vector3 targetPosition = player.position;
        targetPosition.y = transform.position.y;
        // 플레이어를 바라보게
        Vector3 targetDirection = new Vector3(targetPosition.x - transform.position.x, 0, targetPosition.z - transform.position.z).normalized;
        transform.rotation = Quaternion.LookRotation(targetDirection);

        foreach (MeshRenderer mesh in meshs)
            mesh.material.color = Color.green;
        yield return new WaitForSeconds(2f);
        foreach (MeshRenderer mesh in meshs)
            mesh.material.color = Color.white;

        transform.forward = (targetPosition - transform.position).normalized;

        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, dashSpeed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(1f);
    }

    // 패턴 2: 각 손에서 5발씩 비유도 탄환 발사
    IEnumerator StraightShotPattern()
    {
        for (int i = 0; i < 5; i++)
        {
            // 번갈아가며 손을 선택
            Transform fireHand = (i % 2 == 0) ? RHand : LHand;

            FireStraightBullet(fireHand);
            yield return new WaitForSeconds(0.5f); // 간격 조정 가능
        }

        yield return new WaitForSeconds(1f); // 패턴 종료 전 대기
    }

    // 패턴 3: 각 손에서 1발씩 유도 탄환 발사
    IEnumerator HomingShotPattern()
    {
        FireHomingBullet(RHand);
        yield return new WaitForSeconds(1f);

        FireHomingBullet(LHand);
        yield return new WaitForSeconds(1f); // 다음 패턴과의 간격 유지
    }

    void FireStraightBullet(Transform hand)
    {
        GameObject bullet = Instantiate(straightBulletPrefab, hand.position, Quaternion.identity);
        NBullet sb = bullet.GetComponent<NBullet>();
        if (sb != null)
        {
            sb.target = player;
        }
    }

    void FireHomingBullet(Transform hand)
    {
        GameObject bullet = Instantiate(homingBulletPrefab, hand.position, Quaternion.identity);
        Homing homing = bullet.GetComponent<Homing>();
        if (homing != null)
        {
            homing.target = player;
        }
    }
}
