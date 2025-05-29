using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // 게임 시작 버튼 클릭 시
    public void OnStartGame()
    {
        SceneManager.LoadScene("GameScene"); // ← 게임 플레이 씬 이름으로 수정
    }

    // 게임 종료 버튼 클릭 시
    public void OnQuitGame()
    {
        Debug.Log("게임 종료!");
        Application.Quit(); // 빌드된 게임 종료
    }

    //  설정 버튼 클릭 시
    public GameObject settingPanel;
    public void OnOpenSettings()
    {
        settingPanel.SetActive(true);
    }
    public void OnCloseSettings()
    {
        settingPanel.SetActive(false);
    }
}
