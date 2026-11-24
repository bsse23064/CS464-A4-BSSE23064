using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class EndScreens : MonoBehaviour
{
    [SerializeField] PauseMenu pause;
    [SerializeField] GameObject losePanel;
    [SerializeField] GameObject winPanel;
    [SerializeField] GameObject loseFirst;
    [SerializeField] GameObject winFirst;

    public void ShowLose() => Show(losePanel, loseFirst);
    public void ShowWin() => Show(winPanel, winFirst);

    void Show(GameObject panel, GameObject first)
    {
        if (pause != null) pause.enabled = false; // prevents Esc from opening PauseMenu on top
        panel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        EventSystem.current.SetSelectedGameObject(first);
    }

    public void NextLevel()
    {
        if (pause != null) pause.Resume();
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextIndex);
        else
            SceneManager.LoadScene("MainMenu");
    }
}