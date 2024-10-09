using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PermafrostArrow : SpellBlueprint
{
    
    public GameObject targetingCirclePrefab;
    public int maxPotentialProjectiles = 5;
    public float launchSpeed = 5;
    public float projectileDamage = 5;
    public int manaCost;
    public float minCircleRadius = 0.05f;


    private ProjectileShooter shooter;
    private List<GameObject> spawnedPotentialProjectiles = new List<GameObject>();
    private GameObject orientationObject;
    private bool bigCircleSpinning = false;

    //I want the circles to get more spread out as more of them appear, so I could do a thing where its like minCircleDistance * (chargedCircles.Count() - 1). 
    //that way when there is 1 circle it is directly in the center of the players hand and as more are added they get further away from eachother
    private void Awake()
    {
        shooter = GetComponent<ProjectileShooter>();
        shooter.SetDamage(projectileDamage, DamageType.Ice);
    }
    private void Update()
    {
        if (!triggerPressed)
        {
            _VisualsManager.ReturnCircleToHolder(currentHand);
        }
    }

    public override void TriggerPress()
    {
        base.TriggerPress();
        iTween.ScaleTo(_SpellCircle, iTween.Hash("x", .8f, "z", .8f, "time", 1));
        if(!orientationObject)
        {
            orientationObject = new GameObject("Orientation");
        }
        orientationObject.SetActive(true);
        orientationObject.transform.parent = _HandTransform;
        orientationObject.transform.position = _HandTransform.position + (_HandTransform.forward * .07f);
        orientationObject.transform.rotation = _HandTransform.transform.rotation;
        StartCoroutine(nameof(CircleSpawnCoroutine));
    }
    public override void TriggerHold()
    {
        base.TriggerHold();
        //Need to put some kind of transformation thing in here to rotate the spawned circles in a bigger radius once there is 2 of them
        //Can pretty much copy the constant spin code
    }
    public override void TriggerRelease()
    {
        base.TriggerRelease();
        iTween.ScaleTo(_SpellCircle, iTween.Hash("x", 1f, "z", 1f, "time", 1));
        StopCoroutine(nameof(CircleSpawnCoroutine));

        foreach (GameObject circle in spawnedPotentialProjectiles)
        {
            shooter.SpawnProjectile(circle.transform.position, Quaternion.LookRotation(orientationObject.transform.forward));
            Destroy(circle);
        }
        spawnedPotentialProjectiles = new List<GameObject>();

        shooter.LaunchAllProjectiles(orientationObject.transform.forward, launchSpeed);

        orientationObject.SetActive(false);
    }
    private IEnumerator CircleSpawnCoroutine()
    {
        while(true)
        {
            if (spawnedPotentialProjectiles.Count >= maxPotentialProjectiles)
                break;

            if (!bigCircleSpinning && spawnedPotentialProjectiles.Count > 1)
                SpinBigCircle();

            if (CheckCurrentMana(manaCost))
            {
                PlayerSingleton.Instance.SubtractMana(manaCost);

                spawnedPotentialProjectiles.Add(Instantiate(targetingCirclePrefab, orientationObject.transform.position, orientationObject.transform.rotation, orientationObject.transform));

                var currentRadius = Mathf.Clamp((spawnedPotentialProjectiles.Count - 1) * minCircleRadius, 0, .11f);


                for (int i = 0; i < spawnedPotentialProjectiles.Count; i++)
                {
                    var angle = 360 / spawnedPotentialProjectiles.Count * i * Mathf.Deg2Rad;

                    Vector3 calcPosition = ((Mathf.Cos(angle) * currentRadius * orientationObject.transform.up) + (Mathf.Sin(angle) * currentRadius * orientationObject.transform.right));
                    spawnedPotentialProjectiles[i].transform.rotation = orientationObject.transform.rotation * Quaternion.Euler(90, 0, 0);


                    spawnedPotentialProjectiles[i].transform.position = orientationObject.transform.position + (calcPosition);
                    //iTween.MoveTo(_projectileShooter.activeProjectiles[i].gameObject, iTween.Hash("position", calcPosition, "islocal", true, "time", .3));
                }

            }

            yield return new WaitForSeconds(3);
        }
        yield return null;
    }

    private void SpinBigCircle()
    {
        var spin = orientationObject.AddComponent<ConstantSpin>();

        spin.SetNewAxis("Z");
        spin.SetSpeed(-8);

        bigCircleSpinning = true;
    }
    private void StopBigCircle()
    {
        Destroy(orientationObject.GetComponent<ConstantSpin>());
        bigCircleSpinning = false;
    }
}
