using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityBurst : SpellBlueprint
{   
    [SerializeField]
    private float sphereRadius = .2f;
    [SerializeField]
    private float castDistance = 5f;



    private Chargeable _chargeable;
    private ApplyMotion _applyMotion;


    private void Start()
    {
        _chargeable = GetComponent<Chargeable>();
        _applyMotion = GetComponent<ApplyMotion>();
    }
    public override void TriggerPress()
    {
        base.TriggerPress();
        _chargeable.ResetCharge();
        _chargeable.StartCharging();
    }
    public override void TriggerRelease()
    {
        base.TriggerRelease();
        _chargeable.StopCharging();

        RaycastHit[] hits = _targetManager.HandSphereCastAll(whichHand, castDistance, sphereRadius);

        foreach (RaycastHit hit in hits)
        {
            Rigidbody rb = hit.rigidbody;
            if(rb != null)
            {
                Vector3 forceDirection = (rb.position - _handLocation.position).normalized;
                _applyMotion.Cast(rb, forceDirection * _chargeable.GetCurrentCharge());
            }
        }

    }
}
