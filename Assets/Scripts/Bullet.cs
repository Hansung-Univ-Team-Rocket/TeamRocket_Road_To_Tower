using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public float lifeTime = 5f;

    void Start()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Struc")
        {
            Destroy(gameObject);
        }
    }
}
