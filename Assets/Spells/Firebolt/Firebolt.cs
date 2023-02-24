using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firebolt : SpellBlueprint
{
    public ObjectSpawn _smallFireboltSpawn;
    public ObjectSpawn _bigFireboltSpawn;
    private IMovement _requiredGesture;
    private ApplyMotion _applyMotion;

    public AudioClip tinyFireballCastSound;

    //Better throw stuff
    private Vector3 centerOfMass;
    private Vector3 objectGrabOffset;

    private AudioSource spellCircleAudioSource;
    private void Start()
    {
        _requiredGesture = GetComponent<IMovement>();
        _applyMotion = GetComponent<ApplyMotion>();

        spellCircle = circleHolder.transform.GetChild(circleHolder.transform.childCount - 1).gameObject;

        spellCircleAudioSource = spellCircle.GetComponent<AudioSource>();

        centerOfMass = _handLocation.transform.position;
        Vector3 relPos = _palmLocation.transform.position - centerOfMass;
        relPos = Quaternion.Inverse(_handLocation.rotation) * relPos;
        objectGrabOffset = relPos;
    }
    private void Update()
    {
        if (!gripPressed)
        {
            _visualsManager.ReturnCircleToHolder(currentHand);
        }
    }
    public override void GripPress()
    {
        base.GripPress();
        if(_bigFireboltSpawn.instantiatedObject && _bigFireboltSpawn.instantiatedObject.transform.parent == _palmLocation)
            Destroy(_bigFireboltSpawn.instantiatedObject);
        iTween.ScaleTo(spellCircle, Vector3.one * .3f, .1f);
    }
    public override void GripHold()
    {
        base.GripHold();
        if (currentHand == 0)
        {
            iTween.RotateUpdate(spellCircle, (circleHolder.transform.rotation * Quaternion.Euler(-90, 0, 0)).eulerAngles, .4f);
            iTween.MoveUpdate(spellCircle, circleHolder.transform.position + circleHolder.transform.TransformDirection(new Vector3(-.01f, -0.02f, .2f)), .1f);
        }
        else
        {
            iTween.RotateUpdate(spellCircle, (circleHolder.transform.rotation * Quaternion.Euler(90, 0, 0)).eulerAngles, .4f);
            iTween.MoveUpdate(spellCircle, circleHolder.transform.position + circleHolder.transform.TransformDirection(new Vector3(-.025f, -.012f, .2f)), .1f);
        }      
    }
    public override void GripRelease()
    {
        base.GripRelease();
        iTween.ScaleTo(spellCircle, Vector3.one, .1f);

    }
    public override void TriggerPress()
    {
        base.TriggerPress();
        if (!gripPressed)
        {
            _bigFireboltSpawn.Cast(_palmLocation);
        }
        else
        {
            _smallFireboltSpawn.Cast(spellCircle.transform);
            spellCircleAudioSource.PlayOneShot(spellCircleAudioSource.clip);
            _smallFireboltSpawn.LaunchProjectile(_handLocation, currentHand);
        }
    }
    public override void TriggerHold()
    {
        base.TriggerHold();
        if (!gripPressed)
        {
            if (_bigFireboltSpawn.instantiatedObject)
            {
                if (_bigFireboltSpawn.instantiatedObject.transform.localScale.x < 2f)
                {
                    iTween.ScaleAdd(_bigFireboltSpawn.instantiatedObject, Vector3.one * 0.02f, .05f);
                } 
            }
        }
    }
    public override void TriggerRelease()
    {
        base.TriggerRelease();

        if (!gripPressed)
        {
            if (_requiredGesture == null)
                return;
            if (_requiredGesture.GesturePerformed(_gestureManager, out Vector3 direction))
            {
                Vector3 controllerVelocityCross = Vector3.Cross(_gestureManager.angularVel, objectGrabOffset - centerOfMass);
                Vector3 fullThrow = _gestureManager.currVel + controllerVelocityCross;
                
                _bigFireboltSpawn.LaunchProjectile(_palmLocation, currentHand);
                
                _applyMotion.ChangeMotion(_bigFireboltSpawn.instantiatedObject.transform, _gestureManager.currVel, _gestureManager.angularVel);
            }
            else
            {
                Destroy(_bigFireboltSpawn.instantiatedObject);
            } 
        }
    }
}
