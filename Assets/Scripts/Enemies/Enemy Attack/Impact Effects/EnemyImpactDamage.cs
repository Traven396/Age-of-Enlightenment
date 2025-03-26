namespace AgeOfEnlightenment.Enemies
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class EnemyImpactDamage : EnemyImpactEffect
    {
        public float DamageAmount;
        public DamageType Type;
        public override void Impact(Collision collision)
        {
            //Is what we hit damageable
                //If yes then we need to apply damage to it
                //If no then nothing happens
            if(collision.gameObject.TryGetComponent<IDamageable>(out IDamageable hitTarget))
            {
                hitTarget.TakeDamage(DamageAmount, Type);
            }
        }

        public override void Impact(Vector3 hitPoint, Collider hitCollider)
        {
            if (hitCollider.gameObject.TryGetComponent(out IDamageable hitTarget))
            {
                hitTarget.TakeDamage(DamageAmount, Type);
            }
        }
    }

}