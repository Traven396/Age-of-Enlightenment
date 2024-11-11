using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FoxheadDev.GestureDetection;

[RequireComponent(typeof(TargettingIndicator))]
[RequireComponent(typeof(ObjectSpawn))]
[RequireComponent(typeof(ApplyMotion))]
public class GravityControl : SpellBlueprint
{
    public float MaxDistance = 13;

    private Step _CastingSequence;

    private TargettingIndicator _Targeter;
    private ObjectSpawn _ObjectSpawn;
    private ApplyMotion _ApplyMotion;

    private bool inTargetingMode;
    private RaycastHit hit;
    private GravityZoneController latestZoneController;

    private bool zoneAlreadySpawned = false;
    private float initialYLevel = 0;


    private bool inBurstMode;
    private float burstChargeTime;


    private void Start()
    {
        _Targeter = GetComponent<TargettingIndicator>();
            _Targeter.SetupReferences(this);
        _ObjectSpawn = GetComponent<ObjectSpawn>();
        _ApplyMotion = GetComponent<ApplyMotion>();
    }
    public override void OnSelect()
    {
        base.OnSelect();

        _CastingSequence = Step.Start();

        _CastingSequence.Then(PremadeGestureLibrary.PushInViewDirection(_HandPhysicsTracker, ViewSpaceDirection.Forward))
            .Then(PremadeGestureLibrary.ZAxisFlick(_HandPhysicsTracker))
            .Then(PremadeGestureLibrary.PalmPointUpSelfSpace(_HandPhysicsTracker)).Do(EnterTargetMode);

        _CastingSequence.Then(PremadeGestureLibrary.ReversePushInViewDirection(_HandPhysicsTracker, ViewSpaceDirection.OutwardHoriz))
            .Then(PremadeGestureLibrary.ReversePushInViewDirection(_HandPhysicsTracker, ViewSpaceDirection.Back))
            .Do(BeginBurstCharge);



        //Set a boolean and then check that in each of the input events. Set this boolean in the method we just created. Good luck!
    }
    private void Update()
    {
        if(!inTargetingMode && !triggerPressed)
        {
            _VisualsManager.ReturnCircleToHolder(currentHand);
        }
    }

    public override void GripHold()
    {
        base.GripHold();

        _CastingSequence.Update();

        if (inTargetingMode)
        {
            hit = _TargetManager.RaycastFromHandToGround(currentHand, MaxDistance);

            _Targeter.TargetMove(hit);
        }


        if (inBurstMode)
        {
            if (burstChargeTime < 3)
                burstChargeTime += Time.deltaTime;
        }
    }
    public override void GripRelease()
    {
        base.GripRelease();

        _CastingSequence.Reset();

        _Targeter.TargetReturn();

        inTargetingMode = false;


        if (inBurstMode)
        {
            var gestureCompleted = false;
            if(currentHand == LeftRight.Left)
            {
                if (_HandPhysicsTracker.SelfSpaceVelocity.x > 2f && _HandPhysicsTracker.SelfSpaceVelocity.MostlyX())
                {
                    gestureCompleted = true;
                    Debug.Log("Left hand gesture");
                }
            }
            else
            {
                if (_HandPhysicsTracker.SelfSpaceVelocity.x < -2f && _HandPhysicsTracker.SelfSpaceVelocity.MostlyX())
                {
                    Debug.Log("Right hand gesture");
                    gestureCompleted = true;
                }
            }


            if (gestureCompleted)
            {
                RaycastHit[] hits = _TargetManager.HandSphereCastAll(currentHand, 7, 1.5f, _HandPhysicsTracker.UniveralPalm);

                foreach (RaycastHit hit in hits)
                {
                    Rigidbody rb = hit.rigidbody;
                    if (hit.collider.TryGetComponent(out IEntity entity))
                    {
                        Vector3 forceDirection = (rb.position - _HandTransform.position).normalized;
                        entity.ApplyMotion(forceDirection * _ApplyMotion.forceMultiplier * burstChargeTime, _ApplyMotion.forceType);
                    }
                    else if (rb)
                    {
                        Vector3 forceDirection = (rb.position - _HandTransform.position).normalized;
                        _ApplyMotion.Cast(rb, forceDirection * burstChargeTime);
                    }
                }
            }

            inBurstMode = false;
            burstChargeTime = 0;


            iTween.ScaleTo(_SpellCircle, Vector3.one, .2f);
        }
    }

    public override void TriggerPress()
    {
        base.TriggerPress();

        if (inTargetingMode && !zoneAlreadySpawned && _Targeter.readyToCast)
        {
            _Targeter.ConfirmButtonMove(hit);

            _Targeter.ConfirmLocation();

            _ObjectSpawn.Cast(_SpellCircle.transform);

            latestZoneController = _ObjectSpawn.instantiatedObject.GetComponentInChildren<GravityZoneController>();
            latestZoneController.ZoneDeath.AddListener(ZoneDeathEvent);


            inTargetingMode = false;
            zoneAlreadySpawned = true;
            
        }

        initialYLevel = _PlayerRb.transform.InverseTransformPoint(_HandTransform.position).y;
    }
    public override void TriggerHold()
    {
        base.TriggerHold();

        if (zoneAlreadySpawned)
        {
            var newYLevel = _PlayerRb.transform.InverseTransformPoint(_HandTransform.position).y;

            //latestZoneController.GravityModifier = (Mathf.Clamp((newYLevel - initialYLevel), -1, 2));
            latestZoneController.SetGravityModifier(Mathf.Clamp((initialYLevel - newYLevel) * 3.5f, -1, 2));
        }
    }
    public override void TriggerRelease()
    {
        base.TriggerRelease();

        _Targeter.UnconfirmLocation();

        if (zoneAlreadySpawned)
            latestZoneController.StartCountdown();
    }
    private void EnterTargetMode()
    {
        if (!zoneAlreadySpawned)
        {
            inTargetingMode = true; 
        }
    }

    private void ZoneDeathEvent()
    {
        //latestZoneController.ZoneDeath.RemoveAllListeners();
        zoneAlreadySpawned = false;
    }

    private void BeginBurstCharge()
    {
        inBurstMode = true;

        iTween.ScaleAdd(_SpellCircle, Vector3.one * .5f, .2f);
    }
}
