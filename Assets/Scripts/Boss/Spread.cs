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

        // SetDirection�� ���� ȣ��Ǿ��ٰ� ����
        if (Direction != Vector3.zero)
        {
            rigid.linearVelocity = Direction * speed;
            transform.forward = Direction;
        }

        // ���� �ð� �� ����
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
