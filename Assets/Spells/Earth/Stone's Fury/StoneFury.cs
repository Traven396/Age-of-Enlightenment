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
    public override void GripHold()
    {
        base.GripHold();
        
        if (!performed)
        {
            if (_requiredGesture.GesturePerformed(_HandPhysicsTracker, out Vector3 direction) && _targetter.readyToCast)
            {
                if (CheckCurrentMana(manaCost))
                {
                    _objectSpawn.Cast(_SpellCircle.transform);

                    _objectSpawn.instantiatedObject.GetComponent<SummonedObjectBehavior>().BeginLifeCycle();

                    PlayerSingleton.Instance.SubtractMana(manaCost);

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
