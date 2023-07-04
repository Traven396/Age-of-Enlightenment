using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    #endregion

    public GameObject deathMenu;



    public static event Action StatsChanged;
    public static event Action ManaChanged;
    public static event Action HealthChanged;
    #region Stats
    public float playerMoveSpeed = 3f;
    public int maximumMana { get; private set; } = 150;
    public int currentMana { get; private set; }
    public int manaRegenRate { get; private set; } = 2;
    public int maximumHealth { get; private set; } = 100;
    public int currentHealth { get; private set; }
    public int healthRegenRate { get; private set; } = 1; 
    #endregion

    private bool shouldRegenHealth = true;
    [HideInInspector]
    public bool stepOfWindOn = false;

    private void Awake()
    {
        _SpellBook = GetComponent<PlayerSpellbook>();

        currentMana = maximumMana;
        currentHealth = maximumHealth;

        StartCoroutine(RegenMana());
        StartCoroutine(RegenHealth());
    }

    public void AddPlayerMoveSpeed(float newSpeed)
    {
        playerMoveSpeed += newSpeed;
        StatsChanged?.Invoke();
    }
    public void SubtractPlayerMoveSpeed(float newSpeed)
    {
        playerMoveSpeed -= newSpeed;
        StatsChanged?.Invoke();
    }

    public void AddMaximumMana(int value)
    {
        maximumMana += value;
        ManaChanged?.Invoke();
    }
    public void SubtractMaximumMana(int value)
    {
        maximumMana -= value;
        if (currentMana > maximumMana)
            currentMana = maximumMana;
        ManaChanged?.Invoke();
    }

    public void SubtractCurrentMana(int value)
    {
        currentMana -= value;
        if (currentMana < 0)
            currentMana = 0;
        ManaChanged?.Invoke();
    }
    public void AddCurrentMana(int value)
    {
        currentMana += value;
        if (currentMana > maximumMana)
            currentMana = maximumMana;
        ManaChanged?.Invoke();
    }
    public void AddManaRegen(int value)
    {
        manaRegenRate += value;
    }
    public void SubtractManaRegen(int value)
    {
        manaRegenRate -= value;
        //Debug.Log(manaRegenRate);
    }
    public void SubtractCurrentHealth(int value)
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
        if (currentHealth > maximumHealth)
            currentHealth = maximumHealth;
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
            if (currentMana <= maximumMana && currentMana >= 0)
            {
                    AddCurrentMana(manaRegenRate);

                if (currentMana > maximumMana)
                    currentMana = maximumMana;
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
            if (currentHealth < maximumHealth && shouldRegenHealth)
            {
                AddCurrentHealth(healthRegenRate);

                yield return new WaitForSeconds(.5f);
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
        SubtractCurrentHealth((int)DamageAmount);
    }
}
