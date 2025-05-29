using UnityEngine;

public class GameManger : MonoBehaviour
{
    public Transform[] sawSpwanPoint;
    public Transform[] gunSpwanPoint;

    public GameObject gunMobPrefab;
    public GameObject sawMobPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instantiator();
    }

    void Instantiator()
    {
        foreach (Transform t in sawSpwanPoint)
        {
            Instantiate(sawMobPrefab, t.transform.position, Quaternion.identity);
        }

        foreach (Transform t2 in gunSpwanPoint)
        {
            Instantiate(gunMobPrefab, t2.transform.position, Quaternion.identity);
        }
    }
}
