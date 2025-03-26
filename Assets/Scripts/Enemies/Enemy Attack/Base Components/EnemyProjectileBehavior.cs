namespace AgeOfEnlightenment.Enemies
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class EnemyProjectileBehavior : MonoBehaviour
    {
        protected List<EnemyImpactEffect> ImpactEffects;
        protected Rigidbody selfRB;

        protected bool activeProjectile = false;

        private void Awake()
        {
            selfRB = GetComponent<Rigidbody>();
        }
        private void Start()
        {
            ImpactEffects = GetComponents<EnemyImpactEffect>().ToList();
        }


        public virtual void Launch(Transform target, Vector3 shootingVector, float launchStrength)
        {
            activeProjectile = true;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (activeProjectile)
            {
                if (ImpactEffects.Count != 0)
                {
                    foreach (EnemyImpactEffect impactEffect in ImpactEffects)
                    {
                        impactEffect.Impact(collision);
                    } 
                }
                Destroy(gameObject);
            }
        }
    }

}