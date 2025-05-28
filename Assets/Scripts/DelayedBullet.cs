using System.Collections;
using UnityEngine;

public class DelayedBullet : Bullet
{
    private Rigidbody rigid;
    private Transform followTarget;   // 붙어 있을 shootPoint
    private Vector3 direction;
    public float speed = 10f;
    public float delay = 3f;
    private bool isFired = false;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        rigid.isKinematic = true; // 초기엔 움직이지 않음
    }

    public void Init(Transform shootPoint, Transform target)
    {
        followTarget = shootPoint;
        StartCoroutine(DelayedFire(target));
    }

    IEnumerator DelayedFire(Transform target)
    {
        float elapsed = 0f;

        // 3초 동안 shootPoint 따라가기
        while (elapsed < delay)
        {
            if (followTarget != null)
            {
                transform.position = followTarget.position;
                transform.rotation = followTarget.rotation;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 방향 설정
        if (target != null)
        {
            direction = (target.position - transform.position).normalized;
        }
        else
        {
            direction = transform.forward; // fallback
        }

        // 발사 시작
        rigid.isKinematic = false;
        rigid.linearVelocity = direction * speed;
        isFired = true;
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}