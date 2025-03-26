using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour, IDamageable
{
    //Need to fill out some kind of methods that will communicate to the actual stats.
    //Like damage methods and the like
    public void TakeDamage(float DamageAmount, DamageType elementalDamage)
    {
        Debug.Log("We have just taken " + DamageAmount + " " + elementalDamage + " damage!");
    }
}
