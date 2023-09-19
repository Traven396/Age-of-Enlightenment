using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpell : SpellBlueprint
{
    private ProjectileShooter _shooter;
    public float spreadAmount;
    public float Damage;
    public DamageType Typing;
    [field : SerializeField] public bool YN { get; private set; }
    private void Awake()
    {
        _shooter = GetComponent<ProjectileShooter>();
        _shooter.SetSpread(spreadAmount);
        _shooter.SetDamage(Damage, Typing);
    }
    public override void GripPress()
    {
        base.GripPress();
        _shooter.SpawnProjectile(_palmLocation);
    }
    public override void GripRelease()
    {
        base.GripRelease();
        _shooter.LaunchAllProjectiles(_handLocation.forward, 10);
    }
}
