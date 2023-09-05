using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinderBlast : SpellBlueprint
{
    [Header("Mana")]
    public int manaCost = 12;
    [Header("Spell Parameters")]
    public float requiredCharge = 1f;
    public float launchSpeed = 250f;

    private AudioSource spellSoundSource;

    private Chargeable _charge;
    private ObjectSpawn _objectSpawn;

    private void Start()
    {
        _charge = GetComponent<Chargeable>();
        _objectSpawn = GetComponent<ObjectSpawn>();

        spellCircle = circleHolder.transform.GetChild(circleHolder.transform.childCount - 1).gameObject;
        spellSoundSource = spellCircle.GetComponent<AudioSource>();
    }
    public override void TriggerPress()
    {
        base.TriggerPress();
        _charge.ResetCharge();
        _charge.StartCharging();
        iTween.ScaleTo(spellCircle, Vector3.one * 1.4f, 4.5f);
    }
    public override void TriggerRelease()
    {
        base.TriggerRelease();
        if(_charge.GetCurrentCharge() >= requiredCharge)
        {
            if (Player.Instance.currentMana >= manaCost)
            {
                _objectSpawn.Cast(_palmLocation);
                _objectSpawn.LaunchProjectile(_palmLocation, currentHand, launchSpeed);

                spellSoundSource.pitch = Random.Range(0.85f, .95f);
                spellSoundSource.Play();

                Player.Instance.SubtractMana(manaCost);
            }
        }
        _charge.StopCharging();
        
        iTween.ScaleTo(spellCircle, Vector3.one, .5f);
    }
}
