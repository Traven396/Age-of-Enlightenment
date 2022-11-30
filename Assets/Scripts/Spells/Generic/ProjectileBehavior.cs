using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    [Header("Visuals")]
    public GameObject hitEffects;
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
        if (rb == null)
        {
            rb = GetComponentInChildren<Rigidbody>();
        }
    }

    private void Update()
    {
        if (rotateTowardsMotion)
        {
            if(rb.velocity != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(rb.velocity.normalized);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        gameObject.SetActive(false);
        if (hitEffects)
            Instantiate(hitEffects, collision.GetContact(0).point, Quaternion.identity);

        if(trails.Count > 0)
        {
            foreach (GameObject gameObject in trails)
            {
                gameObject.transform.parent = null;
                var ps = gameObject.GetComponent<ParticleSystem>();
                if(ps != null)
                {
                    ps.Stop();
                }
            }
        }
        Destroy(this.gameObject, .05f);
    }

    public void Shoot(Vector3 direction)
    {
        rb.AddForce(direction * speedMultiplier);

    }
}

public enum DamageType{
    Fire,
    Water,
    Ice,
    Earth,
    Air,
    Force
}
