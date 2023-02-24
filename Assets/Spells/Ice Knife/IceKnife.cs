using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceKnife : SpellBlueprint
{
    private ObjectSpawn _objectSpawn;

    private void Start()
    {
        _objectSpawn = GetComponent<ObjectSpawn>();
        if(currentHand == 0)
        {
            _objectSpawn.SetRotationOffset(new Vector3(91, 0));
        }
    }
    private void Update()
    {
        if (!gripPressed)
        {
            if (Quaternion.Angle(spellCircle.transform.rotation, circleHolder.transform.rotation) > .1)
            {
                iTween.RotateUpdate(spellCircle, circleHolder.transform.rotation.eulerAngles, .1f);

            }
            if (Vector3.Distance(circleHolder.transform.position, spellCircle.transform.position) > .001f)
            {
                iTween.MoveUpdate(spellCircle, circleHolder.transform.position, .1f);
            }
        }
    }
    public override void GripPress()
    {
        base.GripPress();
        iTween.ScaleTo(spellCircle, Vector3.one * .5f, .7f);
    }
    public override void GripHold()
    {
        if (!triggerPressed)
        {
            if (currentHand == 0)
            {
                iTween.RotateUpdate(spellCircle, (circleHolder.transform.rotation * Quaternion.Euler(-90, 0, 0)).eulerAngles, .4f);
            }
            else
            {
                iTween.RotateUpdate(spellCircle, (circleHolder.transform.rotation * Quaternion.Euler(90, 0, 0)).eulerAngles, .4f);
            }
            iTween.MoveUpdate(spellCircle, circleHolder.transform.position + circleHolder.transform.TransformDirection(new Vector3(0, .05f, .1f)), .1f);

        }
    }
    public override void GripRelease()
    {
        base.GripRelease();
        iTween.ScaleTo(spellCircle, Vector3.one, .1f);
        if (_objectSpawn.instantiatedObject)
        {
            Destroy(_objectSpawn.instantiatedObject);
        }
    }
    public override void TriggerPress()
    {
        base.TriggerPress();

        if (gripPressed)
        {
            _objectSpawn.Cast(spellCircle.transform);
            iTween.ScaleFrom(_objectSpawn.instantiatedObject, Vector3.zero, .15f);
        }
    }
    public override void TriggerHold()
    {
        base.TriggerHold(); 
        if (gripPressed && _objectSpawn.instantiatedObject)
        {
            iTween.MoveUpdate(spellCircle, circleHolder.transform.position + circleHolder.transform.TransformDirection(new Vector3(0, .05f, -.04f)), 6);
            iTween.ScaleUpdate(_objectSpawn.instantiatedObject, iTween.Hash("time", 6, "z", 2));
            
        }
    }
    public override void TriggerRelease()
    {
        base.TriggerRelease(); 
        if (gripPressed && _objectSpawn.instantiatedObject)
        {
            _objectSpawn.LaunchProjectile(circleHolder.transform, currentHand);
        }
    }
}
