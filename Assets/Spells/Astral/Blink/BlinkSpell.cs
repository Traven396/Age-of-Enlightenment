using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FoxheadDev.GestureDetection;
using TMPro;

[RequireComponent(typeof(TargettingIndicator))]
public class BlinkSpell : SpellBlueprint
{
    public int MaxDistance = 10;
    public GameObject AnchorPrefab;
    public int MaxBlinkCharges = 3;


    private TargettingIndicator _Targeter;

    private Step _CastingSequence;
    private Step _SecondCastingSequence;

    private bool inTargetingMode;
    private RaycastHit hit;
    
    private int currentBlinkCharges;
    private float blinkRechargeTimer;

    private Vector3 lastSavedAnchorPosition;
    private GameObject lastPlacedAnchor;

    private TMP_Text ChargeCounter;
    
    private void Start()
    {
        _Targeter = GetComponent<TargettingIndicator>();
        _Targeter.SetupReferences(this);

        
    }
    private void Update()
    {
        if (!gripPressed)
        {
            _VisualsManager.ReturnCircleToHolder(currentHand);
        }

        if(currentBlinkCharges < MaxBlinkCharges)
        {
            blinkRechargeTimer += Time.deltaTime;

            if(blinkRechargeTimer >= 3.5)
            {
                currentBlinkCharges += 1;

                blinkRechargeTimer = 0;
            }
        }

        ChargeCounter.text = currentBlinkCharges.ToString();
    }

    public override void OnSelect()
    {
        base.OnSelect();

        _CastingSequence = Step.Start();

        _CastingSequence.Then(PremadeGestureLibrary.ReverseSlashInViewDirection(_HandPhysicsTracker, ViewSpaceDirection.Down)).Then(PremadeGestureLibrary.PunchGlobal(_HandPhysicsTracker)).Do(EnterTargetingMode);

        _CastingSequence.Then(PremadeGestureLibrary.SelfSpaceFlick(_HandPhysicsTracker, FlickDirection.Inward)).Then(PremadeGestureLibrary.SelfSpaceFlick(_HandPhysicsTracker, FlickDirection.Outward)).Then(PremadeGestureLibrary.SelfSpaceFlick(_HandPhysicsTracker, FlickDirection.Up)).Then(PremadeGestureLibrary.SelfSpaceFlick(_HandPhysicsTracker, FlickDirection.Down)).Do(PlaceAnchor);

        _CastingSequence.Then(PremadeGestureLibrary.PushInViewDirection(_HandPhysicsTracker, ViewSpaceDirection.InwardHoriz)).Then(PremadeGestureLibrary.ReversePushInViewDirection(_HandPhysicsTracker, ViewSpaceDirection.OutwardHoriz)).Then(PremadeGestureLibrary.PunchGlobal(_HandPhysicsTracker)).Do(TeleportToAnchor);
        
        ///////////////////////////////////////
        
        _SecondCastingSequence = Step.Start();

        _SecondCastingSequence.Then(PremadeGestureLibrary.ReversePunchGlobal(_HandPhysicsTracker)).Do(FinishTeleport);

        ///////////////////////////////////////
        
        lastPlacedAnchor = GameObject.FindGameObjectWithTag("Blink Anchor");
        if (lastPlacedAnchor)
            lastSavedAnchorPosition = lastPlacedAnchor.transform.position;

        currentBlinkCharges = MaxBlinkCharges;

        ChargeCounter = _SpellCircle.GetComponentInChildren<TMP_Text>();
    }
    public override void GripPress()
    {
        base.GripPress();

        _CastingSequence.Reset();
        inTargetingMode = false;
    }
    public override void GripHold()
    {
        base.GripHold();
        
        _CastingSequence.Update();

        if (inTargetingMode && !triggerPressed)
        {
            hit = _TargetManager.RaycastFromHandToGround(currentHand, 10);

            _Targeter.TargetMove(hit);
        }
    }
    public override void GripRelease()
    {
        base.GripRelease();

        if (inTargetingMode)
        {
            _Targeter.TargetReturn();

            _Targeter.UnconfirmLocation();
        }

        inTargetingMode = false;
    }


    public override void TriggerPress()
    {
        base.TriggerPress();

        _SecondCastingSequence.Reset();

        if (inTargetingMode)
        {
            _Targeter.ConfirmButtonMove(hit);

            _Targeter.ConfirmLocation();
        }
    }
    public override void TriggerHold()
    {
        base.TriggerHold();

        if (inTargetingMode)
        {
            _SecondCastingSequence.Update(); 
        }
    }

    public override void TriggerRelease()
    {
        base.TriggerRelease();

        if (inTargetingMode)
        {
            _Targeter.UnconfirmLocation(); 
        }
    }
    private void EnterTargetingMode()
    {
        inTargetingMode = true;
        Debug.Log("Holy shit we targeting");
    }
    private void FinishTeleport()
    {

        if (currentBlinkCharges > 0)
        {
            if (_Targeter.readyToCast)
            {
                _PlayerRb.transform.position = hit.point + Vector3.up * .5f/*(hit.normal * _PlayerRb.transform.position.y)*/;

                currentBlinkCharges -= 1;
            } 
        }
    }
    private void PlaceAnchor()
    {
        lastSavedAnchorPosition = _PlayerRb.transform.position;

        if (lastPlacedAnchor)
            Destroy(lastPlacedAnchor);

        lastPlacedAnchor = Instantiate(AnchorPrefab, _PlayerRb.transform.position, Quaternion.identity);
    }
    private void TeleportToAnchor()
    {
        if (lastPlacedAnchor)
        {

            if (currentBlinkCharges >= 3)
            {
                _PlayerRb.transform.position = lastSavedAnchorPosition;
                Destroy(lastPlacedAnchor);

                lastPlacedAnchor = null;

                currentBlinkCharges -= 3;
            }
        }
    }


}
