using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class SpellBlueprint : MonoBehaviour
{
    protected bool triggerPressed = false;
    protected bool gripPressed = false;

    [HideInInspector] public LeftRight whichHand;

    //Physical world parameters
    [HideInInspector] public Rigidbody playerRb;

    //Location parameters
    [HideInInspector] public Transform _palmLocation;
    [HideInInspector] public Transform _backOfHandLocation;
    [HideInInspector] public Transform _handLocation;
    [HideInInspector] public Transform _playerLocation;

    //Code parameters
    [HideInInspector] public TargetManager _targetManager;
    [HideInInspector] public GestureManager _gestureManager;

    //Button pressed values
    [HideInInspector] public float triggerPressedValue;
    [HideInInspector] public float gripPressedValue;

    #region Input Events

    public virtual void TriggerTest(InputAction.CallbackContext obj) { }
    public virtual void TriggerPress()
    {
        triggerPressed = true;
    }
    public virtual void TriggerRelease()
    {
        triggerPressed = false;
    }
    public void TriggerHoldSafe() { if (triggerPressed){ TriggerHold(); } }
    public virtual void TriggerHold() { }
    public virtual void GripPress()
    {
        gripPressed = true;
    }
    public virtual void GripRelease()
    {
        gripPressed = false;
    }
    public void GripHoldSafe() { if (gripPressed) { GripHold(); } }
    public virtual void GripHold() { }
    public virtual void SpellUpdate() { }
    #endregion
}

