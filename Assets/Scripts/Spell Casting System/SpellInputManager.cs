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


    [Header("Raycast Testing Shit god i hate my life")]
    public LayerMask UIMASK;
    //Current spell references
    private SpellBlueprint currentLeftSpell;
    private SpellBlueprint currentRightSpell;

    private bool leftHandNotPointingAtUI = true;
    private bool rightHandNotPointingAtUI = true;

    #region Changing Current Spells
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
    #endregion
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

    private bool RaycastCheck(LeftRight whichHand)
    {
        if (whichHand == 0)
        {
            if (Physics.Raycast(currentLeftSpell._handLocation.position, currentLeftSpell._handLocation.forward, 6, UIMASK))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            if (Physics.Raycast(currentRightSpell._handLocation.position, currentRightSpell._handLocation.forward, 6, UIMASK))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    private void Update()
    {
        if (currentLeftSpell)
        {
            //Checking for inputs to fire events
            if (leftTrigger.action.WasPressedThisFrame())
            {
                leftHandNotPointingAtUI = RaycastCheck(LeftRight.Left);

                if(leftHandNotPointingAtUI)
                    currentLeftSpell.TriggerPress();
            }
            if (leftTrigger.action.WasReleasedThisFrame() && leftHandNotPointingAtUI)
            {
                currentLeftSpell.TriggerRelease();
                leftHandNotPointingAtUI = false;
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
            if(leftHandNotPointingAtUI)
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
                rightHandNotPointingAtUI = RaycastCheck(LeftRight.Right);

                if(rightHandNotPointingAtUI)
                    currentRightSpell.TriggerPress();
            }
            if (rightTrigger.action.WasReleasedThisFrame() && rightHandNotPointingAtUI)
            {
                currentRightSpell.TriggerRelease();
                rightHandNotPointingAtUI = false;
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
            if(rightHandNotPointingAtUI)
                currentRightSpell.TriggerHoldSafe();
            currentRightSpell.GripHoldSafe();

            //Button pressed values
            currentRightSpell.gripPressedValue = rightGrip.action.ReadValue<float>();
            currentRightSpell.triggerPressedValue = rightTrigger.action.ReadValue<float>(); 
          
        }
    }

    private void FixedUpdate()
    {
        if (currentLeftSpell)
        {
            currentLeftSpell.GripHoldFixed();

            if(leftHandNotPointingAtUI)    
                currentLeftSpell.TriggerHoldFixed();     
        }

        if (currentRightSpell)
        {
            if (RaycastCheck(LeftRight.Right))
            {
                currentRightSpell.GripHoldFixed();

                if(rightHandNotPointingAtUI)
                    currentRightSpell.TriggerHoldFixed();  
            }
        }
    }
}
