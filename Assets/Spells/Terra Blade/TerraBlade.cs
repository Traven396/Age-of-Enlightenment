using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TerraBlade : SpellBlueprint
{
    private bool bladeSpawned = false;
    private bool alreadyShot = false;


    public ObjectSpawn BladeSpawner;
    private IMovement _gesture;

    public InputActionReference overrideBtn;

    private void Start()
    {
        _gesture = GetComponent<IMovement>();
        spellCircle = circleHolder.transform.GetChild(circleHolder.transform.childCount - 1).gameObject;

        if(currentHand == 0)
        {
            BladeSpawner.SetRotationOffset(new Vector3(91, 0));
        }
    }
    private void Update()
    {
        if (!gripPressed)
        {
            if (Quaternion.Angle(spellCircle.transform.rotation, circleHolder.transform.rotation) > .1)
            {
             iTween.RotateUpdate(spellCircle, circleHolder.transform.rotation.eulerAngles, .1f);
                
            }
            if(Vector3.Distance(circleHolder.transform.position, spellCircle.transform.position) > .001f)
            {
                iTween.MoveUpdate(spellCircle, circleHolder.transform.position, .1f);
            }
        }
    }
    public override void GripPress()
    {
        base.GripPress();
        iTween.ScaleTo(spellCircle, Vector3.one * .3f, .7f);
    }

    public override void GripHold()
    {
        base.GripHold();
        if (currentHand == 0)
        {
            iTween.RotateUpdate(spellCircle, (circleHolder.transform.rotation * Quaternion.Euler(-90, 0, 0)).eulerAngles, .4f);
        }
        else
        {
            iTween.RotateUpdate(spellCircle, (circleHolder.transform.rotation * Quaternion.Euler(90, 0, 0)).eulerAngles, .4f);
        }
        iTween.MoveUpdate(spellCircle, circleHolder.transform.position + circleHolder.transform.TransformDirection(new Vector3(0, .05f, .1f)), .1f);
        
        if (_gesture.GesturePerformed(_gestureManager, out var direction) || overrideBtn.action.WasPressedThisFrame())
        {
            if (!bladeSpawned && !alreadyShot)
            {
                bladeSpawned = true;
                BladeSpawner.Cast(spellCircle.transform);
                spellCircle.GetComponent<AudioSource>().Play();
                iTween.ScaleFrom(BladeSpawner.InstantiatedObject, Vector3.zero, .8f);
            } 
        }
    }

    public override void GripRelease()
    {
        base.GripRelease();
        if (bladeSpawned && !alreadyShot) { 
            bladeSpawned = false;

            if(BladeSpawner.InstantiatedObject)
                iTween.ScaleTo(BladeSpawner.InstantiatedObject, Vector3.zero, .5f);

            Destroy(BladeSpawner.InstantiatedObject, .5f);
        }
        iTween.ScaleTo(spellCircle, Vector3.one, .7f);

        alreadyShot = false;
    }
    public override void TriggerPress()
    {
        base.TriggerPress();

        if (bladeSpawned)
        {
            bladeSpawned = false;
            alreadyShot = true;

            BladeSpawner.LaunchProjectile(_handLocation, currentHand);
            //Destroy(BladeSpawner.InstantiatedObject);

            iTween.PunchPosition(spellCircle, new Vector3(0, .1f, 0f), .3f);
        }
    }
}
