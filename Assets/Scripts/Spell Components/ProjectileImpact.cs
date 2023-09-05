using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ProjectileImpact : MonoBehaviour
{
    private ObjectPool<ProjectileImpact> _pool;
    private ParticleSystem[] _particleSystems;

    private float _maxLifetime = 0;
    private void Awake()
    {
        _particleSystems = GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem sys in _particleSystems)
        {
            if (sys.main.duration > _maxLifetime)
                _maxLifetime = sys.main.duration;
        }
        Debug.Log(_maxLifetime);
        Invoke(nameof(DestroyEffect), _maxLifetime);
    }

    public void SetPool(ObjectPool<ProjectileImpact> newPool)
    {
        _pool = newPool;
    }
    
    private void DestroyEffect()
    {
        _pool.Release(this);
    }
}
