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

    private GameObject spellCircle;
    private void Start()
    {
        _gesture = GetComponent<IMovement>();
        spellCircle = circleHolder.transform.GetChild(0).gameObject;
    }
    public override void GripPress()
    {
        base.GripPress();
        iTween.ScaleTo(spellCircle, Vector3.one * .2f, .7f);
        if(whichHand == 0)
            iTween.RotateTo(spellCircle, spellCircle.transform.rotation.eulerAngles + new Vector3(0, 90, 0), .7f);
        else
            iTween.RotateTo(spellCircle, spellCircle.transform.rotation.eulerAngles + new Vector3(0, -90, 0), .7f);
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
        iTween.ScaleTo(spellCircle, Vector3.one, .7f);
        if(whichHand == 0)
            iTween.RotateTo(spellCircle, spellCircle.transform.rotation.eulerAngles + new Vector3(0, -90, 0), .7f);
        else
            iTween.RotateTo(spellCircle, spellCircle.transform.rotation.eulerAngles + new Vector3(0, 90, 0), .7f);
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
