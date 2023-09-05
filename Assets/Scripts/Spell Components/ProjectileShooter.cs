using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ProjectileShooter : MonoBehaviour
{
    //Settings that are set in the inspector, not by code
    [SerializeField] private SpellProjectile ProjectilePrefab;
    [SerializeField] private ProjectileImpact ImpactFXPrefab;
    [SerializeField] private int PoolSize = 5;
    [SerializeField] private string ProjectileLayer = "PlayerProjectile";

    //Things that are read by code, but should never be changed outside this script
    public SpellProjectile latestInstantiatedObject { get; private set; }
    public Rigidbody latestInstantiatedObjectRB { get; private set; }

    //Settings that the different spells should set upon waking up. 
    [Min(0)]
    private float _spreadAmount = 0;
    
        
    //Settings and references required for function by this script alone
    private ObjectPool<SpellProjectile> _projectilePool;
    private ObjectPool<ProjectileImpact> _impactEffectsPool;

    private List<SpellProjectile> _activeProjectiles = new List<SpellProjectile>();

    private Transform _currentSpawnTransform;
    private Vector3? _currentSpawnPosition;
    private Collision _latestCollision;

    private LayerMask _projectileLayer;

    private void Awake()
    {
        _projectilePool = new ObjectPool<SpellProjectile>(createFunc: OnInstantiateProjectile, actionOnGet: OnGetProjectile, actionOnRelease: OnReturnProjectileToPool, maxSize: PoolSize, actionOnDestroy: OnDestroyProjectile);
        _impactEffectsPool = new ObjectPool<ProjectileImpact>(createFunc: OnInstantiateImpact, actionOnGet: OnGetEffect, actionOnRelease: OnReturnImpactToPool, maxSize: PoolSize, actionOnDestroy: OnDestroyEffect);
        
        _projectileLayer = LayerMask.NameToLayer(ProjectileLayer);
    }
    #region Object Pool Related Methods
    private SpellProjectile OnInstantiateProjectile()
    {
        return Instantiate(ProjectilePrefab);
    }
    private void OnReturnProjectileToPool(SpellProjectile projectile)
    {
        projectile.transform.parent = null;
        projectile.gameObject.SetActive(false);
    }
    private void OnGetProjectile(SpellProjectile projectile)
    {
        if (_currentSpawnPosition != null)
            projectile.transform.position = (Vector3)_currentSpawnPosition;
        if (_currentSpawnTransform)
        {
            projectile.transform.SetPositionAndRotation(_currentSpawnTransform.position, _currentSpawnTransform.rotation);
            projectile.transform.parent = _currentSpawnTransform;
        }

        projectile.SetPool(_projectilePool);
        projectile.SetShooter(this);

        projectile.released = false;

        projectile.gameObject.SetActive(true);

        var children = projectile.gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform item in children)
        {
            item.gameObject.layer = _projectileLayer;
        }

        projectile.gameObject.layer = _projectileLayer;

        _activeProjectiles.Add(projectile);
    }
    private void OnDestroyProjectile(SpellProjectile projectile)
    {
        _activeProjectiles.Remove(projectile);
        Destroy(projectile.gameObject);
    }

    public void SpawnProjectile(Vector3 spawnPosition)
    {
        _currentSpawnPosition = spawnPosition;

        latestInstantiatedObject = _projectilePool.Get();
        latestInstantiatedObjectRB = latestInstantiatedObject.GetComponent<Rigidbody>();

        latestInstantiatedObjectRB.velocity = Vector3.zero;

        _currentSpawnPosition = null;
    }
    public void SpawnProjectile(Transform spawnParent)
    {
        _currentSpawnTransform = spawnParent;

        latestInstantiatedObject = _projectilePool.Get();
        latestInstantiatedObjectRB = latestInstantiatedObject.GetComponent<Rigidbody>();

        latestInstantiatedObjectRB.velocity = Vector3.zero;

        latestInstantiatedObjectRB.isKinematic = true;

        _currentSpawnTransform = null;
    }
    #endregion
    #region Impact effect pool related things
    private ProjectileImpact OnInstantiateImpact()
    {
        return Instantiate(ImpactFXPrefab);
    }
    private void OnReturnImpactToPool(ProjectileImpact effect)
    {
        effect.gameObject.SetActive(false);
    }
    private void OnGetEffect(ProjectileImpact effect)
    {
        effect.transform.SetPositionAndRotation(_latestCollision.GetContact(0).point, Quaternion.LookRotation(Vector3.up, _latestCollision.GetContact(0).normal));

        effect.SetPool(_impactEffectsPool);

        effect.gameObject.SetActive(true);
    }
    private void OnDestroyEffect(ProjectileImpact effect)
    {
        Destroy(effect.gameObject);
    }
    public void SpawnImpact(Collision col)
    {
        _latestCollision = col;

        _impactEffectsPool.Get();

        _latestCollision = null;
    }
    #endregion
    private void OnDisable()
    {
        if (_projectilePool != null)
        {
            _projectilePool.Dispose(); 
        }
    }
    
    public void LaunchAllProjectiles(Vector3 firingDirection, float speedModifier)
    {
        foreach (SpellProjectile projectile in _activeProjectiles)
        {
            Vector3 fullLaunchVector = firingDirection;
            if(_spreadAmount > 0)
            {
                var calcSpread = _spreadAmount / 360;
                fullLaunchVector += new Vector3(Random.Range(-calcSpread, calcSpread), Random.Range(-calcSpread, calcSpread), Random.Range(-calcSpread, calcSpread));
                fullLaunchVector.Normalize();
            }

            projectile.Shoot(fullLaunchVector * speedModifier);
        }
        _activeProjectiles.Clear();
    }

    

    public void SetSpread(float newSpread)
    {
        _spreadAmount = newSpread;
    }
}
