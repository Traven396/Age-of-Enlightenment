using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catapult : SpellBlueprint
{
    private ITargetable potentialTarget;
    private ICastable castEffect;
    private IMovement gesture;

    public float launchStrength = 5f;

    private List<TelekinesisTargetable> floatingObjects = new List<TelekinesisTargetable>();

    private void Start()
    {
        castEffect = GetComponent<ICastable>();
        gesture = GetComponent<IMovement>();
    }
    public override void TriggerHold()
    {
        base.TriggerHold();
        potentialTarget = _targetManager.GetClosest(currentHand);
    }
    public override void TriggerRelease()
    {
        base.TriggerRelease();
        if (potentialTarget != null)
        {
            if(potentialTarget is TelekinesisTargetable)
            {
                var targetConverted = (Component)potentialTarget;
                castEffect.Cast(targetConverted.transform);
                if (!floatingObjects.Contains((TelekinesisTargetable)potentialTarget))
                {
                    floatingObjects.Add((TelekinesisTargetable)potentialTarget);
                }
            }
            
        }
    }

    public override void GripHold()
    {
        base.GripHold();
        if(floatingObjects.Count != 0 && gesture.GesturePerformed(_gestureManager, out Vector3 direction))
        {
            foreach (TelekinesisTargetable targetable in floatingObjects)
            {
                targetable.CatapultLaunch(direction * launchStrength);
            }
            floatingObjects = new List<TelekinesisTargetable>();
        }
    }

}
