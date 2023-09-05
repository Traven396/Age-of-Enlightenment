using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpell : SpellBlueprint
{
    private ProjectileShooter _shooter;
    public float spreadAmount;

    private void Awake()
    {
        _shooter = GetComponent<ProjectileShooter>();
        _shooter.SetSpread(spreadAmount);
    }
    public override void TriggerPress()
    {
        base.TriggerPress();
        _shooter.SpawnProjectile(_palmLocation);
    }
    public override void TriggerRelease()
    {
        base.TriggerRelease();
    }
    public override void GripPress()
    {
        base.GripPress();
        _shooter.SpawnProjectile(_palmLocation.position);
    }
    public override void GripRelease()
    {
        base.GripRelease();
        _shooter.LaunchAllProjectiles(_handLocation.forward, 10);
    }
}
