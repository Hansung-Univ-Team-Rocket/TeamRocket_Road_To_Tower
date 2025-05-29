using UnityEngine;

public class Spread : Bullet
{
    private Rigidbody rigid;
    public Transform target;
    public float speed = 10f;
    private Vector3 Direction; 

    void Start()
    {
        rigid = GetComponent<Rigidbody>();

        // SetDirection이 먼저 호출되었다고 가정
        if (Direction != Vector3.zero)
        {
            rigid.linearVelocity = Direction * speed;
            transform.forward = Direction;
        }

        // 일정 시간 후 삭제
        Destroy(gameObject, lifeTime);
    }
    public void SetDirection(Vector3 dir)
    {
        Direction = dir.normalized;
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.LogWarning("In");
        if (other.gameObject.tag == "Player")
        {
            PlayerStatusInfo.playerHP--;
            other.gameObject.GetComponent<PlayerMovementController>().TakeDamaged();
        }
    }
}
