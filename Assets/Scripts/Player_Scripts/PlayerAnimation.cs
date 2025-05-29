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
    // 하드 코딩할 필요가 없음. WeaponManager에서 가저올꺼임.

    public AudioClip moveClip;
    public AudioClip dodgeClip;
    public AudioClip shootingClip;
    public AudioClip damagedClip;
    public AudioClip reloadClip;
    private AudioSource audioSource;

    private Coroutine footstepCoroutine;
    private UpperPlayerState lastUpperState;

    public bool isChangingWeapon = false;

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
        CheckWeapon();

        // 업데이트 끝나고 현 스테이트 저장
        lastUpperState = PMC.upperPlayerState;
    }

    public void SetWeaponIndex(int weaponIndex)
    {
        anim.SetInteger("Weapon", weaponIndex);
    }
    void CheckWeapon()
    {
        // 해당 코드라인도 수정이 필요. 무기 인덱스 따라 애니메이션 설정으로 수정했음.
        ///// 현재 무기 정보 받아와서 애니메이션 변수 설정.
        ///// 딱총 : 0
        ///// 라이플 : 1
        ///// 활 : 2
        ///// 검 : 3

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

        if (WM == null || WM.currentWeapon == null) return; // 웨폰 매니저가 널이거나 현재 무기가 널이면 그냥 리턴해서 에러 방지
        anim.SetInteger("Weapon", WM.currentWeaponIndex); // 하드 코딩할 필요가 없음. 지금 무기 인덱스가 0, 1이 지금 딱총 라이플 순임
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
        // 재장전 효과음
        if (newState == UpperPlayerState.REROADING && lastUpperState != UpperPlayerState.REROADING)
        {
            audioSource.PlayOneShot(reloadClip);
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

    public IEnumerator ChangeWeapon(int fromIdx, int toIdx) // 수정, 
    {
        // 이미 바꾸는 중이면 그대로 종료
        if (isChangingWeapon) yield break;

        isChangingWeapon = true;
        
        // 한번 더 안전장치 추가
        if (PMC.upperPlayerState == UpperPlayerState.SHOOTINGATTACK)
        {
            PMC.upperPlayerState = UpperPlayerState.IDLE;
        }

        PMC.isFire = false;

        anim.SetTrigger("ChangeWeapon");

        yield return new WaitForSeconds(1.37f);

        WM.HideWeapon(fromIdx);

        yield return new WaitForSeconds(0.15f); // 잠깐의 빈손 추가
        
        WM.ShowWeapon(toIdx);

        yield return new WaitForSeconds(1.38f); // 종료 타이밍 맞춰서

        anim.SetInteger("Weapon", toIdx); // 파라메터 인덱스 전환

        isChangingWeapon = false;
    }
}
