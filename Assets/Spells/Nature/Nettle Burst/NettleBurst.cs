using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NettleBurst : SpellBlueprint
{
    [SerializeField] private float ChargeTime = 2.5f;
    [SerializeField] private float FiringCooldown = 0.4f;
    [SerializeField] private float LaunchSpeed = 10f;
    [SerializeField] private float Damage = 3;
    [SerializeField] private int ManaCostPerShot = 3;

    private float currentCharge = 0;

    private ProjectileShooter _shooter;

    private bool currentlyShooting = false;

    private void Awake()
    {
        _shooter = GetComponent<ProjectileShooter>();
        _shooter.SetDamage(Damage, DamageType.Nature);
    }

    public override void TriggerPress()
    {
        base.TriggerPress();

        _VisualsManager.ToggleReticle(currentHand, true);
    }
    public override void TriggerHold()
    {
        base.TriggerHold();
        Debug.Log("We Holding");
        if (currentHand == 0)
        {
            iTween.RotateUpdate(_SpellCircle, (_HandTransform.transform.rotation * Quaternion.Euler(-90, 0, 0)).eulerAngles, .4f);
        }
        else
        {
            iTween.RotateUpdate(_SpellCircle, (_HandTransform.transform.rotation * Quaternion.Euler(90, 0, 0)).eulerAngles, .4f);
        }
        iTween.MoveUpdate(_SpellCircle, _CircleHolderTransform.transform.TransformPoint(new Vector3(0, -.05f, .25f)), .4f);

        if (currentCharge < ChargeTime)
        {
            currentCharge += Time.deltaTime;
        }
        else
        {
            if (!currentlyShooting)
            {
                currentlyShooting = true;
                StartCoroutine(nameof(MachineGun));
            }
        }
    }

    public override void TriggerRelease()
    {
        base.TriggerRelease();

        currentCharge = 0;
        currentlyShooting = false;

        StopCoroutine(nameof(MachineGun));
        _VisualsManager.ToggleReticle(currentHand, false);
    }

    private void Update()
    {
        if (!triggerPressed)
        {
            _VisualsManager.ReturnCircleToHolder(currentHand);
        }
    }

    private IEnumerator MachineGun()
    {   
        while (true)
        {
            if (CheckCurrentMana(ManaCostPerShot))
            {
                PlayerSingleton.Instance._Stats.SubtractMana(ManaCostPerShot);

                _shooter.SpawnProjectile(_SpellCircle.transform.position);
                _shooter.LaunchAllProjectiles(_HandTransform.forward, LaunchSpeed); 
            }

            yield return new WaitForSeconds(FiringCooldown); 
        }
    }
}
