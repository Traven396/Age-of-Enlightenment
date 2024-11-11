using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AgeOfEnlightenment.Enemies;
using UnityEngine.Events;
using System;

/// <summary>
/// Multiplies collision resistance for the specified layers.
/// </summary>
[System.Serializable]
public struct CollisionResistanceMultiplier
{
    public LayerMask layers;

    [Tooltip("Multiplier for the 'Collision Resistance' for these layers.")]
    public float multiplier;

    [Tooltip("Overrides 'Collision Threshold' for these layers.")]
    /// <summary>
    /// Overrides 'Collision Threshold' for these layers.
    /// </summary>
    public float collisionThreshold;
}

//Creates 3 main states for the ragdoll to be in
// Pinned = Fully being forced to follow the animation, snapping to the position
// Unpinned = Losing strength or having no strength at all. The main "ragdoll" state
// GettingUp = Self Explanatory, this is when the ragdoll is shifting back from unpinned to pinned
public enum RagdollState
{
    Pinned,
    Unpinned,
    GettingUp
}
[RequireComponent(typeof(RagdollAnimatorV2))]
public class RagdollKnockoutBehaviour : MonoBehaviour
{
    //The parent script that this script heavily relies on
    [HideInInspector] public RagdollAnimatorV2 ragdollAnimator;

    private RagdollLimbCollisionsBroadcaster broadcaster;
    
    
    [Header("Generic Settings")]
    //The maximum speed the rigidbody can move, makes sure things dont get too snappy
    public float maxRigidbodyVelocity = 10;
    /// <summary>
    /// If false, will not knock over the ragdoll with limbs that have a Force Weight of 0
    /// </summary>
    public bool unpinnedLimbKnockout = true;

    //This is the force the limbs will have when it gets knocked out. Lower values = weaker limbs
    [Range(0, 1)] public float unpinnedTorqueWeightMultiplier = 0.3f;

    [Tooltip("If false, will not knock over the ragdoll with limbs that have a Force Weight of 0")]
    
    /// <summary>
    /// If the limb has drifted past the knock out distance, it will only 'unpin' the limb if its total force value is less than this value. The lower the value the more stable the ragdoll will be when colliding 
    /// </summary>
    [Range(0, 1)] public float forceWeightThreshold = 1;

    [Range(0.001f, 10f)] public float knockoutDistance = 1;
        private float lastKnockoutDistance;
        private float knockoutDistanceSqr;



    #region Collision Settings
    [Header("Collision Settings")]
    /// <summary>
    /// An optimisation. Will only process up to this number of collisions per physics step.
    /// </summary>
    [Range(1, 30)] public int maxCollisions = 30;

    [Tooltip("The collision impulse sqrMagnitude threshold under which collisions will be ignored.")]
    /// <summary>
    /// The collision impulse sqrMagnitude threshold under which collisions will be ignored.
    /// </summary>
    public float collisionThreshold;

    /// <summary>
    /// This is how resistant the character will be to being unpinned from collisions. For a curve it will be based on the limb's velocity
    /// </summary>
    public Weight collisionResistance = new Weight(3f);

    [Tooltip("How resistant the character is to collisions while getting up")]
    public float getUpCollisionResistanceMutliplier = 2;
    #endregion

    #region Getting Up Settings
    [Header("Stand Up Variables")]
    /// <summary>
    /// The amount of minimum time it takes for the ragdoll to stand back up. This isnt always how long it will take. If the ragdoll is still moving it will wait.
    /// </summary>
    public float getUpDelay = 4;

    [Tooltip("The duration that the it will take the puppet to stand up.")]
    /// <summary>
    /// The duration that the it will take the puppet to stand up
    /// </summary>
    public float minGetUpDuration = 1;

    /// <summary>
    /// How fast will the limbs of this group regain their pin weight?
    /// </summary>
    [Range(0.001f, 10f)] public float regainForceSpeed = 1f;

    [Tooltip("Regain force weight speed multiplier while in the GettingUp state. Increasing this will prevent the character from loosing balance again immediatelly after going from Unpinned to GettingUp state.")]
    /// <summary>
    /// Regain pin weight speed multiplier while in the GettingUp state. Increasing this will prevent the character from loosing balance again immediatelly after going from Unpinned to GettingUp state.
    /// </summary>
    public float getUpRegainForceSpeedMultiplier = 2f;

    /// <summary>
    /// The knockout distance multiplier while the ragdoll is in the getting up state. Higher values mean its harder for the ragdoll to fall over immediately upon standing
    /// </summary>
    public float getUpKnockoutDistanceMult = 10f;

    

    /// <summary>
    /// This is how fast the hips can still be moving and the ragdoll will attempt to stand up. Any faster than this and it will continue falling over
    /// </summary>
    public float maxStandupVelocity = 0.4f;

    [Header("Offsets for different getup animations")]
    [Tooltip("Offset of the target character (in character rotation space) from the hip bone when initiating getting up animation from laying on their back. Tweak this value if your character slides a bit when starting to get up.")]
    /// <summary>
    /// Offset of the target character (in character rotation space) from the hip bone when initiating getting up animation from laying on their back. Tweak this value if your character slides a bit when starting to get up.
    /// </summary>
    public Vector3 getUpOffsetBack;

    [Tooltip("Offset of the target character (in character rotation space) from the hip bone when initiating getting up animation from laying on their chest. Tweak this value if your character slides a bit when starting to get up.")]
    /// <summary>
    /// Offset of the target character (in character rotation space) from the hip bone when initiating getting up animation from laying on their chest. Tweak this value if your character slides a bit when starting to get up.
    /// </summary>
    public Vector3 getUpOffsetStomach;
    #endregion



    [Header("Events")]
    public RagdollEvent OnGetupFromBack;
    public RagdollEvent OnGetupFromStomach;
    public RagdollEvent OnLoseBalance;
    public RagdollEvent OnLoseBalanceFromPinned;
    public RagdollEvent OnLoseBalanceWhileGettingUp;
    public RagdollEvent OnRegainBalance;

    [Header("Layers")]
    public LayerMask groundLayers;
    [Tooltip("The layers that the ragdoll will unpin upon hitting")]
    public LayerMask collisionLayers;
    //Allows you to set multiple resistances to different layers
    public CollisionResistanceMultiplier[] collisionResistanceMultipliers;

    
    /// <summary>
    /// If the ragdoll is on a moving platform this will be set to the velocity of that platform
    /// </summary>
    public Vector3 platformVelocity { get; set; }
    private Vector3 standUpPosition { get; set; }


    private RagdollState currentState;
    private Vector3 hipsForward, hipsUp;

    //Timers so the the ragdoll doesn't instantly stand back up
    private float getUpTimer, unpinnedTimer;
    //Used to blend between the stand up animation and normal ones
    private float getupAnimationBlendWeight;
    
    //Used to count how many times we have been hit in a frame
    private int numOfCollisions;

    //Simple bool to check if the ragdoll can stand up in its current state
    private bool standupDisabled = true;
    //This allows us to turn on and off the events firing as a whole
    private bool fireEvents = true;
    private bool getUpTargetFixed;
    private bool hasCollidedSinceGetUp;
    private float lastCollisionTime;


    #region Events called by the RagdollAnimator
    public void Initialize()
    {

        //Could maybe do some nicety checks here to determine if layers are setup correctly

        hipsForward = Quaternion.Inverse(ragdollAnimator.Limbs[0].transform.rotation) * ragdollAnimator.AnimatedTargetRoot.forward;
        hipsUp = Quaternion.Inverse(ragdollAnimator.Limbs[0].transform.rotation) * ragdollAnimator.AnimatedTargetRoot.up;

        currentState = RagdollState.Unpinned;

        fireEvents = true;
    }
    public void OnActivate()
    {
        bool notPinned = true;
        //If the overall ragdoll has an actual weight to it
        if (ragdollAnimator.forceWeight > 0)
        {
            //Loop through every limb
            foreach (RagdollLimb l in ragdollAnimator.Limbs)
            {
                //Does..... something..... not quite sure why its setup this way. I feel it should be backwards but it works
                if (l.forceWeightMultiplier > 0.5f)
                {
                    notPinned = false;
                    break;
                }
            }
        }

        bool fe = fireEvents;

        //Temporarily disable the events since it will cause errors setting up for the first time
        fireEvents = false;
        if (notPinned)
            ChangeState(RagdollState.Unpinned);
        else
            ChangeState(RagdollState.Pinned);

        fireEvents = fe;
    }
    public void KnockoutStart()
    {
        //Make it so it cant get back up. Then loop through all active limbs and set
        //their force multiplier to 0
        standupDisabled = true;

        foreach (RagdollLimb limb in ragdollAnimator.Limbs)
        {
            if (limb.ActiveLimb)
            {
                limb.forceWeightMultiplier = 0;

                //SetCollidersState(limb, true);
            }
        }
    }
    public void KnockoutEnd()
    {
        ChangeState(RagdollState.Unpinned);
    }

    //The method called whenever the animator moves from a "dead" state to alive.
    public void Revive()
    {
        standupDisabled = false;

        if (currentState == RagdollState.Unpinned)
        {
            getUpTimer = Mathf.Infinity;
            unpinnedTimer = Mathf.Infinity;
            getupAnimationBlendWeight = 0;

            foreach (RagdollLimb Limb in ragdollAnimator.Limbs)
            {
                Limb.forceWeightMultiplier = 0;
            }
        }
    }
    public void OnDeactivate()
    {
        currentState = RagdollState.Unpinned;
    }
    public void FixedUpdateLogic(float deltaTime)
    {
        numOfCollisions = 0;

        if (!ragdollAnimator.IsAlive)
        {
            foreach (RagdollLimb Limb in ragdollAnimator.Limbs)
            {
                if (Limb.ActiveLimb)
                {
                    Limb.forceWeightMultiplier = 0;
                    Limb.mappingWeightMultiplier = Mathf.MoveTowards(Limb.mappingWeightMultiplier, 1, deltaTime * 5);
                }
            }
            return;
        }

        if (currentState == RagdollState.Unpinned)
        {
            unpinnedTimer += deltaTime;

            if (unpinnedTimer >= getUpDelay && !standupDisabled)
            {
                Vector3 hipVelocity = ragdollAnimator.Limbs[0].SelfRigidbody.velocity - platformVelocity;
                if (hipVelocity.sqrMagnitude < maxStandupVelocity * maxStandupVelocity)
                {
                    //If the current speed of the hips is less than our set value, change the state to getting up and stop checking
                    ChangeState(RagdollState.GettingUp);
                    return;
                }
            }

            foreach (RagdollLimb Limb in ragdollAnimator.Limbs)
            {
                if (Limb.ActiveLimb)
                {
                    Limb.forceWeightMultiplier = 0;
                    Limb.mappingWeightMultiplier = Mathf.MoveTowards(Limb.mappingWeightMultiplier, 1f, deltaTime);
                }
            }
        }

        if (hasCollidedSinceGetUp && Time.time > lastCollisionTime + 3f) hasCollidedSinceGetUp = false;


        if (currentState != RagdollState.Unpinned && !ragdollAnimator.IsDying)
        {
            if (knockoutDistance != lastKnockoutDistance)
            {
                //Just squaring the knockout distance for faster calculations
                knockoutDistanceSqr = knockoutDistance * knockoutDistance;
                //Set the old value so we dont constantly calculate this
                lastKnockoutDistance = knockoutDistance;
            }

            foreach (RagdollLimb Limb in ragdollAnimator.Limbs)
            {
                //A temp variable for multiplying the force of getting up depending on the state
                float currentMult = 1;

                if (currentState == RagdollState.GettingUp)
                {
                    currentMult = Mathf.Lerp(getUpKnockoutDistanceMult, currentMult, Limb.forceWeightMultiplier);
                }
                //If the bool here is set to true then it sets the offset to a potentially higher value, since the other option is multiplied lower and lower;
                float offset = unpinnedLimbKnockout ? Limb.posOffset.sqrMagnitude : Limb.posOffset.sqrMagnitude * Limb.ForceWeight * ragdollAnimator.forceWeight;

                if (ragdollAnimator.forceWeight < 1f)
                {
                    hasCollidedSinceGetUp = true;
                    lastCollisionTime = Time.time;
                }

                //Combines all the force multipliers together
                float calcForceWeight = Limb.forceWeightMultiplier * Limb.ForceWeight * ragdollAnimator.forceWeight;

                if (hasCollidedSinceGetUp && Limb.ActiveLimb && offset > 0f && calcForceWeight <= forceWeightThreshold && offset > Limb.knockOutDistance * knockoutDistanceSqr * currentMult)
                {
                    if (currentState != RagdollState.GettingUp || getUpTargetFixed)
                    {
                        ///////////////////////////////////////////
                        //This right here is where the ragdoll gets knocked over if too much force has been applied or its limbs are too far apart

                        //If for some reason the logic here is not working and its falling over a lot check out what these values all mean
                        ChangeState(RagdollState.Unpinned);
                    }
                    return;
                }

                if (Limb.ActiveLimb)
                {
                    Limb.limbWeightMultiplier = Mathf.Lerp(unpinnedTorqueWeightMultiplier, 1, Limb.forceWeightMultiplier);

                    if (!ragdollAnimator.IsDying)
                    {
                        float speedFloat = 1;
                        if (currentState == RagdollState.GettingUp)
                            speedFloat = Mathf.Lerp(getUpRegainForceSpeedMultiplier, 1f, Limb.forceWeightMultiplier);

                        Limb.forceWeightMultiplier += deltaTime * regainForceSpeed * speedFloat;
                    }
                }
            }

            float maxForceWeight = 1;
            foreach (RagdollLimb Limb in ragdollAnimator.Limbs)
            {
                Limb.forceWeightMultiplier = Mathf.Clamp(Limb.forceWeightMultiplier, 0, maxForceWeight * 5f);
            }
        }

        //This is where the getting up state ends
        if (currentState == RagdollState.GettingUp)
        {
            //Increment a timer up
            getUpTimer += deltaTime;

            //If we have reached the limit reset the timer and then change state
            if (getUpTimer > minGetUpDuration)
            {
                getUpTimer = 0;
                ChangeState(RagdollState.Pinned);
            }
        }


    }
    public void AnimatorCheckLogic(float deltaTime)
    {
        if (!enabled) return;

        if (!ragdollAnimator.IsFrozen)
        {
            if (currentState == RagdollState.Unpinned /*&& ragdollAnimator.isActive  && !puppetMaster.isBlending */ && ragdollAnimator.Limbs[0].ActiveLimb && ragdollAnimator.Limbs[0].mappingWeightMultiplier >= 1f)
            {
                MoveRagdoll(ragdollAnimator.Limbs[0].SelfRigidbody.position);
                GroundRagdoll(groundLayers);
                standUpPosition = ragdollAnimator.AnimatedTargetRoot.position;
            }
        }

        // Prevents root motion from snapping the target to another position
        bool targetFixedForGettingUp = (currentState == RagdollState.GettingUp && getUpTimer < minGetUpDuration * 0.1f) || getupAnimationBlendWeight > 0f;

        if (targetFixedForGettingUp)
        {
            Vector3 y = Vector3.Project(ragdollAnimator.AnimatedTargetRoot.position - standUpPosition, ragdollAnimator.AnimatedTargetRoot.up);
            standUpPosition += y;
            standUpPosition += platformVelocity * deltaTime;
            MoveRagdoll(standUpPosition);
        }

        if (getupAnimationBlendWeight > 0f)
        {
            getupAnimationBlendWeight = Mathf.MoveTowards(getupAnimationBlendWeight, 0f, deltaTime);

            //Clamps the value to 0 if close enough
            if (getupAnimationBlendWeight < 0.01f)
                getupAnimationBlendWeight = 0f;

            ragdollAnimator.Limbs[0].targetSampledPosition += platformVelocity * deltaTime;

            // Lerps the target pose to last sampled mapped pose. Starting off from the ragdoll pose
            ragdollAnimator.FixTargetToSampledState(InterpolateFloat(getupAnimationBlendWeight));
        }


        getUpTargetFixed = true;
    } 
    #endregion

    #region The Actual Knocking Out Parts
    //This is for the code side of things. Like raycasting
    public void OnLimbHit(RagdollLimbHit hit)
    {
        // Unpin the limb (and other limbs) and add force
        UnPin(hit.limbIndex, hit.unPin);

        // Add force
        ragdollAnimator.Limbs[hit.limbIndex].SetKinematic(false);
        ragdollAnimator.Limbs[hit.limbIndex].SelfRigidbody.AddForceAtPosition(hit.force, hit.position);

    }
    //This is actual collisions being checked
    public void OnLimbCollision(RagdollCollision ragCollision)
    {
        // All the conditions for ignoring this
        if (!enabled) return;
        if (currentState == RagdollState.Unpinned) return;
        if (numOfCollisions > maxCollisions) return;
        if (!LayerContains(collisionLayers, ragCollision.collision.collider.gameObject.layer)) return;

        if (LayerContains(groundLayers, ragCollision.collision.collider.gameObject.layer))
        {
            if (currentState == RagdollState.GettingUp) return; // Do not damage if contact with ground layers and in getup state
            if (ragdollAnimator.Limbs[ragCollision.limbIndex].group == LimbGroup.Foot) return; // Do not damage if feet in contact with ground layers
        }
 
        // Get the collision impulse on the limb
        float collisionThresh = collisionThreshold;
        float impulse = GetImpulse(ragCollision, ref collisionThresh);

        if (impulse <= collisionThresh) return;
        numOfCollisions++;

        // Try to find out if it collided with another puppet's limb
        if (ragCollision.collision.collider.attachedRigidbody != null)
        {
            broadcaster = ragCollision.collision.collider.attachedRigidbody.GetComponent<RagdollLimbCollisionsBroadcaster>();
            if (broadcaster != null)
            {
                if (broadcaster.limbIndex < broadcaster.ragdollAnimator.Limbs.Length)
                {
                    // Multiply impulse (if the other limb has been boosted)
                    impulse *= broadcaster.ragdollAnimator.Limbs[broadcaster.limbIndex].impulseMultiplier;
                }
            }
        }

        // Unpin the limb and others too, but only if I can add the other functionality


        //authors note. I was not able to do this. Maybe in the future I can add it in. But I have been working on
        //translating this thing over for over 3 weeks and I want to die
        UnPin(ragCollision.limbIndex, impulse);
        
    }
    
    /// <summary>
    /// Knock out this puppet.
    /// </summary>
    public void KnockoutRagdoll()
    {
        ChangeState(RagdollState.Unpinned);
    }


    // Calculating the impulse magnitude from a collision
    private float GetImpulse(RagdollCollision L, ref float layerThreshold)
    {
        float i = L.collision.impulse.sqrMagnitude;

        i /= ragdollAnimator.Limbs[L.limbIndex].SelfRigidbody.mass;
        i *= 0.3f; // Coeficient for evening out for pre-0.3 versions

        // Collision resistance multipliers
        foreach (CollisionResistanceMultiplier crm in collisionResistanceMultipliers)
        {
            if (LayerContains(crm.layers, L.collision.collider.gameObject.layer))
            {
                if (crm.multiplier <= 0f) i = Mathf.Infinity;
                else i /= crm.multiplier;

                layerThreshold = crm.collisionThreshold;

                break;
            }
        }

        return i;
    }

    // Unpin a Limb and other limbs linked to it

    // THis behavior above me is not used as of right now. Its a whole much more difficult thing to do
    public void UnPin(int limbIndex, float unpin)
    {
        if (limbIndex >= ragdollAnimator.Limbs.Length) return;


        UnPinLimbSpecific(limbIndex, unpin);
        

        hasCollidedSinceGetUp = true;
        lastCollisionTime = Time.time;
    }

    // Unpin a specific limb according to its collision resistance, immunity and other values
    private void UnPinLimbSpecific(int limbIndex, float unpin)
    {
        // All the conditions to ignore this
        if (unpin <= 0f) return;
        if (ragdollAnimator.Limbs[limbIndex].collisionImmunity >= 1f) return;

        // Making the puppet more resistant to collisions while getting up
        float stateF = 1f;
        if (currentState == RagdollState.GettingUp) stateF = Mathf.Lerp(getUpCollisionResistanceMutliplier, 1f, ragdollAnimator.Limbs[limbIndex].forceWeightMultiplier);

        // Applying collision resistance
        float collisionResist = collisionResistance.mode == Weight.Mode.Float ? collisionResistance.floatValue : collisionResistance.GetValue(ragdollAnimator.Limbs[limbIndex].animatedTargetVelocity.magnitude);
        float damage = unpin / (collisionResist * stateF);
        damage *= 1f - ragdollAnimator.Limbs[limbIndex].collisionImmunity;

        // Finally apply the damage
        if (ragdollAnimator.Limbs[limbIndex].ActiveLimb)
        {
            ragdollAnimator.Limbs[limbIndex].forceWeightMultiplier = ragdollAnimator.Limbs[limbIndex].forceWeightMultiplier - damage;
        }
    }
    #endregion

    //This is like the most important method. It handles swapping between standing, falling, and getting up.
    public void ChangeState(RagdollState newState)
    {
        if (currentState == newState) return;

        switch (newState)
        {
            //If we are swapping to the pinned state
            case RagdollState.Pinned:
                ragdollAnimator.SampleTargetMappedState();
                unpinnedTimer = 0;
                getUpTimer = 0;
                hasCollidedSinceGetUp = false;

                //Are we swapping from the Unpinnd state?
                if(currentState == RagdollState.Unpinned)
                {
                    foreach (RagdollLimb L in ragdollAnimator.Limbs)
                    {
                        if (L.ActiveLimb)
                        {
                            L.forceWeightMultiplier = 1;
                            L.limbWeightMultiplier = 1;

                            L.posOffset = Vector3.zero;

                            //SetCollidersPinState(L, false);
                        }
                    }
                }

                currentState = RagdollState.Pinned;

                if (fireEvents)
                {
                    OnRegainBalance.Trigger(ragdollAnimator);
                }

                break;

            case RagdollState.Unpinned:
                unpinnedTimer = 0;
                getUpTimer = 0;
                getupAnimationBlendWeight = 0f;

                foreach (RagdollLimb L in ragdollAnimator.Limbs)
                {
                    if(maxRigidbodyVelocity != Mathf.Infinity)
                    {
                        L.SelfRigidbody.velocity = Vector3.ClampMagnitude(L.SelfRigidbody.velocity, maxRigidbodyVelocity);
                        L.mappedVelocity = Vector3.ClampMagnitude(L.mappedVelocity, maxRigidbodyVelocity);
                    }

                    if (L.ActiveLimb)
                    {
                        //Check if the character fully died, or just got knocked over and set teh appropriate force
                        L.limbWeightMultiplier = ragdollAnimator.IsAlive ? unpinnedTorqueWeightMultiplier : ragdollAnimator.deadLimbForce;
                    }

                    //Set the actual force value as well. The other one is for rotation
                    L.forceWeightMultiplier = 0f;
                }

                OnLoseBalance.Trigger(ragdollAnimator);

                if(currentState == RagdollState.Pinned)
                {
                    OnLoseBalanceFromPinned.Trigger(ragdollAnimator);
                }
                else
                {
                    OnLoseBalanceWhileGettingUp.Trigger(ragdollAnimator);
                }

                break;
            case RagdollState.GettingUp:
                unpinnedTimer = 0;
                getUpTimer = 0;
                hasCollidedSinceGetUp = false;

                bool isOnBack = !IsLayingOnChest();

                currentState = RagdollState.GettingUp;

                if (isOnBack)
                {
                    OnGetupFromBack.Trigger(ragdollAnimator);
                } else
                {
                    OnGetupFromStomach.Trigger(ragdollAnimator);
                }

                foreach (RagdollLimb limb in ragdollAnimator.Limbs)
                {
                    if (limb.ActiveLimb)
                    {
                        //SetCollidersState(limb, false);
                    }
                }

                Vector3 spineDirection = ragdollAnimator.Limbs[0].SelfRigidbody.rotation * hipsUp;
                Vector3 normal = ragdollAnimator.AnimatedTargetRoot.up;
                
                Vector3.OrthoNormalize(ref normal, ref spineDirection);
                
                //Basically snap the rotation of the ragdoll to either entirely on its back, or entirely on its stomach. Simplifies the amount of getup animations we have to make
                RotateRagdoll(Quaternion.LookRotation(spineDirection, ragdollAnimator.AnimatedTargetRoot.up));

                //Sample mapped state here
                ragdollAnimator.SampleTargetMappedState();

                Vector3 gettingUpOffset = isOnBack ? getUpOffsetBack : getUpOffsetStomach;
                MoveRagdoll(ragdollAnimator.Limbs[0].SelfRigidbody.position + ragdollAnimator.AnimatedTargetRoot.rotation * gettingUpOffset);
                GroundRagdoll(groundLayers);

                standUpPosition = ragdollAnimator.AnimatedTargetRoot.position;

                getupAnimationBlendWeight = 1;
                getUpTargetFixed = false;

                break;
        }

        //Apply the new state after all that fancy code performing checks
        currentState = newState;
    }


    #region Helper Methods
    public void SetAllCollidersState(bool YN)
    {
        foreach (RagdollLimb Limb in ragdollAnimator.Limbs)
        {
            //SetCollidersState(Limb, YN);
        }
    }
    public void SetCollidersState(RagdollLimb currentLimb, bool unpinned)
    {
        if (unpinned)
        {
            foreach (Collider c in currentLimb.limbColliders)
            {
                c.enabled = true;
            }
        }
        else
        {
            foreach (Collider c in currentLimb.limbColliders)
            {
                // Disable colliders
                Vector3 inertiaTensor = currentLimb.SelfRigidbody.inertiaTensor;
                c.enabled = false;
                currentLimb.SelfRigidbody.inertiaTensor = inertiaTensor;
            }
        }
    }
    private bool IsLayingOnChest()
    {
        //Do some vector math to check if the hips are pointing upwards or downwards
        //If the value is less than 0 then they are pointing up
        float dotProd = Vector3.Dot(ragdollAnimator.Limbs[0].transform.rotation * hipsForward, ragdollAnimator.AnimatedTargetRoot.up);
        return dotProd < 0f;
    }

    private void RotateRagdoll(Quaternion rotation)
    {
        ragdollAnimator.AnimatedTargetRoot.rotation = rotation;
    }

    private void MoveRagdoll(Vector3 position)
    {
        ragdollAnimator.AnimatedTargetRoot.position = position;
    }

    protected virtual void GroundRagdoll(LayerMask layers)
    {
        Ray ray = new Ray(ragdollAnimator.AnimatedTargetRoot.position + ragdollAnimator.AnimatedTargetRoot.up, -ragdollAnimator.AnimatedTargetRoot.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 4f, layers))
        {
            if (!float.IsNaN(hit.point.x) && !float.IsNaN(hit.point.y) && !float.IsNaN(hit.point.z))
            {
                ragdollAnimator.AnimatedTargetRoot.position = hit.point;
            }
            else
            {
                Debug.LogWarning("Raycasting against a large collider has produced a NaN hit point.", transform);
            }
        }
    }

    //I just manually coded in one of them since I didn't want to do all potential interpolation methods. There is a LOT
    private float InterpolateFloat(float time)
    {
        //This is an interpolation formula called InOutCubic, or maybe not. Cant remember for sure or care to fully explain EVERYTHING HERE
        float timeSquared = time * time;
        float timeCubed = timeSquared * time;
        return 1 * (-2 * timeCubed + 3 * timeSquared);
    }

    private bool LayerContains(LayerMask layerMask, int layer)
    {
        //If you dont know what this is doing look into bit shifting and how layerm masks really work
        return layerMask == (layerMask | (1 << layer));
    } 
    #endregion

    //This is a struct. It's basically a blueprint for a custom event that the ragdoll will fire
    [Serializable]
    public struct RagdollEvent
    {
        [Tooltip("Animations to cross-fade to on this event. This is separate from the UnityEvent below because UnityEvents can't handle calls with more than one parameter such as Animator.CrossFade.")]
        /// <summary>
        /// Animations to cross-fade to on this event. This is separate from the UnityEvent below because UnityEvents can't handle calls with more than one parameter such as Animator.CrossFade.
        /// </summary>
        public AnimatorEvent[] animationEvents;

        //This is the actual event invoked to fire this whole event
        public UnityEvent standardEvent;

        public void Trigger(RagdollAnimatorV2 ragdollAnimator)
        {
            //Invoke the standard unity event we have set, then call all the animation events
            standardEvent?.Invoke();
            foreach (AnimatorEvent animatorEvent in animationEvents) 
                animatorEvent.Activate(ragdollAnimator.TargetAnimator);
        }
    }
}
/// <summary>
/// Cross-fades to an animation state. UnityEvent can not be used for cross-fading, it requires multiple parameters.
/// </summary>
[Serializable]
public class AnimatorEvent
{
    /// <summary>
    /// The name of the animation state
    /// </summary>
    public string animationState;
    /// <summary>
    /// The crossfading time
    /// </summary>
    public float crossfadeTime = 0.3f;
    /// <summary>
    /// The layer of the animation state (if using Legacy, the animation state will be forced to this layer)
    /// </summary>
    public int layer;
    /// <summary>
    ///  Should the animation always start from 0 normalized time?
    /// </summary>
    public bool resetNormalizedTime;

    private const string empty = "";

    // Activate a Mecanim animation
    public void Activate(Animator animator)
    {
        if (animationState == empty) return;

        if (resetNormalizedTime)
        {
            if (crossfadeTime > 0f) animator.CrossFadeInFixedTime(animationState, crossfadeTime, layer, 0f);
            else animator.Play(animationState, layer, 0f);
        }
        else
        {
            if (crossfadeTime > 0f)
            {
                animator.CrossFadeInFixedTime(animationState, crossfadeTime, layer);
            }
            else animator.Play(animationState, layer);
        }
    }

}
