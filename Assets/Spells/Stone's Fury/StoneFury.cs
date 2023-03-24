using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneFury : SpellBlueprint
{
    private ObjectSpawn _objectSpawn;
    //private PositionTargetIndicator _targetter;
    private IMovement _requiredGesture;

    [SerializeField]
    private float maxDistance;

    private bool performed = false, validCast = true;
    RaycastHit hit = new RaycastHit();

    private void Start()
    {
        _objectSpawn = GetComponent<ObjectSpawn>();
        //_targetter = GetComponent<PositionTargetIndicator>();
        _requiredGesture = GetComponent<IMovement>();
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
            validCast = false;
    }
    public override void TriggerHold()
    {
        base.TriggerHold();
        if (!gripPressed && validCast)
        {
            hit = _targetManager.RaycastFromHand(currentHand, maxDistance);
            if (hit.normal == Vector3.up)
            {
                if (spellCircle.transform.parent != null)
                    spellCircle.transform.parent = null;
                iTween.MoveUpdate(spellCircle, hit.point + new Vector3(0, 0.05f, 0), .5f);
                iTween.RotateUpdate(spellCircle, Vector3.zero, .5f);
            }
            else
            {
                spellCircle.transform.parent = circleHolder.transform;

                _visualsManager.ReturnCircleToHolder(currentHand);
            }
        }
    }
    public override void TriggerRelease()
    {
        base.TriggerRelease();
        spellCircle.transform.parent = circleHolder.transform;

        iTween.ScaleTo(spellCircle, Vector3.one, .6f);
    }
    public override void GripPress()
    {
        base.GripPress();
        if (triggerPressed)
        {
            iTween.ScaleTo(spellCircle, Vector3.one * 3, 1f);
        }
    }
    public override void GripHold()
    {
        base.GripHold();
        if (triggerPressed && validCast)
        {
            if (hit.normal == Vector3.up && !performed)
            {
                if (_requiredGesture.GesturePerformed(_gestureManager, out Vector3 direction))
                {
                    _objectSpawn.Cast(hit.point);
                    performed = true;
                }
            }
            if (Vector3.Distance(spellCircle.transform.position, hit.point) > .01f)
            {
                iTween.MoveUpdate(spellCircle, hit.point + new Vector3(0, 0.05f, 0), .5f);
            } 
        }
    }
    public override void GripRelease()
    {
        base.GripRelease();

        iTween.ScaleTo(spellCircle, Vector3.one, 1f);

        performed = false;
        validCast = true;
    }
}
