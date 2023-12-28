using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ProjectileShooter : MonoBehaviour
{
    //Settings that are set in the inspector, not by code
    [SerializeField] private SpellProjectile ProjectilePrefab;
    [SerializeField] private SpellImpact ImpactFXPrefab;
    [SerializeField] private int PoolSize = 5;
    
    [SerializeField] private LayerMask NonCollidableLayers;
    //Things that are read by code, but should never be changed outside this script
    public GameObject latestInstantiatedObject { get; private set; }
    public Rigidbody latestInstantiatedObjectRB { get; private set; }
    public List<SpellProjectile> activeProjectiles { get; private set; } = new List<SpellProjectile>();
    

    //Settings that the different spells should set upon waking up. 
    [Min(0)]
    private float _spreadAmount = 0;
    [Min(0)]
    private float _damageAmount = 0;
    private DamageType _damageTyping;


    private string ProjectileLayer = "PlayerProjectile";

    //Settings and references required for function by this script alone
    private ObjectPool<SpellProjectile> projectilePool;
    private ObjectPool<SpellImpact> impactEffectsPool;

    private List<SpellImpact> impactCleanupList = new List<SpellImpact>();
    private List<SpellProjectile> projectileCleanupList = new List<SpellProjectile>();

    private Transform currentSpawnTransform;
    private Vector3? currentSpawnPosition;
    private Quaternion? currentSpawnRotation;

    private Collision latestCollision;
    //private Transform latestImpactedProjectile;
    private float latestScaler;

    private LayerMask projectileLayer;

    private void Awake()
    {
        projectilePool = new ObjectPool<SpellProjectile>(createFunc: OnInstantiateProjectile, actionOnGet: OnGetProjectile, actionOnRelease: OnReturnProjectileToPool, maxSize: PoolSize, actionOnDestroy: OnDestroyProjectile);
        impactEffectsPool = new ObjectPool<SpellImpact>(createFunc: OnInstantiateImpact, actionOnGet: OnGetEffect, actionOnRelease: OnReturnImpactToPool, maxSize: PoolSize, actionOnDestroy: OnDestroyEffect);
        
        projectileLayer = LayerMask.NameToLayer(ProjectileLayer);
    }
    private void OnDisable()
    {
        projectilePool.Dispose();

        projectileCleanupList.ForEach(proj => Destroy(proj.gameObject));

        impactEffectsPool.Dispose();

        impactCleanupList.ForEach(impact => Destroy(impact.gameObject));
        
    }
    #region Projectile Pool Related Methods
    private SpellProjectile OnInstantiateProjectile()
    {
        if (currentSpawnPosition != null && currentSpawnRotation != null)
            return Instantiate(ProjectilePrefab, (Vector3)currentSpawnPosition, (Quaternion)currentSpawnRotation);
        else if (currentSpawnTransform != null)
            return Instantiate(ProjectilePrefab, currentSpawnTransform);
        else if (currentSpawnPosition != null)
            return Instantiate(ProjectilePrefab, (Vector3)currentSpawnPosition, Quaternion.identity);
        else
            return Instantiate(ProjectilePrefab);
    }
    private void OnReturnProjectileToPool(SpellProjectile projectile)
    {
        projectile.transform.parent = null;
        projectile.gameObject.SetActive(false);
    }
    private void OnGetProjectile(SpellProjectile projectile)
    {
        if (currentSpawnPosition != null)
            projectile.transform.position = (Vector3)currentSpawnPosition;
        if (currentSpawnRotation != null)
            projectile.transform.rotation = (Quaternion)currentSpawnRotation;
        if (currentSpawnTransform)
        {
            projectile.transform.SetPositionAndRotation(currentSpawnTransform.position, currentSpawnTransform.rotation);
            projectile.transform.parent = currentSpawnTransform;
        }

        projectile.SetPool(projectilePool);
        projectile.SetShooter(this);
        projectile.SetLayers(NonCollidableLayers);

        projectile.transform.localScale = Vector3.one;
        projectile.gameObject.SetActive(true);

        //Set all every part of the projectile, and its children, to the specified ProjectileLayer
        var children = projectile.gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform item in children)
        {
            item.gameObject.layer = projectileLayer;
        }

        projectile.gameObject.layer = projectileLayer;

        activeProjectiles.Insert(0, projectile);

        projectileCleanupList.Insert(0, projectile);
    }
    private void OnDestroyProjectile(SpellProjectile projectile)
    {
        if (projectile.gameObject)
        {
            Destroy(projectile.gameObject);
            activeProjectiles.Remove(projectile);
            projectileCleanupList.Remove(projectile);
        }
    }

    public void SpawnProjectile(Vector3 spawnPosition)
    {
        currentSpawnPosition = spawnPosition;

        latestInstantiatedObject = projectilePool.Get().gameObject;
        latestInstantiatedObjectRB = latestInstantiatedObject.GetComponent<Rigidbody>();

        latestInstantiatedObjectRB.velocity = Vector3.zero;

        currentSpawnPosition = null;
    }
    public void SpawnProjectile(Vector3 spawnPosition, Quaternion spawnRotation)
    {
        currentSpawnPosition = spawnPosition;
        currentSpawnRotation = spawnRotation;

        latestInstantiatedObject = projectilePool.Get().gameObject;
        latestInstantiatedObjectRB = latestInstantiatedObject.GetComponent<Rigidbody>();

        latestInstantiatedObjectRB.velocity = Vector3.zero;

        currentSpawnPosition = null;
        currentSpawnRotation = null;
    }
    public void SpawnProjectile(Transform spawnParent)
    {
        currentSpawnTransform = spawnParent;

        latestInstantiatedObject = projectilePool.Get().gameObject;
        latestInstantiatedObjectRB = latestInstantiatedObject.GetComponent<Rigidbody>();

        latestInstantiatedObjectRB.velocity = Vector3.zero;

        latestInstantiatedObjectRB.isKinematic = true;

        currentSpawnTransform = null;
    }
    #endregion
    #region Impact effect pool related things
    private SpellImpact OnInstantiateImpact()
    {
        return Instantiate(ImpactFXPrefab);
    }
    private void OnReturnImpactToPool(SpellImpact effect)
    {
        effect.gameObject.SetActive(false);
    }
    private void OnGetEffect(SpellImpact effect)
    {
        effect.transform.position = latestCollision.GetContact(0).point;

        effect.transform.up = latestCollision.GetContact(0).normal;

        effect.SetPool(impactEffectsPool);

        effect.SetScale(latestScaler);

        effect.gameObject.SetActive(true);

        effect.FinishedLoading();

        impactCleanupList.Insert(0, effect);
    }
    private void OnDestroyEffect(SpellImpact effect)
    {
        if (effect.gameObject)
        {
            Destroy(effect.gameObject);
            impactCleanupList.Remove(effect);
        }
    }
    public void SpawnImpact(Collision col, Transform spawningProjectile, float scaler)
    {
        if (ImpactFXPrefab)
        {
            latestCollision = col;
            //latestImpactedProjectile = spawningProjectile;
            latestScaler = scaler;

            impactEffectsPool.Get();

            latestCollision = null;
            //latestImpactedProjectile = null;
            latestScaler = 1; 
        }
    }
    #endregion
    
    
    public void LaunchAllProjectiles(Vector3 firingDirection, float speedModifier)
    {
        foreach (SpellProjectile projectile in activeProjectiles)
        {
            Vector3 fullLaunchVector = firingDirection;
            if(_spreadAmount > 0)
            {
                var calcSpread = _spreadAmount / 360;
                fullLaunchVector += new Vector3(Random.Range(-calcSpread, calcSpread), Random.Range(-calcSpread, calcSpread), Random.Range(-calcSpread, calcSpread));
                fullLaunchVector.Normalize();
            }

            projectile.Shoot(fullLaunchVector * speedModifier);
            projectile.SetDamage(_damageAmount, _damageTyping);
        }
        activeProjectiles.Clear();
    }
    public void LaunchSingleProjectile(Vector3 firingDirection, float speedModifier, SpellProjectile projectileToFire)
    {
        if (activeProjectiles.Contains(projectileToFire))
        {
            Vector3 fullLaunchVector = firingDirection;
            if (_spreadAmount > 0)
            {
                var calcSpread = _spreadAmount / 360;
                fullLaunchVector += new Vector3(Random.Range(-calcSpread, calcSpread), Random.Range(-calcSpread, calcSpread), Random.Range(-calcSpread, calcSpread));
                fullLaunchVector.Normalize();
            }
            projectileToFire.Shoot(fullLaunchVector * speedModifier);
            projectileToFire.SetDamage(_damageAmount, _damageTyping);

            activeProjectiles.Remove(projectileToFire);
        }
    }
    public void DespawnAllProjectiles()
    {
        activeProjectiles.ForEach(proj => projectilePool.Release(proj));
        activeProjectiles.Clear();
    }

    public void SetSpread(float newSpread)
    {
        _spreadAmount = newSpread;
    }
    public void SetDamage(float damage, DamageType type)
    {
        _damageAmount = damage;
        _damageTyping = type;
    }
    public LayerMask GetNonCollidableLayers()
    {
        return NonCollidableLayers;
    }
    public (DamageType returnedTyping, float returnedAmount) GetDamageAndTyping()
    {
        return (_damageTyping, _damageAmount);
    }
}
