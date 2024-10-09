using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AgeOfEnlightenment.Player;
[RequireComponent(typeof(PlayerStats))]
public class PlayerSingleton : MonoBehaviour
{
    public static PlayerSingleton Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType(typeof(PlayerSingleton)) as PlayerSingleton;

            return instance;
        }
        set
        {
            instance = value;
        }
    }
    private static PlayerSingleton instance;

    [HideInInspector]
    public InternalPlayerSpellbook _SpellBook;
    [HideInInspector]
    public PlayerStats _Stats;

    private void Awake()
    {
        _SpellBook = GetComponent<InternalPlayerSpellbook>();
        _Stats = GetComponent<PlayerStats>();
    }

    public void SubtractMana(int manaCost)
    {
        _Stats.SubtractMana(manaCost);
    }
}
