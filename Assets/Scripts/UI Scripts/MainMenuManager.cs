
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // ���� ���� ��ư Ŭ�� ��
    public void OnStartGame()
    {
        SceneManager.LoadScene("GameScence"); // �� ���� �÷��� �� �̸����� ����
    }

    // ���� ���� ��ư Ŭ�� ��
    public void OnQuitGame()
    { 
        Application.Quit(); // ����� ���� ����
    }

    //  ���� ��ư Ŭ�� ��
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
