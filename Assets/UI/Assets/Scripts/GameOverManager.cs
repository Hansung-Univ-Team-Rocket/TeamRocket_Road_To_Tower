/*
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverPanel;

    //public TextMeshProUGUI stageText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI killText;
    public TextMeshProUGUI causeText;

    public void ShowGameOver(int stage, float playTime, int score, int kills, string cause)
    {
        gameOverPanel.SetActive(true);

        //stageText.text = $"Clear stage: {stage}";// �������� 

        int minutes = Mathf.FloorToInt(playTime / 60f);
        int seconds = Mathf.FloorToInt(playTime % 60f);
        timeText.text = $"Play Time: {minutes:00}:{seconds:00}";// �÷����� �ð�

        scoreText.text = $"Score: {score}";// ȹ���� ���ھ�
        killText.text = $"Kill Monster: {kills}";// ���� ���� ��
        causeText.text = $"Die cause: {cause}"; // ���� ����
    }

    public void OnRetryButtonClick()
    {
        Time.timeScale = 1f; // ���� ����
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // ���� �� �ٽ� �ε�
    }
}
*/
