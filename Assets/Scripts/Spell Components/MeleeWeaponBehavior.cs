using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponBehavior : MonoBehaviour
{
    public float damageMultiplier;
    public float minimumVelocity = .4f;
    public float attackCooldown;

    private float timer;
    private bool counting = false;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!counting)
        {
            if(collision.gameObject.TryGetComponent(out IDamageable target))
            {
                float velocity = collision.relativeVelocity.magnitude;
                if (velocity >= minimumVelocity)
                {
                    float damageToTake = velocity * damageMultiplier;
                    target.TakeDamage(damageToTake, DamageType.Physical);
                    counting = true; 
                }
            }
        }
    }
    private void Update()
    {
        if (counting)
        {
            timer += Time.deltaTime;
            if(timer >= attackCooldown)
            {
                counting = false;
                timer = 0;
            }
        }
    }

}
