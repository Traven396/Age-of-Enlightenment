using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AgeOfEnlightenment.StabbingPhysics
{
    public class IsStabbable : MonoBehaviour
    {
        public StabbableObjectSettings Settings;

        public List<Collider> IgnoredColliders = new List<Collider>();

        public bool CurrentlyStabbed => Stabbers.Count > 0;
        public List<StabbingObject> Stabbers;

        public Vector3 Velocity { get; private set; }
        private Vector3 _previousPosition;

        [Tooltip("Required velocity to initiate the stab")]
        public float RequiredVelocity = 1f;

        

        private void Awake()
        {
            Stabbers = new List<StabbingObject>();

            if (IgnoredColliders == null) IgnoredColliders = new List<Collider>();
            if (IgnoredColliders.Count == 0)
            {
                RefreshColliders();
            }
        }
        private void FixedUpdate()
        {
            CleanupStabbers();
            Velocity = (transform.position - _previousPosition) / Time.deltaTime;
            _previousPosition = transform.position;
            
        }
        private void CleanupStabbers()
        {
            var cleanup = false;
            foreach (var stabber in Stabbers)
            {
                if (!stabber || !stabber.gameObject.activeInHierarchy || !stabber.enabled)
                {
                    cleanup = true;
                    break;
                }
            }

            if (cleanup)
            {
                Stabbers.RemoveAll(e => e == null || !e.gameObject.activeInHierarchy || !e.enabled);
            }
        }
        public virtual void RefreshColliders()
        {
            IgnoredColliders.Clear();

            if (TryGetComponent(out Rigidbody rb))
            {
                IgnoredColliders.AddRange(rb.GetColliders());
            }
            else
            {
                IgnoredColliders.AddRange(GetComponentsInChildren<Collider>().Where(e => !e.isTrigger));
            }
        }
    } 
}
