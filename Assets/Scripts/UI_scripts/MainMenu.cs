using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        Time.timeScale = 1f;                  // in case you arrive from a paused level
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Play() => SceneManager.LoadScene("Level1");

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}