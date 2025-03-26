namespace AgeOfEnlightenment.Enemies
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class EnemyImpactVisuals : EnemyImpactEffect
    {
        public GameObject ImpactVFX;
        public float TimerToDestroy;
        public override void Impact(Collision collision)
        {
            var instantiatedVFX = Instantiate(ImpactVFX, collision.contacts[0].point, Quaternion.identity);
            Destroy(instantiatedVFX, TimerToDestroy);
        }

        public override void Impact(Vector3 hitPoint, Collider hitCollider)
        {
            var instantiatedVFX = Instantiate(ImpactVFX, hitPoint, Quaternion.identity);
            Destroy(instantiatedVFX, TimerToDestroy);
        }
    }

}