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
    [SerializeField] private GameObject BodyParent;
    [SerializeField] private GameObject TrailParent;

    private ObjectPool<SpellProjectile> _pool;
    private Rigidbody _rb;
    private ProjectileShooter _shooter;
    private LayerMask _nonCollidableLayers;
    private float _damageAmount;
    private DamageType _damageTyping;

    private ParticleSystem[] trails;

    private float minimumExtent;
    private float partialExtent;
    private float sqrMinimumExtent;
    private Vector3 previousPosition;
    private Collider selfCollider;

    private bool alreadyDestroyed;
    private float trailDurations;

    //Anything that should fire only once on the initial spawn should be put in here
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        trails = TrailParent.GetComponentsInChildren<ParticleSystem>();

        selfCollider = GetComponent<Collider>();
        if (!selfCollider)
        {
            selfCollider = GetComponentInChildren<Collider>();
        }

        trailDurations = trails.Max(ps => ps.main.duration + ps.main.startLifetime.constantMax);

        minimumExtent = Mathf.Min(Mathf.Min(selfCollider.bounds.extents.x, selfCollider.bounds.extents.y), selfCollider.bounds.extents.z);
        partialExtent = minimumExtent * (1.0f - .1f);
        sqrMinimumExtent = minimumExtent * minimumExtent;
    }
    //Anything that needs to reset, or be redone whenever the projectile is cycled gets put in here
    private void OnEnable()
    {
        BodyParent.SetActive(true);

        trails.ToList().ForEach(ps => ps.Play(true));

        _rb.detectCollisions = true;

        alreadyDestroyed = false;
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

        trails.ToList().ForEach(ps => ps.Stop(true, ParticleSystemStopBehavior.StopEmitting));
        Invoke(nameof(DestroyTrails), trailDurations);

        alreadyDestroyed = true;
    }
    private void DestroyTrails()
    {
        _pool.Release(this);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!alreadyDestroyed)
        {
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

                //check for obstructions we might have missed 
                if (Physics.Raycast(previousPosition, movementThisStep, out hitInfo, movementMagnitude, ~_nonCollidableLayers))
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
