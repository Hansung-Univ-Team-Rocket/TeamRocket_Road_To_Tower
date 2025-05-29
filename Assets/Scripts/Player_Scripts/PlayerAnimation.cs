using System.Collections;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public PlayerMovementController PMC;
    public WeaponScript WS;
    public Animator anim;

    public GameObject pistol;
    public GameObject rifle;

    public AudioClip moveClip;
    public AudioClip dodgeClip;
    public AudioClip shootingClip;
    public AudioClip damagedClip;
    private AudioSource audioSource;

    private Coroutine footstepCoroutine;
    private UpperPlayerState lastUpperState;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        anim.SetInteger("Up", 0);
        anim.SetInteger("Down", 0);
        anim.SetInteger("Weapon", 0);
    }

    void Update()
    {
        CheckUpperState();
        CheckLowerState();

        // 업데이트 끝나고 현 스테이트 저장
        lastUpperState = PMC.upperPlayerState;
    }

    void CheackWeapon()
    {
        /// 현재 무기 정보 받아와서 애니메이션 변수 설정.
        /// 딱총 : 0
        /// 라이플 : 1
        /// 활 : 2
        /// 검 : 3

        switch (WS.weaponType)
        {
            case WeaponScript.WEAPON_TYPE.PISTOL:
                anim.SetInteger("Weapon", 0);
                break;
            case WeaponScript.WEAPON_TYPE.RIFLE:
                anim.SetInteger("Weapon", 1);
                break;
            case WeaponScript.WEAPON_TYPE.BOW:
                anim.SetInteger("Weapon", 2);
                break;
            case WeaponScript.WEAPON_TYPE.SWORD:
                anim.SetInteger("Weapon", 3);
                break;
            default:
                break;
        }

        anim.SetInteger("Weapon", 1);
    }

    void CheckUpperState()
    {
        var newState = PMC.upperPlayerState;

        // 닷지 효과음
        if (newState == UpperPlayerState.DODGE && lastUpperState != UpperPlayerState.DODGE)
        {
            audioSource.PlayOneShot(dodgeClip);
        }
        // 사격 효과음
        if (newState == UpperPlayerState.SHOOTINGATTACK && lastUpperState != UpperPlayerState.SHOOTINGATTACK)
        {
            audioSource.PlayOneShot(shootingClip);
        }
        // 피격 효과음
        if (newState == UpperPlayerState.DAMAGED && lastUpperState != UpperPlayerState.DAMAGED)
        {
            audioSource.PlayOneShot(damagedClip);
        }

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

        // 걷기, 달리기, 웅크려 걷기 세 상태만 루프 시작
        if (PMC.lowerPlayerState == LowerPlayerState.MOVE ||
            PMC.lowerPlayerState == LowerPlayerState.SPRINT ||
            PMC.lowerPlayerState == LowerPlayerState.CROUCH_MOVE)
        {
            // 코루틴이 아직 시작되지 않았으면 시작
            if (footstepCoroutine == null)
                footstepCoroutine = StartCoroutine(PlayFootsteps());
        }
        else
        {
            // 그 외 상태가 되면 코루틴을 멈추고 참조 초기화
            if (footstepCoroutine != null)
            {
                StopCoroutine(footstepCoroutine);
                footstepCoroutine = null;
            }
        }
    }

    IEnumerator PlayFootsteps()
    {
        // 걷기 상태일 때만 반복
        while (PMC.lowerPlayerState == LowerPlayerState.MOVE ||
               PMC.lowerPlayerState == LowerPlayerState.SPRINT ||
               PMC.lowerPlayerState == LowerPlayerState.CROUCH_MOVE)
        {
            audioSource.PlayOneShot(moveClip);

            // 상태별 간격
            float interval = PMC.lowerPlayerState == LowerPlayerState.SPRINT ? 0.3f
                           : PMC.lowerPlayerState == LowerPlayerState.CROUCH_MOVE ? 0.6f
                           : 0.4f;
            yield return new WaitForSeconds(interval);
        }

        // 상태가 바뀌면 코루틴 종료 후 참조 초기화
        footstepCoroutine = null;
    }

    public IEnumerator ChangeWeapon()
    {
        anim.SetBool("IsWeaponChange", true);
        anim.SetBool("IsWeaponChange", false);

        if (WS.weaponType == 0) //딱총일 때
        {
            yield return new WaitForSeconds(1f);
            pistol.SetActive(false);
            yield return new WaitForSeconds(0.8f);
            rifle.SetActive(true);
        }
        else
        {
            yield return new WaitForSeconds(1.5f);
            rifle.SetActive(false);
            yield return new WaitForSeconds(0.3f);
            pistol.SetActive(true);
        }
    }
}
