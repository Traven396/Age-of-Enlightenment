using UnityEngine;
public class Fly : SpellBlueprint
{
    private ApplyMotion _applyMotion;
    private ObjectSpawn _objectSpawn;

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
        _objectSpawn.Cast(spellCircle.transform);
    }
    public override void TriggerHoldFixed()
    {
        if (currentHand == 0)
        {
            _applyMotion.Cast(playerRb, -_palmLocation.transform.right * triggerPressedValue); 
        }
        else
        {
            _applyMotion.Cast(playerRb, _palmLocation.transform.right * triggerPressedValue);
        }
    }
    public override void TriggerHold()
    {
        base.TriggerHold();
        iTween.MoveUpdate(spellCircle, circleHolder.transform.position + circleHolder.transform.TransformDirection(new Vector3(0, -0.1f, 0.1f)), .1f);
    }
    public override void TriggerRelease()
    {
        base.TriggerRelease();
        iTween.ScaleTo(_objectSpawn.instantiatedObject, Vector3.zero, .2f);
        Destroy(_objectSpawn.instantiatedObject, .3f);
    }
    public override void GripPress()
    {
        base.GripPress();
        playerRb.useGravity = false;
        iTween.ScaleTo(spellCircle, Vector3.one * 1.5f, .2f);
    }
    public override void GripHoldFixed()
    {
        playerRb.velocity = iTween.Vector3Update(playerRb.velocity, new Vector3(0, 0, 0), .7f);
    }
    public override void GripRelease()
    {
        base.GripRelease();
        playerRb.useGravity = true;

        iTween.ScaleTo(spellCircle, Vector3.one, .2f);
    }
}
