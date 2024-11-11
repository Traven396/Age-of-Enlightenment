using System;
using UnityEngine;
using Unity.Collections;
using System.Collections.Generic;

namespace AgeOfEnlightenment.Enemies
{
    
    /*
     * This is the meat and bones of my system, literally lol.
     * Each model will be made up of limbs that are trying to move specific parts of the ragdoll to match the animator. The moving of them will be called by the RagdollAnimator at the head
     * 
     * The RagdollAnimator will keep a list of all Limbs in a model
     * 
     * These will be manually placed on each ragdoll body part by the user
     * 
     * If you want to attempt to understand what a lot of these quaternions are doing, here is a place to kind of start.
     * They are a very complex subject and even I, the one who "made all of this", dont fully understand it
     * https://wirewhiz.com/quaternion-tips/
     */

    /// <summary>
    /// Limb Groups are used to determine what part of the body the limb is
    /// </summary>
    [System.Serializable]
    public enum LimbGroup
    {
        Hips,
        Spine,
        Head,
        Arm,
        Leg,
        Foot,
        Hand,
        Other
    }


    [RequireComponent(typeof(Rigidbody))]
    public class RagdollLimb : MonoBehaviour
    {
        #region References to other pieces

        //Some public references for other scripts
        public Rigidbody SelfRigidbody { get; private set; }
        //The joint 
        public ConfigurableJoint SelfJoint { get; private set; }

        /// <summary>
        /// The Limb collision event broadcaster component on the Rigidbody.
        /// </summary>
        [HideInInspector] public RagdollLimbCollisionsBroadcaster broadcaster;

        //List of all colliders this Limb is responsible for
        [HideInInspector] public Collider[] limbColliders; 
        #endregion

        #region Public Variables/Main Ragdoll Settings

        public LimbGroup group;
        /// <summary>
        /// The object/transform that the limb is going to try following
        /// </summary>
        public Transform AnimatedTarget;

        //This is a boolean you can turn off to make it so this limb will no longer animate
        public bool ActiveLimb = true;

        [Header("Standard Settings")]
        //The multiplier for total force this limb will use to try and follow animation
        [Range(0, 1)]
        public float ForceWeight = 1;
        //Same as above, but for torque applied through the joint to rotate it correctly
        [Range(0, 1)]
        public float TorqueWeight = 1;
        [Range(0, 1)]
        public float RotationDamper = 1;
        /// <summary>
        /// The weight (multiplier) of mapping this limbs's target to the limb.
        /// </summary>
        [Range(0, 1)]
        public float mappingWeight = 1;

        [Header("Knockout Component Settings")]
        /// <summary>
        /// If using the knockout script, this is the distance the limb would have to be pushed from its target to knockout the target
        /// </summary>
        public float knockOutDistance = 1;
        /// <summary>
        /// The multiplier for damage this limb will do to other limbs. Higher values mean it will knock out other characters easier
        /// </summary>
        public float impulseMultiplier = 1;
        /// <summary>
        /// Another thing used for the knockout behavior. This is how resistant this limb is to damage and collisions. Higher values mean it hards to knock these around
        /// </summary>
        [Range(0, 1)]
        public float collisionImmunity = 0; 
        #endregion

        

        #region VARIABLES GALORE

        //There is A LOT of variabels for these sorts of calculations. I'll try and explain it as I write them out

        public Vector3 animatedTargetVelocity { get; private set; }

        /// <summary>
        ///This is the parent of our target we are following
        /// </summary>
        private Transform parentOfTarget;
        /// <summary>
        /// The AnimatedTarget of the Limb that our SelfJoint is connected to.
        /// </summary>
        private Transform connectedLimbTarget;
        /// <summary>
        /// A reference to the transform that this limb's joint is connected to.
        /// </summary>
        private Transform selfJointConnectedBodyTransform;
        private bool sameTargetParent;
        /// <summary>
        ///This is the initial rotation of the target, but in relation to the limb. Basically, how far you have to rotate from this Limb, to get to the target. Not constantly updated, only initial
        ///</summary>
        private Quaternion initialTargetRotationRelToLimb;
        /// <summary>
        /// The initial inverse rotation of our target, but in relation to this limb.
        /// </summary>
        private Quaternion initialTargetRotationRelToLimbINVERSE;
        /// <summary>
        /// The default rotation offset of the Limb from the target. Returns Quaternion.Identity if they are the same
        /// </summary>
        public Quaternion relativeTargetRotation { get; private set; }

        //These below two variables allow us to convert rotations into Joint Space, IE relative to the configurable joint we have on our ragdoll. Rotation's work differently if you are using joint.targetRotation
        private Quaternion jointSpaceConverterINITIAL;
        private Quaternion jointSpaceConverterINVERSE;

        //This is a rotation from the target's parent to this limb's parent
        private Quaternion parentRotationFromTarget;
        //A rotation, in local space, from the target to this limb's rotation, also in local space
        private Quaternion localRotationFromTarget;

        //Now for some initial values so that we can reset the Ragdoll
        private Vector3 defaultPosition;
        private Quaternion defaultRotation;

        //The offset of our limb to the target. For use with the knockout thing mainly
        [HideInInspector] public Vector3 posOffset;

        //Some initial values set in Setup, like all these variables they are for calculations
        private Vector3 targetWorldCenterOfMass;
        private Quaternion targetAnimatedRotation;

        //Another way to remember rotation stuff. This is set once, and used in 1 method. Same pattern as all the other stuff here
        private Quaternion selfRotationRelativeToTarget;

        //Here is all the rotation calculation variables
        private JointDrive slerpDrive;
        private float lastRotationDamper;
        private float lastJointDriveRotationWeight;
        #endregion

        //Now for some time variables. YAY. These are to make sure that no matter the framerate or physics steps or anything the movement stays mostly the same
        private float lastReadTime, lastWriteTime;

        #region Animator Mapping
        //Mapped velocities, positions, and rotations are use to make sure the ragdoll doesn't immediately lose all movement when swapping to full ragdoll and not having force applied
        [HideInInspector] public Vector3 mappedVelocity;
        [HideInInspector] public Vector3 mappedAngularVelocity;

        [HideInInspector] public Vector3 targetSampledPosition;
        [HideInInspector] public Quaternion targetSampledRotation = Quaternion.identity;

        [HideInInspector] public Vector3 animatedTargetMappedPosition;
        [HideInInspector] public Quaternion animatedTargetMappedRotation = Quaternion.identity;
        private Vector3 lastAnimatedTargetMappedPosition;
        private Quaternion lastAnimatedTargetMappedRotation;
        #endregion


        #region Knockout Variables
        //These are variables set by the KnockDown script. They allow that script to influence how strong the ragdoll is
        [HideInInspector] public float forceWeightMultiplier = 1;
        [HideInInspector] public float limbWeightMultiplier = 1;
        [HideInInspector] public float mappingWeightMultiplier = 1;
        #endregion

        //Checking to make sure we dont do the same stuff twice
        private bool FinishedSetup = false;


        #region The "Update" Methods

        private void Awake()
        {
            SelfRigidbody = GetComponent<Rigidbody>();
            SelfJoint = GetComponent<ConfigurableJoint>();
        }

        //This is taking the place of initialize for my implementation
        public void Setup(RagdollLimb[] otherLimbs)
        {
            FinishedSetup = false;
            if (SelfJoint.connectedBody)
            {
                //We loop through all other limbs passed through
                for (int i = 0; i < otherLimbs.Length; i++)
                {
                    //If the limb we are checking, is the one our joint is connected to we save that reference
                    if (otherLimbs[i].SelfRigidbody == SelfJoint.connectedBody)
                    {
                        connectedLimbTarget = otherLimbs[i].AnimatedTarget;
                    }
                }
            }

            SetKinematic(false);

            SetupColliders();
            if (limbColliders.Length == 0)
            {
                Debug.LogError("Somehow there is no colliders for this limb. Fix it. Limb: " + transform.name);
            }

            if (connectedLimbTarget)
                parentOfTarget = connectedLimbTarget;
            else
                parentOfTarget = AnimatedTarget.parent;

            //This gives us a vector perpendicular to both the primary and secondary axis of the joint
            Vector3 tempForward = Vector3.Cross(SelfJoint.axis, SelfJoint.secondaryAxis).normalized;
            //This then gives us the same as above, but instead perpendicular to the one we just calculated
            //Technically we could just use the secondary axis as UP, but we calculate again to just double check that things will be valid since the user could set them up wrong
            Vector3 tempUp = Vector3.Cross(tempForward, SelfJoint.axis).normalized;

            //If both of those calculations returned the same thing, then we have a problem on our hands so we stop and tell the user
            if (tempForward == tempUp)
            {
                Debug.LogError("You can't have the Axis and Secondary Axis be pointing in the same direction. This is on Joint: " + gameObject.name);
                return;
            }

            selfRotationRelativeToTarget = Quaternion.Inverse(AnimatedTarget.rotation) * transform.rotation;

            initialTargetRotationRelToLimb = Quaternion.Inverse(transform.rotation) * AnimatedTarget.rotation;
            initialTargetRotationRelToLimbINVERSE = Quaternion.Inverse(initialTargetRotationRelToLimb);

            //Get ourselves a rotation pointing forward along tempForward, and matching UP vectors. Essentially, the joint's orientation
            Quaternion tempJointSpace = Quaternion.LookRotation(tempForward, tempUp);

            //We convert from out current local rotation, to joint space and then save it. Quaternion magic
            jointSpaceConverterINITIAL = SelfLocalRotation * tempJointSpace;
            jointSpaceConverterINVERSE = Quaternion.Inverse(tempJointSpace);

            parentRotationFromTarget = Quaternion.Inverse(TargetParentRotation) * SelfParentRotation;

            localRotationFromTarget = Quaternion.Inverse(TargetLocalRotation) * SelfLocalRotation;

            if (SelfJoint.connectedBody)
            {
                SelfJoint.autoConfigureConnectedAnchor = false;
                selfJointConnectedBodyTransform = SelfJoint.connectedBody.transform;

                //If our target's parent is the same as what our joint is connected to, then its true
                sameTargetParent = AnimatedTarget.parent == selfJointConnectedBodyTransform;
                SetAnchor(true);
            }

            relativeTargetRotation = Quaternion.Inverse(transform.rotation) * AnimatedTarget.rotation;

            if (!SelfJoint.connectedBody)
            {
                defaultPosition = transform.localPosition;
                defaultRotation = transform.localRotation;
            }
            else
            {
                defaultPosition = SelfJoint.connectedBody.transform.InverseTransformPoint(transform.position);
                defaultRotation = Quaternion.Inverse(SelfJoint.connectedBody.transform.rotation) * transform.rotation;
            }

            SelfJoint.rotationDriveMode = RotationDriveMode.Slerp;
            SelfJoint.configuredInWorldSpace = false;
            SelfJoint.projectionMode = JointProjectionMode.None;

            if (SelfJoint.anchor != Vector3.zero)
            {
                Debug.LogError("The Anchor setting must be set to Vector3(0,0,0). This is not true on: " + transform.name);
                return;
            }

            targetWorldCenterOfMass = SelfRigidbody.worldCenterOfMass;
            targetAnimatedRotation = TargetLocalRotation * localRotationFromTarget;

            //Check what state the animator started in to set a TON MORE values
            CheckAnimator();
            lastReadTime = Time.time;
            lastWriteTime = Time.time;

            FinishedSetup = true;
        }
        public void CheckAnimator()
        {
            //If this limb is not even active, dont run the rest of the code
            if (!ActiveLimb) return;

            float lastReadDeltaTime = Time.time - lastReadTime;
            lastReadTime = Time.time;

            //Get the center of mass of the target, not worrying about scale
            Vector3 targetAnimatedCenterMass = TransformPointUnscaled(AnimatedTarget, initialTargetRotationRelToLimbINVERSE * SelfRigidbody.centerOfMass);

            if (lastReadDeltaTime > 0f)
                animatedTargetVelocity = (targetAnimatedCenterMass - targetWorldCenterOfMass) / lastReadDeltaTime;

            targetWorldCenterOfMass = targetAnimatedCenterMass;

            if (SelfJoint.connectedBody)
                targetAnimatedRotation = TargetLocalRotation * localRotationFromTarget;
        }

        public void AnimateLimb(float forceWeightMaster, float torqueWeightMaster, float deltaTime, float falloffRange, float spring, float limbDamper)
        {
            if (!ActiveLimb)
            {
                forceWeightMultiplier = 0;
                limbWeightMultiplier = 0;
            }

            ClampValues();

            LimbMovement(forceWeightMaster, deltaTime, falloffRange);

            AngularLimbMovement(torqueWeightMaster, spring, limbDamper);
        }
        private void LimbMovement(float forceWeightMaster, float deltaTime, float falloffRange)
        {
            posOffset = targetWorldCenterOfMass - SelfRigidbody.worldCenterOfMass;
            if (float.IsNaN(posOffset.x)) posOffset = Vector3.zero;

            float weight = forceWeightMaster * ForceWeight * forceWeightMultiplier;
            if (weight <= 0) return;

            ForceCalc(posOffset, weight, deltaTime, falloffRange);
        }
        private void AngularLimbMovement(float torqueWeightMaster, float limbSpring, float limbDamper)
        {
            float w = torqueWeightMaster * TorqueWeight * limbWeightMultiplier * limbSpring * 10f;

            // If no connection point, don't rotate;
            if (SelfJoint.connectedBody == null) w = 0f;
            else if (w > 0f) SelfJoint.targetRotation = LocalToJointSpace(targetAnimatedRotation);

            float d = (RotationDamper * limbDamper);

            if (w == lastJointDriveRotationWeight && d == lastRotationDamper) return;
            lastJointDriveRotationWeight = w;

            lastRotationDamper = d;
            slerpDrive.positionSpring = w;
            slerpDrive.maximumForce = Mathf.Max(w, d);
            slerpDrive.positionDamper = d;

            SelfJoint.slerpDrive = slerpDrive;
        }
        #endregion

        #region Helper Methods
        private void ClampValues()
        {
            mappingWeight = Mathf.Clamp(mappingWeight, 0f, 1f);
            ForceWeight = Mathf.Clamp(ForceWeight, 0f, 1f);
            TorqueWeight = Mathf.Clamp(TorqueWeight, 0f, 1f);
            RotationDamper = Mathf.Clamp(RotationDamper, 0f, 1f);

            forceWeightMultiplier = Mathf.Clamp(forceWeightMultiplier, 0f, 1f);
            limbWeightMultiplier = Mathf.Clamp(limbWeightMultiplier, 0f, 1f);
            mappingWeightMultiplier = Mathf.Clamp(mappingWeightMultiplier, 0f, 1f);
        }
        private void ForceCalc(Vector3 offset, float weight, float deltaTime, float forceFalloffRange)
        {

            Vector3 p = offset;
            if (deltaTime > 0f) p /= deltaTime;

            Vector3 force = -SelfRigidbody.velocity + p;
            if (SelfRigidbody.useGravity) force -= Physics.gravity * deltaTime;
            force *= weight;
            if (forceFalloffRange > 0f) force /= 1f + offset.sqrMagnitude * forceFalloffRange;

            SelfRigidbody.AddForce(force, ForceMode.VelocityChange);
        }
        private void SetAnchor(bool YN)
        {
            //If we dont have anything to anchor to, then stop
            if (!SelfJoint.connectedBody || !connectedLimbTarget) return;
            if (sameTargetParent && !YN) return;
            if (!ActiveLimb) return;

            Vector3 initialAnchor = SelfJoint.connectedAnchor = InverseTransformPointUnscaled(connectedLimbTarget.position, connectedLimbTarget.rotation * parentRotationFromTarget, AnimatedTarget.position);
            float scaleScalar = 1 / connectedLimbTarget.lossyScale.x;

            SelfJoint.connectedAnchor = initialAnchor * scaleScalar;
        }
        public void SetKinematic(bool YN)
        {
            if (YN)
                SelfRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;

            SelfRigidbody.isKinematic = YN;
        }

        //A method that directly moves the limb to its target and doesn't attempt to use force and torque
        public void DirectMoveToTarget()
        {
            if (!FinishedSetup) return;
            if (!ActiveLimb) return;

            Vector3 pos = AnimatedTarget.position;
            Quaternion rot = AnimatedTarget.rotation * selfRotationRelativeToTarget;

            transform.SetPositionAndRotation(pos, rot);
            SelfRigidbody.MovePosition(pos);
            SelfRigidbody.MoveRotation(rot);

            posOffset = Vector3.zero;
        }

        private Vector3 TransformPointUnscaled(Transform target, Vector3 point)
        {
            //This returns a point transformed from world space to local space, but doesn't take into account the transform's scale
            return target.position + target.rotation * point;
        }
        private Vector3 InverseTransformPointUnscaled(Vector3 position, Quaternion rotation, Vector3 point)
        {
            return Quaternion.Inverse(rotation) * (point - position);
        }
        private Quaternion LocalToJointSpace(Quaternion localRotation)
        {
            return jointSpaceConverterINVERSE * Quaternion.Inverse(localRotation) * jointSpaceConverterINITIAL;
        }
        private Vector3 CalculateAngularVelocity(Quaternion lastRotation, Quaternion rotation, float deltaTime)
        {
            Quaternion rotationDelta = rotation * Quaternion.Inverse(lastRotation);

            float angle = 0f;
            Vector3 angleVector = Vector3.zero;

            rotationDelta.ToAngleAxis(out angle, out angleVector);

            if (float.IsNaN(angleVector.x))
                return Vector3.zero;
            if (float.IsInfinity(angleVector.x))
                return Vector3.zero;

            angle *= Mathf.Deg2Rad;
            angle /= deltaTime;

            angle = ToBiPolar(angle);

            angleVector *= angle;
            return angleVector;
        }

        public float ToBiPolar(float angle)
        {
            angle = angle % 360f;
            if (angle >= 180f) return angle - 360f;
            if (angle <= -180f) return angle + 360f;
            return angle;
        }
        #endregion

        #region Mapping and Sampling
        //Mapping the target is kinda of like going backwards with an active ragdoll

        //This makes sure that when the ragdoll falls over, or needs to blend to some other pose, the animator follows along with it normally
        //and doesn't cause it to snap back too early
        public void MapTarget(float mappingWeightMaster)
        {
            float totalWeight = mappingWeight * mappingWeightMaster * mappingWeightMultiplier;
            if (totalWeight <= 0f) return;

            // rigidbody.position does not work with interpolation
            Vector3 position = transform.position;
            Quaternion rotation = transform.rotation;

            Vector3 pos = position;
            Quaternion rot = rotation;

            if (totalWeight >= 1f)
            {
                rot = rotation * relativeTargetRotation;
                pos = position;

                if (selfJointConnectedBodyTransform)
                {
                    // Mapping in local space of the parent
                    Vector3 relativePosition = InverseTransformPointUnscaled(selfJointConnectedBodyTransform.position, selfJointConnectedBodyTransform.rotation, position);
                    pos = connectedLimbTarget.position + connectedLimbTarget.rotation * relativePosition;
                }

                AnimatedTarget.SetPositionAndRotation(pos, rot);

                return;
            }

            rot = Quaternion.Lerp(AnimatedTarget.rotation, rotation * relativeTargetRotation, totalWeight);

            if (selfJointConnectedBodyTransform != null)
            {
                // Mapping in local space of the parent
                Vector3 relativePosition = InverseTransformPointUnscaled(selfJointConnectedBodyTransform.position, selfJointConnectedBodyTransform.rotation, position);
                pos = Vector3.Lerp(AnimatedTarget.position, connectedLimbTarget.position + connectedLimbTarget.rotation * relativePosition, totalWeight);
            }
            else
            {
                pos = Vector3.Lerp(AnimatedTarget.position, position, totalWeight);
            }

            AnimatedTarget.SetPositionAndRotation(pos, rot);
        }

        public void StoreMappedPosition()
        {
            animatedTargetMappedPosition = AnimatedTarget.position;
        }
        public void StoreMappedRotation()
        {
            animatedTargetMappedRotation = AnimatedTarget.rotation;
        }
        public void CalculateMappedVelocity()
        {
            float writeDeltaTime = Time.time - lastWriteTime;

            if (writeDeltaTime > 0f)
            {
                mappedVelocity = (AnimatedTarget.position - lastAnimatedTargetMappedPosition) / writeDeltaTime;

                mappedAngularVelocity = CalculateAngularVelocity(lastAnimatedTargetMappedRotation, AnimatedTarget.rotation, writeDeltaTime);

                lastWriteTime = Time.time;
            }

            lastAnimatedTargetMappedPosition = AnimatedTarget.position;
            lastAnimatedTargetMappedRotation = AnimatedTarget.rotation;
        } 

        #endregion

        #region Everything collider related

        public void SetupColliders()
        {
            limbColliders = new Collider[0];

            AddColliders(transform, true, ref limbColliders);

            for (int i = 0; i < transform.childCount; i++) {
                AddChildColliders(transform.GetChild(i), ref limbColliders);
            }
        }
        private void AddColliders(Transform trans, bool includeMeshColliders, ref Collider[] C)
        {
            var colliders = trans.GetComponents<Collider>();
            int cCount = 0;
            foreach (Collider c in colliders)
            {
                bool isMeshCollider = c is MeshCollider;
                if (!c.isTrigger && (!includeMeshColliders || !isMeshCollider)) cCount++;
            }

            if (cCount == 0) return;

            int l = C.Length;
            Array.Resize(ref C, l + cCount);
            int addC = 0;

            for (int i = 0; i < colliders.Length; i++)
            {
                bool isMeshCollider = colliders[i] is MeshCollider;
                if (!colliders[i].isTrigger && (!includeMeshColliders || !isMeshCollider))
                {
                    C[l + addC] = colliders[i];
                    addC++;
                }
            }
        }
        private void AddChildColliders(Transform trans, ref Collider[] colliders)
        {
            //If the transform we are checking has a rigidbody, then its probably a limb we already have so stop
            if (trans.GetComponent<Rigidbody>()) return;

            AddColliders(trans, true, ref limbColliders);

            for (int i = 0; i < trans.childCount; i++)
            {
                //We go through all the childrean of the provided transform and recursively add their colliders
                //This is the reason we have the check for rigidbodies at the start
                //If we keep going down the hierarchy we might actually hit another RagdollLimb, so we stop
                AddChildColliders(trans.GetChild(i), ref colliders);
            }
        }

        #endregion

        //Reset the position and rotation to their default states. Need to do this when enabling and disabling stuff
        public void Reset()
        {
            //Just a few checks to see if this even needs to be done
            if (!FinishedSetup) return;
            if (!SelfJoint) return;
            if (!ActiveLimb) return;

            if (SelfJoint.connectedBody)
            {
                transform.localPosition = defaultPosition;
                transform.localRotation = defaultRotation;
            }
            else
            {
                transform.position = SelfJoint.connectedBody.transform.TransformPoint(defaultPosition);
                transform.rotation = SelfJoint.connectedBody.transform.rotation * defaultRotation;
            }

            lastRotationDamper = -1f;
        }

        #region Fancy Variables
        //Now I dont know the proper c# term for these types of variables. But they calculate their values whenever
        //you try to access them. This way the values are always current


        //This gives us the local rotation of the Limb. More accurately than just doing transform.rotation since the ragdoll can be setup in different ways
        private Quaternion SelfLocalRotation
        {
            get
            {
                return Quaternion.Inverse(SelfParentRotation) * transform.rotation;
            }
        }
        //The rotation of the "parent" of this limb. I go into more detail inside
        private Quaternion SelfParentRotation
        {
            get
            {
                //If this limb has a limb it is connected to with it's joint, then that is the parent of this limb
                if (SelfJoint.connectedBody)
                {
                    return SelfJoint.connectedBody.rotation;
                }
                //If this limb has no actual parent in the hierarchy for some reason, just return the Identity Quaternion. In quaternion terms essentially (0,0,0,0)
                if (!transform.parent)
                {
                    return Quaternion.identity;
                }
                //If both the previous checks failed, then it means we are probably the hip joint, and have a parent. So we just return out parent's actual rotation
                return transform.parent.rotation;
            }
        }
        //The rotation of the "parent" of our target
        private Quaternion TargetParentRotation
        {
            get
            {
                //If we couldnt find a parent for the target, then we just return the Identity quaternion
                if (!parentOfTarget) return Quaternion.identity;

                //Otherwise, actually return the rotation of its target
                return parentOfTarget.rotation;
            }
        }
        private Quaternion TargetLocalRotation
        {
            get
            {
                //This is absolute magic dude.
                //We first add the rotation from the target's parent to our parent TO the target parent. Essentially its our's parent's rotation.
                //Then we INVERSE that and Add it to the Animated target's rotation. This should then return the local rotation of the animated target
                return Quaternion.Inverse(TargetParentRotation * parentRotationFromTarget) * AnimatedTarget.rotation;
            }
        } 
        #endregion
    } 
}
