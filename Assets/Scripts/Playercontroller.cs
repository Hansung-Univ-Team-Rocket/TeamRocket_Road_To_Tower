using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
public class PlayerController : MonoBehaviour
{
    [Header("이동 관련")]
    public float moveSpeed = 5f;
    private Rigidbody rb;
    private Vector3 moveDirection;

    [Header("체력 관련")]
    public float maxHealth = 20f;          //  최대 체력 하트 3개=6hp 변경가능
    public float currentHealth = 20f;      // 현재 체력 0.5 단위 가능

    [Header("하트 UI 관련")]
    public GameObject heartPrefab;        // 하트 하나당 이미지 프리팹
    public Sprite fullHeart, halfHeart, emptyHeart;
    public Transform heartContainer;      // 하트들을 담을 부모 오브젝트
    private List<Image> hearts = new List<Image>();

    [Header("타임txt, 스코어txt")]
    public TextMeshProUGUI timeText; 
    private float timer = 0f;
    public TextMeshProUGUI scoreText;
    private int score = 0;

    public GameOverManager gameOverManager;

    public int clearedStage = 1;
    public int totalKills = 0;
    public string causeOfDeath = "Die by enemy";

    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        currentHealth = maxHealth; //체력 풀피로 초기화
        CreateHearts(); //체력에 맞춰 하트 UI생성
        UpdateHearts(); //현재 체력에 따라 하트 UI생성
    }

    void Update()
    {
        // 이동 입력 처리
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector3(moveX, 0f, moveZ).normalized;

        timer += Time.deltaTime; //시간 텍스트

        if (currentHealth <= 0)
        {
            Die(); //플레이어 죽음
        }

        int minutes = Mathf.FloorToInt(timer / 60f); //분
        int seconds = Mathf.FloorToInt(timer % 60f); //초

        timeText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
    }


    //플레이어 움직임
    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
    }


    //플레이어 죽을 시 패널 창에 뜰 글자들, GameManager에서 불러옴
    void Die()
    {

        if (isDead) return; 
        isDead = true;

        gameOverManager.ShowGameOver(
        clearedStage,
        timer,
        score,
        totalKills,
        causeOfDeath
    );

        Time.timeScale = 0f; // 게임 멈춤
    }

    //스코어 텍스트
    public void AddScore(int amount)
    {
        score += amount;
        scoreText.text = "Score\n"+ score.ToString();
    }

    // 적과 충돌시 데미지
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            TakeDamage(3.5f,"Hit by enemy"); // 데미지 계수 정할 수 있음
            totalKills += 1; // 적 킬수
            AddScore(200); // 적 킬당 점수 
            Destroy(other.gameObject); // 적 삭제
        }
    }


    //데미지 받음
    public void TakeDamage(float amount, string cause)
    {
        currentHealth = Mathf.Max(currentHealth - amount, 0f);
        Debug.Log("현재 체력: " + currentHealth);
        causeOfDeath = cause;
        UpdateHearts();
    }

    // 체력 회복
    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHearts();
    }


    //하트 prefab불러오기
    void CreateHearts()
    {
        int heartCount = Mathf.CeilToInt(maxHealth / 2f);
        for (int i = 0; i < heartCount; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartContainer);
            Image img = heart.GetComponent<Image>();
            hearts.Add(img);
        }
    }


    // 한 칸 하트, 반 칸 하트, 빈 하트 구현
    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            float heartHealth = Mathf.Clamp(currentHealth - (i * 2f), 0f, 2f);

            if (heartHealth >= 2f)
                hearts[i].sprite = fullHeart;  //한 칸=반 칸 2개
            else if (heartHealth >= 1f)
                hearts[i].sprite = halfHeart; //반 칸
            else
                hearts[i].sprite = emptyHeart;//빈 칸
        }
    }
}
