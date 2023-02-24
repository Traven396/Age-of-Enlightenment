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


    [Header("Projectile Behaviors")]
    [SerializeField] private int spawnAmount = 1;
    [SerializeField] private Direction shootDirection;
    [Min(0)]
    [SerializeField] private float spreadAmount = 0;

    private List<GameObject> spawnedObjects = new List<GameObject>();
    public void Cast(Transform target)
    {
        spawnedObjects.Clear();
        for (int i = 0; i < spawnAmount; i++)
        {
            instantiatedObject = ParentToTarget ? Instantiate(objectToSpawn, target.position, (target.rotation * Quaternion.Euler(rotationOffset)), target) : Instantiate(objectToSpawn, target.position, (target.rotation * Quaternion.Euler(rotationOffset)));
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
    public void LaunchProjectile(Transform directionIndicator, LeftRight whichHand)
    {
        foreach (GameObject currentSpawnedProjectile in spawnedObjects)
        {
            currentSpawnedProjectile.transform.parent = null;
            //currentSpawnedProjectile.GetComponent<Rigidbody>().isKinematic = false;

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

            ProjectileBehavior pb;
            if (currentSpawnedProjectile.TryGetComponent(out pb))
            {
                pb.enabled = true;
                pb.Shoot(shootDirectionTransform.normalized);
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
    public void SetGravity(bool YN)
    {
        GravityAffected = YN;
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
