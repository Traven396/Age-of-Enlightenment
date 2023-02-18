using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileFire : MonoBehaviour, ICastable
{
    public GameObject projectile;
    public Direction shootDirection;
    public int spawnAmount = 1;
    public float spreadAmount;
    public bool useGravity = false;

    public void Cast(Transform target)
    {
        
            
            Vector3 shootDirection = target.forward;
            if (spreadAmount != 0)
            {
                //shootDirectionTransform += new Vector3(Random.Range(-spreadAmount, spreadAmount), Random.Range(-spreadAmount, spreadAmount), Random.Range(-spreadAmount, spreadAmount));
            }
            //spawnedProjectile.GetComponent<ProjectileBehavior>().Shoot(shootDirection.normalized);
        
        
    }
    public void LaunchProjectile(Transform target, LeftRight whichHand)
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            var InstantiatedObject = Instantiate(projectile, target.position, target.rotation);
            Vector3 shootDirectionTransform = new Vector3();
            switch (shootDirection)
            {
                case Direction.Left:
                    if (whichHand == 0)
                        shootDirectionTransform = target.right;
                    else
                        shootDirectionTransform = -target.right;
                    break;
                case Direction.Right:
                    if (whichHand == 0)
                        shootDirectionTransform = -target.right;
                    else
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
                currentProjectile.GetComponent<Rigidbody>().useGravity = useGravity;
                currentProjectile.Shoot(shootDirectionTransform.normalized);
            }
            else
            {
                Destroy(InstantiatedObject);
                Debug.Log("Behavior not found");
            }
        }
    }

}
