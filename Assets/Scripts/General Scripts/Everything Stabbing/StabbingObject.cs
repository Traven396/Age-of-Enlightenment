using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AgeOfEnlightenment.StabbingPhysics
{
    [RequireComponent(typeof(Rigidbody))]
    public class StabbingObject : MonoBehaviour
    {
        [Header("Neccesary World Components")]

        public Transform Tip;
        public Transform Base;

        [Space(10f)]

        [Tooltip("The colliders that can initiate a stab on another object")]
        [SerializeField] private Collider[] SharpColliders;

        [Tooltip("Any colliders of the StabbingObject that should't collider with the stabbed object")]
        [SerializeField] private Collider[] CollidersToPhase;

        [Header("Stabbing Settings")]
        public StabbingObjectSettings Settings;

        public bool StabAnything;
        public bool IgnoreVelocity;
        public StabbableObjectSettings DefaultSettings;

        [Space(10f)]
        public JointProjectionMode JointProjection = JointProjectionMode.PositionAndRotation;
        public float JointProjectionDistance = .01f;
        public float JointProjectionAngle = 20f;

        [Space(10f)]
        public float ColliderContactOffset = 0.001f;



        public Vector3 bladeLineWorld => Tip.position - Base.position;
        public Vector3 bladeLineLocal => Rigidbody.transform.InverseTransformPoint(Tip.position) - Rigidbody.transform.InverseTransformPoint(Base.position);

        public float bladeLength => bladeLineWorld.magnitude;

        public Rigidbody Rigidbody { get; private set; }

        public bool CurrentlyStabbing => _stabTrackers.Count > 0;

        private readonly List<StabReference> _stabTrackers = new List<StabReference>(10);
        private readonly List<StabReference> _cleanupTrackers = new List<StabReference>();

        public List<GameObject> StabbedObjects { get; private set; }
        public List<IsStabbable> StabbedStabbables { get; private set; }

        private Vector3 _velocity;

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();

            if (CollidersToPhase == null || CollidersToPhase.Length == 0)
            {
                CollidersToPhase = GetComponentsInChildren<Collider>();
            }


            StabbedObjects = new List<GameObject>(Settings.MaxStabbedObjects);
            StabbedStabbables = new List<IsStabbable>(Settings.MaxStabbedObjects);

            foreach (var sc in SharpColliders)
            {
                if (sc.contactOffset > ColliderContactOffset)
                {
                    sc.contactOffset = ColliderContactOffset;
                }
            }
        }

        private void FixedUpdate()
        {
            if (Rigidbody)
            {
                _velocity = Rigidbody.velocity;
            }

            foreach (var tracker in _stabTrackers)
            {
                if (!tracker.Update())
                {
                    Destroy(tracker.Joint);

                    _cleanupTrackers.Add(tracker);

                    IgnoreCollisions(tracker.StabbedColliders, false);
                }
            }

            for (int i = 0; i < _cleanupTrackers.Count; i++)
            {
                var currentTracker = _cleanupTrackers[i];

                _stabTrackers.Remove(currentTracker);
                StabbedObjects.Remove(currentTracker.StabbedObject);

                if (currentTracker.StabbableObject)
                    StabbedStabbables.Remove(currentTracker.StabbableObject);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_stabTrackers.Count >= Settings.MaxStabbedObjects)
                return;

            for (int i = 0; i < collision.contactCount; i++)
            {
                var currentContact = collision.GetContact(i);

                var isSharpCollider = false;
                foreach (var sharpCollider in SharpColliders)
                {
                    if (sharpCollider == currentContact.thisCollider)
                        isSharpCollider = true;
                }

                if (!isSharpCollider)
                    continue;

                GameObject stabbedGameObject;
                IsStabbable stabbableReference;
                NotStabbable notStabbableReference;

                var stabbedObjectRB = currentContact.otherCollider.attachedRigidbody;

                if (stabbedObjectRB)
                {
                    stabbedGameObject = stabbedObjectRB.gameObject;
                }
                else
                {
                    stabbedGameObject = currentContact.otherCollider.gameObject;
                }

                if (StabbedObjects.Contains(stabbedGameObject))
                    continue;

                StabbableObjectSettings targetsSettings;

                stabbedGameObject.TryGetComponent(out stabbableReference);
                stabbedGameObject.TryGetComponent(out notStabbableReference);

                if (stabbableReference)
                {
                    if (!stabbableReference.enabled)
                    {
                        return;
                    }
                    targetsSettings = stabbableReference.Settings;
                }
                else
                {
                    if (!StabAnything || StabAnything && notStabbableReference)
                    {
                        continue;
                    }

                    targetsSettings = DefaultSettings;
                }

                var stabInstanceDirection = bladeLineWorld.normalized;

                var dot = Vector3.Dot(stabInstanceDirection, -currentContact.normal);

                //If the angle of the stab is not within the range set
                if (dot < Settings.StabAngleThreshold) continue;

                //If it doesn't get hit hard enough, then just skip the rest
                if (!IgnoreVelocity && collision.relativeVelocity.magnitude < targetsSettings.RequiredVelocity) continue;


                Rigidbody.velocity = _velocity;
                if (stabbableReference && stabbedObjectRB)
                {
                    stabbedObjectRB.velocity = stabbableReference.Velocity;
                }

                var joint = SetupJoint(targetsSettings, Tip, stabbedObjectRB);

                StabbedObjects.Add(stabbedGameObject);
                if (stabbableReference)
                {
                    StabbedStabbables.Add(stabbableReference);
                }

                List<Collider> stabbedColliders;
                if (stabbableReference)
                {
                    stabbedColliders = stabbableReference.IgnoredColliders;
                }
                else
                {
                    if (stabbedObjectRB)
                    {
                        stabbedColliders = stabbedObjectRB.GetColliders().ToList();
                    }
                    else
                    {
                        stabbedColliders = currentContact.otherCollider.gameObject.GetColliders();
                    }
                }

                IgnoreCollisions(stabbedColliders);

                var stabTracker = new StabReference(this, stabbableReference, targetsSettings, joint, stabbedGameObject, stabInstanceDirection, Tip, stabbedColliders);

                _stabTrackers.Add(stabTracker);

                //Fire the start event here
                Debug.Log("I " + gameObject.name + " just stabbed " + stabbedGameObject.name);
            }
        }

        private ConfigurableJoint SetupJoint(StabbableObjectSettings settings, Transform tip, Rigidbody targetRB)
        {
            var joint = Rigidbody.gameObject.AddComponent<ConfigurableJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.axis = bladeLineLocal.normalized;
            joint.secondaryAxis = HelpfulScript.OrthogonalVector(joint.axis);

            joint.projectionAngle = JointProjectionAngle;
            joint.projectionDistance = JointProjectionDistance;
            joint.projectionMode = JointProjection;

            joint.yMotion = ConfigurableJointMotion.Locked;
            joint.zMotion = ConfigurableJointMotion.Locked;
            joint.angularXMotion = ConfigurableJointMotion.Locked;
            joint.angularYMotion = ConfigurableJointMotion.Locked;
            joint.angularZMotion = ConfigurableJointMotion.Locked;

            joint.anchor = Rigidbody.transform.InverseTransformPoint(tip.position);
            joint.xMotion = ConfigurableJointMotion.Limited;

            var jointLimit = joint.linearLimit;

            jointLimit.limit = bladeLength;

            joint.linearLimit = jointLimit;

            joint.connectedBody = targetRB;
            if (joint.connectedBody)
            {
                joint.connectedAnchor = joint.connectedBody.transform.InverseTransformPoint(tip.position);
            }
            else
            {
                joint.connectedAnchor = tip.position;
            }

            return joint;
        }

        private void IgnoreCollisions(List<Collider> stabbedColliders, bool ignore = true)
        {
            for (var i = 0; i < stabbedColliders.Count; i++)
            {
                var stabbedCollider = stabbedColliders[i];
                foreach (var stabCollider in SharpColliders)
                {
                    if (stabbedCollider)
                        Physics.IgnoreCollision(stabCollider, stabbedCollider, ignore);
                }

                foreach (var stabberCollider in CollidersToPhase)
                {
                    if (stabbedCollider)
                        Physics.IgnoreCollision(stabberCollider, stabbedCollider, ignore);
                }
            }
        }
    } 
}
