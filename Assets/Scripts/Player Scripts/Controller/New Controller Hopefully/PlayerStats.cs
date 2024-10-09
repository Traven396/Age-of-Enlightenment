using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Kryz.CharacterStats;
using UnityEngine.Events;

public class PlayerStats : MonoBehaviour
{
    #region Stats

    public static UnityEvent StatsChanged = new();
    
    public static event Action ManaChanged;
    public static event Action MaxManaChanged;
    public static event Action HealthChanged;

    public CharacterStat MovementSpeed { get; private set; } = new CharacterStat(3, StatsChanged);
    public CharacterStat MaximumManaValue { get; private set; } = new CharacterStat(150);
    public CharacterStat ManaRegenSpeed { get; private set; } = new CharacterStat(2);
    public CharacterStat MaximumHealthValue { get; private set; } = new CharacterStat(100);
    public CharacterStat HealthRegenSpeed { get; private set; } = new CharacterStat(2);

    public float _CurrentMana { get; private set; }

    public float _CurrentHealth { get; private set; }
    #endregion

    private bool shouldRegenHealth = true;

    private void Awake()
    {
        _CurrentMana = MaximumManaValue.Value;
        _CurrentHealth = MaximumHealthValue.Value;

        StartCoroutine(RegenMana());
        StartCoroutine(RegenHealth());
    }

    public void SubtractMana(int value)
    {
        _CurrentMana -= value;

        if (_CurrentMana < 0)
            _CurrentMana = 0;

        ManaChanged?.Invoke();
    }
    public void AddMana(int value)
    {
        _CurrentMana += value;

        if (_CurrentMana > MaximumManaValue.Value)
            _CurrentMana = MaximumManaValue.Value;

        ManaChanged?.Invoke();
    }

    public void SubtractHealth(int value)
    {
        _CurrentHealth -= value;

        HealthChanged?.Invoke();

        if (_CurrentHealth <= 0)
        {
            //Death();
            Debug.Log("YOU DIED. STILL NEED TO MAKE A DEATH");
            return;
        }

        shouldRegenHealth = false;

        StopCoroutine("HealthRegenCooldown");

        StartCoroutine("HealthRegenCooldown");
    }
    public void AddCurrentHealth(int value)
    {
        _CurrentHealth += value;

        if (_CurrentHealth > MaximumHealthValue.Value)
            _CurrentHealth = MaximumHealthValue.Value;

        HealthChanged?.Invoke();
    }

    IEnumerator RegenMana()
    {
        while (true)
        {
            //Debug.Log("Current Mana: " + currentMana);
            //Debug.Log(manaRegenRate);
            if (_CurrentMana <= MaximumManaValue.Value && _CurrentMana >= 0)
            {
                AddMana((int)ManaRegenSpeed.Value);

                if (_CurrentMana > MaximumManaValue.Value)
                    _CurrentMana = MaximumManaValue.Value;
                if (_CurrentMana < 0)
                    _CurrentMana = 0;

                yield return new WaitForSeconds(1f);
            }
            else
            {
                yield return null;
            }
        }
    }
    IEnumerator RegenHealth()
    {
        while (true)
        {
            //Debug.Log("Current Health: " + currentHealth);
            if (_CurrentHealth < MaximumHealthValue.Value && shouldRegenHealth)
            {
                AddCurrentHealth((int)HealthRegenSpeed.Value);

                yield return new WaitForSeconds(1f);
            }
            else
            {
                yield return null;
            }
        }
    }
    IEnumerator HealthRegenCooldown()
    {
        yield return new WaitForSeconds(4);
        shouldRegenHealth = true;
    }
}
