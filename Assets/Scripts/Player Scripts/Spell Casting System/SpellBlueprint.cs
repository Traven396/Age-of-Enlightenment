using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FoxheadDev.GestureDetection;
using AgeOfEnlightenment.PlayerController;
public abstract class SpellBlueprint : MonoBehaviour
{
    protected bool triggerPressed = false;
    protected bool gripPressed = false;

    [HideInInspector] public LeftRight currentHand;

    //Physical world parameters
    [HideInInspector] public Rigidbody _PlayerRb;
    [HideInInspector] public FrankensteinCharacterController playerPhys;
    [HideInInspector] public PlayerMotionController _MotionController;
    [HideInInspector] public Transform _CircleHolderTransform;
    [HideInInspector] public GameObject _SpellCircle;

    //Location parameters
    [HideInInspector] public Transform _PalmTransform;
    [HideInInspector] public Transform _BackOfHandTransform;
    [HideInInspector] public Transform _HandTransform;
    [HideInInspector] public Transform _PlayerTransform;

    //Code parameters
    [HideInInspector] public TargetManager _TargetManager;
    [HideInInspector] public GestureManager _GestureManager;
    [HideInInspector] public HandPhysicsTracker _HandPhysicsTracker;
    [HideInInspector] public SpellVisualsManager _VisualsManager;

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

    public virtual void OnDeselect() { }
    public virtual void OnSelect() { }
    private void Start()
    {
        _SpellCircle = _CircleHolderTransform.transform.GetChild(_CircleHolderTransform.transform.childCount - 1).gameObject;
    }

    protected bool CheckCurrentMana(int manaCost)
    {
        if (PlayerSingleton.Instance._Stats._CurrentMana >= manaCost)
            return true;
        else
            return false;
    }
}