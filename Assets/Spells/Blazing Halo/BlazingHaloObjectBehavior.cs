using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class BlazingHaloObjectBehavior : MonoBehaviour
{
    private List<Transform> potentialTargets = new List<Transform>();
    private List<GameObject> spawnedFireballs = new List<GameObject>();

    public LayerMask TargettableLayers, LayersThatBlockAttack;
    public Transform CenterTransform;
    public float Radius = .05f;
    public int MaxProjectiles = 7;
    public float LaunchSpeed = 3;
    public GameObject FireballPrefab;

    private void OnTriggerEnter(Collider other)
    {
        if ((TargettableLayers & ( 1 << other.gameObject.layer)) != 0)
        {
            if (other.gameObject.TryGetComponent<EnemyAi>(out _))
            {
                potentialTargets.Add(other.transform);
                SortTargets(); 
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (potentialTargets.Contains(other.gameObject.transform))
        {
            potentialTargets.Remove(other.gameObject.transform);
            SortTargets();
        }
    }
    private void SortTargets()
    {
        for (int i = potentialTargets.Count - 1; i >= 0; i--)
        {
            if (!potentialTargets[i])
            {
                potentialTargets.RemoveAt(i);
            }
        }
        potentialTargets = potentialTargets.OrderBy((d) => (d.position - transform.position).sqrMagnitude).ToList();
    }
    private void Start()
    {
        StartCoroutine(nameof(HaloCoroutine));
    }
    private void OnDestroy()
    {
        StopCoroutine(nameof(HaloCoroutine));
    }

    private IEnumerator HaloCoroutine()
    {
        while (true)
        {
            CreateNewFireball();
            
            yield return new WaitForSeconds(2f);

            LaunchFireball();

            yield return new WaitForSeconds(.5f);

            LaunchFireball();

            yield return new WaitForSeconds(1f);
        }
    }

    private void LaunchFireball()
    {
        //Make sure we have a target to shoot at and that we have a fireball to shoot
        if (potentialTargets.Count != 0 && spawnedFireballs.Count != 0)
        {
            //Sort to gurantee that the first one in the list is the closest
            SortTargets();
            for (int i = 0; i < potentialTargets.Count; i++)
            {
                //Get the first fireball for ease of use
                var fb = spawnedFireballs[0];
                //Shoot a line from the fireball to the target, if it collides with something then we have to move on to the next target
                if (!Physics.Linecast(fb.transform.position, potentialTargets[i].position, LayersThatBlockAttack))
                {
                    //Shoot the projectile
                    fb.GetComponent<ProjectileBehavior>().Shoot((potentialTargets[i].position - fb.transform.position).normalized * LaunchSpeed);

                    //Remove its parent to stop the projectile from moving with the player
                    fb.transform.parent = null;

                    //Reenable the collisions for the object
                    fb.GetComponent<Collider>().enabled = true;

                    //Remove it from the list to keep things clean
                    spawnedFireballs.RemoveAt(0);

                    //Make the halo look nice afterwards
                    ReorientHalo();

                    break;
                }
            } 
        }
    }

    //Calculates each fireballs position around the circle based on their position in the array
    private void ReorientHalo()
    {
        //Loops through whole array, ascending
        for (int i = 0; i < spawnedFireballs.Count; i++){

            //Divides a circle into chunks based on the array size, then multiplies by the current position to get the current chunk
            //Also converts to radians
            var angle = 360 / spawnedFireballs.Count * i * Mathf.Deg2Rad;

            //Takes the COS of the angle which gives us our Z axis position, this is then multiplied by radius to move it out, and forward so that it is realtive to the center
            //Then adds on the SIN of the angle to give us X axis. Same thing, multiplies radius and right so that it stays consistent
            Vector3 calcPosition = ((Mathf.Cos(angle) * Radius * transform.forward) + (Mathf.Sin(angle) * Radius * transform.right));

            //Moves the current fireball to the new calculated position in local space so that when the player is moving it doesnt desync
            iTween.MoveTo(spawnedFireballs[i], iTween.Hash("position", calcPosition, "islocal", true, "time", .3));
        }
    }

    //Creates a new fireball if the max is not already reached. Spawns it as a child of the center, then inserts it to the list, takes mana, and reorients the halo
    private void CreateNewFireball()
    {
        if (spawnedFireballs.Count < MaxProjectiles) {
            var physicalFireball = Instantiate(FireballPrefab, CenterTransform);

            spawnedFireballs.Insert(0,physicalFireball);

            Player.Instance.SubtractMana(5);

            ReorientHalo();
        }
    }
}
