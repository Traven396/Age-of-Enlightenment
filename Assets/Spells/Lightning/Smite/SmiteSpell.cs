using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TargettingIndicator))]
[RequireComponent(typeof(ObjectSpawn))]
public class SmiteSpell : SpellBlueprint
{
    public float maxDistance = 10;
    public int ManaCost = 15;

    private SwingDownGesture _swingDown;
    private TargettingIndicator _targetter;
    private ObjectSpawn _objectSpawn;

    private RaycastHit hit;
    private bool swungDownPerformed;

    private void Start()
    {
        _swingDown = GetComponent<SwingDownGesture>();
        _targetter = GetComponent<TargettingIndicator>();
            _targetter.SetupReferences(this);
        _objectSpawn = GetComponent<ObjectSpawn>();
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
        if (gripPressed)
        {
            _targetter.readyToCast = false;
        }
    }
    public override void TriggerHold()
    {
        base.TriggerHold();

        hit = _TargetManager.RaycastFromHandToGround(currentHand, maxDistance);

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

        swungDownPerformed = false;
    }

    public override void GripHold()
    {
        if (!swungDownPerformed)
        {
            if(_swingDown.GesturePerformed(_HandPhysicsTracker, out Vector3 direction) && CheckCurrentMana(ManaCost))
            {
                _objectSpawn.Cast(_SpellCircle.transform);

                _objectSpawn.instantiatedObject.GetComponent<SummonedObjectBehavior>().BeginLifeCycle();

                swungDownPerformed = true;

                PlayerSingleton.Instance.SubtractMana(ManaCost);
            }
        }
    }

}
