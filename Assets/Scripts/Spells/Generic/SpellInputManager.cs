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
    


    //Current spells to allow for the use of the update method
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
    public void SetControlsLeft(SpellBlueprint spell)
    {
        leftTrigger.action.started += ctx => spell.TriggerPress();
        leftTrigger.action.canceled += ctx => spell.TriggerRelease();

        leftGrip.action.started += ctx => spell.GripPress();
        leftGrip.action.canceled += ctx => spell.GripRelease();

        spell.whichHand = LeftRight.Left;

        currentLeftSpell = spell;
    }

    public void SetControlsRight(SpellBlueprint spell)
    {
        rightTrigger.action.started += ctx => spell.TriggerPress();
        rightTrigger.action.canceled += ctx => spell.TriggerRelease();

        rightGrip.action.started += ctx => spell.GripPress();
        rightGrip.action.canceled += ctx => spell.GripRelease();

        spell.whichHand = LeftRight.Right;

        currentRightSpell = spell;
    }


    private void Update()
    {
        if (currentLeftSpell)
        {
            //Hold events
            currentLeftSpell.TriggerHoldSafe();
            currentLeftSpell.GripHoldSafe();

            //Button pressed values
            currentLeftSpell.gripPressedValue = leftGrip.action.ReadValue<float>();
            currentLeftSpell.triggerPressedValue = leftTrigger.action.ReadValue<float>();
        }
        if (currentRightSpell)
        {
            //Hold events
            currentRightSpell.TriggerHoldSafe();
            currentRightSpell.GripHoldSafe();

            //Button pressed values
            currentRightSpell.gripPressedValue = rightGrip.action.ReadValue<float>();
            currentRightSpell.triggerPressedValue = rightTrigger.action.ReadValue<float>();
        }
    }
}
