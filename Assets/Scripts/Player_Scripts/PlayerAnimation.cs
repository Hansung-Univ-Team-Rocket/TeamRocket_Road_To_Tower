using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public PlayerMovementController PMC;
    public WeaponManager WM;
    public Animator anim;

    //public GameObject pistol;
    //public GameObject rifle;
    // �ϵ� �ڵ��� �ʿ䰡 ����. WeaponManager���� �����ò���.

    public AudioClip moveClip;
    public AudioClip dodgeClip;
    public AudioClip shootingClip;
    public AudioClip damagedClip;
    public AudioClip reloadClip;
    private AudioSource audioSource;

    private Coroutine footstepCoroutine;
    private UpperPlayerState lastUpperState;
    private LowerPlayerState lastLowerState;

    public bool isChangingWeapon = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        anim.SetInteger("Up", 0);
        anim.SetInteger("Down", 0);
        anim.SetInteger("Weapon", 0);

        lastUpperState = PMC.upperPlayerState;
        lastLowerState = PMC.lowerPlayerState;
    }

    void Update()
    {
        CheckUpperState();
        CheckLowerState();
        CheckWeapon();

        // ������Ʈ ������ �� ������Ʈ ����
        lastUpperState = PMC.upperPlayerState;
        lastLowerState = PMC.lowerPlayerState;
        Debug.LogError($"lowerState : {PMC.lowerPlayerState}");
    }

    public void SetWeaponIndex(int weaponIndex)
    {
        anim.SetInteger("Weapon", weaponIndex);
    }
    void CheckWeapon()
    {
        // �ش� �ڵ���ε� ������ �ʿ�. ���� �ε��� ���� �ִϸ��̼� �������� ��������.
        ///// ���� ���� ���� �޾ƿͼ� �ִϸ��̼� ���� ����.
        ///// ���� : 0
        ///// ������ : 1
        ///// Ȱ : 2
        ///// �� : 3

        //switch (WM.currentWeaponIndex)
        //{
        //    case 0:
        //        anim.SetInteger("Weapon", 0);
        //        break;
        //    case 1:
        //        anim.SetInteger("Weapon", 1);
        //        break;
        //    case 2:
        //        anim.SetInteger("Weapon", 2);
        //        break;
        //    case 3:
        //        anim.SetInteger("Weapon", 3);
        //        break;
        //    default:
        //        break;
        //}

        //anim.SetInteger("Weapon", 1);

        if (WM == null || WM.currentWeapon == null) return; // ���� �Ŵ����� ���̰ų� ���� ���Ⱑ ���̸� �׳� �����ؼ� ���� ����
        anim.SetInteger("Weapon", WM.currentWeaponIndex); // �ϵ� �ڵ��� �ʿ䰡 ����. ���� ���� �ε����� 0, 1�� ���� ���� ������ ����
    }

    void CheckUpperState()
    {
        var newState = PMC.upperPlayerState;

        // ���� ��ȯ ����
        if (newState != lastUpperState)
        {
            // ȿ���� (��ȯ �� �� ����)
            switch (newState)
            {
                case UpperPlayerState.DODGE:
                    audioSource.PlayOneShot(dodgeClip);
                    break;
                case UpperPlayerState.SHOOTINGATTACK:
                    audioSource.PlayOneShot(shootingClip);
                    break;
                case UpperPlayerState.DAMAGED:
                    audioSource.PlayOneShot(damagedClip);
                    break;
                case UpperPlayerState.REROADING:
                    audioSource.PlayOneShot(reloadClip);
                    break;
            }

            // Animator �Ķ���͵� ��ȯ �� �� ����
            anim.SetInteger("Up", (int)newState);
        }
    }
    void CheckLowerState()
    {
        var newState = PMC.lowerPlayerState;

        // 1) ��� ���� + ������ ���̸� �߼Ҹ� ���� ���� & ���� ó�� ��ŵ
        //if (PMC.upperPlayerState == UpperPlayerState.REROADING && PMC.lowerPlayerState == LowerPlayerState.IDLE)
        //{
        //    if (footstepCoroutine != null)
        //    {
        //        StopCoroutine(footstepCoroutine);
        //        footstepCoroutine = null;
        //    }
        //    return;
        //}

        // 2) ������ ���� ������ ����: ���°� �ٲ� ���� Down �Ķ���� ����
        if (newState != lastLowerState)
        {
            anim.SetInteger("Down", (int)newState);
        }

        // 3) �ȱ�/�޸���/��ũ�� �ȱ� �߼Ҹ� ���� (������ ����)
        if (newState == LowerPlayerState.MOVE ||
            newState == LowerPlayerState.SPRINT ||
            newState == LowerPlayerState.CROUCH_MOVE)
        {
            if (footstepCoroutine == null)
                footstepCoroutine = StartCoroutine(PlayFootsteps());
        }
        else
        {
            if (footstepCoroutine != null)
            {
                StopCoroutine(footstepCoroutine);
                footstepCoroutine = null;
            }
        }
    }

    IEnumerator PlayFootsteps()
    {
        while (PMC.lowerPlayerState == LowerPlayerState.MOVE ||
               PMC.lowerPlayerState == LowerPlayerState.SPRINT ||
               PMC.lowerPlayerState == LowerPlayerState.CROUCH_MOVE)
        {
            audioSource.PlayOneShot(moveClip);
            float interval = PMC.lowerPlayerState == LowerPlayerState.SPRINT ? 0.3f
                           : PMC.lowerPlayerState == LowerPlayerState.CROUCH_MOVE ? 0.6f
                           : 0.4f;
            yield return new WaitForSeconds(interval);
        }
        footstepCoroutine = null;
    }

    public IEnumerator ChangeWeapon(int fromIdx, int toIdx) // ����, 
    {
        // �̹� �ٲٴ� ���̸� �״�� ����
        if (isChangingWeapon) yield break;

        isChangingWeapon = true;
        
        // �ѹ� �� ������ġ �߰�
        if (PMC.upperPlayerState == UpperPlayerState.SHOOTINGATTACK)
        {
            PMC.upperPlayerState = UpperPlayerState.IDLE;
        }

        PMC.isFire = false;

        anim.SetTrigger("ChangeWeapon");

        yield return new WaitForSeconds(1.37f);

        WM.HideWeapon(fromIdx);

        yield return new WaitForSeconds(0.15f); // ����� ��� �߰�
        
        WM.ShowWeapon(toIdx);

        yield return new WaitForSeconds(1.38f); // ���� Ÿ�̹� ���缭

        anim.SetInteger("Weapon", toIdx); // �Ķ���� �ε��� ��ȯ

        isChangingWeapon = false;
    }
}
