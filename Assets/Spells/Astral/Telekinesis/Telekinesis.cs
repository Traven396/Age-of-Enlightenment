using FoxheadDev.GestureDetection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Telekinesis : SpellBlueprint
{
    public float _PowerModeThrowDistance;
    public Vector3 _PowerModeThrowSize;
    public float _PowerModeThrowPower;


    private enum TelekinesisMode
    {
        Precision,
        Personal,
        Power
    }

    private TelekinesisMode CurrentMode = TelekinesisMode.Power;

    bool throwCooldown = false;

    private float remainingGestureCooldown = 2;
    private bool startGestureCooldown;
    private Step _CastingSequence;



    #region Precision
    //Precision mode Variables
    private TargettableEntity CurrentTarget;
    public GameObject _HookPrefab;

    private GameObject SpawnedHookPrefab;
    private TeleHookScript SpawnedHookReference;
    private ConfigurableJoint SpawnedHookJoint;

    private bool isGrabbingObject;
    private bool usedGravityBefore;
    #endregion
    public override void OnSelect()
    {
        _CastingSequence = Step.Start();

        var root = _CastingSequence.Then(PremadeGestureLibrary.ZAxisFlick(_HandPhysicsTracker, 4)).Do(GestureCooldown);

        //This is for transitioning to power mode
        //A punch forward and then back, the rotation of the hand/pucnh doesnt matter
        root.Then(PremadeGestureLibrary.PunchInViewDirection(_HandPhysicsTracker, ViewSpaceDirection.Forward)).Do(GestureCooldown).Then(PremadeGestureLibrary.ReversePunchInViewDirection(_HandPhysicsTracker, ViewSpaceDirection.Back)).Do(SwapToPowerMode);

        //Transition to precision mode
        //Some kind of sniper bolt action reload. Like a pull down and then forward/up
        root.Then(PremadeGestureLibrary.ReversePunchInViewDirection(_HandPhysicsTracker, ViewSpaceDirection.Down)).Do(GestureCooldown).Then(PremadeGestureLibrary.PunchInViewDirection(_HandPhysicsTracker, ViewSpaceDirection.Up)).Do(SwapToPrecisionMode);

        //Transition to personal mode
        //maybe something like palm inwards, and palm downwards. Like a "cleansing" motion
    }

    public override void OnDeselect()
    {
        base.OnDeselect();

        _TargetManager.ClearCurrentTarget(currentHand);

        if (SpawnedHookPrefab)
            Destroy(SpawnedHookPrefab);
        if(gripPressed && isGrabbingObject)
            CurrentTarget.GetTargetRB().useGravity = usedGravityBefore;
    }

    public override void TriggerPress()
    {
        base.TriggerPress();
    }

    public override void TriggerHold()
    {
        base.TriggerHold();

        switch (CurrentMode)
        {
            case TelekinesisMode.Precision:

                break;
            case TelekinesisMode.Personal:

                break;
            case TelekinesisMode.Power:

                var velocityMagnitude = _HandPhysicsTracker.Velocity.magnitude;
                //Putting a cooldown boolean. Basically the player needs to slow their hand down at least once before the spell will try and throw again. This way it doesnt fire like a thousand times when they initially throw
                if (!throwCooldown)
                {
                    
                    if (velocityMagnitude > 2.5f)
                    {
                        throwCooldown = true;
                        
                        RaycastHit[] hits = _TargetManager.BodyBoxCastAll(_PowerModeThrowDistance, _PowerModeThrowSize);
                        foreach (RaycastHit hit in hits)
                        {
                            if (hit.collider.attachedRigidbody)
                            {
                                var force = _HandPhysicsTracker.Direction * _PowerModeThrowPower * 2.5f;
                                hit.collider.attachedRigidbody.AddForce(force, ForceMode.Acceleration);
                            }
                        }

                        Debug.Log("THROW");
                    }

                    
                }
                if (velocityMagnitude < .8f && throwCooldown)
                {
                    throwCooldown = false;
                    Debug.Log("Throw reset");
                }
                break;
        }
    }

    public override void TriggerRelease()
    {
        base.TriggerRelease();


        throwCooldown = false;
    }

    public override void GripPress()
    {
        base.GripPress();


        switch (CurrentMode)
        {
            case TelekinesisMode.Precision:
                if (CurrentTarget)
                {
                    SpawnedHookPrefab.transform.position = CurrentTarget.transform.position;
                    //SpawnedHookReference._body = CurrentTarget.GetTargetRB();
                    SpawnedHookJoint.connectedBody = CurrentTarget.GetTargetRB();

                    usedGravityBefore = CurrentTarget.GetTargetRB().useGravity;

                    CurrentTarget.GetTargetRB().useGravity = false;
                    isGrabbingObject = true;
                }
                
                break;
            case TelekinesisMode.Personal:
                
                
                break;
            case TelekinesisMode.Power:
         
                
                break;
        }
    }

    public override void GripHold()
    {
        base.GripHold();

        switch (CurrentMode)
        {
            case TelekinesisMode.Precision:
                if (CurrentTarget)
                {
                    
                }

                break;
            case TelekinesisMode.Personal:


                break;
            case TelekinesisMode.Power:


                break;
        }
    }
    public override void GripRelease()
    {
        base.GripRelease();

        switch (CurrentMode)
        {
            case TelekinesisMode.Precision:
                if (SpawnedHookReference._body || SpawnedHookJoint.connectedBody)
                {
                    SpawnedHookReference._body = null;
                    SpawnedHookJoint.connectedBody = null;

                    CurrentTarget.GetTargetRB().useGravity = usedGravityBefore;
                    isGrabbingObject = false;
                }

                break;
            case TelekinesisMode.Personal:


                break;
            case TelekinesisMode.Power:


                break;
        }
    }

    private void GestureCooldown()
    {
        startGestureCooldown = true;
        remainingGestureCooldown = 2;

        Debug.Log("Gesture was just completed");
    }
    private void SwapToPowerMode()
    {
        CurrentMode = TelekinesisMode.Power;
        Debug.Log("In power mode");

        _CastingSequence.Reset();

        if (SpawnedHookPrefab)
            Destroy(SpawnedHookPrefab);
    }
    private void SwapToPrecisionMode()
    {
        CurrentMode = TelekinesisMode.Precision;
        Debug.Log("In precision mode");

        _CastingSequence.Reset();

        SpawnedHookPrefab = Instantiate(_HookPrefab, _HandTransform);
        SpawnedHookReference = SpawnedHookPrefab.GetComponent<TeleHookScript>();
        SpawnedHookJoint = SpawnedHookPrefab.GetComponent<ConfigurableJoint>();
    }
    private void SwapToPersonalMode()
    {
        CurrentMode = TelekinesisMode.Personal;


        if (SpawnedHookPrefab)
            Destroy(SpawnedHookPrefab);
    }
    private void Update()
    {
        if (startGestureCooldown)
        {
            remainingGestureCooldown -= Time.deltaTime;
            if (remainingGestureCooldown <= 0)
            {
                startGestureCooldown = false;
                _CastingSequence.Reset();
            }
        }

        if (!gripPressed && !triggerPressed)
            _CastingSequence.Update();


        if(CurrentMode == TelekinesisMode.Precision && !gripPressed)
        {
            CurrentTarget = _TargetManager.ReturnClosestTelekinesisTargetEntity(currentHand, 15, .2f, _HandPhysicsTracker.UniveralPalm);
        }
    }
}
