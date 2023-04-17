using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firebolt : SpellBlueprint
{
    [Header("MANA")]
    public int bigFireballManaCost = 5;
    public int tinyFireballManaCost = 1;

    [Header("Prefab Stuff")]
    public ObjectSpawn _smallFireboltSpawn;
    public ObjectSpawn _bigFireboltSpawn;
    private IMovement _requiredGesture;
    private ApplyMotion _applyMotion;

    [Header("Haptics")]
    public float tinyLaunchSpeed = 300f;
    public float bigLaunchSpeed = 0f;


    //Better throw stuff
    private Vector3 centerOfMass;
    private Vector3 objectGrabOffset;
    private Vector3[] velocityFrames = new Vector3[5];
    private Vector3[] angularVelocityFrames = new Vector3[5];
    private int currentVelocityFrameStep = 0;

    private AudioSource spellCircleAudioSource;

    private bool doneScaling = false;

    private void Start()
    {
        _requiredGesture = GetComponent<IMovement>();
        _applyMotion = GetComponent<ApplyMotion>();

        spellCircle = circleHolder.transform.GetChild(circleHolder.transform.childCount - 1).gameObject;

        spellCircleAudioSource = spellCircle.GetComponent<AudioSource>();

        centerOfMass = _handLocation.transform.position;
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
        doneScaling = false;
        if (!gripPressed)
        {
            if (Player.Instance.currentMana >= bigFireballManaCost)
            {
                _bigFireboltSpawn.Cast(_palmLocation);
                iTween.ScaleFrom(_bigFireboltSpawn.instantiatedObject, iTween.Hash("scale", Vector3.zero, 
                                                                                    "time", .2f, 
                                                                                    "oncompletetarget", gameObject, 
                                                                                    "oncomplete", "DoneScaling"));
                Player.Instance.SubtractCurrentMana(bigFireballManaCost);
            }
        }
        else
        {
            if (Player.Instance.currentMana >= tinyFireballManaCost)
            {
                _smallFireboltSpawn.Cast(spellCircle.transform);
                spellCircleAudioSource.PlayOneShot(spellCircleAudioSource.clip);

                _smallFireboltSpawn.LaunchProjectile(spellCircle.transform, currentHand, tinyLaunchSpeed);
                Player.Instance.SubtractCurrentMana(tinyFireballManaCost); 
            }
        }
    }
    public override void TriggerHold()
    {
        if (!gripPressed)
        {
            if (_bigFireboltSpawn.instantiatedObject)
            {
                if (_bigFireboltSpawn.instantiatedObject.transform.localScale.x < 2f && doneScaling)
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
            {
                return;
            }
            if (_requiredGesture.GesturePerformed(_gestureManager, out Vector3 direction))
            {
                CalculateOffset();

                Vector3 controllerVelocityCross = Vector3.Cross(_gestureManager.angularVel, objectGrabOffset - centerOfMass);
                Vector3 fullThrow = _gestureManager.currVel + controllerVelocityCross;
                
                _bigFireboltSpawn.LaunchProjectile(_palmLocation, currentHand, bigLaunchSpeed);
                
                _applyMotion.ChangeMotion(_bigFireboltSpawn.instantiatedObject.transform, fullThrow, _gestureManager.angularVel);

                AddVelocity();
                ResetVelocity();
            }
            else
            {
                if (_bigFireboltSpawn.instantiatedObject)
                {
                    iTween.ScaleTo(_bigFireboltSpawn.instantiatedObject, Vector3.zero, .2f);
                    Destroy(_bigFireboltSpawn.instantiatedObject, .2f); 
                }
            } 
        }
    }
    public override void OnDeselect()
    {
        base.OnDeselect();
        Destroy(_bigFireboltSpawn.instantiatedObject);
    }

    void DoneScaling()
    {
        doneScaling = true;
    }
    #region Velocity Things
    void CalculateOffset()
    {
        Vector3 relPos = _palmLocation.position - transform.position;
        relPos = Quaternion.Inverse(transform.rotation) * relPos;
        objectGrabOffset = relPos;
    }
    private void FixedUpdate()
    {
        VelocityUpdate();
    }
    private void VelocityUpdate()
    {
        if (velocityFrames != null)
        {
            currentVelocityFrameStep++;

            if (currentVelocityFrameStep >= velocityFrames.Length)
            {
                currentVelocityFrameStep = 0;
            }
            velocityFrames[currentVelocityFrameStep] = _gestureManager.currVel;
            angularVelocityFrames[currentVelocityFrameStep] = _gestureManager.angularVel;

        }
    }
    private void AddVelocity()
    {
        if (velocityFrames != null)
        {
            Vector3 velocityAverage = HelpfulScript.GetVectorAverage(velocityFrames) * 1.6f;
            Vector3 angularVelocityAverage = HelpfulScript.GetVectorAverage(angularVelocityFrames);
            if (velocityAverage != null && angularVelocityAverage != null)
            {
                _applyMotion.ChangeMotion(_bigFireboltSpawn.instantiatedObject.transform, velocityAverage, angularVelocityAverage);
            }
        }
    }
    private void ResetVelocity()
    {
        currentVelocityFrameStep = 0;
        if (velocityFrames != null && velocityFrames.Length > 0)
        {
            velocityFrames = new Vector3[5];
            angularVelocityFrames = new Vector3[5];
        }
    } 
    #endregion

}
