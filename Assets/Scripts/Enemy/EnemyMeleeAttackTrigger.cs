using UnityEngine;

public class EnemyMeleeAttackTrigger : MonoBehaviour
{
    public Enemy_FSM fsm;
    [SerializeField] int _damageValue = 0;

    private void Start()
    {
        _damageValue = fsm.damage;
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.LogWarning("In");
        if(other.gameObject.tag == "Player")
        {
            PlayerStatusInfo.playerHP -= _damageValue;
        }
    }
}
