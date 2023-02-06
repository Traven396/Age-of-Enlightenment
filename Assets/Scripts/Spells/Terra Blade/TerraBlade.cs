using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TerraBlade : SpellBlueprint
{
    private bool bladeSpawned = false;
    private bool alreadyShot = false;


    public ObjectSpawn BladeSpawner;
    public ObjectSpawn ProjectileSpawner;
    private IMovement _gesture;

    private GameObject spellCircle;
    bool needToRotate = false;
    bool needToRotateBack = false;

    public InputActionReference overrideBtn;

    private void Start()
    {
        _gesture = GetComponent<IMovement>();
        spellCircle = circleHolder.transform.GetChild(0).gameObject;
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
                if (whichHand == 0)
                {
                    iTween.MoveUpdate(spellCircle, circleHolder.transform.position, .1f);
                }
                else
                {
                    iTween.MoveUpdate(spellCircle, circleHolder.transform.position, .1f);
                }
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
        if (whichHand == 0)
        {
            iTween.RotateUpdate(spellCircle, (circleHolder.transform.rotation * Quaternion.Euler(-90, 0, 0)).eulerAngles, .4f);
            iTween.MoveUpdate(spellCircle, circleHolder.transform.position + circleHolder.transform.TransformDirection(new Vector3(0, .05f, .1f)), .1f);
        }
        else
        {
            iTween.RotateUpdate(spellCircle, (circleHolder.transform.rotation * Quaternion.Euler(90, 0, 0)).eulerAngles, .4f);
            iTween.MoveUpdate(spellCircle, circleHolder.transform.position + circleHolder.transform.TransformDirection(new Vector3(0, .05f, .1f)), .1f);

        }
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
        alreadyShot = false;
        if (bladeSpawned) { 
            bladeSpawned = false;
            iTween.ScaleTo(BladeSpawner.InstantiatedObject, Vector3.zero, .5f);
            Destroy(BladeSpawner.InstantiatedObject, .5f);
        }
        iTween.ScaleTo(spellCircle, Vector3.one, .7f);
        
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
            iTween.PunchPosition(spellCircle, new Vector3(0, .1f, 0f), .3f);
        }
    }
}
