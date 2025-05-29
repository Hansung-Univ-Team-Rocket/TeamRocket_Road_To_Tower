using UnityEngine;

public class Spike : MonoBehaviour
{
    public int damage = 0;
    [SerializeField] float damageCooldown = 1.0f; // 데미지 쿨타임 (1초)
    private bool _canDamage = true; // 데미지를 줄 수 있는지 여부

    private void OnTriggerEnter(Collider other)
    {
        if (_canDamage && other.CompareTag("Player"))
        {
            Debug.LogWarning("In");
            PlayerStatusInfo.playerHP--;
            other.GetComponent<PlayerMovementController>().TakeDamaged();
            StartCoroutine(DamageCooldownCoroutine());
        }
    }

    private System.Collections.IEnumerator DamageCooldownCoroutine()
    {
        _canDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        _canDamage = true;
    }
}
