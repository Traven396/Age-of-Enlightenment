using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StonesFury : SpellBlueprint
{
    private ObjectSpawn _objectSpawn;
    private PositionTargetIndicator _positionTarget;
    private IMovement _requiredGesture;

    [SerializeField] private float distance;

    private bool performedThisPress = false;
    RaycastHit hit = new();

    void Start()
    {
        _objectSpawn = GetComponent<ObjectSpawn>();
        _positionTarget = GetComponent<PositionTargetIndicator>();
        _requiredGesture = GetComponent<IMovement>();


        spellCircle = circleHolder.transform.GetChild(circleHolder.transform.childCount - 1).gameObject;
    }

    public override void GripPress()
    {
        base.GripPress();
        if(triggerPressed)
            iTween.ScaleTo(spellCircle, Vector3.one*3, 1f);
    }

    public override void GripRelease()
    {
        base.GripRelease();
        iTween.ScaleTo(spellCircle, Vector3.one, 1f);
    }

    public override void TriggerHold()
    {
        //Check if the grip is pressed, if so continue on to the summoning part
        if (gripPressed)
        {
            //If the hand has not already been summoned, and the targetted surface is the top of something
            if (!performedThisPress && hit.normal == Vector3.up)
            {
                //Wait until the player pulls their hand up to summon it
                if (_requiredGesture.GesturePerformed(_gestureManager, out Vector3 direction))
                {
                    _objectSpawn.Cast(_positionTarget.GetCurrentTran());
                    performedThisPress = true;
                }
            }
            if(Vector3.Distance(spellCircle.transform.position, _positionTarget.GetCurrentTran().position) > .01f)
            {
                iTween.MoveUpdate(spellCircle, _positionTarget.MoveIndicator(hit.point).position + new Vector3(0, 0.05f, 0), .5f);
            }
            return;
        }
        //A raycast from the hand, first step of targetting.
        //This comes after the grip pressed to make sure the circle doesnt move if the grip is pressed
        hit = _targetManager.RaycastFromHand(currentHand, distance);

        //If the hand is not already summoned, constantly move the targetting indicator
        if (!performedThisPress)
        {
            if (hit.normal == Vector3.up)
            {
                if (spellCircle.transform.parent != null)
                    spellCircle.transform.parent = null;
                iTween.MoveUpdate(spellCircle, _positionTarget.MoveIndicator(hit.point).position + new Vector3(0, 0.05f, 0), .5f);
                iTween.RotateUpdate(spellCircle, Vector3.zero, .5f);
            }
            else
            {
                spellCircle.transform.parent = circleHolder.transform;

                spellCircle.transform.position = circleHolder.transform.position;

                spellCircle.transform.rotation = circleHolder.transform.rotation;
            }
        }
    }

    public override void TriggerRelease()
    {
        base.TriggerRelease();

        spellCircle.transform.parent = circleHolder.transform;

        iTween.ScaleTo(spellCircle, Vector3.one, .6f);

        spellCircle.transform.rotation = circleHolder.transform.rotation;

        performedThisPress = false;
    }

    public void Update()
    {
        if (spellCircle)
        {
            if (!triggerPressed)
            {
                if (Vector3.Distance(spellCircle.transform.position, circleHolder.transform.position) > .05f)
                {
                    iTween.MoveUpdate(spellCircle, circleHolder.transform.position, .2f);
                }
            } 
        }
    }
}
