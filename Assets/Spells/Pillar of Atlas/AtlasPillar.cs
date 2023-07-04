using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TargettingIndicator))]
public class AtlasPillar : SpellBlueprint
{
    [Header("Spell Settings")]
    public float maxDistance;
    public float maxAngle;
    public int maxPillars = 2;

    [Header("MANA")]
    public int ManaCost = 30;

    private TargettingIndicator _target;
    private ThrowGesture upGesture;
    private ObjectSpawn _spawn;

    private RaycastHit hit, lastHit;
    private bool gesturePerformed = false, validSurface = false;
    private Vector3 startPos;
    private List<GameObject> spawnedPillars = new List<GameObject>();
    Material pillarMaterial;

    private AtlasPillarObject pilObj;

    private void Start()
    {
        _target = GetComponent<TargettingIndicator>();
            _target.SetupReferences(this);
        upGesture = GetComponent<ThrowGesture>();
        _spawn = GetComponent<ObjectSpawn>();
    }
    private void Update()
    {
        if (!triggerPressed)
        {
            _visualsManager.ReturnCircleToHolder(currentHand);
        }
    }
    public override void TriggerPress()
    {
        base.TriggerPress();

        if (gripPressed)
            _target.readyToCast = false;
    }
    public override void TriggerHold()
    {
        base.TriggerHold();

        hit = _targetManager.RaycastFromHandToGround(currentHand, maxDistance);

        _target.TargetMove(hit);
    }
    public override void TriggerRelease()
    {
        base.TriggerRelease();

        gesturePerformed = false;

        validSurface = false;

        startPos = Vector3.zero;

        _target.TargetReturn();

        if (pilObj != null)
            pilObj.SetRaising(false);
    }
    public override void GripPress()
    {
        base.GripPress();
        if (triggerPressed)
        {
            if (hit.transform.gameObject.TryGetComponent(out SurfaceMaterial entity) && entity.EarthSurface)
            {
                _target.ConfirmButtonMove(hit);

                _target.ConfirmLocation();

                lastHit = hit;
                
                validSurface = true;
            }
        }
    }
    public override void GripRelease()
    {
        base.GripRelease();

        if (gesturePerformed)
        {
            _spawn.instantiatedObject.GetComponent<SummonedObjectBehavior>().StartDeathCountdown();
            
        }

        gesturePerformed = false;

        _target.UnconfirmLocation();

        startPos = Vector3.zero;

        validSurface = false;

        if(pilObj != null)
            pilObj.SetRaising(false);
    }
    public override void GripHold()
    {
        base.GripHold();
        if (validSurface)
        {
            //Check if any pillars have been destroyed since last casting
            for (int i = spawnedPillars.Count - 1; i >= 0; i--)
            {
                if (!spawnedPillars[i])
                {
                    spawnedPillars.Remove(spawnedPillars[i]);
                }
            }
            if (spawnedPillars.Count <= maxPillars)
            {
                if (upGesture.GesturePerformed(_gestureManager, out Vector3 direction) && !gesturePerformed && _target.readyToCast)
                {
                    startPos = _handLocation.position - playerRb.transform.position;
                    if (Vector3.Angle(-_gestureManager.rightVectorSafe.normalized, lastHit.normal.normalized) <= maxAngle)
                    {
                        //Make sure the player cant summon two in one casting
                        gesturePerformed = true;

                        //Set values so that the pillar looks correct upon spawning.
                        _spawn.SetRotationOffset(Quaternion.LookRotation(lastHit.normal).eulerAngles);
                        _spawn.Cast(spellCircle.transform);

                        pilObj = _spawn.instantiatedObject.GetComponent<AtlasPillarObject>();
                        pilObj.SetRaising(true);
                        
                        //pillarMaterial.mainTextureScale = new Vector2(Mathf.Sqrt(pillarMaterial.mainTextureScale.x), Mathf.Sqrt(pillarMaterial.mainTextureScale.y));

                        _spawn.instantiatedObject.GetComponentInChildren<Renderer>().material.color = lastHit.transform.GetComponent<Renderer>().material.color;

                        //Add the new pillar to the list
                        spawnedPillars.Add(_spawn.instantiatedObject);

                        //More visual effects so that the circle for the spell grows in size more
                        iTween.ScaleTo(spellCircle, Vector3.one * 7.5f, .3f);

                        //Subtract mana from the player
                        Player.Instance.SubtractCurrentMana(ManaCost);
                    }
                }  
            }
        }
        if (gesturePerformed)
        {
            var offset = MathF.Round(Vector3.Dot((_handLocation.position - playerRb.transform.position)- startPos, lastHit.normal), 2) * 100;
            offset = Mathf.Clamp(offset, 0, 100);

            pilObj.SetBlendAmount(offset);

            //iTween.ScaleUpdate(_spawn.instantiatedObject, new Vector3(1, 1, .1f + Noffset), .1f);
            //iTween.MoveUpdate(_spawn.instantiatedObject, iTween.Hash("position", spellCircle.transform.position + (lastHit.normal * Noffset)/2, "islocal", true, "time", .1f));
        }
    }
}
