using UnityEngine;

public class Homing : Bullet
{
    public Transform target;
    public float speed = 5f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
        transform.forward = direction;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.LogWarning("In");
        if (other.gameObject.tag == "Player")
        {
            PlayerStatusInfo.playerHP --;
            other.gameObject.GetComponent<PlayerMovementController>().TakeDamaged();
        }
    }
}
