using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Health _health;
    [SerializeField] private Text _healthText;
    [SerializeField] private Slider _healthSlider;

    void Start()
    {
        _health.OnHealthValueChanged.AddListener(UpdateUI);
        UpdateUI(new HealthEventArgs()
        {
            CurrentHealth = _health.CurrentHealth,
            MaxHealth = _health.MaxHealth
        });
    }

    void UpdateUI(HealthEventArgs args)
    {
        if (_healthText != null)
            _healthText.text = $"{(int)args.CurrentHealth} / {(int)args.MaxHealth}";
        if (_healthSlider != null)
        { 
            _healthSlider.maxValue = args.MaxHealth;
            _healthSlider.value = args.CurrentHealth;
        }
    }

    private void OnDestroy()
    {
        _health.OnHealthValueChanged.RemoveListener(UpdateUI);
    }
}
