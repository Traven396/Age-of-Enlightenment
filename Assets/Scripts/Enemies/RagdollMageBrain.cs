namespace AgeOfEnlightenment.Enemies
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.AI;

    public class RagdollMageBrain : MonoBehaviour
    {
        public Enemy_Life_State CurrentLifeState;
        public Enemy_Vision_State CurrentVisionState;
        public Enemy_Movement_State CurrentMovementState;
        public Enemy_Combat_State CurrentCombatState;
        public BaseAttackBehavior.EnemyRange CurrentRange;
        [SerializeField] private float MovementSpeed = 4f;
        [SerializeField] private float TurnSpeed = 4f;
        [SerializeField] private float MaxHealth = 10;
        [SerializeField] private float CurrentHealth;
        [SerializeField] private float WanderRange = 20f;
        [SerializeField] private float VisionRange = 13f;
        [SerializeField] private float MiddleAttackRange = 6;
        [SerializeField] private bool CanWander = false;
        [SerializeField] private bool UseSight = true;
        [SerializeField] private bool StayAroundStartingPoint = false;

        [Space(10)]
        [SerializeField] private Transform EyeTransform;

        [Header("Attack Options")]
        public List<BaseAttackBehavior> ListOfAttacks = new(4);

        [Header("Debug Options")]
        public bool DrawVisionLines = false;
        public bool DisplayMovementTarget = false;
        public bool DisplayPreferredRanges = false;

        public LayerMask EnvironmentLayers, PlayerLayers, EnemyLayers;


        //The setup will be a NavMeshAgent that is the actual object causing movement to the points
        //The navmesh agent will just wander around until it sees the player within range.
        //The Animator object is tied to the NavMeshAgent and moves with it
        //The animator will change animations depending on what state the other 2 are in. From walking to attacking
        //The Ragdoll follows after the animator and applies physics
        //If the ragdoll is "knocked out" it will stop the navmesh agent temporarily
        private RagdollAnimatorV2 _RagdollAnimator;
        private Animator _Animator;
        private NavMeshAgent _Agent;

        private AnimatorOverrideController constructedAnimatorController;
        private bool isAttacking = false;
        private float minPreferredRange;
        private float maxPreferredRange;


        private Transform _PlayerTransform;


        private bool haveWanderTarget = false; //A boolean that is whether or not the AI has a wander position somewhere already
        //private Vector3 currentWanderTarget = Vector3.zero; //A position somewhere on the map that the AI is going to wander to next.
        private float nextWanderCooldown;


        private RagdollMageBrain TargetAI;
        private Vector3 lastSeenPlayerLocation;

        private bool isGrounded;
        private Vector3 CurrentMovementTarget;

        private void Awake()
        {
            CurrentHealth = MaxHealth;

            _Agent = GetComponent<NavMeshAgent>();
            _Agent.speed = MovementSpeed;



            _RagdollAnimator = transform.parent.GetComponentInChildren<RagdollAnimatorV2>();
            if (!_RagdollAnimator)
                Debug.Log("We dont have a ragdoll animator to refer to.");


            _Animator = GetComponent<Animator>();

            _PlayerTransform = FindObjectOfType<PlayerSingleton>().transform;

            if (ListOfAttacks.Count == 0)
                Debug.Log("This enemy has no attacks");


            
        }

        private void Start()
        {
            CreateMageAttackBrain();
        }

        private void CreateMageAttackBrain()
        {
            //We make a copy of the current controller but one that we can change during runtime. This way we can add the correct animations to each of the spell states
            constructedAnimatorController = new AnimatorOverrideController(_Animator.runtimeAnimatorController);
            constructedAnimatorController.name = "Override Controller";
            _Animator.runtimeAnimatorController = constructedAnimatorController;


            int longRangeCounter = 0, mediumRangeCounter = 0, closeRangeCounter = 0;

            foreach (BaseAttackBehavior currentAttack in ListOfAttacks)
            {
                if (currentAttack.AttackRange == BaseAttackBehavior.EnemyRange.Long)
                    longRangeCounter++;
                if (currentAttack.AttackRange == BaseAttackBehavior.EnemyRange.Medium)
                    mediumRangeCounter++;
                if (currentAttack.AttackRange == BaseAttackBehavior.EnemyRange.Close)
                    closeRangeCounter++;

                var spellCount = longRangeCounter + mediumRangeCounter + closeRangeCounter;

                if (currentAttack.AttackAnimation)
                {
                    
                    constructedAnimatorController["Spell " + spellCount + " Default Animation"] = currentAttack.AttackAnimation;

                    AnimationEvent tempEvent = new();

                    tempEvent.time = currentAttack.AttackAnimation.length;
                    tempEvent.functionName = "Spell" + spellCount + "Finished";

                    constructedAnimatorController["Spell " + spellCount + " Default Animation"].AddEvent(tempEvent);
                    
                }
            }

            if(longRangeCounter > mediumRangeCounter && longRangeCounter > closeRangeCounter)
            {
                CurrentRange = BaseAttackBehavior.EnemyRange.Long;

                maxPreferredRange = MiddleAttackRange * 2;
                minPreferredRange = MiddleAttackRange * 1.5f;
            }
            else if (mediumRangeCounter > longRangeCounter && mediumRangeCounter > closeRangeCounter)
            {
                CurrentRange = BaseAttackBehavior.EnemyRange.Medium;

                maxPreferredRange = MiddleAttackRange * 1.4f;
                minPreferredRange = MiddleAttackRange * 0.9f;
            }
            else if(closeRangeCounter > longRangeCounter && closeRangeCounter > mediumRangeCounter)
            {
                CurrentRange = BaseAttackBehavior.EnemyRange.Close;

                maxPreferredRange = 3;
                minPreferredRange = 1f;
            }
            else
            {
                CurrentRange = BaseAttackBehavior.EnemyRange.Medium;
                Debug.Log("We had a tie for an enemy. This should be fixed somehow but what are ya gonna do for random stuff");
            }

            if (VisionRange < maxPreferredRange)
                VisionRange = maxPreferredRange;
        }

        #region Spell finish methods
        public void Spell1Finished()
        {
            _Animator.SetTrigger("FinishSpell1");
            isAttacking = false;
        }
        public void Spell2Finished()
        {
            _Animator.SetTrigger("FinishSpell2");
            isAttacking = false;
        }
        public void Spell3Finished()
        {
            _Animator.SetTrigger("FinishSpell3");
            isAttacking = false;
        }
        public void Spell4Finished()
        {
            _Animator.SetTrigger("FinishSpell4");

             isAttacking = false;
        }
        #endregion
        private void Update()
        {
            EnemyControllerUpdate();

            IncrementTimers();

            AnimationUpdate();
        }

        private void AnimationUpdate()
        {
            if(CurrentMovementState == Enemy_Movement_State.Standing || CurrentMovementState == Enemy_Movement_State.AttackingPlayer)
            {
                _Animator.SetBool("Moving", false);
            }
            else
            {
                _Animator.SetBool("Moving", true);
            }
        }

        private void OnDrawGizmos()
        {
            if (DisplayMovementTarget)
                Gizmos.DrawSphere(CurrentMovementTarget, .3f);

            if (DisplayPreferredRanges && Application.isPlaying)
            {
                Gizmos.DrawWireSphere(_PlayerTransform.position, minPreferredRange);
                Gizmos.DrawWireSphere(_PlayerTransform.position, maxPreferredRange);
            }
        }

        private void IncrementTimers()
        {
            if(nextWanderCooldown > 0)
            {
                nextWanderCooldown -= Time.deltaTime;
            }
        }

        #region StateMethods
        private void UpdateEnemyState()
        {
            //Check if we have died
            DetermineLifeState();

            if (CurrentLifeState == Enemy_Life_State.Dead)
                return;

            //Check if we have fallen over
            DetermineBalanceState();

            //What can we see
            DetermineVisionState();

            //How are we moving
            DetermineMovementState();

            //Are we in a state where we can try and attack the player
            DetermineCombatState();
        }

        private void DetermineCombatState()
        {
            if (CurrentVisionState != Enemy_Vision_State.CanSeePlayer)
            {
                CurrentCombatState = Enemy_Combat_State.NotCombatReady;

                Debug.Log("Cant see the player so no combat");

                return;
            }
            if(CurrentLifeState == Enemy_Life_State.Ragdoll)
            {
                CurrentCombatState = Enemy_Combat_State.NotCombatReady;

                Debug.Log("We are ragdoll so no combat");

                return;
            }

            if(CurrentMovementState == Enemy_Movement_State.Wandering ||
                CurrentMovementState == Enemy_Movement_State.Falling ||
                CurrentMovementState == Enemy_Movement_State.ChasingOtherAI ||
                CurrentMovementState == Enemy_Movement_State.ChasingLastPlayerLocation ||
                CurrentMovementState == Enemy_Movement_State.FallenOver)
            {
                CurrentCombatState = Enemy_Combat_State.NotCombatReady;

                Debug.Log("We arent moving near the player so no combat");

                return;
            }

            CurrentCombatState = Enemy_Combat_State.CombatReady;

        }

        private void DetermineMovementState()
        {
            var distToPlayer = Vector3.Distance(transform.position, _PlayerTransform.position);

            //Are we falling over, or already fallen over

            //I need to go through the code for the active ragdolls and see if I can figure out a way to detect loss of balance. Maybe through an event


            //Are we on the ground
            isGrounded = Physics.Raycast(transform.position, -transform.up, 3, EnvironmentLayers);

            //We only move if we are on the ground
            if (isGrounded)
            {
                if(MovementSpeed > 0)
                {
                    if (CurrentVisionState == Enemy_Vision_State.CanSeePlayer) 
                    {
                        if (distToPlayer >= maxPreferredRange)
                            CurrentMovementState = Enemy_Movement_State.ChasingPlayer;
                        else if (distToPlayer <= minPreferredRange)
                            CurrentMovementState = Enemy_Movement_State.Retreating;
                        else
                        {
                            CurrentMovementState = Enemy_Movement_State.Standing;


                            Vector3 lookPos = _PlayerTransform.position - _Agent.transform.position;
                            
                            lookPos.y = 0;

                            Quaternion rotation = Quaternion.LookRotation(lookPos);
                            
                            _Agent.transform.rotation = Quaternion.Slerp(_Agent.transform.rotation, rotation, TurnSpeed);
                        }
                        return;
                    }

                    if(CurrentVisionState == Enemy_Vision_State.CanSeeAI)
                    {
                        CurrentMovementState = Enemy_Movement_State.ChasingOtherAI;


                        return;
                    }

                    if(CurrentVisionState == Enemy_Vision_State.CanSeeNothing)
                    {
                        if(lastSeenPlayerLocation != Vector3.zero)
                        {
                            CurrentMovementState = Enemy_Movement_State.ChasingLastPlayerLocation;
                            return;
                        }

                        if(nextWanderCooldown > 0 || !CanWander)
                        {
                            CurrentMovementState = Enemy_Movement_State.Standing;
                            return;
                        }

                        if (CanWander)
                        {
                            CurrentMovementState = Enemy_Movement_State.Wandering;

                            return;
                        }


                    }
                }
            }
            else
            {
                CurrentMovementState = Enemy_Movement_State.Falling;
            }
            //Are we following some other object
                //The player
                //The last known player location
                //Another AI that can see the Player

            //If we reach close enough to the last known location of the player set it to null and wait there for a few seconds

            //We are wandering around, finding a new locaiton

            //Just standing around and not moving
        }

        #region Vision Methods
        private void DetermineVisionState()
        {
            //If we are close enough and can see the player, we see them
            CheckForPlayer();

            if (CurrentVisionState == Enemy_Vision_State.CanSeePlayer)
                return;

            //If we cant see the player but can see another AI that can see the player, follow them
            CheckForOtherAi();

            if (CurrentVisionState == Enemy_Vision_State.CanSeeAI)
                return;
            //If we previously saw the player, but lost them, travel to where they last were seen

            CurrentVisionState = Enemy_Vision_State.CanSeeNothing;
        }

        private void CheckForOtherAi()
        {
            //Are we already following an AI?
            //If we are can we still see them?
            if (TargetAI)
            {
                if (Physics.Linecast(EyeTransform.position, TargetAI.transform.position, ~EnemyLayers))
                {
                    //If we hit something thats not an enemy, then we have lost sight of the otehr AI
                    CurrentVisionState = Enemy_Vision_State.CanSeeNothing;
                    TargetAI = null;
                }
                else
                {
                    //Is the AI we are tracking still able to see the player too. Or are they chasing the last seen player location
                    if (TargetAI.CurrentVisionState == Enemy_Vision_State.CanSeePlayer || TargetAI.CurrentMovementState == Enemy_Movement_State.ChasingLastPlayerLocation)
                    {
                        //Every check is passed. We continue to follow the AI
                        CurrentVisionState = Enemy_Vision_State.CanSeeAI;

                        if (DrawVisionLines)
                            Debug.DrawLine(EyeTransform.position, TargetAI.transform.position, Color.yellow);
                    }
                    else
                    {
                        CurrentVisionState = Enemy_Vision_State.CanSeeNothing;
                        TargetAI = null;
                    }
                }
            }
            else
            {
                //Is there any other AI close enough to us
                //Do the close AI see the player
                Collider[] nearbyEnemies = Physics.OverlapSphere(_Agent.transform.position, VisionRange, EnemyLayers, QueryTriggerInteraction.Collide);

                if (nearbyEnemies.Length != 0)
                {
                    foreach (Collider collider in nearbyEnemies)
                    {
                        if (collider.TryGetComponent(out RagdollMageBrain currentEnemy))
                        {
                            if (currentEnemy != this)
                            {
                                if (currentEnemy.CurrentVisionState == Enemy_Vision_State.CanSeePlayer || currentEnemy.CurrentMovementState == Enemy_Movement_State.ChasingLastPlayerLocation)
                                {
                                    TargetAI = currentEnemy;

                                    CurrentVisionState = Enemy_Vision_State.CanSeeAI;
                                } 
                            }
                        }
                    }
                }
            }
        }

        private void CheckForPlayer()
        {
            if (_PlayerTransform)
            {
                if (Vector3.Distance(_Agent.transform.position, _PlayerTransform.position) <= VisionRange)
                {
                    //Do we use sight to find the player
                    if (UseSight)
                    {
                        if (Physics.Linecast(EyeTransform.position, _PlayerTransform.position, ~(PlayerLayers | EnemyLayers)))
                        {

                            if (CurrentVisionState == Enemy_Vision_State.CanSeePlayer)
                            {
                                if (DrawVisionLines)
                                    Debug.DrawLine(EyeTransform.position, _PlayerTransform.position, Color.red, 10);
                            }


                            //If something is in the way of the enemy's eyes and the player, the player is not visible.
                            //If we previously saw the player, then we just lost sight and should travel to the last seen location
                            CurrentVisionState = Enemy_Vision_State.CanSeeNothing;
                        }
                        else
                        {
                            //If we dont hit anything, we can see the player and should transition to that state
                            CurrentVisionState = Enemy_Vision_State.CanSeePlayer;
                            lastSeenPlayerLocation = _PlayerTransform.position;

                            if (DrawVisionLines)
                                Debug.DrawLine(EyeTransform.position, _PlayerTransform.position, Color.blue);
                        }
                    }
                    else
                    {
                        //If we dont use sight, just check some kind of search range for the player.
                        //If they are within range we see them and should begin chasing
                        CurrentVisionState = Enemy_Vision_State.CanSeePlayer;
                        lastSeenPlayerLocation = _PlayerTransform.position;
                    }
                }
                else
                {
                    if (CurrentVisionState == Enemy_Vision_State.CanSeePlayer)
                    {
                        CurrentVisionState = Enemy_Vision_State.CanSeeNothing;

                        if (DrawVisionLines)
                            Debug.DrawLine(EyeTransform.position, _PlayerTransform.position, Color.green, 10);
                    }
                }

            }
        } 
        #endregion

        private void DetermineBalanceState()
        {

            //Is the ragdoll following the animator? We are balanced, or standing up

            //Is the ragdoll no longer following? We have fallen over, full priority is in getting up
        }

        private void DetermineLifeState()
        {
            //Is our health at 0? We are dead
            if (CurrentHealth <= 0)
            {
                CurrentLifeState = Enemy_Life_State.Dead;
            }

            //Maybe could add some kind of wounded mode
        } 
        #endregion

        private void EnemyControllerUpdate()
        {

            //If the Enemy is alive we need to do all the regular checks.
            //These state changes will determine which behavior the enemy does afterwards
            UpdateEnemyState();
            
            switch (CurrentLifeState)
            {
                case Enemy_Life_State.Alive:

                    

                    //We should wander if we dont see the player, dont see an AI that can see the player, and we arent traveling to where we last saw the player
                    if(CurrentMovementState == Enemy_Movement_State.ChasingPlayer)
                    {
                        ChasePlayer();
                    }

                    if(CurrentMovementState == Enemy_Movement_State.Retreating)
                    {
                        Retreat();
                    }

                    if(CurrentMovementState == Enemy_Movement_State.ChasingOtherAI)
                    {
                        ChaseOtherAI();
                    }
                    if(CurrentMovementState == Enemy_Movement_State.ChasingLastPlayerLocation)
                    {
                        ChaseLastSeenLocation();
                    }

                    if(CurrentMovementState == Enemy_Movement_State.AttackingPlayer)
                    {
                        if (!isAttacking)
                        {
                            var num = UnityEngine.Random.Range(2, 5);
                            Debug.Log(num);
                            switch (num)
                            {
                                case 1:
                                    _Animator.SetTrigger("CastSpell1");
                                    isAttacking = true;
                                    break;
                                case 2:
                                    _Animator.SetTrigger("CastSpell2");
                                    isAttacking = true;
                                    break;
                                case 3:
                                    _Animator.SetTrigger("CastSpell3");
                                    isAttacking = true;
                                    break;
                                case 4:
                                    _Animator.SetTrigger("CastSpell4");
                                    isAttacking = true;
                                    break;
                            } 
                        }
                        
                    }

                    if(CurrentMovementState == Enemy_Movement_State.Wandering)
                        Wander();

                    break;
                case Enemy_Life_State.Ragdoll:


                    //In ragdoll mode we tick down some kind knockout timer. Or at least check how long until the ragdoll gets back up

                    break;
                case Enemy_Life_State.Dead:

                    //We dont do anything when its dead except for maybe tick down a despawn timer
                    return;
            }

            //If we can see the player move to some kind of chasing mode

            //If we are close enough to the player then attack
        }

       
        #region Wandering Methods
        private void Wander()
        {
            Vector3 distanceToWalkPoint = _Agent.transform.position - CurrentMovementTarget;
            //If we are close enough to the current wander point we clear it and activate a little cooldown time
            if (distanceToWalkPoint.magnitude < 1f && haveWanderTarget)
            {
                haveWanderTarget = false;
                WaitAround();
            }


            if (nextWanderCooldown <= 0)
            {

                //If we dont already have a point we are traveling to find one
                if (!haveWanderTarget)
                    SearchForWanderTarget();

                //If we are not close to the wander point we continue moving towards it.
                if (haveWanderTarget)
                    _Agent.SetDestination(CurrentMovementTarget); 
            }



        }

        private void SearchForWanderTarget()
        {
            //First check if this AI can wander at all. Not every AI will be able to. Some are stationary
            if (CanWander)
            {
                //We generate some random values using our WanderRange variable

                float randomZ = UnityEngine.Random.Range(-WanderRange, WanderRange);
                float randomX = UnityEngine.Random.Range(-WanderRange, WanderRange);


                //We add those values to our current positon to get the potential target position
                int LoopCounter = 0;
                var validWalkPoint = false;
                while (!validWalkPoint)
                {
                    if (StayAroundStartingPoint)
                        CurrentMovementTarget = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
                    else
                        CurrentMovementTarget = new Vector3(_Agent.transform.position.x + randomX, _Agent.transform.position.y, _Agent.transform.position.z + randomZ);


                    //We find the nearest position of our random location on the NavMesh, this way the target position will always be reachable hopefully

                    if (NavMesh.SamplePosition(CurrentMovementTarget, out var NewHit, 5, NavMesh.AllAreas))
                    {
                        validWalkPoint = true;
                        CurrentMovementTarget = NewHit.position;
                    }
                        

                    //In case somehow we get into an infinte loop we break out. We would go right back in after, but this way at least I can see that we broke once
                    LoopCounter++;
                    if (LoopCounter >= 500)
                    {
                        Debug.Log("Infinite loop");
                        break;

                    }
                }

                haveWanderTarget = true;
            }
            


            //We can check to see if the wander position is somewhere we can see but thats not completely necessary I think
        }

        private void WaitAround()
        {
            //We make some random amount of time, and just kinda sit around before wandering again
            nextWanderCooldown = UnityEngine.Random.Range(3, 10);

        }
        #endregion
        #region Combat Movement 

        private void Retreat()
        {
            CurrentMovementTarget = (_Agent.transform.position - _PlayerTransform.position).normalized + _Agent.transform.position;

            _Agent.SetDestination(CurrentMovementTarget);
        }
        private void ChasePlayer()
        {
            CurrentMovementTarget = _PlayerTransform.position;

            //var distToPlayer = Vector3.Distance(transform.position, PlayerTransform.position);

            //if (distToPlayer >= MiddleAttackRange)
            //{
            //    _Agent.SetDestination(CurrentMovementTarget);
            //}
            //else
            //{
            //    _Agent.SetDestination(_Agent.transform.position);
            //    //The agent shouldnt move anymore and we transition to the AttackingPLayerState


            //    CurrentMovementState = Enemy_Movement_State.Standing;

            //}

            _Agent.SetDestination(CurrentMovementTarget);
        }
        private void ChaseOtherAI()
        {
            if (TargetAI)
            {
                CurrentMovementTarget = TargetAI.transform.position;

                _Agent.SetDestination(CurrentMovementTarget);
            }
        }
        private void ChaseLastSeenLocation()
        {
            CurrentMovementTarget = lastSeenPlayerLocation;
            if (lastSeenPlayerLocation != Vector3.zero)
            {
                _Agent.SetDestination(CurrentMovementTarget);

                if(_Agent.remainingDistance < 1)
                {
                    lastSeenPlayerLocation = Vector3.zero;
                    CurrentMovementTarget = Vector3.zero;

                    CurrentMovementState = Enemy_Movement_State.Standing;
                }
            }
        }
        #endregion
    }

    public enum Enemy_Life_State
    {
        Alive, //This mode is for when the AI is moving towards any point the NavmeshAgent decides on
        Ragdoll, //This is for when the enemy has been knocked over or is completely in ragdoll mode for any reason
        Dead //The enemy health has reached 0 and is going to fall over
    }
    public enum Enemy_Combat_State
    {
        CombatReady,
        NotCombatReady
    }
    public enum Enemy_Movement_State
    {
        Wandering,
        Standing,
        FallenOver,
        Falling,
        Retreating,
        ChasingPlayer,
        AttackingPlayer,
        ChasingLastPlayerLocation,
        ChasingOtherAI
    }
    public enum Enemy_Vision_State
    {
        CanSeeNothing,
        CanSeePlayer,
        CanSeeAI
    }
}