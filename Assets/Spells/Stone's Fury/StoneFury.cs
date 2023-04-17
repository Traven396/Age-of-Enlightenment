using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneFury : SpellBlueprint
{
    private ObjectSpawn _objectSpawn;
    private TargettingIndicator _targetter;
    private IMovement _requiredGesture;

    [SerializeField]
    private float maxDistance;
    [SerializeField]
    private int manaCost = 10;

    private bool performed = false;
    RaycastHit hit = new RaycastHit();

    private void Start()
    {
        _objectSpawn = GetComponent<ObjectSpawn>();

        _targetter = GetComponent<TargettingIndicator>();
        _targetter.SetupReferences(this);

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
    public override void GripHold()
    {
        base.GripHold();
        if (triggerPressed)
        {
            if (!performed)
            {
                if (_requiredGesture.GesturePerformed(_gestureManager, out Vector3 direction) && _targetter.readyToCast)
                {
                    _objectSpawn.Cast(spellCircle.transform);

                    Player.Instance.SubtractCurrentMana(manaCost);
                    
                    performed = true;
                }
            }
        }
    }
    public override void GripRelease()
    {
        base.GripRelease();

        _targetter.UnconfirmLocation();

        performed = false;
    }
}
