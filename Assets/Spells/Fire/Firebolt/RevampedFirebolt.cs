using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevampedFirebolt : SpellBlueprint
{
    [Header("Mana Costs")]
    [SerializeField] private int PrimaryFireManaCost = 5;
    [SerializeField] private int SecondaryFireManaCost = 1;

    [Header("Specific Components")]
    [SerializeField] private ProjectileShooter PrimaryShooter;
    [SerializeField] private ProjectileShooter SecondaryShooter;

    [Header("Projectile Settings")]
    [SerializeField] private float PrimaryLaunchSpeed = 0;
    [SerializeField] private float SecondaryLaunchSpeed = 10f;
    [SerializeField] private float PrimaryDamage;
    [SerializeField] private float SecondaryBaseDamage;
    [SerializeField] private float SecondaryMaxCharge = 4;

    //Things for better velocity tracking
    private Vector3 centerOfMass;
    private Vector3 objectGrabOffset;
    private Vector3[] velocityFrames = new Vector3[5];
    private Vector3[] angularVelocityFrames = new Vector3[5];
    private int currentVelocityFrameStep = 0;

    //Necessary components that dont need to be told in advance
    private IMovement _requiredGesture;
    private ApplyMotion _applyMotion;
    private AudioSource _spellCircleAudioSource;

    private bool doneScaling = false;
    private bool inSecondaryFire = false;
    private void Awake()
    {
        _requiredGesture = GetComponent<IMovement>();
        _applyMotion = GetComponent<ApplyMotion>();

        
    }

    public override void OnSelect()
    {
        _spellCircleAudioSource = _SpellCircle.GetComponent<AudioSource>();

        centerOfMass = _HandTransform.transform.position;
    }
    private void Update()
    {
        if (!gripPressed)
        {
            _VisualsManager.ReturnCircleToHolder(currentHand);
        }
    }
    private void FixedUpdate()
    {
        VelocityUpdate();
    }
    public override void TriggerPress()
    {
        base.TriggerPress();
        if (!inSecondaryFire)
        {
            PrimaryShooter.SpawnProjectile(_PalmTransform);
        }
        else
        {
            SecondaryShooter.SpawnProjectile(_SpellCircle.transform);
        }
        
    }
    public override void TriggerRelease()
    {
        base.TriggerRelease();
        if (!inSecondaryFire)
        {
            if (_requiredGesture.GesturePerformed(_HandPhysicsTracker, out _))
            {
                PrimaryShooter.latestInstantiatedObject.GetComponent<ControllableProjectile>().SetController(_GestureManager);
                PrimaryShooter.LaunchAllProjectiles(_PalmTransform.forward, PrimaryLaunchSpeed);
                AddVelocity();
            }
            else
            {
                if (PrimaryShooter.latestInstantiatedObject)
                {
                    //iTween.ScaleTo(PrimaryShooter.latestInstantiatedObject, Vector3.zero, .2f);
                    PrimaryShooter.DespawnAllProjectiles();
                }
            }
        }
        else
        {
            SecondaryShooter.LaunchAllProjectiles(_HandTransform.forward, SecondaryLaunchSpeed);
        }
    }

    public override void GripPress()
    {
        base.GripPress();

        _VisualsManager.SetRotationOffset(Vector3.right * 90, currentHand);
        _VisualsManager.SetPositionOffset(new Vector3(-.025f, -.012f, .26f), currentHand);

        iTween.ScaleTo(_SpellCircle, Vector3.one * .3f, .6f);

        ChangeFiringMode(true);
    }
    public override void GripRelease()
    {
        base.GripRelease();

        _VisualsManager.ResetRotationOffset(currentHand);
        _VisualsManager.ResetRotationOffset(currentHand);

        iTween.ScaleTo(_SpellCircle, Vector3.one, .6f);

        ChangeFiringMode(false);
    }

    private void ChangeFiringMode(bool NewValue)
    {
        if (NewValue)
        {
            if (inSecondaryFire)
            {
                return;
            }
            else
            {
                if (PrimaryShooter.latestInstantiatedObject && PrimaryShooter.latestInstantiatedObject.transform.parent == _PalmTransform)
                    PrimaryShooter.DespawnAllProjectiles();
                inSecondaryFire = true;
            }
        }
        else
        {
            if (!inSecondaryFire)
            {
                return;
            }
            else
            {
                if (SecondaryShooter.latestInstantiatedObject && SecondaryShooter.latestInstantiatedObject.transform.parent == _SpellCircle.transform)
                    SecondaryShooter.DespawnAllProjectiles();
                inSecondaryFire = false;
            }
        }
    }

    #region Velocity Things
    void CalculateOffset()
    {
        Vector3 relPos = _PalmTransform.position - transform.position;
        relPos = Quaternion.Inverse(transform.rotation) * relPos;
        objectGrabOffset = relPos;
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
            velocityFrames[currentVelocityFrameStep] = _HandPhysicsTracker.Velocity;
            angularVelocityFrames[currentVelocityFrameStep] = _HandPhysicsTracker.AngularVelocity;

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
                _applyMotion.ChangeMotion(PrimaryShooter.latestInstantiatedObject.transform, velocityAverage, angularVelocityAverage);
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
