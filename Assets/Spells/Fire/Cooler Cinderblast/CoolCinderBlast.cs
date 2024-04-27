using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AgeOfEnlightenment.GestureDetection;

public class CoolCinderBlast : SpellBlueprint
{
    public AudioClip GestureCompleteSound;
    public AudioClip CastCompleteSound;

    private Step _CastingSequence;
    
    private AudioSource circleAudioSource;
    private ProjectileShooter projectileShooter;

    private GameObject secondarySpellCircle;
    
    public override void OnSelect()
    {
        base.OnSelect();

        circleAudioSource = spellCircle.GetComponent<AudioSource>();

        projectileShooter = GetComponent<ProjectileShooter>();

        projectileShooter.SetDamage(5, DamageType.Fire);
        projectileShooter.SetSpread(65);

        _CastingSequence = Step.Start();

        _CastingSequence.Then(PremadeGestureLibrary.PushInViewDirection(_handPhysics, ViewSpaceDirection.InwardH))
            .Do(IncreaseCircleSizeFirst, "First circle size increase")
            .After(0.2f)
            .Then(PremadeGestureLibrary.ReversePushInViewDirection(_handPhysics, ViewSpaceDirection.OutwardH))
            .Do(IncreaseCircleSizeSecond, "Second circle size increase")
            .After(.2f)
            .Then(PremadeGestureLibrary.PushInViewDirection(_handPhysics, ViewSpaceDirection.Forward))
            .Do(FinishCasting, "Final cast");
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
    }
    private void FinishCasting()
    {
        circleAudioSource.PlayOneShot(CastCompleteSound);

        CancelCast();

        if (Player.Instance.currentMana >= 12)
        {
            for (int i = 0; i < 5; i++)
            {
                projectileShooter.SpawnProjectile(_palmLocation);
            }
            projectileShooter.LaunchAllProjectiles(_palmLocation.forward, 5);

            Player.Instance.SubtractMana(12);
        }
    }

    private void IncreaseCircleSizeFirst()
    {
        iTween.ScaleAdd(spellCircle, Vector3.one * .4f, .1f);

        AudioSource.PlayClipAtPoint(GestureCompleteSound, _handLocation.position);
    }

    private void IncreaseCircleSizeSecond()
    {
        iTween.ScaleAdd(spellCircle, Vector3.one * .4f, .1f);

        AudioSource.PlayClipAtPoint(GestureCompleteSound, _handLocation.position);

        secondarySpellCircle = Instantiate(spellCircle, circleHolder.transform.TransformPoint(new(0, .06f, 0)), circleHolder.transform.rotation, circleHolder.transform);
        iTween.Stop(secondarySpellCircle);
    
    }

    private void CancelCast()
    {
        if(secondarySpellCircle)
            Destroy(secondarySpellCircle);

        iTween.ScaleTo(spellCircle, Vector3.one, .3f);
    }
}
