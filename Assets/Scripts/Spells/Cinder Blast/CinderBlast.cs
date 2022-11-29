using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinderBlast : SpellBlueprint
{
    private Chargeable _charge;
    private ObjectSpawn _objectSpawn;
    private IMovement _gesture;

    public float requiredCharge = 1f;

    private void Start()
    {
        _charge = GetComponent<Chargeable>();
        _objectSpawn = GetComponent<ObjectSpawn>();
        _gesture = GetComponent<IMovement>();
    }
    public override void TriggerPress()
    {
        base.TriggerPress();
        _charge.ResetCharge();
        _charge.StartCharging();
    }
    public override void TriggerRelease()
    {
        base.TriggerRelease();
        if(_charge.GetCurrentCharge() >= requiredCharge && _gesture.GesturePerformed(_gestureManager, out Vector3 Direction))
        {
            _objectSpawn.Cast(_palmLocation);
        }
        _charge.StopCharging();
    }
}
