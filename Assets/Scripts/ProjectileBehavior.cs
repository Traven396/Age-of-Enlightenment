using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    [Header("Visuals")]
    public GameObject hitEffects;
    public int hitEffectLifetime;
    public float hitSizeScaler = 1;
    [Space(10)]
    public List<GameObject> trails = new List<GameObject>();
    public bool rotateTowardsMotion = false;
  
    [Header("Projectile Stats")]
    public float speedMultiplier = 100f;
    public DamageType damageType;
    public float lifeTime = 3;
    [Space(20)]
    public bool Explosive = false;
    public float explosionRadius = 0;

    private Rigidbody rb;

    private void Awake()
    {
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
        UnparentTrails();
        gameObject.SetActive(false);
        if (hitEffects)
        {
            var instantHit = Instantiate(hitEffects, collision.GetContact(0).point, Quaternion.LookRotation(Vector3.up, collision.GetContact(0).normal));
            instantHit.transform.localScale = transform.localScale * hitSizeScaler;
            Destroy(instantHit, hitEffectLifetime);
        }
        
        Destroy(this.gameObject, .05f);
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

    public void Shoot(Vector3 direction)
    {
        rb.isKinematic = false;
        rb.AddForce(direction * speedMultiplier);

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
