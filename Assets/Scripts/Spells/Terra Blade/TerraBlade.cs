using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerraBlade : SpellBlueprint
{
    private bool bladeSpawned = false;
    private bool alreadyShot = false;


    public ObjectSpawn BladeSpawner;
    public ObjectSpawn ProjectileSpawner;
    private IMovement _gesture;

    private void Start()
    {
        _gesture = GetComponent<IMovement>();
    }
    public override void GripHold()
    {
        base.GripHold();
        if (_gesture.GesturePerformed(_gestureManager, out var direction))
        {
            if (!bladeSpawned && !alreadyShot)
            {
                bladeSpawned = true;
                BladeSpawner.Cast(_backOfHandLocation);
            } 
        }
    }

    public override void GripRelease()
    {
        base.GripRelease();
        alreadyShot = false;
        if (bladeSpawned) { 
            bladeSpawned = false;
            Destroy(BladeSpawner.InstantiatedObject);
        }
    }
    public override void TriggerPress()
    {
        base.TriggerPress();

        if (bladeSpawned)
        {
            bladeSpawned = false;
            alreadyShot = true;
            Destroy(BladeSpawner.InstantiatedObject);
            ProjectileSpawner.Cast(_backOfHandLocation);
        }
    }
}