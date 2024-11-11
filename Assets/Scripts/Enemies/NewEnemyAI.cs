using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AgeOfEnlightenment.Enemies
{
    [RequireComponent(typeof(Rigidbody))]
    public class NewEnemyAI : MonoBehaviour, IDamageable
    {
        //Enemy stats
        [Header("Stats")]
        [SerializeField] public float Health = 10;
        [SerializeField] public float MovementSpeed = 3;
        [SerializeField] public float RotationSpeed = 5;
        [SerializeField] public float VisionRange;
        [SerializeField] public float AttackRange;
        [SerializeField] public float GroundedCheckDistance = 0.1f;

        [Header("Lots of Settings")]
        public bool CanFollowPlayer;
        public bool CanFollowBreadcrumbs;
        public bool CanFollowOtherAI;
        public bool AvoidEdges;
        public float EdgeAvoidanceRange = 0.4f;
        public bool AvoidWalls;
        public float WallAvoidanceRange = 1;
        public bool CanWander = true;
        public bool NoWanderRange;
        public float WanderTimeLimit;
        public float TimeBetweenWander;
        public float WanderRange = 15;
        public bool CanFlee;
        public bool HasVision;
        public bool OnlyFrontVision;
        [Range(0, 1)]
        public float FrontVisionRange = 0.2f;



        [HideInInspector] public LayerMask PlayerLayer;
        [HideInInspector] public LayerMask EnemyLayer;

        [HideInInspector] public LeaveBreadcrumbs PlayerBreadcrumbs;

        //Object targets for tracking
        [HideInInspector]
        public Transform _Player,
                          _PreviousPlayer,
                          _TargetBreadcrumb,
                          _EdgeStart,
                          _TargetAI;
        public enum LIFE_STATE
        {
            Alive, //Living, self explanatory. Performs all actions like normal
            Dead, //Health has reached 0, no longer does anything and will despawn after a while
            Dazed //Currently stunned in some way, is not dead, but will not perform actions
        }
        public LIFE_STATE lifeState = LIFE_STATE.Alive;

        public enum MOVEMENT_STATE
        {
            Idle, //Doing nothing
            FollowingPlayer, //Currently following a player
            FollowingBreadcrumb, //Was following a player, but is now following their breadcrumbs
            FollowingAI, //Following anotherAI that can see the player
            Wandering //Wandering to random points
        }
        public MOVEMENT_STATE moveState = MOVEMENT_STATE.Idle;

        public enum VISION_STATE
        {
            CanSeeNothing,
            CanSeePlayer, 
            CanSeeBreadcrumb,
            CanSeeAI
        }
        public VISION_STATE visionState = VISION_STATE.CanSeeNothing;

        private bool _HasNextWanderPos, _CurrentlyWandering;
        

        private RaycastHit hit;
        private Vector3 currentWanderTarget, nextWanderPos;
        private float wanderTimer, wanderCooldown;

        private Collider _collider;
        private Rigidbody _rigidbody;
        

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            this.AI_Layer_Setup();
            this.General_AI_Setup();
        }

        private void Update()
        {
            LifeState_Update();
        }
        private void FixedUpdate()
        {
            Controller_Update();
            WallAvoidance();
        }

        private void LifeState_Update()
        {
            if(Health <= 0)
            {
                lifeState = LIFE_STATE.Dead;
            }
        }

        private void Controller_Update()
        {
            if (lifeState == LIFE_STATE.Dead)
                return;
            //If we see the player and can follow them, then start chasing them
            if (CanFollowPlayer && this.FindPlayer())
            {
                _HasNextWanderPos = false;
                visionState = VISION_STATE.CanSeePlayer;

                float distanceToTarget = Vector3.Distance(transform.position, _Player.position);
                //CHECK IF ITS MELEE OR RANGED ATTACK
                //MAYBE MAKE THEM SEPERATE THINGS AND USE MODULAR ATTACKS?

                //If we are outside the attack range of the AI then that means we need to get closer
                if(distanceToTarget > AttackRange)
                {

                    moveState = MOVEMENT_STATE.FollowingPlayer;
                    Movement(_Player.position);
                }
                else
                {
                    moveState = MOVEMENT_STATE.Idle;
                    Debug.Log("RAWR I AM ATTACKING");
                    Rotation(_Player.position);
                }
            }
            else if (CanFollowBreadcrumbs && _PreviousPlayer && this.FindBreadcrumb())
            {
                //If we have a breadcrumb target, then we are not wandering
                _HasNextWanderPos = false;
                moveState = MOVEMENT_STATE.FollowingBreadcrumb;
                //SET ATTACK STATE TO NO HERE
                Movement(_TargetBreadcrumb.position);
            }
            else if (CanFollowOtherAI && this.FindOtherAI())
            {
                Debug.Log("Follwongi my frineds");
                _HasNextWanderPos = false;
                visionState = VISION_STATE.CanSeeAI;
                moveState = MOVEMENT_STATE.FollowingAI;
                //SET ATTACK TO NO HERE
                Movement(_TargetAI.position);
            }
            else if(CanWander)
            {
                visionState = VISION_STATE.CanSeeNothing;
                //SET ATTACK STATE TO NO HERE

                //So the main change I am doing here is that I am making Wandering the DefaultState of the AI. The one I was basing this off of would limit the amount of time
                //the ai could wander, and reset itself after that time. I wont be doing that here, just making it wander until it sees something to follow
                //The time limit could have been added as a fail safe in case an AI got stuck in a loop somehow, but I will keep that in mind as I expand
                WanderAround();
            }
        }

        //private bool IsGrounded()
        //{
        //    if (_collider)
        //    {
        //        return Physics.Raycast(transform.position, -transform.up, _collider.bounds.extents.y + GroundedCheckDistance);
        //    }
        //    else
        //    {
        //        return true;
        //    }
        //}

        private void Movement(Vector3 targetPosition)
        {
            //Could do jumping things here. Before just allowing it to run

            if (EdgeAvoidance())
            {
                _rigidbody.MovePosition(_rigidbody.position + transform.forward * Time.deltaTime * MovementSpeed);
            }
            else
            {
                //Do some sort of long jump here?
            }
            Rotation(targetPosition);
        }

        private void Rotation(Vector3 targetRotatedPosition)
        {
            var playerPos = new Vector3(targetRotatedPosition.x, transform.position.y, targetRotatedPosition.z);
            //Do an if check for if its in the air or on the ground here, since rotating would be different in either case
            _rigidbody.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerPos - transform.position), RotationSpeed));
        }
        private bool EdgeAvoidance()
        {
            if (AvoidEdges)
            {
                //Cast a basic ray downwards from the ai at the edge of its radius, if it hits something then there is still ground there. 
                //If it was false then the other code would stop executing and the ai would go another way
                return Physics.Raycast(_EdgeStart.position, _EdgeStart.position + -_EdgeStart.up * EdgeAvoidanceRange);
            }
            else
            {
                return true;
            }
        }
        private void WallAvoidance()
        {
            if (AvoidWalls)
            {
                if(Physics.BoxCast(transform.position, new Vector3(WallAvoidanceRange, 0.01f, WallAvoidanceRange), transform.up, out hit,transform.rotation, .01f, ~(PlayerLayer | EnemyLayer)))
                {
                    _rigidbody.AddForce((hit.point - transform.position).normalized, ForceMode.Acceleration);
                }
            }
        }
        private void WanderAround()
        {
            //wanderTimer += Time.deltaTime;
            //if(wanderTimer >= WanderTimeLimit)
            //{
            //    _CurrentlyWandering = false;
            //} 
            //else
            //{
            //    _CurrentlyWandering = true;
            //}

            //Checks if it can wander anywhere. Gives us a default position to calculate off of either way
            if(!NoWanderRange)
            {
                currentWanderTarget = transform.position;
            } else {
                if (!_HasNextWanderPos)
                {
                    currentWanderTarget = transform.position;
                    _HasNextWanderPos = true;
                }
            }

            //Here we check if enough time has passed to set our next wanderPosition
            if(Time.time > wanderCooldown)
            {
                wanderCooldown = Time.time + TimeBetweenWander;
                float wanderX = Random.Range(currentWanderTarget.x - WanderRange, currentWanderTarget.x + WanderRange);
                float wanderZ = Random.Range(currentWanderTarget.z - WanderRange, currentWanderTarget.z + WanderRange);

                RaycastHit newHit;

                nextWanderPos = new Vector3(wanderX, currentWanderTarget.y, wanderZ);

                if(Physics.Linecast(transform.position, nextWanderPos, out newHit, ~(PlayerLayer | EnemyLayer)))
                {
                    var BabyVectorYAY = newHit.point - (newHit.point - transform.position).normalized * Mathf.Max(_collider.bounds.size.x, _collider.bounds.size.z);

                    nextWanderPos = new Vector3(BabyVectorYAY.x, transform.position.y, BabyVectorYAY.z);
                }
            }


            Debug.DrawLine(transform.position, nextWanderPos, Color.red);


            //If we havent already reached our next wander position, keep moving towards it
            if(Vector3.Distance(transform.position, nextWanderPos) > 0.5f)
            {
                Movement(nextWanderPos);
                moveState = MOVEMENT_STATE.Wandering;
            } else {
                moveState = MOVEMENT_STATE.Idle;
            }
        }
        public void TakeDamage(float DamageAmount, DamageType elementalDamage)
        {
            Health -= DamageAmount;
        }
    } 
}
