using UnityEngine;
public class Fly : SpellBlueprint
{
    public int flyManaDrain = 1;
    public int brakeManaDrain = 2;


    private ApplyMotion _applyMotion;
    private ObjectSpawn _objectSpawn;

    private bool brakeActive = false, flightActive = false;

    private void Start()
    {
        _applyMotion = GetComponent<ApplyMotion>();
        _objectSpawn = GetComponent<ObjectSpawn>();
    }
    private void Update()
    {
        if (!triggerPressed)
        {
            _visualsManager.ReturnCircleToHolder(currentHand);
        }
    }
    public override void TriggerPress()
    {
        base.TriggerPress();
        if (_objectSpawn.instantiatedObject)
            Destroy(_objectSpawn.instantiatedObject);

        _objectSpawn.Cast(spellCircle.transform);

        //InvokeRepeating("FlyDrain", 0, 1);
        Player.Instance.SubtractManaRegen(flyManaDrain);

        flightActive = true;
    }
    public override void TriggerHoldFixed()
    {
        if (triggerPressed)
        {
            if (Player.Instance.currentMana >= flyManaDrain)
            {
                if (currentHand == 0)
                {
                    //_applyMotion.Cast(playerRb, -_palmLocation.transform.right * triggerPressedValue);
                    playerPhys.AddVelocity(-_palmLocation.transform.right * triggerPressedValue * 12);
                }
                else
                {
                    //_applyMotion.Cast(playerRb, _palmLocation.transform.right * triggerPressedValue);
                    playerPhys.AddVelocity(_palmLocation.transform.right * triggerPressedValue * 12);
                }

                if (!_objectSpawn.instantiatedObject)
                {
                    _objectSpawn.Cast(spellCircle.transform);

                    Player.Instance.SubtractManaRegen(flyManaDrain);

                    flightActive = true;
                }
            }
            else
            {
                if (_objectSpawn.instantiatedObject)
                {
                    Destroy(_objectSpawn.instantiatedObject);
                    Player.Instance.AddManaRegen(flyManaDrain);
                    flightActive = false;
                }
            }
        }
    }
    public override void TriggerHold()
    {
        iTween.MoveUpdate(spellCircle, circleHolder.transform.position + circleHolder.transform.TransformDirection(new Vector3(0, -0.1f, 0.1f)), .1f);
    }
    public override void TriggerRelease()
    {
        base.TriggerRelease();

        if(_objectSpawn.instantiatedObject)
            iTween.ScaleTo(_objectSpawn.instantiatedObject, Vector3.zero, .2f);

        Destroy(_objectSpawn.instantiatedObject, .3f);

        Player.Instance.AddManaRegen(flyManaDrain);

        flightActive = false;

        //CancelInvoke("FlyDrain");
    }
    public override void GripPress()
    {
        base.GripPress();

        playerRb.useGravity = false;

        iTween.ScaleTo(spellCircle, Vector3.one * 1.5f, .2f);

        BrakeDrain(true);

    }
    public override void GripHoldFixed()
    {
        if (gripPressed)
        {
            if (Player.Instance.currentMana >= brakeManaDrain)
            {
                playerRb.velocity = iTween.Vector3Update(playerRb.velocity, new Vector3(0, 0, 0), .7f);

                if (spellCircle.transform.localScale != Vector3.one * 1.5f)
                {
                    iTween.ScaleUpdate(spellCircle, Vector3.one * 1.5f, .2f);
                }

                BrakeDrain(true);
            }
            else
            {
                if (spellCircle.transform.localScale != Vector3.one)
                {
                    iTween.ScaleUpdate(spellCircle, Vector3.one, .2f);
                }
                BrakeDrain(false);
            }
        }
    }
    public override void GripRelease()
    {
        base.GripRelease();

        playerRb.useGravity = true;

        iTween.ScaleTo(spellCircle, Vector3.one, .2f);

        BrakeDrain(false);

    }

    public override void OnDeselect()
    {
        base.OnDeselect();
        if (gripPressed)
        {
            Player.Instance.AddManaRegen(brakeManaDrain);
            playerRb.useGravity = true;
        }
        if (triggerPressed)
        {
            Player.Instance.AddManaRegen(flyManaDrain);
        }
    }
    void BrakeDrain(bool active)
    {
        if (active)
        {
            if (!brakeActive)
            {
                Player.Instance.SubtractManaRegen(brakeManaDrain);
                brakeActive = true;
            }
        }
        else
        {
            if (brakeActive)
            {
                Player.Instance.AddManaRegen(brakeManaDrain);
                brakeActive = false; 
            }
        }
    }
}
