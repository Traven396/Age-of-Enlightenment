using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Rigidbody))]
public class SpellProjectile : MonoBehaviour
{
    public bool released = false;

    [SerializeField] private float LifeTime = 3;

    private ObjectPool<SpellProjectile> _pool;
    private Rigidbody _rb;
    private ProjectileShooter _shooter;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    public void SetPool(ObjectPool<SpellProjectile> pool)
    {
        _pool = pool;
    }
    public void SetShooter(ProjectileShooter newShooter)
    {
        _shooter = newShooter;
    }
    public void Shoot(Vector3 launchVector)
    {
        transform.parent = null;

        _rb.isKinematic = false;

        _rb.AddForce(launchVector, ForceMode.Impulse);
        Invoke(nameof(DestroyProjectile), LifeTime);
    }
    private void OnCollisionEnter(Collision collision)
    {
        _shooter.SpawnImpact(collision);

        DestroyProjectile();
    }
    private void DestroyProjectile()
    {
        if (!released)
        {
            _pool.Release(this);
            released = true;
        }
    }
}
