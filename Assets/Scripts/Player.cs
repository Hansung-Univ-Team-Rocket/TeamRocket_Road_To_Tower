using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    float hAxis;
    float vAxis;
    Vector3 moveVec;
    public int hp = 100;
    private bool isDamaged;
    MeshRenderer mesh;

    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
    }

    
    void Update()
    {
        hAxis = Input.GetAxisRaw("Vertical");
        vAxis = Input.GetAxisRaw("Horizontal");

        moveVec = new Vector3(-hAxis, 0, vAxis).normalized;

        transform.position += moveVec * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Bullet")
        {
            if (!isDamaged)
            {
                Bullet enemyBullet = other.GetComponent<Bullet>();
                hp -= enemyBullet.damage;
                Destroy(other.gameObject);
                StartCoroutine(OnDamage());
            }
        }
    }

    IEnumerator OnDamage()
    {
        isDamaged = true;
        mesh.material.color = Color.yellow;

        yield return new WaitForSeconds(1);

        isDamaged = false;
        mesh.material.color = Color.white;
    }
}
