using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] LevelLoader loader;
    [SerializeField] Button[] levelButtons;   // size 5: index 0 = Level 1 ... index 4 = Level 5
    [SerializeField] TMP_Text[] bestLabels;     // matching TMP_Text on or under each button

    void OnEnable()
    {
        if (loader == null) loader = FindFirstObjectByType<LevelLoader>();
        int unlocked = SaveData.Unlocked;

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i + 1; // buildIndex 1..5
            bool isUnlocked = levelIndex <= unlocked;

            levelButtons[i].interactable = isUnlocked;

            if (bestLabels != null && i < bestLabels.Length && bestLabels[i] != null)
            {
                float best = SaveData.GetBest(levelIndex);
                bestLabels[i].text = isUnlocked
                    ? (best > 0f ? $"Level {levelIndex}\nBest: {best:F2}s" : $"Level {levelIndex}\nBest: --")
                    : $"Level {levelIndex}\nLocked";
            }

            levelButtons[i].onClick.RemoveAllListeners();
            levelButtons[i].onClick.AddListener(() => loader.Load(levelIndex));
        }
    }
}