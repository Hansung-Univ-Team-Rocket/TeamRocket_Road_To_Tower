using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class PlayerStatusInfo : MonoBehaviour
{

    [Header("Player Basic Info")]
    public static int maxPlayerHP = 20;
    public static int playerHP = 20;

    [Header("체력 관련")]
    public float maxHealth = 20f;          // Inspector 에 보이도록
    private float currentHealth;

    [Header("하트 UI 관련")]
    public GameObject heartPrefab;
    public Sprite fullHeart, halfHeart, emptyHeart;
    public Transform heartContainer;
    private List<Image> hearts = new List<Image>();

    [Header("타이머 & 스코어")]
    public TextMeshProUGUI timeText;
    private float timer = 0f;
    public TextMeshProUGUI scoreText;
    private int score = 0;

    [Header("게임오버 매니저")]
    public GameOverManager gameOverManager;
    public int totalKills = 0;
    private string causeOfDeath = "Die by enemy";
    private bool isDead = false;

    void Start()
    {
        // 1) 인스펙터용 maxHealth 로 초기화
        currentHealth = maxHealth;
        playerHP = Mathf.RoundToInt(maxHealth);
        maxPlayerHP = Mathf.RoundToInt(maxHealth);

        CreateHearts();
        UpdateHearts();

        // 타이머·스코어 초기화
        timer = 0f;
        scoreText.text = "Score\n0";
        totalKills = 0;
    }

    void Update()
    {
        timer += Time.deltaTime;
        int m = Mathf.FloorToInt(timer / 60f);
        int s = Mathf.FloorToInt(timer % 60f);
        timeText.text = $"Time: {m:00}:{s:00}";

        if (currentHealth <= 0f && !isDead)
            Die();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        TakeDamage(3.5f, "Hit by enemy");
        totalKills++;
        AddScore(100);
    }

    public void TakeDamage(float amount, string cause)
    {
        currentHealth = Mathf.Max(currentHealth - amount, 0f);
        playerHP = Mathf.RoundToInt(currentHealth);   // static 필드 동기화
        causeOfDeath = cause;
        UpdateHearts();
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        playerHP = Mathf.RoundToInt(currentHealth);
        UpdateHearts();
    }

    public void AddScore(int amount)
    {
        score += amount;
        scoreText.text = "Score\n" + score;
    }

    private void Die()
    {
        isDead = true;
        gameOverManager.ShowGameOver(
            timer,
            score,
            totalKills,
            causeOfDeath
        );
        Time.timeScale = 0f;
    }

    private void CreateHearts()
    {
        int heartCount = Mathf.CeilToInt(maxHealth / 2f);
        for (int i = 0; i < heartCount; i++)
        {
            var h = Instantiate(heartPrefab, heartContainer);
            hearts.Add(h.GetComponent<Image>());
        }
    }

    private void UpdateHearts()
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            float hpUnit = Mathf.Clamp(currentHealth - i * 2f, 0f, 2f);
            if (hpUnit >= 2f) hearts[i].sprite = fullHeart;
            else if (hpUnit >= 1f) hearts[i].sprite = halfHeart;
            else hearts[i].sprite = emptyHeart;
        }
    }
}
