namespace AgeOfEnlightenment.Enemies
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class EnemyForceMissile : EnemyProjectileBehavior
    {
        public float SearchRadius;
        public float HomingStrength;

        private Transform CurrentTarget;

        private float flySpeed;
        private float sqrSearchRadius;

        private void Awake()
        {
            sqrSearchRadius = SearchRadius * SearchRadius;
            selfRB = GetComponent<Rigidbody>();
        }



        public override void Launch(Transform target, Vector3 shootingVector, float launchStrength)
        {
            base.Launch(target, shootingVector, launchStrength);

            transform.rotation = Quaternion.LookRotation(shootingVector);

            flySpeed = launchStrength;

            CurrentTarget = target;
        }

        private void FixedUpdate()
        {
            if (activeProjectile)
            {
                if (selfRB)
                {
                    Vector3 vectorToTarget = (CurrentTarget.position - transform.position);


                    selfRB.AddForce(transform.forward * flySpeed, ForceMode.Force);
 

                    if (vectorToTarget.sqrMagnitude <= sqrSearchRadius)
                    {
                        

                        float angleDifferece = Vector3.Angle(transform.forward, vectorToTarget.normalized);
                        //THIS IS A MESS


                        Vector3 crossVector = Vector3.Cross(transform.forward, vectorToTarget.normalized);

                        selfRB.AddTorque(angleDifferece * HomingStrength * crossVector);

                    } 
                }
                else
                {
                    Debug.Log("THERS NO FUCKING RIGIDBODY");
                }
            }
        }
    }

}