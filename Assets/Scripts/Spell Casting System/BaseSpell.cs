using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSpell : ScriptableObject
{
    public GameObject castHoldVisuals;

    public LeftRight hand;

    protected bool triggerPressed = false;
    protected bool gripPressed = false;

    protected Transform palmPoint;
    protected Transform backOfHand;
    protected GameObject player;
    protected Vector3 handVelocity;

    protected TargetManager _targetManager;

    //protected List<GameObject> potentialObjectTargets = new List<GameObject>();



    public void SetTargetManager(TargetManager manager)
    {
        _targetManager = manager;
    }
    public void SetupParamters(List<GameObject> spellParameters)
    {
        
        palmPoint = spellParameters[0].transform;
        backOfHand = spellParameters[1].transform;
        
        player = spellParameters[spellParameters.Count - 1];
        
    }
    public void SetControllerVelocity(Vector3 inputVelocity)
    {
        handVelocity = inputVelocity;
    }
    public virtual void TriggerPress() {
        triggerPressed = true;
    }
    public virtual void TriggerRelease() {
        triggerPressed = false;
    }
    public virtual void TriggerHold() { 
    }
    public virtual void GripPress()
    {
        gripPressed = true;
    }
    public virtual void GripRelease()
    {
        gripPressed = false;
    }

    public virtual void GripHold() { }

    public virtual void SpellUpdate()
    {}
}



public enum LeftRight
{
    Left,
    Right
}
