using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    public Enemy_FSM fsm;
    public Animator anim;

    void Start()
    {
        anim.SetInteger("State", 0);
    }

    void Update()
    {
        CheckState();
    }

    void CheckState()
    {
        switch (fsm.state)
        {
            case Enemy_FSM.STATE.SPAWN:
                anim.SetInteger("State", 0);
                break;
            case Enemy_FSM.STATE.IDLE:
                anim.SetInteger("State", 1);
                break;
            case Enemy_FSM.STATE.DAMAGED:
                anim.SetInteger("State", 2);
                break;
            case Enemy_FSM.STATE.FIND:
                anim.SetInteger("State", 3);
                break;
            case Enemy_FSM.STATE.ATTACK:
                anim.SetInteger("State", 4);
                break;
            case Enemy_FSM.STATE.DEAD:
                anim.SetInteger("State", 5);
                break;
            default: 
                break;
        }
    }

}
