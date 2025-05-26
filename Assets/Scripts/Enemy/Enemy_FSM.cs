using NUnit.Framework.Internal.Filters;
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
        ATTACK,
        DEAD,
    }

    public STATE state;


    [Header("Enemy Stat | if enemy's attack type is melee, change is MeleeType flag value to true")]
    public int damage = 1;
    public int hp = 300;
    public float moveSpeed = 6f;
    public bool isMeleeType = false;
    public float attackDistance = 40f;
    public float attackRPM = 1f;
    public float attackTImeChecker = 0f;
    public float staggerTime = 0.5f;
    public float staggerTimeChecker = 0f;

    public GameObject projectile;

    [SerializeField] NavMeshAgent _nav;
    [SerializeField] Animator _animator;
    [SerializeField] GameObject _spawnEffect;
    public          float spwanEffectTime;

    [Header("Bullet Data")]
    public GameObject bullet;
    public Transform shotRocation;

    [Header("Flag Values")]
    [SerializeField] bool _isDead = false;


    [Header("Enemy's Finder Value")]
    public float fovDegrees = 65f; // 적 유닛이 볼 수 있는 시야각 기본값 65
    public float maxDistance = 60; // 적 유닛이 볼 수 있는 최대 시야 거리 기본값 60

    private void Start()
    {
        _nav = GetComponent<NavMeshAgent>();
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        _animator = GetComponentInChildren<Animator>();
    }

    void SpwanEffect()
    {
        GameObject spwanEffect = Instantiate(_spawnEffect, this.transform.position, Quaternion.identity);
        Destroy(spwanEffect, spwanEffectTime);
    }

    public void Damaged(int hitDamage)
    {
        hp -= hitDamage;
        if (hp <= 0)
        {
            _isDead = true;

            state = STATE.DEAD;
            _animator.SetBool("isDead", true);
        }
        state = STATE.DAMAGED;
    }

    /// <summary>
    /// 원거리 공격 유닛일 경우, 이동 거리 및 공격 가능 거리에 대한 일부 값 보정
    /// </summary>
    /// <param name="attackDis"></param>
    /// <returns></returns>
    bool AttackAdjust(float attackDis)
    {
        // 적 유닛과 플레이어 유닛간의 거리 판별
        float resultDistance =
            Vector3.Distance(this.gameObject.transform.position, _player.transform.position);

        Debug.Log($"현재 거리 : {resultDistance}, 공격 범위 : {attackDis * 2 / 3}");

        if (resultDistance <= attackDis * 2 / 3)
            return true;
        else return false;
    }

    void FarHited()
    {
        while (true)
        {
            if (!IsTragetInSight(this.transform.forward, _player.transform.position, fovDegrees, maxDistance))
            {
                this.gameObject.transform.LookAt(_player.transform.position);
                _nav.SetDestination(_player.transform.position);
            }
            else
            {
                state = STATE.FIND;
                break;
            }
        }
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
        GameObject bullet_ = Instantiate(bullet);
        bullet_.transform.position = shotRocation.position;
        bullet_.transform.rotation = shotRocation.rotation;
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

        Debug.Log($"현재 거리 : {resultDistance}");

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

    private void Update()
    {
        if (_animator != null) Debug.Log("NULL");
        Vector3 toTarget = (_player.transform.position - transform.position).normalized;

        Debug.Log(state);

        if (_isDead)
        {
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

                if(IsTragetInSight(this.transform.forward, toTarget, fovDegrees, maxDistance))
                {
                    state = STATE.FIND;
                }
                // 애니메이터 FSM은 Integer로 작성
                break;

            case STATE.DAMAGED:
                staggerTimeChecker += Time.deltaTime;
                _nav.SetDestination(this.transform.position);

                if (staggerTimeChecker >= staggerTime)
                {
                    if (IsTragetInSight(this.transform.forward, toTarget, fovDegrees, maxDistance))
                    {
                        if (AttackAdjust(attackDistance))
                        {
                            state = STATE.ATTACK;
                        }
                    }
                    state = STATE.FIND;
                }

                break;

            case STATE.FIND:
                this.gameObject.transform.LookAt(_player.transform.position);
                if (_animator != null)
                    _animator.SetInteger("State", 3);
                else
                {
                    Debug.LogError("Animator is null");
                }

                _nav.SetDestination(_player.transform.position);

                if (!IsTragetInSight(this.transform.forward, toTarget, fovDegrees, maxDistance))
                {
                    state = STATE.IDLE;
                }

                if (AttackAdjust(attackDistance))
                {
                    state = STATE.ATTACK;
                }
                

                break;

            case STATE.ATTACK:
                this.gameObject.transform.LookAt(_player.transform.position);
                _nav.SetDestination(this.transform.position);
                attackTImeChecker += Time.deltaTime;

                if (_animator != null)
                    _animator.SetInteger("State", 4);
                else
                {
                    Debug.LogError("Animator is null");
                }

                if (attackTImeChecker >= attackRPM)
                {
                    Attack();
                    attackTImeChecker = 0;
                }
                if (!AttackAdjust(attackDistance))
                {
                    state = STATE.FIND;
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