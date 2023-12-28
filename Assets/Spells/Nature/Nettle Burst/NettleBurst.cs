using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NettleBurst : SpellBlueprint
{
    [SerializeField] private float ChargeTime = 2.5f;
    [SerializeField] private float FiringCooldown = 0.4f;
    [SerializeField] private float LaunchSpeed = 10f;
    [SerializeField] private int ManaCostPerShot = 3;

    private float currentCharge = 0;

    private ProjectileShooter _shooter;

    private bool currentlyShooting = false;

    private void Awake()
    {
        _shooter = GetComponent<ProjectileShooter>();
    }
    public override void TriggerPress()
    {
        base.TriggerPress();


    }
    public override void TriggerHold()
    {
        base.TriggerHold();

        if (currentHand == 0)
        {
            iTween.RotateUpdate(spellCircle, (circleHolder.transform.rotation * Quaternion.Euler(-90, 0, 0)).eulerAngles, .4f);
        }
        else
        {
            iTween.RotateUpdate(spellCircle, (circleHolder.transform.rotation * Quaternion.Euler(90, 0, 0)).eulerAngles, .4f);
        }
        iTween.MoveUpdate(spellCircle, circleHolder.transform.TransformPoint(new Vector3(0, -.05f, .25f)), .4f);

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
    }

    private void Update()
    {
        if (!triggerPressed)
        {
            _visualsManager.ReturnCircleToHolder(currentHand);
        }
    }

    private IEnumerator MachineGun()
    {   
        while (true)
        {
            if (Player.Instance.currentMana >= ManaCostPerShot)
            {
                Player.Instance.SubtractMana(ManaCostPerShot);

                _shooter.SpawnProjectile(spellCircle.transform.position);
                _shooter.LaunchAllProjectiles(spellCircle.transform.up, LaunchSpeed); 
            }

            yield return new WaitForSeconds(FiringCooldown); 
        }
    }
}
