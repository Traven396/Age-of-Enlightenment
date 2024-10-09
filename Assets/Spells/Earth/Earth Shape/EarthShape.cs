using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TargettingIndicator))]
public class EarthShape : SpellBlueprint
{
    [Header("Spell Settings")]
    public float _maxDistance;
    public float _maxAngle;
    public float _timeBetweenChangingObjects = 2;
    public int _maxSummonedObjects = 2;
    [Space(10f)]
    public GameObject _pillarPrefab;
    public GameObject _wallPrefab;

    [Header("MANA")]
    public int _manaCost = 30;

    private TargettingIndicator Targetter;
    private ThrowGesture PullGesture;
    private ObjectSpawn Spawner;

    private RaycastHit hit, lastHit;
    private bool gestureAlreadyPerformed = false, validSurface = false, timerGoing = false, pillarModeActive = false;
    private Vector3 startPos;
    private List<GameObject> currentSpawnedObjects = new List<GameObject>();
    private float timer = 0;

    private EarthShapeObjectBehaviour spawnedObject;

    private void Start()
    {
        Targetter = GetComponent<TargettingIndicator>();
            Targetter.SetupReferences(this);
        PullGesture = GetComponent<ThrowGesture>();
        Spawner = GetComponent<ObjectSpawn>();
    }
    private void Update()
    {
        if (!triggerPressed)
        {
            _VisualsManager.ReturnCircleToHolder(currentHand);
        }
        if (timerGoing)
        {
            timer += Time.deltaTime;
        }
    }
    public override void TriggerPress()
    {
        base.TriggerPress();

        if (gripPressed)
            Targetter.readyToCast = false;
    }
    public override void TriggerHold()
    {
        base.TriggerHold();

        hit = _TargetManager.RaycastFromHandToGround(currentHand, _maxDistance);

        Targetter.TargetMove(hit);
    }
    public override void TriggerRelease()
    {
        base.TriggerRelease();

        gestureAlreadyPerformed = false;

        validSurface = false;

        startPos = Vector3.zero;

        Targetter.TargetReturn();
    }
    public override void GripPress()
    {
        base.GripPress();
        if (triggerPressed)
        {
            Spawner.SetSpawnObject(_wallPrefab);

            if (hit.transform)
            {
                if (hit.transform.gameObject.TryGetComponent(out SurfaceMaterial entity) && entity.EarthSurface)
                {
                    Targetter.ConfirmButtonMove(hit);

                    Targetter.ConfirmLocation();

                    lastHit = hit;

                    validSurface = true;

                    timerGoing = true;

                    pillarModeActive = false;
                } 
            }
        }
    }
    public override void GripRelease()
    {
        base.GripRelease();

        if (gestureAlreadyPerformed)
        {
            Spawner.instantiatedObject.GetComponent<SummonedObjectBehavior>().BeginLifeCycle();
            
        }

        gestureAlreadyPerformed = false;

        Targetter.UnconfirmLocation();

        startPos = Vector3.zero;

        validSurface = false;

        pillarModeActive = false;

        timerGoing = false;
        timer = 0;
    }
    public override void GripHold()
    {
        base.GripHold();
        if (validSurface)
        {
            //If the timer passes the threshold for changing objects, change it and stop counting
            if(timer >= _timeBetweenChangingObjects)
            {
                Spawner.SetSpawnObject(_pillarPrefab);
                timerGoing = false;
                pillarModeActive = true;

                iTween.ScaleTo(_SpellCircle, Vector3.one * 7.5f, .3f);
            }



            //Check if any pillars have been destroyed since last casting
            for (int i = currentSpawnedObjects.Count - 1; i >= 0; i--)
            {
                if (!currentSpawnedObjects[i])
                {
                    currentSpawnedObjects.Remove(currentSpawnedObjects[i]);
                }
            }
            if (currentSpawnedObjects.Count <= _maxSummonedObjects)
            {
                if (PullGesture.GesturePerformed(_HandPhysicsTracker, out Vector3 direction) && !gestureAlreadyPerformed && Targetter.readyToCast)
                {
                    if (Vector3.Angle(-_GestureManager.rightVectorSafe.normalized, lastHit.normal.normalized) <= _maxAngle)
                    {
                        timerGoing = false;

                        startPos = _HandTransform.position - _PlayerRb.transform.position;

                        //Make sure the player cant summon two in one casting
                        gestureAlreadyPerformed = true;

                        //Create teh actual pillar
                        Spawner.Cast(_SpellCircle.transform);



                        #region Rotate towards player only on local Y axis
                        //var child = Spawner.instantiatedObject.transform.GetChild(0);

                        

                        Vector3 playerInLocalSpace = Spawner.instantiatedObject.transform.InverseTransformPoint(playerPhys.transform.position);

                        playerInLocalSpace = new Vector3(playerInLocalSpace.x, 0, playerInLocalSpace.z);

                        Vector3 playerInNewWorldSpace = Spawner.instantiatedObject.transform.TransformPoint(playerInLocalSpace);

                        //Spawner.instantiatedObject.transform.LookAt(playerInNewWorldSpace, spellCircle.transform.up);

                        #endregion


                        spawnedObject = Spawner.instantiatedObject.GetComponentInChildren<EarthShapeObjectBehaviour>();

                        spawnedObject.ChangeMaterial(lastHit.transform.GetComponent<Renderer>().material.color);

                        spawnedObject.RotateTowardsPoint(playerInNewWorldSpace, _SpellCircle.transform.up);

                        //Add the new pillar to the list
                        currentSpawnedObjects.Add(Spawner.instantiatedObject);

                        //Subtract mana from the player
                        PlayerSingleton.Instance.SubtractMana(_manaCost);
                    }
                }  
            }
        }
    }
    public override void GripHoldFixed()
    {
        base.GripHoldFixed();
        if (gestureAlreadyPerformed)
        {
            var offset = MathF.Round(Vector3.Dot((_HandTransform.position - _PlayerRb.transform.position) - startPos, lastHit.normal), 4) * 10;
            if(pillarModeActive)
                offset = Mathf.Clamp(offset, 0, 6.5f);
            else
                offset = Mathf.Clamp(offset, 0, 2f);

            spawnedObject.ChangeHeight(offset);
        }
    }
}
