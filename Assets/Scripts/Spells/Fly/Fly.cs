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
}
