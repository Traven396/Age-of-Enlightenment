using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewFirebolt : SpellBlueprint
{
    [Header("Settings")]
    public int bigFireballManaCost = 5;
    public float BigFireballDamage = 10;
    public int smallFireboltSpawn = 1;
    public float SmallFireballDamage = 10;

    [Header("Prefab Stuff")]
    public ProjectileShooter _smallFireboltSpawn;
    public ProjectileShooter _bigFireboltSpawn;
    private IMovement _requiredGesture;
    private ApplyMotion _applyMotion;

    [Header("Haptics")]
    public float smallLaunchSpeed = 300f;


    //Better throw stuff
    private Vector3 centerOfMass;
    private Vector3 objectGrabOffset;
    private Vector3[] velocityFrames = new Vector3[5];
    private Vector3[] angularVelocityFrames = new Vector3[5];
    private int currentVelocityFrameStep = 0;

    private AudioSource spellCircleAudioSource;

    private bool doneScaling = false;

    private void Awake()
    {
        _requiredGesture = GetComponent<IMovement>();
        _applyMotion = GetComponent<ApplyMotion>();
    }
    private void Start()
    {
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
        if (_bigFireboltSpawn.latestInstantiatedObject && _bigFireboltSpawn.latestInstantiatedObject.transform.parent == _palmLocation)
            _bigFireboltSpawn.DespawnAllProjectiles();
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
                _bigFireboltSpawn.SetDamage(BigFireballDamage, DamageType.Fire);

                _bigFireboltSpawn.SpawnProjectile(_palmLocation);
                iTween.ScaleFrom(_bigFireboltSpawn.latestInstantiatedObject, iTween.Hash("scale", Vector3.zero, 
                                                                                    "time", .2f, 
                                                                                    "oncompletetarget", gameObject, 
                                                                                    "oncomplete", "DoneScaling"));
                Player.Instance.SubtractMana(bigFireballManaCost);
            }
        }
        else
        {
            if (Player.Instance.currentMana >= smallFireboltSpawn)
            {
                _smallFireboltSpawn.SetDamage(SmallFireballDamage, DamageType.Fire);

                _smallFireboltSpawn.SpawnProjectile(spellCircle.transform);
                spellCircleAudioSource.PlayOneShot(spellCircleAudioSource.clip);

                
                Player.Instance.SubtractMana(smallFireboltSpawn); 
            }
        }
    }
    public override void TriggerHold()
    {
        if (!gripPressed)
        {
            if (_bigFireboltSpawn.latestInstantiatedObject)
            {
                if (_bigFireboltSpawn.latestInstantiatedObject.transform.localScale.x < 2f && doneScaling)
                {
                    iTween.ScaleAdd(_bigFireboltSpawn.latestInstantiatedObject, Vector3.one * 0.02f, .05f);
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

                _bigFireboltSpawn.LaunchAllProjectiles(_palmLocation.forward, 0);

                //_applyMotion.ChangeMotion(_bigFireboltSpawn.instantiatedObject.transform, fullThrow, _gestureManager.angularVel);

                AddVelocity();
                ResetVelocity();
            }
            else
            {
                if (_bigFireboltSpawn.latestInstantiatedObject)
                {
                    iTween.ScaleTo(_bigFireboltSpawn.latestInstantiatedObject, Vector3.zero, .2f);
                    _bigFireboltSpawn.DespawnAllProjectiles();
                }
            }
        }
        else
        {
            _smallFireboltSpawn.LaunchAllProjectiles(spellCircle.transform.up, smallLaunchSpeed);
        }
    }
    public override void OnDeselect()
    {
        base.OnDeselect();
        _bigFireboltSpawn.DespawnAllProjectiles();
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
                _applyMotion.ChangeMotion(_bigFireboltSpawn.latestInstantiatedObject.transform, velocityAverage, angularVelocityAverage);
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
