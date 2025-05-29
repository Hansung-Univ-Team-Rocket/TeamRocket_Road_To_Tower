using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("��Ʈ UI ����")]
    public GameObject heartPrefab;
    public Sprite fullHeart, halfHeart, emptyHeart;
    public Transform heartContainer;
    private List<Image> hearts = new List<Image>();

    [Header("Ÿ�̸� & ���ھ�")]
    public TextMeshProUGUI timeText;
    private float timer = 0f;
    public TextMeshProUGUI scoreText;
    private int score = 0;

    [Header("���ӿ��� �Ŵ���")]
    public GameOverManager gameOverManager;
    public int totalKills = 0;
    private string causeOfDeath = "Die by enemy";
    private bool isDead = false;

    // ���� HP ������
    private int previousHP;

    void Start()
    {
        // ��Ʈ UI ����
        CreateHearts();
        UpdateHearts();

        // �ʱ� HP ����
        previousHP = PlayerStatusInfo.playerHP;

        // Ÿ�̸ӡ����ھ� �ʱ�ȭ
        timer = 0f;
        scoreText.text = "Score\n0";
        totalKills = 0;
    }

    void Update()
    {
        // Ÿ�̸� ������Ʈ
        timer += Time.deltaTime;
        int m = Mathf.FloorToInt(timer / 60f);
        int s = Mathf.FloorToInt(timer % 60f);
        timeText.text = $"Time: {m:00}:{s:00}";

        // HP ��ȭ ����
        if (previousHP != PlayerStatusInfo.playerHP)
        {
            UpdateHearts();
            previousHP = PlayerStatusInfo.playerHP;
        }

        // ��� üũ
        if (PlayerStatusInfo.playerHP <= 0 && !isDead)
        {
            Die();
        }
    }

    // �÷��̾ �������� ���� �� ȣ��
    public void OnPlayerDamaged(int damage, string cause = "Hit by enemy")
    {
        PlayerStatusInfo.playerHP = Mathf.Max(PlayerStatusInfo.playerHP - damage, 0);
        causeOfDeath = cause;
        UpdateHearts();
    }

    // �÷��̾ ȸ���� �� ȣ��
    public void OnPlayerHealed(int healAmount)
    {
        PlayerStatusInfo.playerHP = Mathf.Min(PlayerStatusInfo.playerHP + healAmount, PlayerStatusInfo.maxPlayerHP);
        UpdateHearts();
    }

    // ���� �߰�
    public void AddScore(int amount)
    {
        score += amount;
        scoreText.text = "Score\n" + score;
    }

    // ų ī��Ʈ �߰�
    public void AddKill()
    {
        totalKills++;
        AddScore(100); // ų�� 100��
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
        // ��Ʈ ���� ��� (HP 2�� ��Ʈ 1��)
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
            // �� ��Ʈ�� ��Ÿ���� HP ���
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

    // ����׿� �޼���
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