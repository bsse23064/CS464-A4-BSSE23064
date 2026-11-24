using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int max = 3;
    [SerializeField] float iFrames = 1f;
    [SerializeField] HealthBar healthBar;
    public bool shielded;
    int current; float invulnerableUntil;

    public int Current => current;
    public int Max => max;

    void Awake() => current = max;

    void Start()
    {
        if (healthBar == null) healthBar = FindFirstObjectByType<HealthBar>();
        RefreshBar();
    }

    public bool TakeDamage(int amount)
    {
        if (current <= 0 || Time.time < invulnerableUntil) return false;
        invulnerableUntil = Time.time + iFrames;
        if (shielded) { shielded = false; return false; }
        current -= amount;
        RefreshBar();
        if (GameManager.Instance != null) GameManager.Instance.UpdateUI();
        if (current <= 0) GameManager.Instance.PlayerDied();
        return true;
    }

    public void Restore(float protection)
    {
        current = max; invulnerableUntil = Time.time + protection;
        RefreshBar();
        GameManager.Instance.UpdateUI();
    }

    void RefreshBar()
    {
        if (healthBar != null) healthBar.Set(current, max);
    }
}