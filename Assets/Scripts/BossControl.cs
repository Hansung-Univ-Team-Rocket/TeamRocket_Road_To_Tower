using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

public class BossControl : MonoBehaviour
{
    public Transform player;
    MeshRenderer[] meshs;

    public Transform LHand;
    public Transform RHand;
    public Transform shootPoint;

    public GameObject straightBulletPrefab;
    public GameObject homingBulletPrefab;
    public GameObject SpreadBulletPrefab;
    public GameObject warningPlanePrefab;
    public GameObject spikePrefab;
    private GameObject Floor;

    public float dashSpeed = 20f;
    public float patternCooldown = 2f;
    public int damage = 5;
    public float fanAngle = 60f;
    public int bulletCount = 10;
    public float warningDuration = 3f;
    public float spikeDuration = 2f;
    public int spikeCount = 5;

    private Vector3 spikeAreaSize = new Vector3(25f, 0f, 25f);

    private bool isAttacking = false;

    void Start()
    {
        Floor = GameObject.Find("Floor");
        meshs = GetComponentsInChildren<MeshRenderer>();
        StartCoroutine(PatternLoop());
    }

    IEnumerator PatternLoop()
    {
        while (true)
        {
            if (!isAttacking)
            {
                isAttacking = true;

                int pattern = Random.Range(0, 5);
                switch (pattern)
                {
                    case 0:
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
        // 플레이어 위치 인식
        Vector3 targetPosition = player.position;
        targetPosition.y = transform.position.y;
        // 플레이어를 바라보게
        Vector3 targetDirection = new Vector3(targetPosition.x - transform.position.x, 0, targetPosition.z - transform.position.z).normalized;
        transform.rotation = Quaternion.LookRotation(targetDirection);

        foreach (MeshRenderer mesh in meshs)
            mesh.material.color = Color.green;
        yield return new WaitForSeconds(2f);
        foreach (MeshRenderer mesh in meshs)
            mesh.material.color = Color.white;

        transform.forward = (targetPosition - transform.position).normalized;

        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, dashSpeed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(1f);
    }

    // 패턴 2: 각 손에서 5발씩 비유도 탄환 발사
    IEnumerator StraightShotPattern()
    {
        for (int i = 0; i < 5; i++)
        {
            // 번갈아가며 손을 선택
            Transform fireHand = (i % 2 == 0) ? RHand : LHand;

            FireStraightBullet(fireHand);
            yield return new WaitForSeconds(0.5f); // 간격 조정 가능
        }

        yield return new WaitForSeconds(1f); // 패턴 종료 전 대기
    }

    // 패턴 3: 각 손에서 1발씩 유도 탄환 발사
    IEnumerator HomingShotPattern()
    {
        FireHomingBullet(RHand);
        yield return new WaitForSeconds(1f);

        FireHomingBullet(LHand);
        yield return new WaitForSeconds(1f); // 다음 패턴과의 간격 유지
    }

    IEnumerator SpreadPattern()
    {
        for (int i = 0; i < 5; i++) // 총 5회 반복
        {
            Vector3 directionToPlayer = (player.position - shootPoint.position).normalized;
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
                    sb.target = player;
                    sb.SetDirection(rotatedDirection);
                }

                yield return new WaitForSeconds(0.05f); // 발사 간격
            }

            yield return new WaitForSeconds(0.5f); // 각 부채꼴 사이 간격
        }

        yield return new WaitForSeconds(1f); // 패턴 종료 대기
    }

    IEnumerator SpikePattern()
    {
        for (int count = 0; count < 3; count++)
        {
            List<Vector3> spikePositions = new List<Vector3>();
            List<GameObject> warningPlanes = new List<GameObject>();

            Vector3 floorSize = Floor.transform.localScale; // Floor 객체의 크기 (localScale)을 사용
            Vector3 floorCenter = Floor.transform.position; // Floor 중심 좌표

            float realSizeX = floorSize.x * 10f; // Plane의 실제 크기 고려
            float realSizeZ = floorSize.z * 10f;
            float cellSize = 10f; // 셀 한 칸 크기

            // 가능한 셀 목록 생성
            List<Vector3> availableCells = new List<Vector3>();

            for (int x = 0; x < Mathf.FloorToInt(realSizeX / cellSize); x++)
            {
                for (int z = 0; z < Mathf.FloorToInt(realSizeZ / cellSize); z++)
                {
                    float worldX = floorCenter.x - realSizeX / 2f + x * cellSize + cellSize / 2f;
                    float worldZ = floorCenter.z - realSizeZ / 2f + z * cellSize + cellSize / 2f;
                    availableCells.Add(new Vector3(worldX, 0, worldZ));
                }
            }

            // 셀 무작위 선택
            for (int i = 0; i < spikeCount && availableCells.Count > 0; i++)
            {
                int index = Random.Range(0, availableCells.Count);
                Vector3 cellPos = availableCells[index];
                availableCells.RemoveAt(index);

                GameObject warning = Instantiate(warningPlanePrefab, cellPos + Vector3.up * 0.01f, Quaternion.identity);
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
            yield return new WaitForSeconds(1f); // 반복 간 간격
        }

        yield return new WaitForSeconds(1f);
    }

    void FireStraightBullet(Transform hand)
    {
        GameObject bullet = Instantiate(straightBulletPrefab, hand.position, Quaternion.identity);
        NBullet sb = bullet.GetComponent<NBullet>();
        if (sb != null)
        {
            sb.target = player;
        }
    }

    void FireHomingBullet(Transform hand)
    {
        GameObject bullet = Instantiate(homingBulletPrefab, hand.position, Quaternion.identity);
        Homing homing = bullet.GetComponent<Homing>();
        if (homing != null)
        {
            homing.target = player;
        }
    }
}
