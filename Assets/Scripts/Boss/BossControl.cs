using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

public class BossControl : MonoBehaviour
{
    public GameObject player;

    public Transform LHand;
    public Transform RHand;
    public Transform shootPoint;

    public GameObject straightBulletPrefab;
    public GameObject homingBulletPrefab;
    public GameObject SpreadBulletPrefab;
    public GameObject warningPlanePrefab;
    public GameObject spikePrefab;
    public GameObject childMonsterPrefab;
    private GameObject Floor;
    private Animator anim;
    // private GameObject headChild = null;
    // private GameObject leftShoulderChild = null;
    // private GameObject rightShoulderChild = null;

    public AudioClip AwakeSound;
    public AudioClip DashSound;
    public AudioClip ShootSound;
    public AudioClip SpreadSound;
    public AudioClip WarningSound;
    public AudioClip DeathSound;
    private AudioSource audioSource;


    public float HP = 2000f;
    public float dashSpeed = 20f;
    public float patternCooldown = 2f;
    public int damage = 5;
    public float fanAngle = 60f;
    public int bulletCount = 10;
    public float warningDuration = 3f;
    public float spikeDuration = 2f;
    public int spikeCount = 5;
    [SerializeField] float damageCooldown = 1.0f; // 데미지 쿨타임 (1초)
    [SerializeField] bool _isDead = false;
    [SerializeField] CapsuleCollider _capsuleCollider;
    private bool _canDamage = true; // 데미지를 줄 수 있는지 여부
    // private float headHeight = 3.0f;
    // private float shoulderHeight = 1.5f;
    // private float shoulderOffset = 1.5f;

    private Vector3 spikeAreaSize = new Vector3(25f, 0f, 25f);

    private bool isAttacking = false;
    private bool Dash = false;

    void Start()
    {
        player = GameObject.FindWithTag("CameraPos");
        Floor = GameObject.FindWithTag("Floor");
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
        audioSource.PlayOneShot(AwakeSound);

        StartCoroutine(Delayed());
    }

    void Update()
    {
        if (player == null) return;
        if (_isDead == false)
        {
            if (!Dash)
            {
                Vector3 directionToPlayer = player.transform.position - transform.position;
                directionToPlayer.y = 0; // 수평 회전만 고려
                if (directionToPlayer != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer.normalized);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f); // 부드럽게 회전
                }
            }
        }
    }
    void ChangeBaseMapToRed()
    {
        // 모든 자식 MeshRenderer + SkinnedMeshRenderer 가져오기
        var meshRenderers = GetComponentsInChildren<Renderer>();

        foreach (var renderer in meshRenderers)
        {
            foreach (var mat in renderer.materials) // material == sharedMaterial 아님
            {
                if (mat.HasProperty("_BaseColor"))
                {
                    mat.SetColor("_BaseColor", Color.red);
                }
            }
        }
    }
    void ChangeBaseMapToWhite()
    {
        // 모든 자식 MeshRenderer + SkinnedMeshRenderer 가져오기
        var meshRenderers = GetComponentsInChildren<Renderer>();

        foreach (var renderer in meshRenderers)
        {
            foreach (var mat in renderer.materials) // material == sharedMaterial 아님
            {
                if (mat.HasProperty("_BaseColor"))
                {
                    mat.SetColor("_BaseColor", Color.white);
                }
            }
        }
    }

    IEnumerator Delayed()
    {
        yield return new WaitForSeconds(3f); // 등장 후 대기 시간 (3초 등 자유 설정)
        StartCoroutine(PatternLoop());
    }

    IEnumerator PatternLoop()
    {
        while (true)
        {
            if (_isDead)
            {
                yield break;
            }
            if (!isAttacking)
            {
                isAttacking = true;

                int pattern = Random.Range(0, 5);
                switch (pattern)
                {
                    case 0:
                        ChangeBaseMapToRed();
                        yield return StartCoroutine(DashPattern());
                        break;
                    case 1:
                        yield return StartCoroutine(StraightShotPattern());
                        break;
                    case 2:
                        yield return StartCoroutine(HomingShotPattern());
                        break;
                    case 3:
                        yield return StartCoroutine(SpreadPattern());
                        break;
                    case 4:
                        yield return StartCoroutine(SpikePattern());
                        break;
                        // case 5:
                        //     yield return StartCoroutine(CreateChildMonsters());
                        //     break;
                }

                yield return new WaitForSeconds(patternCooldown);
                isAttacking = false;
            }

            yield return null;
        }
    }

    // 패턴 1: 플레이어를 향한 돌진
    IEnumerator DashPattern()
    {
        float preparationTime = 0f;

        Dash = true;
        // 준비 시간 동안 플레이어를 계속 바라봄
        while (preparationTime < 2.0f)
        {
            if (_isDead)
            {
                yield break;
            }
            Vector3 directionToPlayer = (player.transform.position - transform.position);
            directionToPlayer.y = 0;
            if (directionToPlayer != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(directionToPlayer.normalized);

            preparationTime += Time.deltaTime;
            yield return null;
        }

        // 돌진 시작 시점의 플레이어 위치를 기준으로 돌진
        ChangeBaseMapToWhite();
        Vector3 dashTarget = player.transform.position;
        dashTarget.y = transform.position.y;

        transform.forward = (dashTarget - transform.position).normalized;

        // 해당 위치까지 돌진

        anim.SetBool("run", true);
        audioSource.PlayOneShot(DashSound);
        while (Vector3.Distance(transform.position, dashTarget) > 0.1f)
        {
            if (_isDead)
            {
                yield break;
            }
            transform.position = Vector3.MoveTowards(transform.position, dashTarget, dashSpeed * Time.deltaTime);
            yield return null;
        }
        anim.SetBool("run", false);

        yield return new WaitForSeconds(1f);
        Dash = false;
    }

    // 패턴 2: 각 손에서 5발씩 비유도 탄환 발사
    IEnumerator StraightShotPattern()
    {
        anim.SetBool("shoot", true);


        for (int i = 0; i < 4; i++)
        {
            if (_isDead)
            {
                yield break;
            }
            Transform fireHand = RHand;

            FireStraightBullet(fireHand);
            audioSource.PlayOneShot(ShootSound);
            yield return new WaitForSeconds(0.6f); // 간격 조정 가능
        }
        anim.SetBool("shoot", false);

        yield return new WaitForSeconds(1f); // 패턴 종료 전 대기
    }

    // 패턴 3: 각 손에서 1발씩 유도 탄환 발사
    IEnumerator HomingShotPattern()
    {
        // 플레이어 위치 인식
        Vector3 targetPosition = player.transform.position;
        targetPosition.y = transform.position.y;
        // 플레이어를 바라보게
        Vector3 targetDirection = new Vector3(targetPosition.x - transform.position.x, 0, targetPosition.z - transform.position.z).normalized;
        transform.rotation = Quaternion.LookRotation(targetDirection);

        anim.SetBool("shoot", true);
        if (_isDead)
        {
            yield break;
        }
        FireHomingBullet(RHand);
        audioSource.PlayOneShot(ShootSound);
        yield return new WaitForSeconds(1f);

        FireHomingBullet(RHand);
        audioSource.PlayOneShot(ShootSound);
        yield return new WaitForSeconds(1f);
        anim.SetBool("shoot", false);
    }

    // 패턴 4: 몸의 중앙에서 부채꼴 모양으로 탄환을 흩뿌리며 발사
    IEnumerator SpreadPattern()
    {
        // 플레이어 위치 인식
        Vector3 targetPosition = player.transform.position;
        targetPosition.y = transform.position.y;
        // 플레이어를 바라보게
        Vector3 targetDirection = new Vector3(targetPosition.x - transform.position.x, 0, targetPosition.z - transform.position.z).normalized;
        transform.rotation = Quaternion.LookRotation(targetDirection);

        for (int i = 0; i < 4; i++) // 총 4회 반복
        {
            if (_isDead)
            {
                yield break;
            }
            if (i % 2 == 0)
            {
                anim.SetBool("spread", false);
                anim.SetBool("spread", true);
                audioSource.PlayOneShot(SpreadSound);
            }
            Vector3 directionToPlayer = (player.transform.position - shootPoint.position).normalized;
            float startAngle = -fanAngle / 2f;
            float angleStep = fanAngle / (bulletCount - 1);

            // 좌→우(기본), 우→좌(반전)
            if (i % 2 == 1)
            {
                startAngle = fanAngle / 2f;
                angleStep = -angleStep;
            }

            for (int j = 0; j < bulletCount; j++)
            {
                float angle = startAngle + (angleStep * j);
                Vector3 rotatedDirection = Quaternion.AngleAxis(angle, Vector3.up) * directionToPlayer;

                GameObject bullet = Instantiate(SpreadBulletPrefab, shootPoint.position, Quaternion.identity);
                Spread sb = bullet.GetComponent<Spread>();
                if (sb != null)
                {
                    sb.target = player.transform;
                    sb.SetDirection(rotatedDirection);
                }

                yield return new WaitForSeconds(0.05f); // 발사 간격
            }
            yield return new WaitForSeconds(0.5f); // 각 부채꼴 사이 간격
            if (i % 2 == 1)
            {
                anim.SetBool("spread", false);
                yield return new WaitForSeconds(1f); // 각 부채꼴 사이 간격
            }
        }

        yield return new WaitForSeconds(1f); // 패턴 종료 대기
    }

    // 패턴 5: 격자무늬의 장판을 바닥에 무작위로 생성
    IEnumerator SpikePattern()
    {
        // 플레이어 위치 인식
        Vector3 targetPosition = player.transform.position;
        targetPosition.y = transform.position.y;
        // 플레이어를 바라보게
        Vector3 targetDirection = new Vector3(targetPosition.x - transform.position.x, 0, targetPosition.z - transform.position.z).normalized;
        transform.rotation = Quaternion.LookRotation(targetDirection);

        for (int count = 0; count < 3; count++)
        {
            if (_isDead)
            {
                yield break;
            }
            anim.SetBool("spike", true);
            audioSource.PlayOneShot(WarningSound);
            List<Vector3> spikePositions = new List<Vector3>();
            List<GameObject> warningPlanes = new List<GameObject>();

            Vector3 floorSize = Floor.transform.localScale; // Floor 객체의 크기 (localScale)을 사용
            Vector3 floorCenter = Floor.transform.position; // Floor 중심 좌표

            float realSizeX = floorSize.x * 5f; // Plane의 실제 크기 고려
            float realSizeZ = floorSize.z * 5f;
            float cellSize = 5f; // 셀 한 칸 크기

            // 가능한 셀 목록 생성
            List<Vector3> availableCells = new List<Vector3>();

            for (int x = 0; x < Mathf.FloorToInt(realSizeX / cellSize); x++)
            {
                for (int z = 0; z < Mathf.FloorToInt(realSizeZ / cellSize); z++)
                {
                    float worldX = floorCenter.x - realSizeX / 2f + x * cellSize + cellSize / 2f;
                    float worldZ = floorCenter.z - realSizeZ / 2f + z * cellSize + cellSize / 2f;
                    availableCells.Add(new Vector3(worldX, 16, worldZ));
                }
            }

            // 셀 무작위 선택
            for (int i = 0; i < spikeCount && availableCells.Count > 0; i++)
            {
                int index = Random.Range(0, availableCells.Count);
                Vector3 cellPos = availableCells[index];
                availableCells.RemoveAt(index);

                GameObject warning = Instantiate(warningPlanePrefab, cellPos + Vector3.up * 0.1f, Quaternion.identity);
                warning.transform.localScale = new Vector3(cellSize, 0.1f, cellSize);
                warningPlanes.Add(warning);
                spikePositions.Add(cellPos);
            }

            // 일정 시간 대기 (경고 유지)
            yield return new WaitForSeconds(warningDuration);

            //  경고 제거, 가시 생성
            foreach (GameObject warning in warningPlanes)
            {
                Destroy(warning);
            }

            foreach (Vector3 pos in spikePositions)
            {
                GameObject spike = Instantiate(spikePrefab, pos, Quaternion.identity);
                spike.transform.localScale = new Vector3(cellSize, 0.1f, cellSize);
                Destroy(spike, spikeDuration);
            }
            anim.SetBool("spike", false);
            yield return new WaitForSeconds(1f); // 반복 간 간격
        }

        yield return new WaitForSeconds(1f);
    }
    // IEnumerator CreateChildMonsters()
    // {
    //     // 이미 생성된 자식 몬스터가 있으면 생성하지 않음
    //     if (headChild != null || leftShoulderChild != null || rightShoulderChild != null)
    //     {
    //         Debug.Log("자식 몬스터 이미 존재함, 생성 중단");
    //         yield break; // 코루틴 종료
    //     }
    // 
    //     float moveDuration = 1f;
    // 
    //     Vector3 bossCenterPos = transform.position;
    // 
    //     Vector3 headTargetPos = transform.position + Vector3.up * 3f;
    //     Vector3 leftShoulderTargetPos = LHand.position + Vector3.up * 1.5f;
    //     Vector3 rightShoulderTargetPos = RHand.position + Vector3.up * 1.5f;
    // 
    //     headChild = Instantiate(childMonsterPrefab, bossCenterPos, Quaternion.identity);
    //     leftShoulderChild = Instantiate(childMonsterPrefab, bossCenterPos, Quaternion.identity);
    //     rightShoulderChild = Instantiate(childMonsterPrefab, bossCenterPos, Quaternion.identity);
    // 
    //     headChild.transform.parent = transform;
    //     leftShoulderChild.transform.parent = transform;
    //     rightShoulderChild.transform.parent = transform;
    // 
    //     float elapsed = 0f;
    // 
    //     while (elapsed < moveDuration)
    //     {
    //         elapsed += Time.deltaTime;
    //         float t = Mathf.Clamp01(elapsed / moveDuration);
    // 
    //         headChild.transform.position = Vector3.Lerp(bossCenterPos, headTargetPos, t);
    //         leftShoulderChild.transform.position = Vector3.Lerp(bossCenterPos, leftShoulderTargetPos, t);
    //         rightShoulderChild.transform.position = Vector3.Lerp(bossCenterPos, rightShoulderTargetPos, t);
    // 
    //         Vector3 forwardDir = transform.forward;
    //         headChild.transform.rotation = Quaternion.LookRotation(forwardDir);
    //         leftShoulderChild.transform.rotation = Quaternion.LookRotation(forwardDir);
    //         rightShoulderChild.transform.rotation = Quaternion.LookRotation(forwardDir);
    // 
    //         yield return null;
    //     }
    // 
    //     headChild.transform.localPosition = Vector3.up * 3f;
    //     leftShoulderChild.transform.localPosition = transform.InverseTransformPoint(LHand.position) + Vector3.up * 1.5f;
    //     rightShoulderChild.transform.localPosition = transform.InverseTransformPoint(RHand.position) + Vector3.up * 1.5f;
    // 
    //     headChild.transform.localRotation = Quaternion.identity;
    //     leftShoulderChild.transform.localRotation = Quaternion.identity;
    //     rightShoulderChild.transform.localRotation = Quaternion.identity;
    // }


    void FireStraightBullet(Transform hand)
    {
        GameObject bullet = Instantiate(straightBulletPrefab, hand.position, Quaternion.identity);
        NBullet sb = bullet.GetComponent<NBullet>();
        if (sb != null)
        {
            sb.target = player.transform;
        }
    }

    void FireHomingBullet(Transform hand)
    {
        GameObject bullet = Instantiate(homingBulletPrefab, hand.position, Quaternion.identity);
        Homing homing = bullet.GetComponent<Homing>();
        if (homing != null)
        {
            homing.target = player.transform;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!_isDead && _canDamage && other.CompareTag("Player"))
        {
            Debug.LogWarning("In");
            PlayerStatusInfo.playerHP--;
            other.GetComponent<PlayerMovementController>().TakeDamaged();
            StartCoroutine(DamageCooldownCoroutine());
        }
    }
    private System.Collections.IEnumerator DamageCooldownCoroutine()
    {
        _canDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        _canDamage = true;
    }
    public void Damaged(int HitDamage)
    {
        Debug.Log("Boss Hit");
        HP -= HitDamage;
        if (HP <= 0)
        {
            _isDead = true;
            anim.SetBool("Death", true);
        }
    }
}
