using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinderBlast : SpellBlueprint
{
    [Header("Spell Settings")]
    public int ManaCost = 12;
    public float RequiredCharge = 1f;
    public float LaunchSpeed = 250f;
    public int NumOfProjectiles = 5;
    public float SpreadAmount = 15;
    public float DamageAmount = 5;

    private DamageType ProjectileTyping = DamageType.Fire;

    private AudioSource spellSoundSource;

    private Chargeable _charge;
    private ObjectSpawn _objectSpawn;
    private ProjectileShooter _shooter;

    private void Start()
    {
        _charge = GetComponent<Chargeable>();
        _objectSpawn = GetComponent<ObjectSpawn>();
        _shooter = GetComponent<ProjectileShooter>();

        _shooter.SetDamage(DamageAmount, ProjectileTyping);
        _shooter.SetSpread(SpreadAmount);

        _SpellCircle = _CircleHolderTransform.transform.GetChild(_CircleHolderTransform.transform.childCount - 1).gameObject;
        spellSoundSource = _SpellCircle.GetComponent<AudioSource>();
    }
    public override void TriggerPress()
    {
        base.TriggerPress();
        _charge.ResetCharge();
        _charge.StartCharging();
        iTween.ScaleTo(_SpellCircle, Vector3.one * 1.4f, 4.5f);
    }
    public override void TriggerRelease()
    {
        base.TriggerRelease();
        if(_charge.GetCurrentCharge() >= RequiredCharge)
        {
            if (CheckCurrentMana(ManaCost))
            {
                for (int i = 0; i < NumOfProjectiles; i++)
                {
                    _shooter.SpawnProjectile(_PalmTransform);
                }
                _shooter.LaunchAllProjectiles(_PalmTransform.forward, LaunchSpeed);

                spellSoundSource.pitch = Random.Range(0.85f, .95f);
                spellSoundSource.Play();

                PlayerSingleton.Instance.SubtractMana(ManaCost);
            }
        }
        _charge.StopCharging();
        
        iTween.ScaleTo(_SpellCircle, Vector3.one, .5f);
    }
}
