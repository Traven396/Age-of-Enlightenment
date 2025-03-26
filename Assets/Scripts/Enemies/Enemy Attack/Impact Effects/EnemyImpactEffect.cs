namespace AgeOfEnlightenment.Enemies
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class EnemyImpactEffect : MonoBehaviour
    {
        public abstract void Impact(Collision collision);
        public abstract void Impact(Vector3 hitPoint, Collider hitCollider);
    }

}