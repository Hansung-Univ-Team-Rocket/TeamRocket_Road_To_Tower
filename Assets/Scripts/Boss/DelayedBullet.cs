using UnityEngine;
using System.Collections;

public class DelayedBullet : Bullet
{
    private Rigidbody rigid;
    public Transform target;
    public Transform originPoint; // shootPoint 참조 저장용
    public float speed = 10f;

    private Vector3 direction;
    private bool isMoving = false;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    public void StartMovingAfterDelay(float delaySeconds, Transform shootOrigin)
    {
        originPoint = shootOrigin;
        StartCoroutine(DelayedFire(delaySeconds));
    }

    IEnumerator DelayedFire(float delaySeconds)
    {
        // 지연 시간 동안 shootPoint에 위치 고정
        float elapsed = 0f;
        while (elapsed < delaySeconds)
        {
            if (originPoint != null)
            {
                transform.position = originPoint.position;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 방향 설정 및 발사
        if (target != null)
        {
            direction = (target.position - transform.position).normalized;
            transform.forward = direction;
        }

        isMoving = true;
        rigid.linearVelocity = direction * speed;

        // 수명 시간 후 자동 파괴
        Destroy(gameObject, lifeTime);
    }
}
