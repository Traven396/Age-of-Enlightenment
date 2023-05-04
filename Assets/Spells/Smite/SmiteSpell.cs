using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TargettingIndicator))]
[RequireComponent(typeof(ObjectSpawn))]
public class SmiteSpell : SpellBlueprint
{
    public float maxDistance = 10;

    private SwingDownGesture _swingDown;
    private SwingUpGesture _swingUp;
    private TargettingIndicator _targetter;
    private ObjectSpawn _objectSpawn;

    private RaycastHit hit;
    private bool swungUpPerformed;
    private bool swungDownPerformed;

    private List<LightningStrike> summonedLightningPoints = new List<LightningStrike>();
    private void Start()
    {
        _swingDown = GetComponent<SwingDownGesture>();
        _swingUp = GetComponent<SwingUpGesture>();
        _targetter = GetComponent<TargettingIndicator>();
            _targetter.SetupReferences(this);
        _objectSpawn = GetComponent<ObjectSpawn>();
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
        {
            _targetter.readyToCast = false;
        }
    }
    public override void TriggerHold()
    {
        base.TriggerHold();

        hit = _targetManager.RaycastFromHandToGround(currentHand, maxDistance);

        _targetter.TargetMove(hit);
    }
    public override void TriggerRelease()
    {
        base.TriggerRelease();

        _targetter.TargetReturn();
    }
    public override void GripPress()
    {
        base.GripPress();
        if (triggerPressed)
        {
            _targetter.ConfirmButtonMove(hit);

            _targetter.ConfirmLocation();
        }
    }

    public override void GripRelease()
    {
        base.GripRelease();

        _targetter.UnconfirmLocation();

        swungUpPerformed = false;
        swungDownPerformed = false;
    }

    public override void GripHold()
    {
        if (!swungUpPerformed)
        {
            if (_swingUp.GesturePerformed(_gestureManager, out Vector3 direction) && _targetter.readyToCast)
            {
                var yOffset = 4f - Random.Range(0, .3f);
                if (Physics.Raycast(spellCircle.transform.position + new Vector3(0, .1f, 0), Vector3.up, out RaycastHit smallHit, 4))
                {
                    yOffset = smallHit.point.y - spellCircle.transform.position.y;
                }
                _objectSpawn.Cast(spellCircle.transform);
                iTween.MoveBy(_objectSpawn.instantiatedObject, iTween.Hash("y", yOffset,
                                                                            "oncompletetarget", gameObject,
                                                                            "oncomplete", "FinishedGoingUp",
                                                                            "time", 1f,
                                                                            "oncompleteparams", _objectSpawn.instantiatedObject));
                iTween.ScaleTo(_objectSpawn.instantiatedObject, Vector3.one * 5f, 2f);

                swungUpPerformed = true;
            }
        }
        if (!swungDownPerformed)
        {
            if(_swingDown.GesturePerformed(_gestureManager, out Vector3 direction))
            {
                foreach (LightningStrike lStrike in summonedLightningPoints)
                {
                    if (lStrike.gameObject != null)
                    {
                        lStrike.Strike();
                    }
                    else
                    {
                        return;
                    }
                }
                summonedLightningPoints = new List<LightningStrike>();
                swungDownPerformed = true;
            }
        }
    }
    private void FinishedGoingUp(GameObject passthrough)
    {
        summonedLightningPoints.Add(passthrough.GetComponent<LightningStrike>());
    }
    /*Ok, future me. Heres the plan. We should make it so when you perform the gesture for the spell
     it starts a coroutine that moves the circle up and does the lightning strike. Do the raycast thing we thought
    about where it checks if there is a ceiling or not above it to see how high to move it

    While this corotuine is running, or at the start, set a boolean to continue the rest of the script to false. That way the spell circle
    doesnt come flying back in the middle of the lightning strike. At the end of the coroutine call all the targetting and other
    return things so that the spell circle comes back and can be cast again.

    You might be wondering, like I just was, what happens if the player deactivates the spell. Well I thought I had a plan, but upon
    just thinking it over more I realized it wouldnt work. So here are some ideas that might work
    - Put all the strike spell on the spell circle and make it so the script checks if lightning is currently striking before
    deleting the spell circle, instead destroying it on a timer once deactivated. You just need to set this spells reference to the circle to null 
    after doing everything
    - Make a copy of the spell circle and do a substitution jutsu style thing to make it so there is a version that wont get destroyed
    along with the spell when swapping. Have this be the object that is spawned and just make it a big animation thing
    - 
     */

}
