using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public float MaxHealth = 100f;
    public float criticalHealthRatio = 0.3f;
    public float currentHelath { get; set; }
    public float getHealthRatio() => currentHelath / MaxHealth;
    public bool isCritical() => getHealthRatio() < criticalHealthRatio;

    public UnityAction Die;

    private void Start()
    {
        currentHelath = MaxHealth;
    }

    public void Heal(float hp)
    {
        currentHelath += hp;
        currentHelath = Mathf.Clamp(currentHelath, 0f, MaxHealth);
    }

    public void Damage(float hp)
    {
        currentHelath -= hp;
        currentHelath = Mathf.Clamp(currentHelath, 0f, MaxHealth);
        DetectLife();
    }

    public void SetZero()
    {
        currentHelath = 0f;
        DetectLife();
    }

    private void DetectLife()
    {
        if (currentHelath <= 0f && Die != null)
        {
            Die();
        }
    }


}
