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

        //stageText.text = $"Clear stage: {stage}";// 스테이지 

        int minutes = Mathf.FloorToInt(playTime / 60f);
        int seconds = Mathf.FloorToInt(playTime % 60f);
        timeText.text = $"Play Time: {minutes:00}:{seconds:00}";// 플레이한 시간

        scoreText.text = $"Score: {score}";// 획득한 스코어
        killText.text = $"Kill Monster: {kills}";// 죽인 몬스터 수
        causeText.text = $"Die cause: {cause}"; // 죽은 원인
    }

    public void OnRetryButtonClick()
    {
        Time.timeScale = 1f; // 정지 해제
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 현재 씬 다시 로드
    }
}

