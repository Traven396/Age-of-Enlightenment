using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class SpellBlueprint : MonoBehaviour
{
    protected bool triggerPressed = false;
    protected bool gripPressed = false;

    [HideInInspector] public LeftRight currentHand;

    //Physical world parameters
    [HideInInspector] public Rigidbody playerRb;
    [HideInInspector] public FrankensteinCharacterController playerPhys;
    [HideInInspector] public GameObject circleHolder;
    [HideInInspector] public GameObject spellCircle;

    //Location parameters
    [HideInInspector] public Transform _palmLocation;
    [HideInInspector] public Transform _backOfHandLocation;
    [HideInInspector] public Transform _handLocation;
    [HideInInspector] public Transform _playerLocation;

    //Code parameters
    [HideInInspector] public TargetManager _targetManager;
    [HideInInspector] public GestureManager _gestureManager;
    [HideInInspector] public SpellVisualsManager _visualsManager;

    //Button pressed values
    [HideInInspector] public float triggerPressedValue;
    [HideInInspector] public float gripPressedValue;

    #region Input Events

    public virtual void TriggerPress()
    {
        triggerPressed = true;
    }
    public virtual void TriggerRelease()
    {
        triggerPressed = false;
    }
    public void TriggerHoldSafe() { if (triggerPressed){ TriggerHold(); } }
    public void TriggerHoldFixedSafe() { if (triggerPressed) { TriggerHoldFixed(); } }
    public virtual void TriggerHold() { }
    public virtual void TriggerHoldFixed() { }
    public virtual void GripPress()
    {
        gripPressed = true;
    }
    public virtual void GripRelease()
    {
        gripPressed = false;
    }
    public  void GripHoldSafe() { if (gripPressed) { GripHold(); } }
    public  void GripHoldFixedSafe() { if (gripPressed) { GripHoldFixed(); } }
    public virtual void GripHold() { }

    public virtual void GripHoldFixed()
    {
    }
    #endregion

    public virtual void OnDeselect()
    {

    }
    private void Start()
    {
        spellCircle = circleHolder.transform.GetChild(circleHolder.transform.childCount - 1).gameObject;
    }
}

public enum SpellSchool
{
    Pyrokinesis,
    Geomancy,
    Tempestia,
    Hydromorphy,
    Frostweaving,
    Voltcraft,
    Druidic,
    Metallurgy,
    Astral
}