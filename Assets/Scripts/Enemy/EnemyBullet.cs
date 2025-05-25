using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float force = 250.0f;
    /*[SerializeField]*/
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.AddForce(transform.forward * force);

        Destroy(this.gameObject, 3f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other) // 유저, 벽, 적을 만났을 때 총알 소모. 적끼리는 영향이 없음.
    {
        Debug.Log("Collided with: " + other.gameObject.name + ", Tag: " + other.gameObject.tag);

        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Wall"))
        {
            if (other.gameObject.CompareTag("Player"))
            {
                PlayerStatusInfo.playerHP--;

                if(PlayerStatusInfo.playerHP <= 0)
                {
                    other.gameObject.GetComponent<PlayerMovementController>().Dead();
                }

            }
        }

        if (!other.gameObject.CompareTag("Enemy") && !other.gameObject.CompareTag("Bullet")) Destroy(this.gameObject);

    }
}