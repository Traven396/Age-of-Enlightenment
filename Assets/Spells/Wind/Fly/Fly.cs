using UnityEngine;
using Kryz.CharacterStats;
public class Fly : SpellBlueprint
{
    public float _flyManaCost;
    public float _brakeManaCost;

    private StatModifier FlightRegenModifier;
    private StatModifier BrakeRegenModifier;
    [Range(0.00001f, 1)]
    public float flySpeed = 0.7f;
    public float brakeSlowSpeed = 10;


    private ApplyMotion _applyMotion;
    private ObjectSpawn _objectSpawn;

    private bool brakeActive = false, flightActive;

    private void Start()
    {
        _applyMotion = GetComponent<ApplyMotion>();
        _objectSpawn = GetComponent<ObjectSpawn>();

        FlightRegenModifier = new StatModifier(-_flyManaCost, StatModType.Flat, this);
        BrakeRegenModifier = new StatModifier(-_brakeManaCost, StatModType.Flat, this);
    }
    private void Update()
    {
        if (!triggerPressed)
        {
            _VisualsManager.ReturnCircleToHolder(currentHand);
        }
    }
    public override void TriggerPress()
    {
        base.TriggerPress();
        if (_objectSpawn.instantiatedObject)
            Destroy(_objectSpawn.instantiatedObject);

        _objectSpawn.Cast(_SpellCircle.transform);

        //InvokeRepeating("FlyDrain", 0, 1);
        //Player.Instance.AddManaRegenModifier(FlightRegenModifier);

        flightActive = true;
    }
    public override void TriggerHoldFixed()
    {
        if (triggerPressed)
        {
            if (CheckCurrentMana((int)FlightRegenModifier.Value))
            {
                if (currentHand == 0)
                {
                    //_applyMotion.Cast(playerRb, -_palmLocation.transform.right * triggerPressedValue);
                    //playerPhys.AddVelocity(-_palmLocation.transform.right * triggerPressedValue * 12);
                    _MotionController.AddMomentum(-_PalmTransform.transform.right * triggerPressedValue * flySpeed);
                }
                else
                {
                    //_applyMotion.Cast(playerRb, _palmLocation.transform.right * triggerPressedValue);
                    //playerPhys.AddVelocity(_palmLocation.transform.right * triggerPressedValue * 12);
                    _MotionController.AddMomentum(_PalmTransform.transform.right * triggerPressedValue * flySpeed);
                }

                if (!_objectSpawn.instantiatedObject)
                {
                    _objectSpawn.Cast(_SpellCircle.transform);

                    //Player.Instance.AddManaRegenModifier(FlightRegenModifier);

                    flightActive = true;
                }
            }
            else
            {
                if (_objectSpawn.instantiatedObject)
                {
                    Destroy(_objectSpawn.instantiatedObject);
                    //Player.Instance.RemoveManaRegenModifier(FlightRegenModifier);
                    flightActive = false;
                }
            }
        }
    }
    public override void TriggerHold()
    {
        iTween.MoveUpdate(_SpellCircle, _CircleHolderTransform.transform.position + _CircleHolderTransform.transform.TransformDirection(new Vector3(0, -0.1f, 0.1f)), .1f);
    }
    public override void TriggerRelease()
    {
        base.TriggerRelease();

        if(_objectSpawn.instantiatedObject)
            iTween.ScaleTo(_objectSpawn.instantiatedObject, Vector3.zero, .2f);

        Destroy(_objectSpawn.instantiatedObject, .3f);

        //Player.Instance.RemoveManaRegenModifier(FlightRegenModifier);

        flightActive = false;

        //CancelInvoke("FlyDrain");
    }
    public override void GripPress()
    {
        base.GripPress();

        _MotionController.ChangeGravity(-brakeSlowSpeed, false);

        iTween.ScaleTo(_SpellCircle, Vector3.one * 1.5f, .2f);

        BrakeDrain(true);

    }
    public override void GripHoldFixed()
    {
        if (gripPressed)
        {
            if (CheckCurrentMana((int)BrakeRegenModifier.Value))
            {
                _MotionController.SetMomentum(iTween.Vector3Update(_MotionController.GetMomentum(), new Vector3(0, 0, 0), .7f));

                if (_SpellCircle.transform.localScale != Vector3.one * 1.5f)
                {
                    iTween.ScaleUpdate(_SpellCircle, Vector3.one * 1.5f, .2f);
                }

                BrakeDrain(true);
            }
            else
            {
                if (_SpellCircle.transform.localScale != Vector3.one)
                {
                    iTween.ScaleUpdate(_SpellCircle, Vector3.one, .2f);
                }
                BrakeDrain(false);
            }
        }
    }
    public override void GripRelease()
    {
        base.GripRelease();

        _MotionController.ChangeGravity(brakeSlowSpeed, false);

        iTween.ScaleTo(_SpellCircle, Vector3.one, .2f);

        BrakeDrain(false);

    }

    public override void OnDeselect()
    {
        base.OnDeselect();
        if (gripPressed)
        {
            //Player.Instance.RemoveManaRegenModifier(BrakeRegenModifier);
            _MotionController.ChangeGravity(10, false);
        }
        if (triggerPressed)
        {
            //Player.Instance.RemoveManaRegenModifier(FlightRegenModifier);
        }
    }
    void BrakeDrain(bool active)
    {
        if (active)
        {
            if (!brakeActive)
            {
                //Player.Instance.AddManaRegenModifier(BrakeRegenModifier);
                brakeActive = true;
            }
        }
        else
        {
            if (brakeActive)
            {
                //Player.Instance.RemoveManaRegenModifier(BrakeRegenModifier);
                brakeActive = false; 
            }
        }
    }
}
