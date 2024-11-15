using System.Collections;
using UnityEngine;

public class GravityBurst : SpellBlueprint
{
    [Header("Spell Stats")]
    public int baseManaCost = 7;
    public float DamageModifier = 1;

    [Header("Raycast Stats")]
    [SerializeField]
    private float sphereRadius = .2f;
    [SerializeField]
    private float castDistance = 5f;
    [SerializeField]
    private float minimumCharge = 1;

    

    private Chargeable _chargeable;
    private ApplyMotion _applyMotion;

    private bool visualMotion = false;

    private AudioSource spellSound;


    private void Start()
    {
        _chargeable = GetComponent<Chargeable>();
        _applyMotion = GetComponent<ApplyMotion>();

        _SpellCircle = _CircleHolderTransform.transform.GetChild(_CircleHolderTransform.transform.childCount - 1).gameObject;
        spellSound = _SpellCircle.GetComponent<AudioSource>();

    }
    public override void TriggerPress()
    {
        base.TriggerPress();

        _chargeable.ResetCharge();
        _chargeable.StartCharging();

        visualMotion = false;
        StopAllCoroutines();
    }
    public override void TriggerHold()
    {
        base.TriggerHold();

        iTween.ScaleUpdate(_SpellCircle, Vector3.one * 3, 6);

        if (currentHand == 0)
        {
            iTween.RotateUpdate(_SpellCircle, (_CircleHolderTransform.transform.rotation * Quaternion.Euler(-90, 0, 0)).eulerAngles, .4f);
        }
        else
        {
            
            iTween.RotateUpdate(_SpellCircle, (_CircleHolderTransform.transform.rotation * Quaternion.Euler(90, 0, 0)).eulerAngles, .4f);
        }
        iTween.MoveUpdate(_SpellCircle, _CircleHolderTransform.transform.position + _CircleHolderTransform.transform.TransformDirection(new Vector3(0, -.05f, .25f)), .4f);
    }
    public override void TriggerRelease()
    {
        base.TriggerRelease();
        _chargeable.StopCharging();

        if (_chargeable.GetCurrentCharge() >= minimumCharge)
        {
            int curManaCost = (int)(baseManaCost * _chargeable.GetCurrentCharge()) / 2;
            
            if (CheckCurrentMana(curManaCost))
            {
                spellSound.pitch = Random.Range(.35f, .45f);
                spellSound.Play();

                RaycastHit[] hits = _TargetManager.HandSphereCastAll(currentHand, castDistance, sphereRadius, _HandPhysicsTracker.UniveralPalm);

                foreach (RaycastHit hit in hits)
                {
                    Rigidbody rb = hit.rigidbody;
                    if (hit.collider.TryGetComponent(out IEntity entity))
                    {
                        Vector3 forceDirection = (rb.position - _HandTransform.position).normalized;
                        entity.ApplyMotion(forceDirection * _applyMotion.forceMultiplier * _chargeable.GetCurrentCharge(), _applyMotion.forceType);
                    }
                    else if (rb != null)
                    {
                        Vector3 forceDirection = (rb.position - _HandTransform.position).normalized;
                        _applyMotion.Cast(rb, forceDirection * _chargeable.GetCurrentCharge());
                    }
                    if (hit.collider.TryGetComponent(out IDamageable target))
                    {
                        target.TakeDamage(_applyMotion.forceMultiplier * DamageModifier * _chargeable.GetCurrentCharge(), DamageType.Force);
                    }
                }
                StartCoroutine(CirclePushVisuals());

                PlayerSingleton.Instance._Stats.SubtractMana(curManaCost);
            }
        }
        else
        {
            visualMotion = true;
        }
        iTween.ScaleTo(_SpellCircle, Vector3.one, .5f);
    }
    private void Update()
    {
        if (!triggerPressed && visualMotion)
        {
            if (Vector3.Distance(_SpellCircle.transform.position, _CircleHolderTransform.transform.position) > .005f)
            {
                iTween.MoveUpdate(_SpellCircle, _CircleHolderTransform.transform.position, .4f);
            }
            else
            {
                visualMotion = false;
            }
            if(Quaternion.Angle(_SpellCircle.transform.rotation, _CircleHolderTransform.transform.rotation) > .1)
            {
                iTween.RotateUpdate(_SpellCircle, _CircleHolderTransform.transform.rotation.eulerAngles, .1f);
            }
        }
    }

    IEnumerator CirclePushVisuals()
    {
        iTween.PunchPosition(_SpellCircle, new Vector3(0, 0, .1f), .3f);

        yield return new WaitForSeconds(.5f);

        visualMotion = true;

        yield return null;
    }
}
