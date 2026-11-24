using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Image fill;             // Image Type: Filled, Fill Method: Horizontal, Origin: Left
    [SerializeField] Image lag;              // the same, behind it, in a paler colour
    [SerializeField] Gradient colour;        // red at 0, amber at 0.5, teal at 1
    [SerializeField] float lagDelay = 0.4f;  // seconds before the lag bar starts to catch up
    [SerializeField] float lagSpeed = 0.8f;  // fill per second it catches up at
    float target = 1f;
    float lagStartsAt;

    public void Set(int current, int max)
    {
        float next = Mathf.Clamp01((float)current / max);
        if (next < target) lagStartsAt = Time.unscaledTime + lagDelay;
        else lag.fillAmount = next;          // healing: the lag bar moves up with it
        target = next;
        fill.fillAmount = target;
        fill.color = colour.Evaluate(target);
    }

    void Update()
    {
        if (Time.unscaledTime < lagStartsAt) return;
        lag.fillAmount = Mathf.MoveTowards(lag.fillAmount, target, lagSpeed * Time.unscaledDeltaTime);
    }
}