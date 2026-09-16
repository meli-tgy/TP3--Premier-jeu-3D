using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class FloatFloatEvent : UnityEvent<float, float> { }

public class Health : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float attackPower = 10f;

    public float MaxHealth => maxHealth;
    public float CurrentHealth { get; private set; }
    public float AttackPower => attackPower;

    public FloatFloatEvent OnHealthChanged;
    public UnityEvent OnDeath;

    private bool isDead;

    void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

        if (CurrentHealth <= 0f)
        {
            isDead = true;
            OnDeath?.Invoke();
        }
    }
}