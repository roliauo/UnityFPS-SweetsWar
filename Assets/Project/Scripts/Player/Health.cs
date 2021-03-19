using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public float maxHelath = 100f;
    public float criticalHealthRatio = 0.3f;
    public float currentHelath { get; set; }
    public float getHealthRatio() => currentHelath / maxHelath;
    public bool isCritical() => getHealthRatio() < criticalHealthRatio;

    public UnityAction onDie;

    private void Start()
    {
        currentHelath = maxHelath;
    }

    public void Heal(float hp)
    {
        currentHelath += hp;
        currentHelath = Mathf.Clamp(currentHelath, 0f, maxHelath);
    }

    public void Damage(float hp)
    {
        currentHelath -= hp;
        currentHelath = Mathf.Clamp(currentHelath, 0f, maxHelath);
        DetectLife();
    }

    public void SetZero()
    {
        currentHelath = 0f;
        DetectLife();
    }

    private void DetectLife()
    {
        if (currentHelath <= 0f && onDie != null)
        {
            onDie();
        }
    }


}
