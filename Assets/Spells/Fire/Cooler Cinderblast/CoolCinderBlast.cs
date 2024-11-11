using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FoxheadDev.GestureDetection;

public class CoolCinderBlast : SpellBlueprint
{
    public AudioClip GestureCompleteSound;
    public AudioClip CastCompleteSound;
    public GameObject secondarySpellCirclePrefab;

    private Step _CastingSequence;
    
    private AudioSource circleAudioSource;
    private ProjectileShooter projectileShooter;

    private GameObject secondarySpellCircle;


    private bool spellPrimed;

    public override void OnSelect()
    {
        base.OnSelect();

        Debug.Log("Selected");

        circleAudioSource = _SpellCircle.GetComponent<AudioSource>();

        projectileShooter = GetComponent<ProjectileShooter>();

        projectileShooter.SetDamage(5, DamageType.Fire);
        projectileShooter.SetSpread(65);

        _CastingSequence = Step.Start();

        _CastingSequence.Then(PremadeGestureLibrary.PushInViewDirection(_HandPhysicsTracker, ViewSpaceDirection.InwardHoriz))
            .Do(IncreaseCircleSizeFirst, "First circle size increase")
            .After(0.2f)
            .Then(PremadeGestureLibrary.ReversePushInViewDirection(_HandPhysicsTracker, ViewSpaceDirection.OutwardHoriz))
            .Do(IncreaseCircleSizeSecond, "Second circle size increase")
            .After(.2f)
            .Then(PremadeGestureLibrary.PushGlobal(_HandPhysicsTracker))
            .Do(FinishCasting, "Final cast");

        
        //_CastingSequence.Then(YesOrNo).Do()

    }
    public override void GripPress()
    {
        base.GripPress();
        _CastingSequence.Reset();
    }
    public override void GripHold()
    {
        base.GripHold();

        _CastingSequence.Update();

    }

    public override void GripRelease()
    {
        base.GripRelease();

        CancelCast();

        if (spellPrimed)
        {
            if (CheckCurrentMana(12))
            {
                for (int i = 0; i < 5; i++)
                {
                    projectileShooter.SpawnProjectile(_PalmTransform);
                }
                projectileShooter.LaunchAllProjectiles(_HandPhysicsTracker.UniveralPalm, 5);

                PlayerSingleton.Instance.SubtractMana(12);
            } 
        }

        spellPrimed = false;
    }
    private void FinishCasting()
    {
        circleAudioSource.PlayOneShot(CastCompleteSound);

        spellPrimed = true;
    }

    private void IncreaseCircleSizeFirst()
    {
        iTween.ScaleAdd(_SpellCircle, Vector3.one * .4f, .1f);

        AudioSource.PlayClipAtPoint(GestureCompleteSound, _HandTransform.position);
    }

    private void IncreaseCircleSizeSecond()
    {
        iTween.ScaleAdd(_SpellCircle, Vector3.one * .4f, .1f);

        AudioSource.PlayClipAtPoint(GestureCompleteSound, _HandTransform.position);

        secondarySpellCircle = Instantiate(secondarySpellCirclePrefab, _CircleHolderTransform.transform.TransformPoint(new(0, .06f, 0)), _CircleHolderTransform.transform.rotation, _CircleHolderTransform.transform);
        iTween.Stop(secondarySpellCirclePrefab);
    }

    private void CancelCast()
    {
        if(secondarySpellCircle)
            Destroy(secondarySpellCircle);

        iTween.ScaleTo(_SpellCircle, Vector3.one, .3f);
    }
}
