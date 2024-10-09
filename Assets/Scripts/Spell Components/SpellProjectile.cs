using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Rigidbody))]
public class SpellProjectile : MonoBehaviour
{
    [SerializeField] private float LifeTime = 3;
    [SerializeField] private float HitEffectScaler = 1;
    [SerializeField] private bool PointTowardsMotion = false;
    [SerializeField] private GameObject BodyParent;
    [SerializeField] private GameObject TrailParent;
    [Space(8)]
    [SerializeField] private bool DebugMode = false;

    private ObjectPool<SpellProjectile> _pool;
    private Rigidbody _rb;
    private ProjectileShooter _shooter;
    private LayerMask _nonCollidableLayers;
    private float _damageAmount;
    private DamageType _damageTyping;

    private ParticleSystem[] particleSystemTrails;
    private TrailRenderer[] trailRenderTrails;

    private float minimumExtent;
    private float partialExtent;
    private float sqrMinimumExtent;
    private Vector3 previousPosition;
    private Collider selfCollider;

    private bool alreadyDestroyed;
    private float particleTrailDurations;
    private float trailRenderDurations = 0;

    //Anything that should fire only once on the initial spawn should be put in here
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (TrailParent)
        {
            
            
            particleSystemTrails = TrailParent.GetComponentsInChildren<ParticleSystem>();

            if (particleSystemTrails.Length > 0)
                particleTrailDurations = particleSystemTrails.Max(ps => ps.main.duration + ps.main.startLifetime.constantMax); 
            
            trailRenderTrails = TrailParent.GetComponentsInChildren<TrailRenderer>();

            if (trailRenderTrails.Length > 0)
                trailRenderDurations = trailRenderTrails.Max(tr => tr.time); 
            
        }
        selfCollider = GetComponent<Collider>();
        if (!selfCollider)
        {
            selfCollider = GetComponentInChildren<Collider>();
        }

        

        minimumExtent = Mathf.Min(Mathf.Min(selfCollider.bounds.extents.x, selfCollider.bounds.extents.y), selfCollider.bounds.extents.z);
        partialExtent = minimumExtent * (1.0f - .1f);
        sqrMinimumExtent = minimumExtent * minimumExtent;
    }
    //Anything that needs to reset, or be redone whenever the projectile is cycled gets put in here
    private void OnEnable()
    {
        BodyParent.SetActive(true);

        if (TrailParent)
        {
            if(particleSystemTrails.Length > 0)
                particleSystemTrails.ToList().ForEach(ps => ps.Play(true));
            if(trailRenderTrails.Length > 0)
                trailRenderTrails.ToList().ForEach(tr => tr.emitting = true);
        }

        _rb.detectCollisions = true;

        alreadyDestroyed = false;

        if (DebugMode)
            Debug.Log("Spawned at " + transform.position + " in the world");
    }
    
    public void Shoot(Vector3 launchVector)
    {
        transform.parent = null;

        _rb.isKinematic = false;

        previousPosition = _rb.position;

        _rb.AddForce(launchVector, ForceMode.Impulse);

        Invoke(nameof(DestroyBody), LifeTime);
    }
    private void DestroyBody()
    {
        CancelInvoke(nameof(DestroyBody));

        BodyParent.SetActive(false);
        _rb.detectCollisions = false;
        _rb.isKinematic = true;

        if (TrailParent)
        {
            if(particleSystemTrails.Length > 0)
                particleSystemTrails.ToList().ForEach(ps => ps.Stop(true, ParticleSystemStopBehavior.StopEmitting));
            
            Invoke(nameof(DestroyTrails), particleTrailDurations + trailRenderDurations);
        }
        else
        {
            ReleaseProjectile();
        }
        alreadyDestroyed = true;
    }
    private void DestroyTrails()
    {
        if(trailRenderTrails.Length > 0)
            trailRenderTrails.ToList().ForEach(tr => tr.emitting = false);

        ReleaseProjectile();
    }
    private void ReleaseProjectile()
    {
        _pool.Release(this);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!alreadyDestroyed)
        {
            if (DebugMode)
                Debug.Log(collision.gameObject.name);
            DestroyBody();
            _shooter.SpawnImpact(collision, transform, HitEffectScaler);
            if (collision.gameObject.TryGetComponent(out IDamageable target))
            {
                target.TakeDamage(_damageAmount, _damageTyping);
            }
        }
    }

    private void FixedUpdate()
    {
        if (!_rb.isKinematic)
        {
            Vector3 movementThisStep = _rb.position - previousPosition;

            
            float movementSqrMagnitude = movementThisStep.sqrMagnitude;

            if (movementSqrMagnitude > sqrMinimumExtent)
            {
                float movementMagnitude = Mathf.Sqrt(movementSqrMagnitude);
                RaycastHit hitInfo;

                //Point in the direction the rigidbody just moved.
                if (PointTowardsMotion)
                    BodyParent.transform.rotation = Quaternion.LookRotation(movementThisStep);


                //check for obstructions we might have missed 
                if (Physics.Raycast(previousPosition, movementThisStep, out hitInfo, movementMagnitude, ~_nonCollidableLayers, QueryTriggerInteraction.Ignore))
                    _rb.position = hitInfo.point - (movementThisStep / movementMagnitude) * partialExtent;
            }

            previousPosition = _rb.position;
        }
    }
    public void SetPool(ObjectPool<SpellProjectile> pool)
    {
        _pool = pool;
    }
    public void SetShooter(ProjectileShooter newShooter)
    {
        _shooter = newShooter;
    }
    public void SetLayers(LayerMask newMask)
    {
        _nonCollidableLayers = newMask;
    }
    public void SetDamage(float newAmount, DamageType newType)
    {
        _damageAmount = newAmount;
        _damageTyping = newType;
    }
    public ProjectileShooter GetShooter()
    {
        return _shooter;
    }

}
public enum DamageType
{
    Fire,
    Water,
    Air,
    Earth,
    Force,
    Lightning,
    Metal,
    Ice,
    Nature,
    Physical
}
