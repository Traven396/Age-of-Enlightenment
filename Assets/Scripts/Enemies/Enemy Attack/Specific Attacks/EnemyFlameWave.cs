namespace AgeOfEnlightenment.Enemies
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class EnemyFlameWave : EnemyProjectileBehavior
    {
        //Object moves forward for a distance. Creates fire that raises up after certain distance thresholds
        //Make it follow the normal of the ground?
        //Wave objects have the particles
        //They have a small delay to moving up?
        public float Lifetime = 5;
        public float TravelSpeed = 1;
        //public float FireSegementsPerUnit = 3;
        //public GameObject FireSegmentPrefab;
        public float LevitateHeight;
        public LayerMask GroundLayer;
        public float DamageAmount = 9;

        private Vector3 shootingDirection;
        private Vector3 lastSegmentPosition;
        private float segmentDistanceValue;

        private bool visualsEnabled;
        private void Awake()
        {
            
            //segmentDistanceValue = (1 / FireSegementsPerUnit) * (1 / FireSegementsPerUnit);
        }
        private void FixedUpdate()
        {
            RaycastHit groundHit;

            if(Physics.Raycast(transform.position, -transform.up, out groundHit, 5, GroundLayer))
            {
                Ray upRay = new Ray(groundHit.point, transform.position - groundHit.point);

                Vector3 upDist = upRay.GetPoint(LevitateHeight);

                transform.position = upDist;

                Debug.DrawLine(transform.position, groundHit.point);

                if (!visualsEnabled)
                {
                    foreach (Transform child in transform)
                    {
                        child.gameObject.SetActive(true);
                    }
                    visualsEnabled = true;
                }
            }
            else
            {
                //We should just destroy this fire if t here is nothing beneath it.
            }

            transform.position += shootingDirection.normalized * .1f;

            //if ((transform.position - lastSegmentPosition).sqrMagnitude >= segmentDistanceValue && FireSegmentPrefab)
                //CreateSegment();
        }
        public override void Launch(Transform target, Vector3 shootingVector, float launchStrength)
        {
            shootingDirection = shootingVector;

            transform.LookAt(transform.TransformPoint(shootingDirection));

            Destroy(gameObject, Lifetime);

            //CreateSegment();
        }

        //void CreateSegment()
        //{
        //    if (FireSegmentPrefab)
        //    {
        //        lastSegmentPosition = transform.position;

        //        Instantiate(FireSegmentPrefab, transform.position, transform.rotation); 
        //    }
        //}
        private void OnTriggerEnter(Collider other)
        {
            if(other.TryGetComponent<IDamageable>(out var Target))
            {
                Target.TakeDamage(DamageAmount, DamageType.Fire);
            }
        }
    }

}