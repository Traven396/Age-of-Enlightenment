using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
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
    #endregion

    public static event Action StatsChanged;
    public float playerMoveSpeed { get; private set; } = .5f;


    public bool stepOfWindOn = false;

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
}
