using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceKnife : SpellBlueprint
{
    private ProjectileShooter shooter;

    [Header("Spell Stats")]
    public int ManaCost = 6;
    public float LaunchSpeed;
    public float Damage = 10;
    public float MaxProjectileSize = 4;

    private DamageType ProjectileDamageType = DamageType.Ice;

    private void Awake()
    {
        shooter = GetComponent<ProjectileShooter>();
    }
    private void Update()
    {
        if (!gripPressed)
        {
            _visualsManager.ReturnCircleToHolder(currentHand);
        }
    }
    public override void GripPress()
    {
        base.GripPress();
        
        iTween.ScaleTo(spellCircle, Vector3.one * .5f, .7f);
    }
    public override void GripHold()
    {
        if (currentHand == 0)
        {
            iTween.RotateUpdate(spellCircle, (circleHolder.transform.rotation * Quaternion.Euler(-90, 0, 0)).eulerAngles, .4f);
        }
        else
        {
            iTween.RotateUpdate(spellCircle, (circleHolder.transform.rotation * Quaternion.Euler(90, 0, 0)).eulerAngles, .4f);
        }
        iTween.MoveUpdate(spellCircle, circleHolder.transform.TransformPoint(new Vector3(0, .05f, .1f)), .1f);
    }
    public override void GripRelease()
    {
        base.GripRelease();
        iTween.ScaleTo(spellCircle, Vector3.one, .1f);
        if(shooter.latestInstantiatedObject && shooter.latestInstantiatedObject.transform.parent != null)
        {
            shooter.DespawnAllProjectiles();
        }
    }
    public override void TriggerPress()
    {
        base.TriggerPress();

        if (gripPressed)
        {
            if (Player.Instance.currentMana >= ManaCost)
            {
                //Spawn the projectile as a child of the spell circle
                shooter.SpawnProjectile(spellCircle.transform);

                //Grow it from nothing for a visual spawn effect
                iTween.ScaleFrom(shooter.latestInstantiatedObject, Vector3.zero, .15f);
                
                //Subtract the mana cost for spawning a projectile
                Player.Instance.SubtractMana(ManaCost);
            }
        }
    }
    public override void TriggerHold()
    {
        if (gripPressed && shooter.latestInstantiatedObject)
        {
            iTween.MoveUpdate(spellCircle, circleHolder.transform.TransformPoint(new Vector3(0, .05f, -.04f)), 6);
            iTween.ScaleUpdate(shooter.latestInstantiatedObject, iTween.Hash("time", 6, "y", MaxProjectileSize * 2));
            
        }
    }
    public override void TriggerRelease()
    {
        base.TriggerRelease(); 
        if (gripPressed && shooter.latestInstantiatedObject)
        {
            shooter.SetDamage(Damage * shooter.latestInstantiatedObject.transform.localScale.z, ProjectileDamageType);
            shooter.LaunchAllProjectiles(circleHolder.transform.forward, LaunchSpeed);
        }
    }
}