namespace AgeOfEnlightenment.Enemies
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class EnemyJoltAttack : EnemyProjectileBehavior
    {
        public LayerMask IgnoredLayers;
        public GameObject BeamPrefab;
        public float AttackDelay = 3;
        public float RiseSpeed = 1;
        
        private float _timer;
        private bool timerActive = false;
        private Transform _TargetTransform;
        private float maxDistance;
        private float randomRaiseModifier;

        GameObject spawnedBeam;
        private void Awake()
        {
            _timer = AttackDelay;
            randomRaiseModifier = UnityEngine.Random.Range(0, 0.2f);
        }
        public override void Launch(Transform target, Vector3 shootingVector, float launchStrength)
        {
            base.Launch(target, shootingVector, launchStrength);

            //Spawn the cloud prefab, and start the timer

            maxDistance = Vector3.Distance(transform.position, target.position) * 1.25f;

            _TargetTransform = target;

            timerActive = true;

            //Do we collide with anything when shooting towards the player
            
        }
        private void Update()
        {
            if (_timer <= 0 && timerActive)
            {
                LaunchBeam();

                timerActive = false;
            }

            if (_timer > 0 && timerActive)
            { 
                _timer -= Time.deltaTime;
                transform.position += RiseSpeed * Time.deltaTime * randomRaiseModifier * Vector3.up;
            }
        }

        private void LaunchBeam()
        {
            //Spawn the prefab
            //Shoot if the player is still within range
            //Scale prefab to the distance and halfway to it
            //Fire impact effects
            //Destroy object
            if (Vector3.Distance(transform.position, _TargetTransform.position) > maxDistance)
                Despawn();

            if (Physics.Linecast(transform.position, _TargetTransform.position, out RaycastHit hit, ~IgnoredLayers))
            {
                spawnedBeam = Instantiate(BeamPrefab, transform);
                transform.LookAt(hit.point);

                spawnedBeam.transform.localPosition = new Vector3(0, 0, hit.distance / 2);
                spawnedBeam.transform.localScale = new Vector3(1, 1, hit.distance / 2);

                iTween.ScaleFrom(spawnedBeam, iTween.Hash("x", 0, "y", 0, "time", 0.2f, "oncomplete", "BeamVanish", "oncompletetarget", gameObject));

                foreach (EnemyImpactEffect impactEffect in ImpactEffects)
                {
                    impactEffect.Impact(hit.point, hit.collider);
                }

                
            }

            Despawn();
        }

        void Despawn()
        {
            //Cleanup any leftovers
            //Destroy this object & instantiated objects

            Destroy(gameObject, 1);
        }
        void BeamVanish()
        {
            iTween.ScaleTo(spawnedBeam, iTween.Hash("x", 0, "y", 0, "time", 0.2f));
        }
    }

}