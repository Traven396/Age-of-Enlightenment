using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinderBlast : SpellBlueprint
{
    private Chargeable _charge;
    private ObjectSpawn _objectSpawn; 

    public float requiredCharge = 1f;

    private GameObject spellCircle;
    private AudioSource spellSoundSource;

    private void Start()
    {
        _charge = GetComponent<Chargeable>();
        _objectSpawn = GetComponent<ObjectSpawn>();

        spellCircle = circleHolder.transform.GetChild(0).gameObject;
        spellSoundSource = spellCircle.GetComponent<AudioSource>();
    }
    public override void TriggerPress()
    {
        base.TriggerPress();
        _charge.ResetCharge();
        _charge.StartCharging();
        
    }
    public override void TriggerHold()
    {
        if(_charge.GetCurrentCharge() < requiredCharge)
        {
            iTween.ScaleUpdate(spellCircle, Vector3.one * 1.4f, 1);
        }
    }
    public override void TriggerRelease()
    {
        base.TriggerRelease();
        if(_charge.GetCurrentCharge() >= requiredCharge)
        {
            _objectSpawn.Cast(_palmLocation);
            spellSoundSource.pitch = Random.Range(0.85f, .95f);
            spellSoundSource.Play();
            //iTween.PunchScale(spellCircle, Vector3.one * 1.5f, 1f);
        }
        _charge.StopCharging();
        
        iTween.ScaleTo(spellCircle, Vector3.one, .5f);
    }
}
