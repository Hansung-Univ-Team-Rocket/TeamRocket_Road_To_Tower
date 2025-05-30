
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    //public GameObject gameOverPanel;

    //public TextMeshProUGUI stageText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI scoreText;
    //public TextMeshProUGUI killText;
    //public TextMeshProUGUI causeText;

    public void ShowGameOver( float playTime, int score)
    {
        //gameOverPanel.SetActive(true);

        //stageText.text = $"Clear stage: {stage}";// �������� 

        int minutes = Mathf.FloorToInt(playTime / 60f);
        int seconds = Mathf.FloorToInt(playTime % 60f);
        timeText.text = $"Play Time: {minutes:00}:{seconds:00}";// �÷����� �ð�

        scoreText.text = $"Score: {score}";// ȹ���� ���ھ�
        //killText.text = $"Kill Monster: {kills}";// ���� ���� ��
        //causeText.text = $"Die cause: {cause}"; // ���� ����
    }

    public void OnRetryButtonClick()
    {
        Debug.LogError("In???????????");
        Time.timeScale = 1f; // ���� ����
        PlayerStatusInfo.playerHP = PlayerStatusInfo.maxPlayerHP;
        SceneManager.LoadScene("MainMenu"); // ���� �� �ٽ� �ε�
    }
}

