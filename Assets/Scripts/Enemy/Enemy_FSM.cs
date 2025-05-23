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

    [SerializeField] NavMeshAgent _nav;
    [SerializeField] Animator _animator;
    [SerializeField] GameObject _spawnEffect;
    public          float spwanEffectTime;

    [SerializeField] bool _isDead = false;

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

    private void Update()
    {
        if (_animator != null) Debug.Log("NULL");

        Debug.Log(state);

        if (_isDead)
        {
            // 사망 애니메이터 플레그값 입력
            _animator.SetBool("isDead", true); 
            return;
        }

        if (_nav.enabled)
        {

        }

        switch (state)
        {
            case STATE.SPAWN:
                if(_animator != null)
                    _animator.SetInteger("State", 0);
                else
                {
                    Debug.LogError("Animator is null");
                }
                // 애니메이터 FSM은 Integer로 작성
                break;

            case STATE.IDLE:
                break;

            case STATE.DAMAGED:
                break;

            case STATE.FIND:
                break;

            case STATE.ATTACK:
                break;

            case STATE.DEAD:
                break;

            default:
                break;
        }
    }
}


/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_FSM : MonoBehaviour
{
    //Gun gun;
    Transform player;
    public Animator animator;
    public bool canShot = false;
    //public Enemy_Patrol_Node curnode;
    //public Enemy_Patrol_Node backupNode;
    public Vector3 startpos;
    public Vector3 nextpos; // ��Ʈ�ѿ� ����
    public bool nullChecker = false;
    public float max_Angle = 205f;
    public float min_Angle = 35f;
    public float roamer_Deviation = 3f;
    public float fireChecker = 0f;
    //[SerializeField] ClearChecker clear;


    public GameObject bloodEffect;

    AudioSource SFX;
    public AudioClip deadSound;

    public float raycastDistance;
    public float meeleTimer = 0;
    int[] rotationlist = new int[4] { -90, 90, 180, -180 };

    public GameObject range;

    public enum STATE
    {
        IDLE = 0,
        IDLE_PATROL,
        ROAMER,
        FIND,
        ATTACK,
        DEAD
    }
    //public                          GameObject[] patrolPoints;

    int roamerCount = 0;
    Vector3 roamerPoints;

    Vector3 startPoint;

    public float rotationTimer = 0f;

    public float roamerTimer = 0f;
    public float follow_Spare_Time = 0f;

    public bool lostUser = false;
    public bool isPatrol;
    public bool isRotateEnemy = false;
    public bool nowDead = false;
    public bool needNewRocation = false;

    public bool coroutineChecker = false;

    public STATE state;

    Transform tr;

    //public Enemy_Seacher es;
    public NavMeshAgent nav;

    public float bugCountDown = 0f;

    public void SetStateFInd()
    {
        state = STATE.FIND;
    }
    void SetroamerDestination() // NavMesh 경로 지정 (ROAMER 상태일 때만)
    {
        //if(roamerCount < 4)
        //{
        roamerCount++;

        nav.isStopped = true;

        transform.Rotate(0, Random.Range(min_Angle, max_Angle), 0);


        nav.speed = 7f;

        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, 50f, LayerMask.GetMask("Wall")))
        {
            roamerPoints = hit.collider.transform.position;
        }

        nav.SetDestination(roamerPoints);

        nav.isStopped = false;

        //Vector3 dir = this.transform.position
        //}
    }

    public void SetStateDead()
    {
        nowDead = true;
    }

    void RotationIdle() // 적을 회전시키는 함수. isRotateEnemy가 true일 때, IDLE 상태일 때 적을 회전, 아닐 때 회전 X (IDLE일 때만)
    {
        if (rotationTimer > 3f)
        {
            //this.gameObject.transform.rotation
            //    = Quaternion.Slerp(this.gameObject.transform.rotation,
            //                    Quaternion.Euler(this.transform.rotation.x, 
            //                                    this.gameObject.transform.rotation.y + rotationlist[Random.Range(0, 3)],
            //                                    this.gameObject.transform.rotation.z),
            //                    0.1f);

            //this.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(this.transform.position.x, rotationlist[Random.Range(0, 3)], this.transform.position.z), Time.deltaTime * 5f);

            this.transform.Rotate(0, rotationlist[Random.Range(0, 3)], 0);
            rotationTimer = 0;
        }
    }

    void RotationEnemy(Vector3 location) // 적을 회전시키는 함수
    {
        Vector3 dir = location - this.transform.position;
        dir.y = 0;


        if (dir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            this.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }
    // Start is called before the first frame update

    private void Awake()
    {
        //animator = GetComponentInChildren<Animator>();

    }
    void Start()
    {
        tr = GetComponent<Transform>();
        es = GetComponent<Enemy_Seacher>();
        nav = GetComponent<NavMeshAgent>();
        gun = GetComponentInChildren<Gun>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        SFX = GetComponent<AudioSource>();
        clear = GameObject.FindGameObjectWithTag("Clear_Manager").GetComponent<ClearChecker>();

        startPoint = this.transform.position;

        animator = GetComponentInChildren<Animator>();

        startpos = this.transform.position;
        //nextpos = curnode.Get_Now_Node();

    }

    // Update is called once per frame
    void Update()
    {
        if (animator == null) Debug.Log("NULL");

        Debug.Log(state);

        if (GameObject.FindGameObjectWithTag("Manager").GetComponent<UIManager>().playerDeadChecker)
            return;

        if (nowDead)
        {
            animator.SetBool("isDead", true);
            state = STATE.DEAD;
        }


        if (nav.enabled)
        {

            if (state != STATE.FIND)
                follow_Spare_Time = 0;

            if (state != STATE.IDLE_PATROL) nav.autoBraking = false;

            if (state != STATE.ATTACK) { fireChecker = 0; nav.isStopped = false; }

        }
        if (state != STATE.ROAMER)
        {
            roamerCount = 0;
            roamerTimer = 0;
        }

        if (nav.velocity != Vector3.zero) bugCountDown = 0;
        // 해당 상태가 ROAMER 상태일 때에만 작동함.
        // 만약, 이동 속도가 0이 되면 bugCountDown을 증가시킴
        // 속도가 0이 아니라면, BugCountDown을 0으로 초기화


        //==========================  FSM ==========================


        switch (state)
        {
            //==========================  IDLE ==========================

            case STATE.IDLE:

                if (animator != null)
                {
                    animator.SetInteger("State", 0);
                }
                else
                {
                    Debug.LogError("Animator is null in Update!");
                }
                rotationTimer += Time.deltaTime;

                nav.SetDestination(startPoint);

                float dis = Vector3.Distance(this.transform.position, startPoint);

                if (dis > 1f)
                {
                    animator.SetInteger("State", 1);
                    RotationEnemy(startPoint);
                }
                else animator.SetInteger("State", 0);


                if (isRotateEnemy)
                    RotationIdle();

                break;




            //==========================  IDLE_PATROL ==========================

            case STATE.IDLE_PATROL:
                animator.SetInteger("State", 0);
                if (!isPatrol)
                {
                    state = STATE.IDLE;
                }
                else
                {
                    animator.SetBool("isPatrol", true);
                    state = STATE.IDLE_PATROL;
                }
                //PatrolEnemy(patrolPoints);
                nav.autoBraking = true;
                nav.speed = 5f;
                nav.velocity = nav.desiredVelocity;
                if (nav.destination != null) RotationEnemy(nav.destination);
                if (nullChecker)
                {
                    nav.SetDestination(startpos);
                    //Debug.Log(nav.destination);
                    if (Vector3.Distance(startpos, this.transform.position) < 1.5f)
                    {
                        Debug.Log(nav.destination);

                        curnode = backupNode;
                        nullChecker = false;
                    }
                }

                else
                {

                    if (curnode.next_Node == null)
                    {
                        nav.SetDestination(curnode.Get_Now_Node());

                        if (Vector3.Distance(curnode.Get_Now_Node(), this.transform.position) < 0.3f)
                        {
                            nullChecker = true;
                        }
                    }

                    else
                    {
                        nextpos = curnode.Get_Next_Node();

                        nav.SetDestination(nextpos);

                        if (Vector3.Distance(nextpos, this.transform.position) < 0.3f)
                        {
                            curnode = curnode.next_Node;
                            nextpos = curnode.Get_Now_Node();
                        }
                    }
                }


                break;




            //==========================  ROAMER ==========================

            case STATE.ROAMER:
                animator.SetInteger("State", 1);

                if (nav.destination != null) RotationEnemy(nav.destination);

                roamerTimer += Time.deltaTime;
                bugCountDown += Time.deltaTime;
                if (bugCountDown > 1.5f)
                {
                    SetroamerDestination();
                }

                // ROAMER 상태일 때, 도착을 하지 못했을 경우 실행.
                // 1.5초가 넘게 되면 새로운 경로를 찾도록 SetRoamerDestination() 함수를 호출함.

                if (roamerCount < 10 || roamerTimer <= 8f)
                {
                    if (roamerCount == 0)
                    {
                        SetroamerDestination();
                    }
                    else
                    {
                        if (IsReachedroamerDestination())
                        {
                            SetroamerDestination();
                        }
                    }
                }
                else
                {
                    if (isPatrol) state = STATE.IDLE_PATROL;
                    else state = STATE.IDLE;
                }

                break;





            //==========================  FIND ==========================

            case STATE.FIND:
                animator.SetInteger("State", 1);

                //if (gun.gunName == "Meele")
                //{
                //    raycastDistance = es.raycastDistance_;

                //    if (raycastDistance < 1.2f) state = STATE.ATTACK;
                //}

                if (!lostUser)
                {
                    if (canShot)
                    {
                        this.state = STATE.ATTACK;
                    }
                    follow_Spare_Time = 0;

                    es.RotateToUser();
                    es.FollowUser();
                }
                else
                {
                    follow_Spare_Time += Time.deltaTime;
                    if (follow_Spare_Time <= 3.5f)
                    {
                        es.RotateToUser();
                        es.FollowUser();
                    }
                    else
                    {
                        state = STATE.ROAMER;
                    }
                }

                break;





            //==========================  ATTACT  ==========================

            case STATE.ATTACK:

                // gun class를 상속하고, start 또는 awake 함수에서 getcomponentchild로 컴포넌트를 초기화한다.
                // gun class에서 gunName을 가져온다.
                // switch 문에서 gunName을 사용해 무기를 설정.


                //this.transform.LookAt(player);

                //fireChecker += Time.deltaTime;
                //nav.isStopped = true;

                if (lostUser) state = STATE.FIND;

                switch (gun.gunName)
                {
                    case "SMG":  //SMG
                                 //if (!canShot)
                                 //{
                                 //    this.state = STATE.FIND;
                                 //    break;
                                 //}
                                 //else StartCoroutine(gun.Enemy_fire());

                        if (!canShot)
                        {

                            this.state = STATE.FIND;
                            break;
                        }
                        else if (es.SearchUser())
                        {
                            animator.SetInteger("State", 2);
                            RotationEnemy(player.transform.position);
                            StartCoroutine(gun.Enemy_fire());
                        }

                        //if(gun.Enemy_Fire(fireChecker)) fireChecker = 0;

                        break;


                    case "Rifle":  // �������� ��

                        if (!canShot)
                        {
                            this.state = STATE.FIND;
                            break;
                        }
                        else if (es.SearchUser())
                        {
                            animator.SetInteger("State", 2);
                            RotationEnemy(player.transform.position);
                            StartCoroutine(gun.Enemy_fire());
                        }
                        break;


                    case "HandGun":

                        if (!canShot)
                        {
                            this.state = STATE.FIND;
                            break;
                        }
                        else if (es.SearchUser())
                        {
                            animator.SetInteger("State", 2);
                            RotationEnemy(player.transform.position);
                            StartCoroutine(gun.Enemy_fire());
                        }
                        break;


                    case "Shotgun":

                        if (!canShot)
                        {
                            this.state = STATE.FIND;
                            break;
                        }
                        else if (es.SearchUser())
                        {
                            animator.SetInteger("State", 2);
                            RotationEnemy(player.transform.position);
                            StartCoroutine(gun.Enemy_Shotgun_Fire());
                        }
                        break;


                    default:

                        if (!canShot)
                        {
                            this.state = STATE.FIND;
                            break;
                        }
                        else if (es.SearchUser())
                        {
                            animator.SetInteger("State", 2);

                            StartCoroutine(MeeleAttack());
                        }
                        //StartCoroutine(MeeleAttack());

                        //RaycastHit[] hits;

                        //if (0.3f < meeleTimer && meeleTimer <= 1f)
                        //{
                        //    hits = Physics.SphereCastAll(transform.position, 1.5f, Vector3.forward, 1.5f);
                        //    foreach (RaycastHit hit in hits)
                        //    {
                        //        if (hit.collider.CompareTag("Player") && hit.collider != null)
                        //        {
                        //            Debug.Log("���� ���");
                        //        }

                        //        else state = STATE.FIND;
                        //    }
                        //}                       


                        break;
                }

                break;




            //==========================  DEAD  ==========================

            case STATE.DEAD:
                //animator.SetBool("isDead", true);

                //this.gameObject.transform.Rotate(90, 0, 0);
                Destroy(range.gameObject);
                AlertImDead();

                if (GetComponent<Rigidbody>() == null)
                {
                    gameObject.AddComponent<Rigidbody>();
                    GameObject blood = Instantiate(bloodEffect);
                    blood.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 1f, this.transform.position.z);
                    blood.transform.rotation = Quaternion.identity;

                    DropGun();
                    //this.gameObject.GetComponent<Rigidbody>().mass = 3;
                    this.gameObject.GetComponent<Rigidbody>().useGravity = false;
                    this.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    this.gameObject.GetComponent<CapsuleCollider>().radius = 1.5f;
                    SFX.PlayOneShot(deadSound);
                    nav.enabled = false;

                    clear.RemoveListEnemy(this.gameObject);
                }
                //this.GetComponent<Rigidbody>(). = true;

                //return;

                break;
            default:
                break;
        }
    }
    // 이걸 총알로 재활용 하면 될 듯함
    public void DropGun()
    {
        GameObject dropGun = Instantiate(gameObject);
        dropGun.transform.position = this.gameObject.transform.position;
        dropGun.transform.rotation = Quaternion.identity;
        dropGun.transform.Rotate(0, 0, 90f);
        dropGun.transform.Rotate(0, 45f, 0);
        dropGun.transform.Translate(0, -2f, 0);

        dropGun.AddComponent<BoxCollider>();
        dropGun.AddComponent<Rigidbody>();

        dropGun.GetComponent<Rigidbody>().isKinematic = true;

        //Destroy(gun.gameObject);

    }

    // 플레이어 콘트롤러가 하나 더 필요함.
    private IEnumerator MeeleAttack()
    {
        if (!coroutineChecker)
        {

            animator.SetInteger("State", 2);
            gun.MeeleSFX();

            coroutineChecker = true;
            // 애니메이션 종료 후 (재실행 대기 시간)

            yield return new WaitForSeconds(1.1f);

            if (state != STATE.DEAD)
            {
                RaycastHit[] hits;
                hits = Physics.SphereCastAll(transform.position, 3.2f, Vector3.up);

                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider != null && hit.collider.CompareTag("Player"))
                    {
                        if (hit.collider != null)
                        {
                            Player_Controller playerController = hit.collider.GetComponent<Player_Controller>();
                            if (playerController != null)
                            {
                                playerController.DeadEffect();
                            }
                        }
                    }
                }
            }

            coroutineChecker = false;

        }
        else
        {
            yield return null;
        }

    }

}

*/