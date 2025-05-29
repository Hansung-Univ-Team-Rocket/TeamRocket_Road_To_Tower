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

        // 업데이트 끝나고 현 스테이트 저장
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

        // 상태 전환 감지
        if (newState != lastUpperState)
        {
            // 효과음 (전환 시 한 번만)
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

            // Animator 파라미터도 전환 시 한 번만
            anim.SetInteger("Up", (int)newState);
        }
    }
    void CheckLowerState()
    {
        var newState = PMC.lowerPlayerState;

        // 1) 대기 상태 + 재장전 중이면 발소리 강제 정지 & 하위 처리 스킵
        //if (PMC.upperPlayerState == UpperPlayerState.REROADING && PMC.lowerPlayerState == LowerPlayerState.IDLE)
        //{
        //    if (footstepCoroutine != null)
        //    {
        //        StopCoroutine(footstepCoroutine);
        //        footstepCoroutine = null;
        //    }
        //    return;
        //}

        // 2) 구르기 상태 재진입 방지: 상태가 바뀔 때만 Down 파라미터 세팅
        if (newState != lastLowerState)
        {
            anim.SetInteger("Down", (int)newState);
        }

        // 3) 걷기/달리기/웅크려 걷기 발소리 로직 (종전과 동일)
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
