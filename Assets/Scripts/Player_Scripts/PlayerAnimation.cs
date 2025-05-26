using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public PlayerMovementController PMC;
    public WeaponScript WS;
    public Animator anim;

    void Start()
    {
        anim.SetInteger("Up", 0);
        anim.SetInteger("Down", 0);
        anim.SetInteger("Weapon", 0);
    }

    void Update()
    {
        //CheackWeapon();
        CheckUpperState();
        CheckLowerState();
    }

    void CheackWeapon()
    {
        /// ���� ���� ���� �޾ƿͼ� �ִϸ��̼� ���� ����.
        /// ���� : 0
        /// ������ : 1
        /// Ȱ : 2
        /// �� : 3

        switch (WS.weaponType)
        {
            case "":
                break;
            default:
                break;
        }

        anim.SetInteger("Weapon", 1);
    }

    void CheckUpperState()
    {
        switch (PMC.upperPlayerState)
        {
            case UpperPlayerState.IDLE:
                anim.SetInteger("Up", 0);
                break;
            case UpperPlayerState.MELEEATTACK:
                anim.SetInteger("Up", 1);
                break;
            case UpperPlayerState.SHOOTINGATTACK:
                anim.SetInteger("Up", 2);
                break;
            case UpperPlayerState.REROADING:
                anim.SetInteger("Up", 3);
                break;
            case UpperPlayerState.SPRINT:
                anim.SetInteger("Up", 4);
                break;
            case UpperPlayerState.DODGE:
                anim.SetInteger("Up", 5);
                break;
            case UpperPlayerState.DAMAGED:
                anim.SetInteger("Up", 6);
                break;
            case UpperPlayerState.DEAD:
                anim.SetInteger("Up", 7);
                break;
            default:
                break;
        }
    }

    void CheckLowerState()
    {
        switch (PMC.lowerPlayerState)
        {
            case LowerPlayerState.IDLE:
                anim.SetInteger("Down", 0);
                break;
            case LowerPlayerState.MOVE:
                anim.SetInteger("Down", 1);
                break;
            case LowerPlayerState.SPRINT:
                anim.SetInteger("Down", 2);
                break;
            case LowerPlayerState.CROUCH:
                anim.SetInteger("Down", 3);
                break;
            case LowerPlayerState.CROUCH_MOVE:
                anim.SetInteger("Down", 4);
                break;
            case LowerPlayerState.DODGE:
                anim.SetInteger("Down", 5);
                break;
            case LowerPlayerState.DAMAGED:
                anim.SetInteger("Down", 6);
                break;
            case LowerPlayerState.DEAD:
                anim.SetInteger("Down", 7);
                break;
            default:
                break;
        }
    }
}
