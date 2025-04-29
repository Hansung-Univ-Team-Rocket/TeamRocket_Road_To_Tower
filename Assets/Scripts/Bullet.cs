using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public bool isMelee;
    public float lifeTime = 5f;

    void Start()
    {
        Destroy(gameObject, lifeTime);  
    }

    void OnTriggerEnter(Collider other)
    {
        if(isMelee! && other.gameObject.tag == "Struc")
        {
            Destroy(gameObject);
        }
    }
}
