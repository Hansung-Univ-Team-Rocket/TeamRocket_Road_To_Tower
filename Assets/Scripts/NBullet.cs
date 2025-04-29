using UnityEngine;
using UnityEngine.UIElements;

public class NBullet : Bullet
{
    private Rigidbody rigid;
    public Transform target;
    public float speed = 10f;
    private Vector3 Direction;
    void Start()
    {
        rigid = GetComponent<Rigidbody>();

        if (target != null)
        {
            // 생성 시 타겟을 향한 방향 벡터를 정규화하여 저장
            Direction = (target.position - transform.position).normalized;
        }

        // Rigidbody를 사용하여 초기 속도 설정
        rigid.linearVelocity = Direction * speed;

        // 탄환이 향하는 방향으로 회전
        transform.forward = Direction;
    }
}
