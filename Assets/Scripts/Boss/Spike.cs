using UnityEngine;

public class Spike : MonoBehaviour
{
    public int damage = 0;
    [SerializeField] float damageCooldown = 1.0f; // ������ ��Ÿ�� (1��)
    private bool _canDamage = true; // �������� �� �� �ִ��� ����

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
