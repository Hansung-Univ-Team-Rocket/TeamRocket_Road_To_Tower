﻿using NUnit.Framework.Internal.Filters;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Enemy_FSM : MonoBehaviour
{
    Transform _player;

    public enum STATE
    { 
        SPAWN,
        IDLE,
        DAMAGED,
        FIND,
        INVESTIGATE,
        ATTACK,
        DEAD,
    }

    public STATE state;


    [Header("Enemy Stat | if enemy's attack type is melee, change is MeleeType flag value to true")]
    public int damage = 1;
    public int hp = 300;
    public float moveSpeed = 6f;
    public float attackDistance = 40f;
    public float attackRPM = 1f;
    public float attackTImeChecker = 0f;
    public float staggerTime = 0.5f;
    public float staggerTimeChecker = 0f;
    public Collider meleeCollider;

    [SerializeField] NavMeshAgent _nav;
    [SerializeField] Animator _animator;
    [SerializeField] GameObject _spawnEffect;
    public          float spwanEffectTime;

    [Header("Hold Timer")]
    [SerializeField] float minAttackStateTime = 1.5f;
    [SerializeField] float attackStateDurationTimer = 0f;
    [SerializeField] float lostSightCooldown = 2f;
    [SerializeField] float lostSightTimer = 0f;
    [SerializeField] float idleTimeChecker = 0f;
    [SerializeField] float idleThreshold = 1f;

    [Header("Bullet Data")]
    public GameObject bullet;
    public Transform shotRocation;

    [Header("Flag Values")]
    [SerializeField] bool _isDead = false;
    [SerializeField] bool _isDamaged = false;
    public bool isMeleeType = false;

    [Header("Death Sinking Value")]
    public float sinkSpeed = 1.5f;
    public float destroyDelay = 5f;

    [Header("Investigate Value")]
    [SerializeField] Vector3 lastKnownPlayerPosition;
    [SerializeField] bool hasLastKnownPosition = false;
    [SerializeField] float investigateTime = 3f; // 마지막 위치를 조사하는 시간
    [SerializeField] float investigateTimer = 0f;

    [Header("Melee Attack Timing")]
    [SerializeField] float meleeAnimationDuration = 3.21f;
    [SerializeField] float meleeColliderOnTime = 0.15f;
    [SerializeField] float meleeColliderOffTime = 0.28f;
    [SerializeField] float meleeAnimTimer = 0f;
    [SerializeField] bool _isMeleeAnimPlaying = false;

    [SerializeField] CapsuleCollider _capsuleCollider;
    
    public GameObject deadEffect;
    public GameObject attackEffect;

    [Header("Enemy's Finder Value")]
    public float fovDegrees = 65f; // 적 유닛이 볼 수 있는 시야각 기본값 65
    public float maxDistance = 60; // 적 유닛이 볼 수 있는 최대 시야 거리 기본값 60

    [Header("Item List")]
    public GameObject healItemPrefab;
    public GameObject ammoItemPrefab;

    private void Start()
    {
        _nav = GetComponent<NavMeshAgent>();
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        _animator = GetComponentInChildren<Animator>();
        _capsuleCollider = GetComponent<CapsuleCollider>();

        if (isMeleeType || meleeCollider != null)
        {
            meleeCollider.enabled = false;
        }

        HealthBar healthBar = GetComponentInChildren<HealthBar>();
        if (healthBar != null)
        {
            healthBar.UpdateMaxHealth(hp);
        }
    }

    void SpwanEffect()
    {
        GameObject spwanEffect = Instantiate(_spawnEffect, this.transform.position, Quaternion.identity);
        Destroy(spwanEffect, spwanEffectTime);
    }

    public void Damaged(int hitDamage)
    {
        if (state == STATE.DEAD) return;

        hp -= hitDamage;
        _isDamaged = true;
        staggerTimeChecker = 0f;

        if (hp <= 0)
        {
            _capsuleCollider.enabled = false;
            _isDead = true;
            _nav.enabled = false;
            state = STATE.DEAD;
            GameObject deadVFX = Instantiate(deadEffect, this.transform.position, Quaternion.identity);
            Destroy(deadVFX, 3f);
            InsItemDrop();
            _animator.SetInteger("State", 5);
            GameObject.FindGameObjectWithTag("Manager").GetComponent<PlayerHealthUI>().AddKill();
            Invoke("DestroyEnemy", destroyDelay);
        }
        else
            state = STATE.DAMAGED;
    }

    void InsItemDrop()
    {
        // 8 : 1 : 1
        int randomValue = Random.Range(1, 11);

        Debug.LogWarning($"{randomValue} 결과");
        if(randomValue < 2) // 이때가 1
        {
            GameObject healItem = Instantiate(healItemPrefab, this.gameObject.transform.position, Quaternion.identity);
        }
        else if(randomValue >= 2 && randomValue <= 9) // 이때가 2~9
        {
            GameObject ammoItem = Instantiate(ammoItemPrefab, this.gameObject.transform.position, Quaternion.identity);
        }
        else // 이때가 10
        {
            return;
        }
    }

    void DestroyEnemy()
    {
        Destroy(this.gameObject);
    }
    /// <summary>
    /// 원거리 공격 유닛일 경우, 이동 거리 및 공격 가능 거리에 대한 일부 값 보정
    /// </summary>
    /// <param name="attackDis"></param>
    /// <returns></returns>
    /*
    bool AttackAdjust(float attackDis)
    {
        // 적 유닛과 플레이어 유닛간의 거리 판별
        float resultDistance =
            Vector3.Distance(this.gameObject.transform.position, _player.transform.position);

        Debug.Log($"현재 거리 : {resultDistance}, 공격 범위 : {attackDis * 0.7}");

        if (resultDistance <= attackDis * 0.7f) return true;
        else return false;
    }*/

    bool IsInAttackRange(float distance)
    {
        return Vector3.Distance(transform.position, _player.position) <= distance * 0.7f;
    }

    bool IsStillInAttackRange(float distance)
    {
        return Vector3.Distance(transform.position, _player.position) <= distance * 1.2f;
    }
    void Attack()
    {
        if (!isMeleeType)
        {
            ShotBulletIns();
        }
        else
        {

        }
    }

    public void ShotBulletIns() // 적 유닛 전용. 유저는 레이케이스트 사용
    {
        GameObject bullet_ = Instantiate(bullet, shotRocation.position, shotRocation.rotation);
        NBullet sb = bullet.GetComponent<NBullet>();
        if (sb != null)
        {
            sb.target = GameObject.FindGameObjectWithTag("CameraPos").transform;
        }
        GameObject attackVFX = Instantiate(attackEffect, shotRocation.position, shotRocation.rotation);
        Destroy(attackVFX, 1.8f);
    }

    /// <summary>
    /// 적 유닛 기준에서 플레이어가 시야에 들어 왔는지 확인하는 함수
    /// </summary>
    /// <param name="lookDir">적 유닛이 바라보는 방향</param>
    /// <param name="toTargetDIr">바라보는 대상(플레이어)</param>
    /// <param name="fovAngle">적 유닛의 시야각</param>
    /// <returns>
    /// 만약에 시야에 들어왔으면 true를 리턴, 그게 아니라면 false
    /// </returns>
    bool IsTragetInSight(Vector3 lookDir, Vector3 toTargetDIr, float fovAngle, float maxFIndDIstance)
    {
        // 적 유닛과 플레이어 유닛간의 거리 판별
        float resultDistance =
            Vector3.Distance(this.gameObject.transform.position, _player.transform.position);

        //Debug.Log($"현재 거리 : {resultDistance}");

        if (resultDistance > maxFIndDIstance)
        {
            return false;
            // 만약에 적 유닛의 최대 시야보다 플레이어 거리간의 거리가 더 길면 false 리턴 
        }

        float dot = Vector3.Dot(lookDir.normalized, toTargetDIr.normalized);
        // 백터 내적

        float thres = Mathf.Cos(fovAngle / 2 * Mathf.Deg2Rad);
        // 시야각을 1/2함. 또, 라디안 값을 도로 바꿈
        
        bool checker = dot >= thres;
        // 목표물이 시야 범위 안에 있으면 참, 아니면 거짓

        if (!checker) return false;

        // 벽 건너인지 확인
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, toTargetDIr.normalized, out hit, maxFIndDIstance))
        {
            if (hit.collider.tag == "Player") return true;
        }
        return false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance * 0.9f); // 공격 진입 범위
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackDistance * 1.1f); // 공격 유지 범위
    }

    private void Update()
    {
        if (_animator != null) Debug.Log("NULL");
        Vector3 toTarget = (_player.transform.position - transform.position).normalized;

        Debug.Log(state);

        if (_isDead)
        {
            this.gameObject.transform.position += Vector3.down * sinkSpeed * Time.deltaTime;
            return;
        }

        if (_nav.enabled)
        {

        }

        switch (state)
        {
            case STATE.SPAWN:

                state = STATE.IDLE;

                break;

            case STATE.IDLE:
                if (_animator != null)
                    _animator.SetInteger("State", 0);
                else
                {
                    Debug.LogError("Animator is null");
                }

                _nav.SetDestination(this.transform.position);

                idleTimeChecker += Time.deltaTime;

                if(idleTimeChecker >= idleThreshold && IsTragetInSight(this.transform.forward, toTarget, fovDegrees, maxDistance) && !_isDamaged)
                {
                    idleTimeChecker = 0;
                    state = STATE.FIND;
                }
                // 애니메이터 FSM은 Integer로 작성
                break;

            case STATE.DAMAGED:
                if (_animator != null)
                    _animator.SetInteger("State", 3);

                if (isMeleeType && _isMeleeAnimPlaying)
                {
                    _isMeleeAnimPlaying = false;
                    meleeAnimTimer = 0f;
                    meleeCollider.enabled = false;
                    _nav.isStopped = false;
                    Debug.Log("Melee attack interrupted by damage");
                }

                staggerTimeChecker += Time.deltaTime;
                _nav.SetDestination(this.transform.position);

                if (staggerTimeChecker >= staggerTime)
                {
                    _isDamaged = false;
                    lastKnownPlayerPosition = _player.transform.position;
                    hasLastKnownPosition = true;

                    if (IsTragetInSight(this.transform.forward, toTarget, fovDegrees, maxDistance))
                    {
                        if (IsInAttackRange(attackDistance))
                        {
                            state = STATE.ATTACK;
                        }
                        else
                            state = STATE.FIND;
                    }
                    else
                    {
                        // 시야에 없으면 INVESTIGATE
                        investigateTimer = 0f;
                        state = STATE.INVESTIGATE;
                    }
                }                
                break;

            case STATE.INVESTIGATE:
                if (_animator != null)
                    _animator.SetInteger("State", 2); // 이동 애니메이션 (힛 후, 추격용 state)

                investigateTimer += Time.deltaTime;

                _nav.SetDestination(lastKnownPlayerPosition); // 히트 당시 마지막 플레이어 위치로

                if (IsTragetInSight(this.transform.forward, toTarget, fovDegrees, maxDistance))
                {
                    if (IsInAttackRange(attackDistance))
                    {
                        state = STATE.ATTACK;
                    }
                    else
                        state = STATE.FIND;
                }
                else if (investigateTimer >= investigateTime ||
                         Vector3.Distance(transform.position, lastKnownPlayerPosition) < 2f) // 마지막 플레이어 발견 위치에 도달했거나 시간이 지나면 IDLE로
                {
                    hasLastKnownPosition = false;
                    state = STATE.IDLE;
                }
                break;

            case STATE.FIND:
                this.gameObject.transform.LookAt(_player.transform.position);
                if (_animator != null)
                    _animator.SetInteger("State", 2);
                else
                {
                    Debug.LogError("Animator is null");
                }

                _nav.SetDestination(_player.transform.position);

                if (!IsTragetInSight(this.transform.forward, toTarget, fovDegrees, maxDistance) && !_isDamaged)
                {
                    lostSightTimer += Time.deltaTime;

                    if (lostSightCooldown <= lostSightTimer)
                    {
                        lostSightTimer = 0;
                        state = STATE.IDLE;
                    }
                }
                else
                {
                    lostSightTimer = 0f;
                }

                if (IsInAttackRange(attackDistance) && !_isDamaged)
                {
                    attackStateDurationTimer = 0f;
                    state = STATE.ATTACK;
                }
                

                break;

            case STATE.ATTACK:
                this.gameObject.transform.LookAt(_player.transform.position);

                // 원거리만 이동
                if (!isMeleeType)
                    _nav.SetDestination(_player.transform.position);

                attackTImeChecker += Time.deltaTime;
                attackStateDurationTimer += Time.deltaTime;

                if (_animator != null)
                    _animator.SetInteger("State", 4);

                if (isMeleeType)
                {
                    AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0); // 현재 애니메이션 시간 가져오기 (0~1 normalized)
                    float normalizedTime = stateInfo.normalizedTime % 1;

                    // 콜라이더 타이밍 (0~1 기준으로 변환)
                    if (normalizedTime >= 0.1f && normalizedTime <= 0.3f)
                    {
                        if (!meleeCollider.enabled)
                            meleeCollider.enabled = true;
                    }
                    else
                    {
                        if (meleeCollider.enabled)
                            meleeCollider.enabled = false;
                    }
                }
                // 원거리 공격 - 타이머 기반
                else
                {
                    if (attackTImeChecker >= attackRPM)
                    {
                        _nav.SetDestination(this.transform.position);
                        ShotBulletIns();
                        attackTImeChecker = 0;
                    }
                }

                // State 전환 체크
                if (attackStateDurationTimer >= minAttackStateTime)
                {
                    if (!IsTragetInSight(this.transform.forward, toTarget, fovDegrees, maxDistance) ||
                            !IsStillInAttackRange(attackDistance))
                    {
                        if (isMeleeType)
                            meleeCollider.enabled = false;

                        state = STATE.FIND;
                    }
                }
                break;

            case STATE.DEAD:

                // 총알 드랍과 관련된 스크립트가 들어가야 함.
                
                break;

            default:
                break;
        }
    }
}