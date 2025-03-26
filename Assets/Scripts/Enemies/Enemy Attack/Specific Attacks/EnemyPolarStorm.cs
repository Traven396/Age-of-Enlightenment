namespace AgeOfEnlightenment.Enemies
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class EnemyPolarStorm : EnemyProjectileBehavior
    {
        //This attack will be a large particle system with a collider on it
        //It will slowly begin following the player and stick around for maybe 15-20 seconds
        //Any entities that enter the collider will take damage every couple seconds
        //
        //This object will have a rigidbody, and maybe just make it have no gravity and slowly push towards the player

        //IMPORTANT NOTE
        //I need to make some kind of thing to make sure this attack doesnt get spammed a shit ton
        //Cause multiple of these things chasing after you could be an abolute nightmare

        public float DamageAmount = 2;
        public float DamageCooldown = 3;
        public float ChaseSpeed = 2;

        private List<IDamageable> currentTargets = new List<IDamageable>();
        private Transform _ActiveTarget;

        private float damageTimer;


        private void Awake()
        {
            selfRB = GetComponent<Rigidbody>();
            selfRB.useGravity = false;
        }

        public override void Launch(Transform target, Vector3 shootingVector, float launchStrength)
        {
            base.Launch(target, shootingVector, launchStrength);


            _ActiveTarget = target;

        }
        private void Update()
        {
            damageTimer += Time.deltaTime;
            if(damageTimer >= DamageCooldown)
            {
                DealDamage();
                damageTimer = 0;
            }
        }
        private void FixedUpdate()
        {
            //Slowly move towards it
            Vector3 vectorToTarget = (_ActiveTarget.position - transform.position);

            selfRB.AddForce(vectorToTarget * ChaseSpeed, ForceMode.Force);

            if (selfRB.velocity.magnitude > ChaseSpeed)
                selfRB.velocity = selfRB.velocity.normalized * ChaseSpeed;
        }

        private void DealDamage()
        {
            if(currentTargets.Count != 0)
            {
                foreach (IDamageable damageTarget in currentTargets)
                {
                    damageTarget.TakeDamage(DamageAmount, DamageType.Ice);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.TryGetComponent(out IDamageable potentailTarget))
            {
                if (!currentTargets.Contains(potentailTarget))
                {
                    currentTargets.Add(potentailTarget);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.TryGetComponent(out IDamageable potentailTarget))
            {
                if (currentTargets.Contains(potentailTarget))
                {
                    currentTargets.Remove(potentailTarget);
                }
            }
        }


    }

}