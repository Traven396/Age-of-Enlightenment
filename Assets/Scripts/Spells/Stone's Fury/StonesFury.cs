using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StonesFury : SpellBlueprint
{
    private ObjectSpawn _objectSpawn;
    private TargettingIndicator _targettingIndicator;
    private IMovement _requiredGesture;

    [SerializeField] private float distance;



    private bool performedThisPress = false;
    RaycastHit hit = new RaycastHit();

    void Start()
    {
        _objectSpawn = GetComponent<ObjectSpawn>();
        _targettingIndicator = GetComponent<TargettingIndicator>();
        _requiredGesture = GetComponent<IMovement>();
    }

    public override void TriggerHold()
    {
        if (gripPressed)
        {
            if (!performedThisPress && hit.normal == Vector3.up)
            {
                
                if (_requiredGesture.GesturePerformed(_gestureManager, out Vector3 direction))
                {
                    _objectSpawn.Cast(_targettingIndicator.GetCurrentTarget());
                    performedThisPress = true;
                } 
            }
            return;
        }
        
        hit = _targetManager.RaycastFromHand(whichHand, distance);
        
        if (hit.normal == Vector3.up)
        {
            _targettingIndicator.Cast(hit.point, false);
        }
        else
        {
            _targettingIndicator.Cast(Vector3.zero, true);
        }
    }

    public override void TriggerRelease()
    {
        base.TriggerRelease();
        _targettingIndicator.Cast(Vector3.zero, true);
        performedThisPress = false;
    }
}
