using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public float MaxHealth => _maxHealth;
    public float CurrentHealth => _currentHealth;
    public UnityEvent OnHealthEnd;
    public UnityEventHealth OnHealthValueChanged;

    [SerializeField, Min(0)] private float _maxHealth;
    private float _currentHealth;

    void Awake()
    {
        if (OnHealthEnd == null)
            OnHealthEnd = new UnityEvent();
        if (OnHealthValueChanged == null)
            OnHealthValueChanged = new UnityEventHealth();
        _currentHealth = _maxHealth;
    }

    public void Hit(float damage)
    {
        if (damage <= 0)
            return;

        _currentHealth = Mathf.Clamp(_currentHealth - damage, 0, _maxHealth);
        OnHealthValueChanged.Invoke(new HealthEventArgs()
        {
            CurrentHealth = _currentHealth,
            MaxHealth = _maxHealth
        });

        if (_currentHealth <= 0)
            OnHealthEnd.Invoke();
    }

    public void AddHealth(float health)
    {
        if (health <= 0)
            return;
        _currentHealth = Mathf.Clamp(_currentHealth + health, 0, _maxHealth);
        OnHealthValueChanged.Invoke(new HealthEventArgs()
        {
            CurrentHealth = _currentHealth,
            MaxHealth = _maxHealth
        });
    }
}
