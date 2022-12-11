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
        playerRb.velocity = Vector3.Lerp(playerRb.velocity, new Vector3(playerRb.velocity.x, 0, playerRb.velocity.z), Time.deltaTime);
    }
    public override void GripRelease()
    {
        base.GripRelease();
        playerRb.useGravity = true;
    }
}
