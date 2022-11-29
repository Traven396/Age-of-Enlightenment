using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileFire : MonoBehaviour, ICastable
{
    public GameObject projectile;
    public int projectileAmount = 1;
    //public float scatterAmount;

    public void Cast(Transform target)
    {
        for (int i = 0; i < projectileAmount; i++)
        {
            var spawnedProjectile = Instantiate(projectile, target.position, target.rotation);
            Vector3 shootDirection = target.forward;
            //if (scatterAmount != 0)
            //{
            //    float x = Random.Range(-scatterAmount, -scatterAmount);
            //    float y = Random.Range(-scatterAmount, scatterAmount);
            //    shootDirection += new Vector3(x, y, 0);
            //}
            spawnedProjectile.GetComponent<ProjectileBehavior>().Shoot(shootDirection.normalized);
        }
        
    }
}
