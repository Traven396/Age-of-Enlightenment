using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TerraBlade : SpellBlueprint
{
    [Header("Mana Cost")]
    public int manaCost = 15;
    public float launchSpeed = 500f;

    private bool bladeSpawned = false;
    private bool alreadyShot = false;


    private ObjectSpawn BladeSpawner;
    private IMovement _gesture;

    public InputActionReference overrideButton;
    private void Start()
    {
        _gesture = GetComponent<IMovement>();
        BladeSpawner = GetComponent<ObjectSpawn>();
        if(currentHand == 0)
        {
            BladeSpawner.SetRotationOffset(new Vector3(91, 0));
        }
    }
    private void Update()
    {
        if (!gripPressed)
        {
            _VisualsManager.ReturnCircleToHolder(currentHand);
        }
    }
    public override void GripPress()
    {
        base.GripPress();
        iTween.ScaleTo(_SpellCircle, Vector3.one * .3f, .7f);
    }

    public override void GripHold()
    {
        if (currentHand == 0)
        {
            iTween.RotateUpdate(_SpellCircle, (_CircleHolderTransform.transform.rotation * Quaternion.Euler(-90, 0, 0)).eulerAngles, .4f);
        }
        else
        {
            iTween.RotateUpdate(_SpellCircle, (_CircleHolderTransform.transform.rotation * Quaternion.Euler(90, 0, 0)).eulerAngles, .4f);
        }
        iTween.MoveUpdate(_SpellCircle, _CircleHolderTransform.transform.position + _CircleHolderTransform.transform.TransformDirection(new Vector3(0, .05f, .1f)), .1f);
        
        if (_gesture.GesturePerformed(_HandPhysicsTracker, out var direction) || overrideButton.action.WasPressedThisFrame())
        {
            if (!bladeSpawned && !alreadyShot)
            {
                if (Player.Instance.currentMana >= manaCost)
                {
                    Player.Instance.SubtractMana(manaCost);

                    bladeSpawned = true;

                    BladeSpawner.Cast(_SpellCircle.transform);

                    _SpellCircle.GetComponent<AudioSource>().Play();

                    BladeSpawner.instantiatedObject.transform.localScale = Vector3.one * .25f;
                    iTween.ScaleFrom(BladeSpawner.instantiatedObject, Vector3.zero, .8f); 
                }
            } 
        }
        if (bladeSpawned)
        {
            BladeSpawner.instantiatedObject.transform.rotation = _SpellCircle.transform.rotation * Quaternion.Euler(BladeSpawner.GetRotationOffset());

            var distance = Vector3.Distance(BladeSpawner.instantiatedObject.transform.position, _SpellCircle.transform.position);
            BladeSpawner.instantiatedRB.velocity = (_SpellCircle.transform.position - BladeSpawner.instantiatedObject.transform.position).normalized * 35f * distance;
        }
    }

    public override void GripRelease()
    {
        base.GripRelease();
        if (bladeSpawned && !alreadyShot) { 
            bladeSpawned = false;

            if(BladeSpawner.instantiatedObject)
                iTween.ScaleTo(BladeSpawner.instantiatedObject, Vector3.zero, .5f);

            Destroy(BladeSpawner.instantiatedObject, .5f);
        }
        iTween.ScaleTo(_SpellCircle, Vector3.one, .7f);

        alreadyShot = false;
    }
    public override void TriggerPress()
    {
        base.TriggerPress();

        if (bladeSpawned)
        {
            bladeSpawned = false;
            alreadyShot = true;
            BladeSpawner.LaunchProjectile(_SpellCircle.transform, currentHand, launchSpeed);

            iTween.PunchPosition(_SpellCircle, new Vector3(0, .1f, 0f), .3f);
        }
    }
    public override void OnDeselect()
    {
        base.OnDeselect();

        if(BladeSpawner.instantiatedObject != null)
            Destroy(BladeSpawner.instantiatedObject);
    }
}
