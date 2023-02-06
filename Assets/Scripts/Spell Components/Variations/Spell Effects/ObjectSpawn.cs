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

    [HideInInspector] public GameObject InstantiatedObject;


    [Header("Projectile Behaviors")]
    [SerializeField] private bool isProjectile = false;
    [SerializeField] private int spawnAmount = 1;
    [SerializeField] private Direction shootDirection;
    [Min(0)]
    [SerializeField] private float spreadAmount = 0;
    public void Cast(Transform target)
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            InstantiatedObject = ParentToTarget ? Instantiate(objectToSpawn, target.position, (target.rotation * Quaternion.Euler(rotationOffset)), target) : Instantiate(objectToSpawn, target.position, (target.rotation * Quaternion.Euler(rotationOffset)));
            
            #region Setting up Rigidbody
            Rigidbody rb = InstantiatedObject.GetComponent<Rigidbody>();
            if (rb == null)
            {
                return;
            }
            if (LockPosition)
            {
                rb.constraints = RigidbodyConstraints.FreezePosition;
            }
            if (LockRotation)
            {
                rb.constraints = RigidbodyConstraints.FreezeRotation;
            }
            rb.useGravity = GravityAffected;
            #endregion
            
            #region Projectile Stuff
            if (isProjectile)
            {
                Vector3 shootDirectionTransform = new Vector3();
                switch (shootDirection)
                {
                    case Direction.Left:
                        shootDirectionTransform = -target.right;
                        break;
                    case Direction.Right:
                        shootDirectionTransform = target.right;
                        break;
                    case Direction.Up:
                        shootDirectionTransform = target.up;
                        break;
                    case Direction.Down:
                        shootDirectionTransform = -target.up;
                        break;
                    case Direction.Forward:
                        shootDirectionTransform = target.forward;
                        break;
                    case Direction.Back:
                        shootDirectionTransform = -target.forward;
                        break;
                }
            
                if (spreadAmount != 0)
                {
                    shootDirectionTransform += new Vector3(Random.Range(-spreadAmount, spreadAmount), Random.Range(-spreadAmount, spreadAmount), Random.Range(-spreadAmount, spreadAmount));
                }

                ProjectileBehavior currentProjectile;
                if (InstantiatedObject.TryGetComponent<ProjectileBehavior>(out currentProjectile))
                {
                    currentProjectile.Shoot(shootDirectionTransform.normalized);
                }
                else
                {
                    Debug.Log("Behavior not found");
                }
            }
            #endregion 
        }
    }
    public void LaunchProjectile()
    {

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
