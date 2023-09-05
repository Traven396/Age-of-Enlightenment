
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAi : MonoBehaviour, IEntity, IDamageable
{
    public LayerMask whatIsGround, whatIsPlayer;
    public Slider healthSlider;

    private Rigidbody rb;
    private NavMeshAgent agent;
    private Transform player;
    private AudioSource source;

    //Patroling
    [Header("Patrol Stats")]
    private Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    [Header("Attack Stats")]
    public float timeBetweenAttacks;
    private bool alreadyAttacked;
    public int damage = 10;
    public float projectileSpeed;
    public GameObject projectile;
    public LayerMask affectedLayers;

    //States
    [Header("Stats")]
    public float sightRange, attackRange;
    private bool playerInSightRange, playerInAttackRange, isFalling = false, isDead = false, alreadyHitRecently = false;
    public float MaxHealth = 10;
    private float Health;
    public float minimumVelocityThreshold = .3f;
    public int chanceOfSound = 3;
    public float maxTimeBetweenAmbientSounds = 24f;
    public float minTimeBetweenAmbientSounds = 8f;

    private float currentTime;
    private float timer = 0;

    [Header("Helpful Toggle")]
    public bool Freeze = false;

    private LayerMask enemyLayer;
    private void Awake()
    {
        player = Player.Instance.transform.GetChild(0).transform;
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        enemyLayer = LayerMask.NameToLayer("EnemyProjectile");
        source = GetComponentInChildren<AudioSource>();

        Health = MaxHealth;

        UpdateSlider();

        currentTime = Random.Range(minTimeBetweenAmbientSounds, maxTimeBetweenAmbientSounds);
    }

    private void UpdateSlider()
    {
        healthSlider.value = Health / MaxHealth;
        
    }

    private void Update()
    {
        if (!Freeze)
        {
            if (!isDead)
            {
                HandleAmbientSound();
                if (!isFalling)
                {
                    //Check for sight and attack range
                    playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
                    playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

                    healthSlider.transform.LookAt(player);

                    if (!playerInSightRange && !playerInAttackRange) Patroling();
                    if (playerInSightRange && !playerInAttackRange) ChasePlayer();
                    if (playerInAttackRange && playerInSightRange) AttackPlayer();

                }
            } 
        }
    }

    private void HandleAmbientSound()
    {
        timer += Time.deltaTime;
        if(timer >= currentTime)
        {
            if (!source.isPlaying)
            {
                source.clip = EnemyManager.Instance.GetRandomSoundEffect();
                source.Play();
            }
            timer = 0;
            currentTime = Random.Range(minTimeBetweenAmbientSounds, maxTimeBetweenAmbientSounds);
        }
    }

    #region AI Stuff
    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        transform.LookAt(player, Vector3.up);

        if (!alreadyAttacked)
        {
            ///Attack code here
            ProjectileBehavior pb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<ProjectileBehavior>();

            pb.gameObject.layer = enemyLayer;
            foreach (Transform child in pb.gameObject.transform)
            {
                child.gameObject.layer = enemyLayer;
            }
            
            pb.explosionLayers = affectedLayers;

            pb.nonCollidableLayers = affectedLayers;

            pb.damageAmount = damage;

            Vector3 shootVector = (Camera.main.transform.position - transform.position).normalized;
            //shootVector.y += 0.5f;
            shootVector *= projectileSpeed;
            pb.Shoot(shootVector);
            ///End of attack code

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
    #endregion

    #region Knockback Stuff
    public void ApplyMotion(Vector3 force, ForceMode forceMode)
    {
        StopAllCoroutines();
        StartCoroutine(ForceApplied(force, forceMode));
    }

    IEnumerator ForceApplied(Vector3 forceI, ForceMode forceMode)
    {
        rb.isKinematic = false;

        agent.enabled = false;

        isFalling = true;


        rb.AddForce(forceI, forceMode);

        yield return new WaitForSeconds(.1f);
        yield return new WaitUntil(IsRigidBodyNotMoving);

        rb.isKinematic = true;
        agent.enabled = true;

        agent.ResetPath();

        isFalling = false;

        yield break;
    }

    bool IsRigidBodyNotMoving()
    {
        if (rb.velocity.magnitude >= .1f)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    #endregion

    #region Some of that Dark Sheet
    void Die()
    {
        isDead = true;
        agent.enabled = false;

        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.None;

        rb.AddForce(Vector3.up * .1f, ForceMode.VelocityChange);
        Destroy(gameObject, 3);
    }
    public void TakeDamage(float damage, DamageType elementalDamage)
    {
        if (!isDead)
        {
            Health -= damage;

            PlayHurtSound();

            UpdateSlider();

            if (Health <= 0)
            {
                Die();
            }   
        }
    }

    private void PlayHurtSound()
    {
        if (!source.isPlaying)
        {
            var roll = Random.Range(1, chanceOfSound);
            if(roll == 1)
            {
                source.clip = EnemyManager.Instance.GetRandomHurtSound();
                source.Play();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!alreadyAttacked)
        {
            if (!collision.gameObject.TryGetComponent(out ProjectileBehavior pb))
            {
                float damageToTake = collision.relativeVelocity.magnitude;
                if (collision.rigidbody != null)
                {
                    damageToTake *= collision.rigidbody.mass;
                }
                else
                {
                    damageToTake *= rb.mass;
                }

                if (collision.relativeVelocity.magnitude >= minimumVelocityThreshold)
                {
                    TakeDamage(damageToTake, DamageType.Force);
                    alreadyAttacked = true;
                    StartCoroutine(ImpactCooldown());
                } 
            }
        }
    }

    IEnumerator ImpactCooldown()
    {
        yield return new WaitForSeconds(.1f);
        alreadyAttacked = false;
    }
    #endregion
}
