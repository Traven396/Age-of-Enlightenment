using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawn : MonoBehaviour
{
    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private bool ParentToTarget = false;
    [SerializeField] private Vector3 rotationOffset;
    [Space(5)]
    [SerializeField] private bool LockRotation = false;
    [Space(5)]
    [SerializeField] private bool LockPosition = false;
    [Space(5)]
    [SerializeField] private bool GravityAffected = true;

    [HideInInspector] public GameObject instantiatedObject;
    [HideInInspector] public Rigidbody instantiatedRB;


    [Header("Projectile Behaviors")]
    [SerializeField] private int spawnAmount = 1;
    [SerializeField] private Direction shootDirection;
    //This is mainly just for terra blade now. I need more comments I realize lmao
    [SerializeField] private bool projectileGravity = false;
    [Min(0)]
    [SerializeField] private float spreadAmount = 0;
    private LayerMask playerProjectileLayer;

    private List<GameObject> spawnedObjects = new List<GameObject>();
    private void Start()
    {
        playerProjectileLayer = LayerMask.NameToLayer("PlayerProjectile");
    }
    public void Cast(Transform target)
    {
        spawnedObjects.Clear();
        for (int i = 0; i < spawnAmount; i++)
        {
            instantiatedObject = ParentToTarget ? Instantiate(objectToSpawn, target.position, (target.rotation * Quaternion.Euler(rotationOffset)), target) : Instantiate(objectToSpawn, target.position, (target.rotation * Quaternion.Euler(rotationOffset)));
            spawnedObjects.Add(instantiatedObject);
            #region Setting up Rigidbody
            instantiatedRB = instantiatedObject.GetComponent<Rigidbody>();
            if (instantiatedRB == null)
            {
                return;
            }
            if (LockPosition && !LockRotation)
            {
                instantiatedRB.constraints = RigidbodyConstraints.FreezePosition;
            }
            else if (LockRotation && !LockPosition)
            {
                instantiatedRB.constraints = RigidbodyConstraints.FreezeRotation;
            }
            else if (LockPosition && LockRotation)
            {
                instantiatedRB.constraints = RigidbodyConstraints.FreezeAll;
            }
            instantiatedRB.useGravity = GravityAffected;
            #endregion  
        }
    }

    public void Cast(Vector3 target)
    {
        spawnedObjects.Clear();
        for (int i = 0; i < spawnAmount; i++)
        {
            instantiatedObject = ParentToTarget ? Instantiate(objectToSpawn, target, Quaternion.identity * Quaternion.Euler(rotationOffset)) : Instantiate(objectToSpawn, target, (Quaternion.identity * Quaternion.Euler(rotationOffset)));
            spawnedObjects.Add(instantiatedObject);
            #region Setting up Rigidbody
            Rigidbody rb = instantiatedObject.GetComponent<Rigidbody>();
            if (rb == null)
            {
                return;
            }
            if (LockPosition && !LockRotation)
            {
                rb.constraints = RigidbodyConstraints.FreezePosition;
            }
            else if (LockRotation && !LockPosition)
            {
                rb.constraints = RigidbodyConstraints.FreezeRotation;
            }
            else if (LockPosition && LockRotation)
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }
            rb.useGravity = GravityAffected;
            #endregion  
        }
    }

    public void LaunchProjectile(Transform directionIndicator, LeftRight whichHand, float launchModifier)
    {
        foreach (GameObject currentSpawnedProjectile in spawnedObjects)
        {
            currentSpawnedProjectile.transform.parent = null;
            currentSpawnedProjectile.layer = playerProjectileLayer;
            //currentSpawnedProjectile.GetComponent<Rigidbody>().isKinematic = false;
            instantiatedRB.useGravity = projectileGravity;

            Vector3 shootDirectionTransform = new Vector3();
            switch (shootDirection)
            {
                case Direction.Left:
                    if (whichHand == 0)
                        shootDirectionTransform = directionIndicator.right;
                    else
                        shootDirectionTransform = -directionIndicator.right;
                    break;
                case Direction.Right:
                    if (whichHand == 0)
                        shootDirectionTransform = -directionIndicator.right;
                    else
                        shootDirectionTransform = directionIndicator.right;
                    break;
                case Direction.Up:
                    if (whichHand == 0)
                        shootDirectionTransform = -directionIndicator.up;
                    else
                        shootDirectionTransform = directionIndicator.up;
                    break;
                case Direction.Down:
                    shootDirectionTransform = -directionIndicator.up;
                    break;
                case Direction.Forward:
                    shootDirectionTransform = directionIndicator.forward;
                    break;
                case Direction.Back:
                    shootDirectionTransform = -directionIndicator.forward;
                    break;
            }

            if (spreadAmount != 0)
            {
                shootDirectionTransform += new Vector3(Random.Range(-spreadAmount, spreadAmount), Random.Range(-spreadAmount, spreadAmount), Random.Range(-spreadAmount, spreadAmount));
            }

            if (currentSpawnedProjectile.TryGetComponent(out ProjectileBehavior pb))
            {
                pb.enabled = true;
                pb.Shoot(shootDirectionTransform * launchModifier);
            }
            else
            {
                Debug.Log("Behavior not found");
            }
        }
    }
    public void SetShootDirection(Direction newDirection)
    {
        shootDirection = newDirection;
    }
    public void SetRotationOffset(Vector3 newOffset)
    {
        rotationOffset = newOffset;
    }
    public Vector3 GetRotationOffset()
    {
        return rotationOffset;
    }
    public void SetGravity(bool YN)
    {
        GravityAffected = YN;
    }
    public void SetSpawnObject(GameObject newObject)
    {
        objectToSpawn = newObject;
    }
}
public enum Direction
{
    Left,
    Right,
    Up,
    Down,
    Forward,
    Back
}
