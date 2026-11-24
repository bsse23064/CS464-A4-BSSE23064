using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] int creditsIndex = 6;
    [SerializeField] GameObject loadingPanel;
    [SerializeField] Image progressBar;

    bool loading;

    public void Load(int buildIndex)
    {
        if (loading) return;
        Time.timeScale = 1f;
        StartCoroutine(LoadRoutine(buildIndex));
    }

    public void Reload() => Load(SceneManager.GetActiveScene().buildIndex);

    public void LoadNext()
    {
        int current = SceneManager.GetActiveScene().buildIndex;
        int next = (current >= creditsIndex) ? 0 : current + 1;
        Load(next);
    }

    public void LoadMenu() => Load(0);

    IEnumerator LoadRoutine(int buildIndex)
    {
        loading = true;
        if (loadingPanel != null) loadingPanel.SetActive(true);

        AsyncOperation op = SceneManager.LoadSceneAsync(buildIndex);
        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / 0.9f);
            if (progressBar != null) progressBar.fillAmount = progress;
            yield return null;
        }
    }
}