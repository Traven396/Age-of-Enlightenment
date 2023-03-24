using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    [Header("Visuals")]
    public GameObject hitEffects;
    public float hitEffectLifetime;
    public float hitSizeScaler = 1;
    [Space(10)]
    public List<GameObject> trails = new List<GameObject>();
    public bool rotateTowardsMotion = false;

    [Header("Projectile Stats")]
    public float damageAmount = 1;
    public DamageType damageType;
    public float lifeTime = 3;
    public bool collisionsFromStart = false;
    [Space(20)]
    public bool isExplosive = false;
    public bool explosionDependentOnScale = false;
    public float explosionRadius = 0;
    public float explosionForce = 1;
    public LayerMask affectedLayers;

    private Rigidbody rb;

    private bool enableCollisions = false;

    private void Awake()
    {
        enableCollisions = collisionsFromStart;
        rb = GetComponent<Rigidbody>();
        if (rb)
        {
            rb = GetComponentInChildren<Rigidbody>();
        }
     
    }

    private void Update()
    {
        if (rotateTowardsMotion)
        {
            if(rb.velocity != Vector3.zero)
            {
                rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(rb.velocity.normalized), 1080));
                //transform.LookAt(transform.TransformDirection(rb.velocity.normalized));
            }

        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (enableCollisions)
        {
            UnparentTrails();
            gameObject.SetActive(false);
            if (hitEffects)
            {
                var instantHit = Instantiate(hitEffects, collision.GetContact(0).point, Quaternion.LookRotation(Vector3.up, collision.GetContact(0).normal));
                instantHit.transform.localScale = transform.localScale * hitSizeScaler;
                Destroy(instantHit, hitEffectLifetime);
            }

            if (isExplosive)
            {
                HandleExplosion();
            }

            if(collision.gameObject.TryGetComponent(out IDamageable target))
            {
                target.TakeDamage(damageAmount, damageType);
            }

            Destroy(this.gameObject);
            
        }
    }

    private void UnparentTrails()
    {
        if (trails.Count > 0)
        {
            foreach (GameObject gameObject in trails)
            {
                gameObject.transform.parent = null;
                var ps = gameObject.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    ps.Stop();
                    Destroy(gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
                }
            }
        }
    }
    private void HandleExplosion()
    {
        if (explosionDependentOnScale)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius * transform.localScale.magnitude, affectedLayers);
            foreach (Collider hitObject in hits)
            {
                if (hitObject.TryGetComponent(out Rigidbody rb))
                {
                    rb.AddExplosionForce(explosionForce * transform.localScale.magnitude, transform.position, explosionRadius * transform.localScale.magnitude);
                }
                if(hitObject.TryGetComponent(out IDamageable target))
                {
                    target.TakeDamage(damageAmount, damageType);
                }
            } 
        }
        else
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius, affectedLayers);
            foreach (Collider hitObject in hits)
            {
                if (hitObject.TryGetComponent(out Rigidbody rb))
                {
                    rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
                }
                if (hitObject.TryGetComponent(out IDamageable target))
                {
                    target.TakeDamage(damageAmount, damageType);

                }
            }
        }
    }
    public void Shoot(Vector3 launchVector)
    {
        rb.isKinematic = false;
        rb.AddForce(launchVector, ForceMode.Impulse);
        enableCollisions = true;
        Invoke(nameof(UnparentTrails), lifeTime - 0.01f);
        Destroy(gameObject, lifeTime);
    }
}

public enum DamageType{
    Fire,
    Water,
    Lightning,
    Ice,
    Earth,
    Air,
    Force
}
