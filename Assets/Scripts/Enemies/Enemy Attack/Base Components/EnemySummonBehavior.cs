namespace AgeOfEnlightenment.Enemies
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class EnemySummonBehavior : MonoBehaviour, IDamageable
    {
        public float LifeExpectancy;
        
        [Tooltip("Health of the summon. Set to 0 for infinite health")]
        public float MaxHealth;

        private float lifeTimer;
        private float currentHealth;

        private Transform PlayerTransform;
        private EnemyPositions SpawnPositions;

        private void Start()
        {
            
        }

        protected void Update()
        {
            if(lifeTimer > 0 && currentHealth > 0)
            {
                lifeTimer -= Time.deltaTime;

                if(lifeTimer <= 0)
                {
                    EndOfLife();
                }
            }
        }
        public virtual void StartOfLife(PlayerSingleton TargetPlayer, EnemyPositions SpawnPositions)
        {
            currentHealth = MaxHealth;
            lifeTimer = LifeExpectancy;

            PlayerTransform = TargetPlayer.transform;
            this.SpawnPositions = SpawnPositions;
        }
        protected virtual void EndOfLife() { }

        public void TakeDamage(float DamageAmount, DamageType elementalDamage)
        {
            if (MaxHealth != 0)
            {
                currentHealth -= DamageAmount;

                if (currentHealth <= 0)
                    EndOfLife();
            }
        }
    }

}