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
        _SpellCircle = _CircleHolderTransform.transform.GetChild(_CircleHolderTransform.transform.childCount - 1).gameObject;

        spellCircleAudioSource = _SpellCircle.GetComponent<AudioSource>();

        centerOfMass = _HandTransform.transform.position;
    }
    private void Update()
    {
        if (!gripPressed)
        {
            _VisualsManager.ReturnCircleToHolder(currentHand);
        }
    }
    public override void GripPress()
    {
        base.GripPress();
        if (_bigFireboltSpawn.latestInstantiatedObject && _bigFireboltSpawn.latestInstantiatedObject.transform.parent == _PalmTransform)
            _bigFireboltSpawn.DespawnAllProjectiles();
        iTween.ScaleTo(_SpellCircle, Vector3.one * .3f, .1f);

        _VisualsManager.ToggleReticle(currentHand, true);
    }
    public override void GripHold()
    {
        if (currentHand == 0)
        {
            iTween.RotateUpdate(_SpellCircle, (_CircleHolderTransform.transform.rotation * Quaternion.Euler(-90, 0, 0)).eulerAngles, .4f);
            iTween.MoveUpdate(_SpellCircle, _CircleHolderTransform.transform.position + _CircleHolderTransform.transform.TransformDirection(new Vector3(-.01f, -0.02f, .2f)), .1f);
        }
        else
        {
            iTween.RotateUpdate(_SpellCircle, (_CircleHolderTransform.transform.rotation * Quaternion.Euler(90, 0, 0)).eulerAngles, .4f);
            iTween.MoveUpdate(_SpellCircle, _CircleHolderTransform.transform.position + _CircleHolderTransform.transform.TransformDirection(new Vector3(-.025f, -.012f, .2f)), .1f);
        }      
    }
    public override void GripRelease()
    {
        base.GripRelease();
        iTween.ScaleTo(_SpellCircle, Vector3.one, .1f);
        _VisualsManager.ToggleReticle(currentHand, false);
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

                _bigFireboltSpawn.SpawnProjectile(_PalmTransform);
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

                _smallFireboltSpawn.SpawnProjectile(_SpellCircle.transform);
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
            if (_requiredGesture.GesturePerformed(_HandPhysicsTracker, out Vector3 direction))
            {
                CalculateOffset();

                Vector3 controllerVelocityCross = Vector3.Cross(_GestureManager.angularVel, objectGrabOffset - centerOfMass);
                Vector3 fullThrow = _GestureManager.currVel + controllerVelocityCross;

                _bigFireboltSpawn.LaunchAllProjectiles(_PalmTransform.forward, 0);

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
            _smallFireboltSpawn.LaunchAllProjectiles(_HandTransform.forward, smallLaunchSpeed);
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
        Vector3 relPos = _PalmTransform.position - transform.position;
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
            velocityFrames[currentVelocityFrameStep] = _GestureManager.currVel;
            angularVelocityFrames[currentVelocityFrameStep] = _GestureManager.angularVel;

        }
    }
    private void AddVelocity()
    {
        if (velocityFrames != null)
        {
            Vector3 velocityAverage = HandyHelperScript.GetVectorAverage(velocityFrames) * 1.6f;
            Vector3 angularVelocityAverage = HandyHelperScript.GetVectorAverage(angularVelocityFrames);
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
