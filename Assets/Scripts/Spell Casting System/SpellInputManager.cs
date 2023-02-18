using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpellInputManager : MonoBehaviour
{
    [Header("Left Hand Controls")]
    public InputActionReference leftTrigger;
    public InputActionReference leftGrip;
    public InputActionReference leftAButton;
  

    [Header("Right Hand Controls")]
    public InputActionReference rightTrigger;
    public InputActionReference rightGrip;
    public InputActionReference rightAButton;
    


    //Current spell references
    private SpellBlueprint currentLeftSpell;
    private SpellBlueprint currentRightSpell;

    public void ClearLeftSpell()
    {
        currentLeftSpell = null;
    }
    public void ClearRightSpell()
    {
        currentRightSpell = null;
    }
    public void SetLeftSpell(SpellBlueprint spell)
    {
        spell.currentHand = LeftRight.Left;
        currentLeftSpell = spell;
    }
    public void SetRightSpell(SpellBlueprint spell)
    {
        spell.currentHand = LeftRight.Right;
        currentRightSpell = spell;
    }
    //public void SetControlsLeft(SpellBlueprint spell)
    //{
    //    leftTrigger.action.started += ctx => spell.TriggerPress();
    //    leftTrigger.action.canceled += ctx => spell.TriggerRelease();

    //    leftGrip.action.started += ctx => spell.GripPress();
    //    leftGrip.action.canceled += ctx => spell.GripRelease();

    //    spell.whichHand = LeftRight.Left;

    //    currentLeftSpell = spell;
    //}

    //public void SetControlsRight(SpellBlueprint spell)
    //{
    //    rightTrigger.action.started += ctx => spell.TriggerPress();
    //    rightTrigger.action.canceled += ctx => spell.TriggerRelease();

    //    rightGrip.action.started += ctx => spell.GripPress();
    //    rightGrip.action.canceled += ctx => spell.GripRelease();

    //    spell.whichHand = LeftRight.Right;

    //    currentRightSpell = spell;
    //}

    //public void ClearControlsLeft(SpellBlueprint spell)
    //{
    //    leftTrigger.action.started -= ctx => spell.TriggerPress();
    //    leftTrigger.action.canceled -= ctx => spell.TriggerRelease();

    //    leftGrip.action.started -= ctx => spell.GripPress();
    //    leftGrip.action.canceled -= ctx => spell.GripRelease();

    //    currentLeftSpell = null;
    //}

    //public void ClearControlsRight(SpellBlueprint spell)
    //{
    //    rightTrigger.action.started -= ctx => spell.TriggerPress();
    //    leftTrigger.action.canceled -= ctx => spell.TriggerRelease();

    //    rightGrip.action.started -= ctx => spell.GripPress();
    //    rightGrip.action.canceled -= ctx => spell.GripRelease();

    //    currentRightSpell = null;

    //}



    private void Update()
    {
        if (currentLeftSpell)
        {
            //Checking for inputs to fire events
            if (leftTrigger.action.WasPressedThisFrame())
            {
                currentLeftSpell.TriggerPress();
            }
            if (leftTrigger.action.WasReleasedThisFrame())
            {
                currentLeftSpell.TriggerRelease();
            }
            if (leftGrip.action.WasPressedThisFrame())
            {
                currentLeftSpell.GripPress();
            }
            if (leftGrip.action.WasReleasedThisFrame())
            {
                currentLeftSpell.GripRelease();
            }

            //Hold events
            currentLeftSpell.TriggerHoldSafe();
            currentLeftSpell.GripHoldSafe();

            //Button pressed values
            currentLeftSpell.gripPressedValue = leftGrip.action.ReadValue<float>();
            currentLeftSpell.triggerPressedValue = leftTrigger.action.ReadValue<float>();
        }
        if (currentRightSpell)
        {
            //Checking for inputs to fire events
            if (rightTrigger.action.WasPressedThisFrame())
            {
                currentRightSpell.TriggerPress();
            }
            if (rightTrigger.action.WasReleasedThisFrame())
            {
                currentRightSpell.TriggerRelease();
            }
            if (rightGrip.action.WasPressedThisFrame())
            {
                currentRightSpell.GripPress();
            }
            if (rightGrip.action.WasReleasedThisFrame())
            {
                currentRightSpell.GripRelease();
            }

            //Hold events
            currentRightSpell.TriggerHoldSafe();
            currentRightSpell.GripHoldSafe();

            //Button pressed values
            currentRightSpell.gripPressedValue = rightGrip.action.ReadValue<float>();
            currentRightSpell.triggerPressedValue = rightTrigger.action.ReadValue<float>();
        }
    }
}
