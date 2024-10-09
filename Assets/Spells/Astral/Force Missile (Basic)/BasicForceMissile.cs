using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FoxheadDev.GestureDetection;
public class BasicForceMissile : SpellBlueprint
{
    public AudioClip ShootSoundClip;

    private ProjectileShooter _Shooter;

    private Step _CastingSequence;
    private AudioSource _CircleAudioSource;
    public override void OnSelect()
    {
        base.OnSelect();
        _CastingSequence = Step.Start();

        _CastingSequence.ThenRepeatable(PremadeGestureLibrary.SlashGlobal(_HandPhysicsTracker, PremadeGestureLibrary.LargeGestureSpeed))
            .Do(LaunchMissile);

        _CastingSequence.ThenRepeatable(PremadeGestureLibrary.ReverseSlashGlobal(_HandPhysicsTracker, PremadeGestureLibrary.LargeGestureSpeed))
            .Do(LaunchMissile);

        _Shooter = GetComponent<ProjectileShooter>();
        _CircleAudioSource = _SpellCircle.GetComponent<AudioSource>();
    }


    //Direction is pointing from body to hand, ignoring the Y axis
    //Y is that of the hand at time of gesture completion
    //X & Z are that of the hand as well.

    public override void GripHold()
    {
        base.GripHold();

        _CastingSequence.Update();
    }
    public override void GripRelease()
    {
        base.GripRelease();
        _CastingSequence.Reset();
    }
    private void LaunchMissile()
    {
        Debug.Log("Missile Launched Sir!");

        Vector3[] directionalVectors = new Vector3[] { (_HandTransform.position - _PlayerRb.transform.position).normalized, _HandTransform.forward, Camera.main.transform.forward, Camera.main.transform.forward, Camera.main.transform.forward };

        Vector3 averageShootDirection = HandyHelperScript.GetVectorAverage(directionalVectors);

        _Shooter.SpawnProjectile(_HandTransform.position, Quaternion.LookRotation(averageShootDirection));

        _Shooter.LaunchAllProjectiles(averageShootDirection, 10);

        _CircleAudioSource.pitch = Random.Range(0.9f, 1.1f);
        _CircleAudioSource.PlayOneShot(ShootSoundClip);
    }
}
