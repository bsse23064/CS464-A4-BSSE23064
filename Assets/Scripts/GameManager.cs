using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Level Rules")]
    [SerializeField] int targetScore = 10;
    [SerializeField] int lives = 3;
    [SerializeField] float respawnDelay = 0.6f;
    [SerializeField] float spawnProtection = 1.5f;
    [SerializeField] Health player;
    [SerializeField] Transform respawnPoint;

    [Header("HUD & End Screens")]
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text livesText;
    [SerializeField] TMP_Text winTimeText;        // optional: shows completion time & "NEW BEST!"
    [SerializeField] PauseMenu pauseMenu;
    [SerializeField] LevelLoader loader;
    [SerializeField] GameObject winPanel;
    [SerializeField] GameObject losePanel;
    [SerializeField] GameObject winFirstButton;
    [SerializeField] GameObject loseFirstButton;

    [Header("Audio (Part B)")]
    [SerializeField] AudioClip winClip;
    [SerializeField] AudioClip loseClip;

    [Header("Locked Final Coin (Optional)")]
    [SerializeField] GameObject finalCoin;
    [SerializeField] int coinsToUnlock = 3;

    int score;
    bool ended;

    void Awake() => Instance = this;

    void Start()
    {
        AutoBindReferences();
        UpdateUI();
    }

    void AutoBindReferences()
    {
        if (pauseMenu == null) pauseMenu = FindFirstObjectByType<PauseMenu>();
        if (loader == null) loader = FindFirstObjectByType<LevelLoader>();
        if (player == null) player = FindFirstObjectByType<Health>();

        if (respawnPoint == null)
        {
            GameObject rp = GameObject.FindWithTag("Respawn");
            if (rp != null) respawnPoint = rp.transform;
        }

        // Auto-locate HUD_Canvas children if left unassigned on the prefab
        Canvas canvas = pauseMenu != null ? pauseMenu.GetComponent<Canvas>() : FindFirstObjectByType<Canvas>();
        if (canvas != null)
        {
            if (scoreText == null)
                scoreText = canvas.transform.Find("HUD/ScorePanel/ScoreText")?.GetComponent<TMP_Text>();
            if (livesText == null)
                livesText = canvas.transform.Find("HUD/LivesPanel/LivesText")?.GetComponent<TMP_Text>();

            if (winPanel == null)
                winPanel = canvas.transform.Find("WinPanel")?.gameObject;
            if (losePanel == null)
                losePanel = canvas.transform.Find("LosePanel")?.gameObject;

            if (winPanel != null)
            {
                if (winTimeText == null)
                    winTimeText = winPanel.transform.Find("Box/WinTimeText")?.GetComponent<TMP_Text>();
                if (winFirstButton == null)
                    winFirstButton = winPanel.transform.Find("Box/ButtonGroup/NextLevelButton")?.gameObject;
            }

            if (losePanel != null && loseFirstButton == null)
                loseFirstButton = losePanel.transform.Find("Box/ButtonGroup/RestartButton")?.gameObject;
        }
    }

    public void AddScore(int amount)
    {
        if (ended) return;
        score += amount;
        UpdateUI();

        if (finalCoin != null && score >= coinsToUnlock)
            finalCoin.SetActive(true);

        if (score >= targetScore)
            Win();
    }

    public void UpdateUI()
    {
        if (scoreText != null) scoreText.text = $"Score: {score} / {targetScore}";
        if (livesText != null) livesText.text = $"Lives: {lives}";
    }

    public void PlayerDied()
    {
        if (ended) return;
        lives--;
        UpdateUI();

        if (lives <= 0) Lose();
        else StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnDelay);
        if (player == null || respawnPoint == null) yield break;

        Rigidbody body = player.GetComponent<Rigidbody>();
        if (body != null)
        {
            body.position = respawnPoint.position;
            body.linearVelocity = Vector3.zero;
        }
        player.Restore(spawnProtection);
    }

    void Win()
    {
        if (ended) return;
        ended = true;

        int level = gameObject.scene.buildIndex;
        float time = Time.timeSinceLevelLoad;
        bool isNewBest = SaveData.Record(level, time);
        SaveData.Unlock(level + 1);

        if (winTimeText != null)
            winTimeText.text = isNewBest
                ? $"Time: {time:F2}s  (NEW BEST!)"
                : $"Time: {time:F2}s  (Best: {SaveData.GetBest(level):F2}s)";

        if (SfxPlayer.Instance != null && winClip != null)
            SfxPlayer.Instance.Play(winClip);

        OpenEndScreen(winPanel, winFirstButton);
    }

    void Lose()
    {
        if (ended) return;
        ended = true;

        if (SfxPlayer.Instance != null && loseClip != null)
            SfxPlayer.Instance.Play(loseClip);

        OpenEndScreen(losePanel, loseFirstButton);
    }

    void OpenEndScreen(GameObject panel, GameObject firstSelected)
    {
        if (pauseMenu != null) pauseMenu.enabled = false; // prevents Esc from opening PauseMenu on top
        if (panel != null) panel.SetActive(true);

        Time.timeScale = 0f;
        AudioListener.pause = false; // keep win/lose SFX and UI sounds audible
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (EventSystem.current != null && firstSelected != null)
            EventSystem.current.SetSelectedGameObject(firstSelected);
    }

    // Button handlers routed strictly through LevelLoader
    public void Restart()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        if (loader != null) loader.Reload();
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        if (loader != null) loader.LoadNext();
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        if (loader != null) loader.LoadMenu();
    }
}