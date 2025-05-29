using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class PlayerHealthUI : MonoBehaviour
{
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

    // 이전 HP 추적용
    private int previousHP;

    void Start()
    {
        // 하트 UI 생성
        CreateHearts();
        UpdateHearts();

        // 초기 HP 저장
        previousHP = PlayerStatusInfo.playerHP;

        // 타이머·스코어 초기화
        timer = 0f;
        scoreText.text = "Score\n0";
        totalKills = 0;
    }

    void Update()
    {
        // 타이머 업데이트
        timer += Time.deltaTime;
        int m = Mathf.FloorToInt(timer / 60f);
        int s = Mathf.FloorToInt(timer % 60f);
        timeText.text = $"Time: {m:00}:{s:00}";

        // HP 변화 감지
        if (previousHP != PlayerStatusInfo.playerHP)
        {
            UpdateHearts();
            previousHP = PlayerStatusInfo.playerHP;
        }

        // 사망 체크
        if (PlayerStatusInfo.playerHP <= 0 && !isDead)
        {
            Die();
        }
    }

    // 플레이어가 데미지를 받을 때 호출
    public void OnPlayerDamaged(int damage, string cause = "Hit by enemy")
    {
        PlayerStatusInfo.playerHP = Mathf.Max(PlayerStatusInfo.playerHP - damage, 0);
        causeOfDeath = cause;
        UpdateHearts();
    }

    // 플레이어가 회복할 때 호출
    public void OnPlayerHealed(int healAmount)
    {
        PlayerStatusInfo.playerHP = Mathf.Min(PlayerStatusInfo.playerHP + healAmount, PlayerStatusInfo.maxPlayerHP);
        UpdateHearts();
    }

    // 점수 추가
    public void AddScore(int amount)
    {
        score += amount;
        scoreText.text = "Score\n" + score;
    }

    // 킬 카운트 추가
    public void AddKill()
    {
        totalKills++;
        AddScore(100); // 킬당 100점
    }

    private void Die()
    {
        isDead = true;

        if (gameOverManager != null)
        {
            gameOverManager.ShowGameOver(
                timer,
                score,
                totalKills,
                causeOfDeath
            );
        }

        Time.timeScale = 0f;
    }

    private void CreateHearts()
    {
        // 하트 개수 계산 (HP 2당 하트 1개)
        int heartCount = Mathf.CeilToInt(PlayerStatusInfo.maxPlayerHP / 2f);

        for (int i = 0; i < heartCount; i++)
        {
            GameObject heartGO = Instantiate(heartPrefab, heartContainer);
            Image heartImage = heartGO.GetComponent<Image>();
            if (heartImage != null)
            {
                hearts.Add(heartImage);
            }
        }
    }

    private void UpdateHearts()
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            // 각 하트가 나타내는 HP 계산
            float hpForThisHeart = Mathf.Clamp(PlayerStatusInfo.playerHP - (i * 2), 0, 2);

            if (hpForThisHeart >= 2f)
            {
                hearts[i].sprite = fullHeart;
            }
            else if (hpForThisHeart >= 1f)
            {
                hearts[i].sprite = halfHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }
        }
    }

    // 디버그용 메서드
    [ContextMenu("Test Damage")]
    void TestDamage()
    {
        OnPlayerDamaged(1, "Test damage");
    }

    [ContextMenu("Test Heal")]
    void TestHeal()
    {
        OnPlayerHealed(2);
    }
}