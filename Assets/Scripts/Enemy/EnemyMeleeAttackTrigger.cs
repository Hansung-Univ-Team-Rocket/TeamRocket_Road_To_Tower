using UnityEngine;

public class EnemyMeleeAttackTrigger : MonoBehaviour
{
    int _damageValue = 0;
    private void Start()
    {
        _damageValue = gameObject.transform.parent.GetComponent<Enemy_FSM>().damage;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            PlayerStatusInfo.playerHP -= _damageValue;

            if(PlayerStatusInfo.playerHP <= 0)
            {
                other.gameObject.GetComponent<PlayerMovementController>().Dead();
            }
        }
    }
}
