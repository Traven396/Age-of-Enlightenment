using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AgeOfEnlightenment.Enemies
{
    [Serializable]
    public enum RagdollState
    {
        Alive,
        Dead,
        Frozen
    }

    public class RagdollAnimatorV2 : MonoBehaviour
    {
        #region Ragdoll Status Variables

        //All sorts of stuff for testing what state and what the animator is doing in other parts of the script
        [Header("Living Status")]

        public RagdollState CurrentState = RagdollState.Alive;
        private RagdollState lastState;
        private RagdollState activeState;

        public bool IsDying { get; private set; }
        public bool IsAlive { get { return CurrentState == RagdollState.Alive; } }
        public bool IsDead { get { return CurrentState == RagdollState.Dead; } }
        public bool IsFrozen { get { return CurrentState == RagdollState.Frozen; } }

        //A bool to check for if the animator will be frozen in the next frame
        private bool freezeFlag;

        #endregion

        [Header("All Limbs: Hips First")]
        ///<summary>
        /// The hips must be the first limb you set. A lot of things rely on this
        /// </summary>
        public RagdollLimb[] Limbs = new RagdollLimb[0];

        #region The Main Settings

        [Header("Ragdoll Limb Variables")]

        [Range(0, 1)] public float forceWeight = 1;
        [Range(0, 1)] public float angularWeight = 1;
        [Range(0, 1)] public float mappingWeight = 1;

        public float limbForceFalloff = 5;
        public float limbSpring = 100f;
        public float limbDamper = 0; 

        #endregion


        #region Knockout Settings

        [Header("Knocked Out Settings")]

        [Tooltip("How long does it take to transition to dead force values?")]
        public float killDuration;
        //This is how strong the limbs that are considered "dead" will be. Its for knocking out stuff
        public float deadLimbForce = 0.01f;
        //How much to affect the torque and weight of the limb while its dead
        public float deadMuscleWeight;
        [Tooltip("The max square velocity of the ragdoll bones for freezing the whole ragdoll.")]
        public float maxFreezeSqrVelocity;

        //So this is an optional behavior you can add to the ragdoll where when it collides with stuff
        //or gets pushed around it can be knocked over and have to get back up
        //That script must be added to the same script as this one
        [HideInInspector] public RagdollKnockoutBehaviour knockoutComponent;

        #endregion


        #region Animator References

        [Header("Animator Root")]

        //You need to set this variable manually, it should be the root of the animated character.
        //IE the piece that is moving around with it, like the GameObject with the animator component or what have you
        public Transform AnimatedTargetRoot;

        //The actual reference to the animator component for the character
        public Animator TargetAnimator { get; private set; }
        #endregion

        #region Mapping Animator Values
        //Some boolean flags to track if we know what state the animator is in
        [HideInInspector] public bool storeTargetMappedState = true;
        [HideInInspector] public bool storeTargetSampledState = true;


        //Bool to see if we have a saved state of the ragdoll
        private bool targetMappedStateSaved; 
        #endregion

        private void Awake()
        {
            TargetAnimator = AnimatedTargetRoot.GetComponent<Animator>();
            knockoutComponent = GetComponent<RagdollKnockoutBehaviour>();
            if (knockoutComponent)
                knockoutComponent.ragdollAnimator = this;
        }
        private void Start()
        {
            SetupRagdollAnimator();
        }

        private void SetupRagdollAnimator()
        {
            if (AnimatedTargetRoot != null && TargetAnimator == null)
            {
                TargetAnimator = AnimatedTargetRoot.GetComponentInChildren<Animator>();
            }

            ChangeState();

            if (knockoutComponent)
            {
                knockoutComponent.Initialize();
                knockoutComponent.OnActivate();
            }
            for (int i = 0; i < Limbs.Length; i++)
            {
                Limbs[i].Setup(Limbs);

                if (knockoutComponent)
                {
                    //If we have a knockdown component, then we add this muscle collision behavior to all the limbs
                    Limbs[i].broadcaster = Limbs[i].gameObject.AddComponent<RagdollLimbCollisionsBroadcaster>();
                    //Give them a reference to this script
                    Limbs[i].broadcaster.ragdollAnimator = this;
                    //Also tell them which limb they are in the list
                    Limbs[i].broadcaster.limbIndex = i;
                }
                //Set each limb's initial values it will try and move to in the first frame
                Limbs[i].CheckAnimator();
            }
        }
        private void FixedUpdate()
        {
            if (!IsFrozen)
            {
                //Loop through every limb we have set and run the Animate method, passing through a ton of variables for it
                for (int i = 0; i < Limbs.Length; i++)
                {
                    Limbs[i].AnimateLimb(forceWeight, angularWeight, Time.deltaTime, limbForceFalloff, limbSpring, limbDamper);
                }
                if (knockoutComponent)
                    knockoutComponent.FixedUpdateLogic(Time.deltaTime);
            }
        }

        private void LateUpdate()
        {

            ChangeState();

            if (knockoutComponent)
                knockoutComponent.AnimatorCheckLogic(Time.deltaTime);

            UpdateLimbAnimatorValues();


            //Mapping the animator to the ragdoll slightly
            if (!IsFrozen)
            {
                mappingWeight = Mathf.Clamp(mappingWeight, 0, 1);
                float masterWeight = mappingWeight;

                if(masterWeight > 0)
                {
                    foreach (RagdollLimb Limb in Limbs)
                    {
                        Limb.MapTarget(masterWeight);
                    }
                }

                StoreTargetMappedState();

                foreach (RagdollLimb L in Limbs) L.CalculateMappedVelocity();

            }

            if (freezeFlag)
                OnFreezeFlag();
        }
        
        private void UpdateLimbAnimatorValues()
        {
            if (!IsAlive) return;

            foreach (RagdollLimb L in Limbs) L.CheckAnimator();

        }


        #region Mapping and Sampling
        public void StoreTargetMappedState()
        {
            if (!storeTargetMappedState) return;

            for (int i = 0; i < Limbs.Length; i++)
            {
                if (i == 0)
                    Limbs[i].StoreMappedPosition();
                Limbs[i].StoreMappedRotation();
            }

            targetMappedStateSaved = true;
        }
        /// <summary>
		/// The pose that the target will be fixed to if calling FixTargetToSampledState(). This should normally be used only by the Puppet Behaviours.
		/// </summary>
        public void SampleTargetMappedState()
        {
            storeTargetSampledState = true;

            if (!targetMappedStateSaved)
            {
                storeTargetSampledState = true;
                return;
            }

            for (int i = 0; i < Limbs.Length; i++)
            {
                if (i == 0) 
                    Limbs[i].targetSampledPosition = Limbs[i].animatedTargetMappedPosition;
                Limbs[i].targetSampledRotation = Limbs[i].animatedTargetMappedRotation;
            }

            targetMappedStateSaved = true;
        }

        /// <summary>
        /// Blend the target to the pose that was sampled by the last SampleTargetMappedState call. This should normally be used only by the Puppet Behaviours.
        /// </summary>
        public void FixTargetToSampledState(float weight)
        {
            if (weight <= 0f) return;

            if (!targetMappedStateSaved)
            {
                return;
            }

            for (int i = 0; i < Limbs.Length; i++)
            {
                //Specific logic just for when on the hip bone
                if (i == 0) 
                    Limbs[i].AnimatedTarget.position = Vector3.Lerp(Limbs[i].AnimatedTarget.position, Limbs[i].targetSampledPosition, weight);
                Limbs[i].AnimatedTarget.rotation = Quaternion.Lerp(Limbs[i].AnimatedTarget.rotation, Limbs[i].targetSampledRotation, weight);
            }

            foreach (RagdollLimb L in Limbs) L.posOffset = L.AnimatedTarget.position - L.SelfRigidbody.position;
        }
        #endregion

        #region State Switching
        private void ChangeState()
        {
            if (CurrentState == lastState) return;
            if (IsDying) return;

            if (freezeFlag)
            {
                if (CurrentState == RagdollState.Alive)
                {
                    activeState = RagdollState.Dead;
                    lastState = RagdollState.Dead;
                    freezeFlag = false;
                }
                else if (CurrentState == RagdollState.Dead)
                {
                    lastState = RagdollState.Dead;
                    freezeFlag = false;
                    return;
                }

                //If we made it through both checks, we are already frozen
                if (freezeFlag) return;
            }

            if (lastState == RagdollState.Alive)
            {
                if (CurrentState == RagdollState.Dead)
                    StartCoroutine(AliveToDead(false));
                else if (CurrentState == RagdollState.Frozen)
                    StartCoroutine(AliveToDead(true));
            }
            else if (lastState == RagdollState.Dead)
            {
                if (CurrentState == RagdollState.Alive)
                    DeadToAlive();
                else if (CurrentState == RagdollState.Frozen)
                    DeadToFrozen();
            }
            else if (lastState == RagdollState.Frozen)
            {
                if (CurrentState == RagdollState.Alive)
                    FrozenToAlive();
                else if (CurrentState == RagdollState.Dead)
                    FrozenToDead();
            }

            lastState = CurrentState;
        }
        private IEnumerator AliveToDead(bool freeze)
        {
            IsDying = true;

            // Set pin weight to 0 to play with joint target rotations only
            foreach (RagdollLimb L in Limbs)
            {
                if (L.ActiveLimb)
                {
                    L.forceWeightMultiplier = 0f;

                    //L.SelfRigidbody.velocity = L.mappedVelocity;
                    //L.SelfRigidbody.angularVelocity = L.mappedAngularVelocity;
                }
            }

            float range = Limbs[0].forceWeightMultiplier - deadMuscleWeight;
            if (knockoutComponent)
                knockoutComponent.KnockoutStart();

            if (killDuration > 0f && range > 0f)
            {
                float LimbWeight = Limbs[0].limbWeightMultiplier;

                while (LimbWeight > deadMuscleWeight)
                {
                    LimbWeight = Mathf.Max(LimbWeight - Time.deltaTime * (range / killDuration), deadMuscleWeight);

                    foreach (RagdollLimb L in Limbs) L.limbWeightMultiplier = LimbWeight;

                    yield return null;
                }
            }
            foreach (RagdollLimb L in Limbs) L.limbWeightMultiplier = deadMuscleWeight;

            // Disable the Animator
            SetAnimationEnabled(false);

            IsDying = false;
            activeState = RagdollState.Dead;

            if (freeze) freezeFlag = true;
            if (knockoutComponent)
                knockoutComponent.KnockoutEnd();

            ///////////////////////////////////
            ///Event for death here if you want
        }
        private void DeadToAlive()
        {
            foreach (RagdollLimb Limb in Limbs)
            {
                Limb.forceWeightMultiplier = 1;
                Limb.limbWeightMultiplier = 1;
            }

            if (knockoutComponent)
                knockoutComponent.Revive();

            SetAnimationEnabled(true);

            activeState = RagdollState.Alive;
        }
        private void FrozenToAlive()
        {
            freezeFlag = false;

            foreach (RagdollLimb L in Limbs)
            {
                L.forceWeightMultiplier = 1f;
                L.limbWeightMultiplier = 1f;
            }

            ActivateRagdoll();

            if (knockoutComponent)
                knockoutComponent.Revive();

            if (TargetAnimator != null) TargetAnimator.enabled = true;

            activeState = RagdollState.Alive;

            ///////////////////////////////////////////////
            ///Events for both reviving and unfreezing
        }
        private void OnFreezeFlag()
        {
            //If our limbs aren't able to be frozen rn, then just skip through all of this
            if (!CanFreeze()) return;

            SetAnimationEnabled(false);

            foreach (RagdollLimb Limb in Limbs)
                Limb.gameObject.SetActive(false);

            freezeFlag = false;
            activeState = RagdollState.Frozen;

            /////////////////////////////
            ///You could put an event for when the ragdoll freezes here if you need it
            ///
        }
        private void FrozenToDead()
        {
            freezeFlag = false;

            ActivateRagdoll();

            activeState = RagdollState.Dead;
            
            ///////////////////////////////////
            ////You could put an event here to check for when the ragdoll is unfrozen.
        }
        private void DeadToFrozen()
        {
            freezeFlag = true;
        }

        private bool CanFreeze()
        {
            foreach (RagdollLimb L in Limbs)
            {
                if (L.SelfRigidbody.velocity.sqrMagnitude > maxFreezeSqrVelocity) return false;
            }
            return true;
        }
        #endregion
        private void SetAnimationEnabled(bool changeTo)
        {
            if (TargetAnimator)
            {
                TargetAnimator.enabled = changeTo;
            }
        }
        private void ActivateRagdoll(bool kinematic = false)
        {
            foreach (RagdollLimb Limb in Limbs)
            {
                Limb.Reset();
            
                Limb.SelfJoint.gameObject.SetActive(true);
                if (kinematic) Limb.SelfRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
                Limb.SetKinematic(kinematic);
                Limb.SelfRigidbody.velocity = Vector3.zero;
                Limb.SelfRigidbody.angularVelocity = Vector3.zero;
            }

            //FlagInternalCollisionsForUpdate();

            UpdateLimbAnimatorValues();

            foreach (RagdollLimb Limb in Limbs)
            {
                Limb.DirectMoveToTarget();
            }

        }
    } 
}
