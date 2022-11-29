using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firebolt : SpellBlueprint
{
    [SerializeField] private ObjectSpawn _projectileOS;
    [SerializeField] private ObjectSpawn _precastOS;
    private IMovement _requiredGesture;

    private void Start()
    {
        _requiredGesture = GetComponent<IMovement>();
    }
    public override void TriggerPress()
    {
        base.TriggerPress();
        _precastOS.Cast(_palmLocation);
    }
    public override void TriggerRelease()
    {
        base.TriggerRelease();
        Destroy(_precastOS.InstantiatedObject);
        if (_requiredGesture.GesturePerformed(_gestureManager, out Vector3 direction))
        {
            _projectileOS.Cast(_palmLocation);
        }
    }
}
