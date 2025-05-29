using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject spawnPrefab;
    public Transform spawnPoint;

    private bool hasSpawned = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasSpawned && other.CompareTag("Player"))
        {
            Debug.Log("Spawn");
            Instantiate(spawnPrefab, spawnPoint.position, Quaternion.identity);
            hasSpawned = true;
        }
    }
}
