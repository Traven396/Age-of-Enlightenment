using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Kryz.CharacterStats;
using UnityEngine.Events;

public class Player : MonoBehaviour, IDamageable
{
    #region Singleton Behavior
    public static Player Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType(typeof(Player)) as Player;

            return instance;
        }
        set
        {
            instance = value;
        }
    }
    private static Player instance;

    [HideInInspector]
    public PlayerSpellbook _SpellBook;
    [HideInInspector]
    public PlayerPassiveSpellManager _PassiveManager;
    #endregion

    public GameObject deathMenu;

    public static UnityEvent StatsChanged = new();
    //public static event Action StatsChanged;
    public static event Action ManaChanged;
    public static event Action MaxManaChanged;
    public static event Action HealthChanged;
    #region Stats
    public CharacterStat movementSpeed { get; private set; } = new CharacterStat(3, StatsChanged);
    public CharacterStat maximumMana { get; private set; } = new CharacterStat(150);
    public CharacterStat manaRegen { get; private set; } = new CharacterStat(2);
    public CharacterStat maximumHealth { get; private set; } = new CharacterStat(100);
    public CharacterStat healthRegen { get; private set; } = new CharacterStat(2);

    public float currentMana { get; private set; }

    public float currentHealth { get; private set; }
    #endregion

    private bool shouldRegenHealth = true;
    [HideInInspector]
    public bool stepOfWindOn = false;

    private void Awake()
    {
        _SpellBook = GetComponent<PlayerSpellbook>();
        _PassiveManager = GetComponent<PlayerPassiveSpellManager>();

        currentMana = maximumMana.Value;
        currentHealth = maximumHealth.Value;

        StartCoroutine(RegenMana());
        StartCoroutine(RegenHealth());
    }

    #region Movement Speed Methods
    public void AddMoveSpeedModifier(StatModifier modifier)
    {
        movementSpeed.AddModifier(modifier);

        StatsChanged?.Invoke();
    }
    public void RemoveMoveSpeedModifier(StatModifier modifier)
    {
        movementSpeed.RemoveModifier(modifier);

        StatsChanged?.Invoke();
    }
    public void RemoveAllMoveSpeedModifier(object source)
    {
        movementSpeed.RemoveAllModifiersFromSource(source);

        StatsChanged?.Invoke();
    }
    public bool QueryMovementSpeedModifiers(StatModifier modifier)
    {
        return movementSpeed.StatModifiers.Contains(modifier);
    }
    #endregion

    #region Maximum Mana Methods
    public void AddMaximumManaModifier(StatModifier modifier)
    {
        maximumMana.AddModifier(modifier);

        if (currentMana > maximumMana.Value)
            currentMana = maximumMana.Value;

        MaxManaChanged?.Invoke();
    }
    public void RemoveMaxiumumManaModifier(StatModifier modifier)
    {
        maximumMana.RemoveModifier(modifier);

        if (currentMana > maximumMana.Value)
            currentMana = maximumMana.Value;

        MaxManaChanged?.Invoke();
    }

    public void RemoveAllMaxiumumManaModifier(object source)
    {
        maximumMana.RemoveAllModifiersFromSource(source);

        MaxManaChanged?.Invoke();
    }
    #endregion

    #region Mana Regen Methods
    public void AddManaRegenModifier(StatModifier modifier)
    {
        manaRegen.AddModifier(modifier);
    }
    public void RemoveManaRegenModifier(StatModifier modifier)
    {
        manaRegen.RemoveModifier(modifier);
    }
    public void RemoveAllManaRegenModifierFromSource(object source)
    {
        manaRegen.RemoveAllModifiersFromSource(source);
    }
    #endregion

    public void SubtractMana(int value)
    {
        currentMana -= value;

        if (currentMana < 0)
            currentMana = 0;

        ManaChanged?.Invoke();
    }
    public void AddMana(int value)
    {
        currentMana += value;

        if (currentMana > maximumMana.Value)
            currentMana = maximumMana.Value;

        ManaChanged?.Invoke();
    }
    
    public void SubtractHealth(int value)
    {
        currentHealth -= value;

        HealthChanged?.Invoke();

        if(currentHealth <= 0)
        {
            Death();
            return;
        }

        shouldRegenHealth = false;

        StopCoroutine("HealthRegenCooldown");

        StartCoroutine("HealthRegenCooldown");
    }
    public void AddCurrentHealth(int value)
    {
        currentHealth += value;

        if (currentHealth > maximumHealth.Value)
            currentHealth = maximumHealth.Value;

        HealthChanged?.Invoke();
    }

    void Death()
    {
        var target = Camera.main.gameObject;
        
        GameObject menu = Instantiate(deathMenu, target.transform.position + new Vector3(0, 0, 2f), Quaternion.identity, this.transform);
        
        menu.transform.LookAt(target.transform, Vector3.up);

        AudioListener.volume = 0;
        Time.timeScale = 0;
    }

    IEnumerator RegenMana()
    {
        while (true)
        {
            //Debug.Log("Current Mana: " + currentMana);
            //Debug.Log(manaRegenRate);
            if (currentMana <= maximumMana.Value && currentMana >= 0)
            {
                    AddMana((int)manaRegen.Value);

                if (currentMana > maximumMana.Value)
                    currentMana = maximumMana.Value;
                if (currentMana < 0)
                    currentMana = 0;

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
            if (currentHealth < maximumHealth.Value && shouldRegenHealth)
            {
                AddCurrentHealth((int)healthRegen.Value);

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

    public void TakeDamage(float DamageAmount, DamageType elementalDamage)
    {
        SubtractHealth((int)DamageAmount);
    }
}
