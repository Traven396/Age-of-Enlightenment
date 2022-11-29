using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepOfTheWind : SpellBlueprint
{
    public float maxDistance;


    private TargettingIndicator _targettingIndicator;
    private ApplyMotion _motionEffect;

    RaycastHit targettedPoint;
    bool dashing = false;

    float sqrMagnitude;
    float lastMag;

    private void Start()
    {
        _targettingIndicator = GetComponent<TargettingIndicator>();
        _motionEffect = GetComponent<ApplyMotion>();
    }
    public override void TriggerHold()
    {
        targettedPoint = _targetManager.RaycastFromHand(whichHand, maxDistance);
        if (targettedPoint.normal == Vector3.up) {
            _targettingIndicator.Cast(targettedPoint.point, false);
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
        if (targettedPoint.normal == Vector3.up)
        {
            dashing = true;
            _motionEffect.Cast(playerRb, new Vector3(targettedPoint.point.x - playerRb.transform.position.x, 0, targettedPoint.point.z - playerRb.transform.position.z));
            lastMag = Mathf.Infinity;
        }
    }

    private void Update()
    {
        if (dashing)
        {
            sqrMagnitude = (targettedPoint.point - _handLocation.position).sqrMagnitude;
            if(sqrMagnitude > lastMag)
            {
                playerRb.velocity = Vector3.zero;
                dashing = false;
            }
            lastMag = sqrMagnitude;
        }
    }
}
