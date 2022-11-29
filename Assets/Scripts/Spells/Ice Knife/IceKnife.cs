using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceKnife : SpellBlueprint
{
    private ObjectSpawn _objectSpawn;
    private Chargeable _charger;

    [Min(0)]
    public float requiredCharge = 1f;
    public override void TriggerPress()
    {
        base.TriggerPress();
        _charger.ResetCharge();
        _charger.StartCharging();
    }
    public override void TriggerRelease()
    {
        base.TriggerRelease();
        if(_charger.GetCurrentCharge() >= requiredCharge)
        {
            _objectSpawn.Cast(_palmLocation);
        }
        _charger.StopCharging();
    }
}
