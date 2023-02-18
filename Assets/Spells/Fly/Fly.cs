using UnityEngine;
public class Fly : SpellBlueprint
{
    private ApplyMotion _applyMotion;

    private void Start()
    {
        _applyMotion = GetComponent<ApplyMotion>();
    }
    public override void TriggerHold()
    {
        _applyMotion.Cast(playerRb, _handLocation.transform.forward * triggerPressedValue);
    }
    public override void GripPress()
    {
        base.GripPress();
        playerRb.useGravity = false;
    }
    public override void GripHold()
    {
        playerRb.velocity = iTween.Vector3Update(playerRb.velocity, new Vector3(0, 0, 0), .7f);
    }
    public override void GripRelease()
    {
        base.GripRelease();
        playerRb.useGravity = true;
    }
}
