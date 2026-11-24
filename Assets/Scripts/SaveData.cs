using UnityEngine;

public static class SaveData
{
    const string UnlockedKey = "unlocked";
    const string BestPrefix = "best";

    public static int Unlocked => PlayerPrefs.GetInt(UnlockedKey, 1);

    public static void Unlock(int level)
    {
        if (level > Unlocked)
        {
            PlayerPrefs.SetInt(UnlockedKey, level);
            PlayerPrefs.Save();
        }
    }

    public static float GetBest(int level)
    {
        string key = BestPrefix + level;
        return PlayerPrefs.HasKey(key) ? PlayerPrefs.GetFloat(key, 0f) : 0f;
    }

    public static bool Record(int level, float time)
    {
        string key = BestPrefix + level;
        float prev = PlayerPrefs.GetFloat(key, float.MaxValue);
        if (time < prev)
        {
            PlayerPrefs.SetFloat(key, time);
            PlayerPrefs.Save();
            return true;
        }
        return false;
    }
}