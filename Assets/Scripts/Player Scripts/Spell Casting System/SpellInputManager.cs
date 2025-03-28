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

    public void NewRightSpell(SpellSwapCallbackContext ctx)
    {
        if (ctx.spawnedScript != null)
        {
            ctx.spawnedScript.currentHand = LeftRight.Right;
            currentRightSpell = ctx.spawnedScript; 
        }
    }
    public void NewLeftSpell(SpellSwapCallbackContext ctx)
    {
        if (ctx.spawnedScript != null)
        {
            ctx.spawnedScript.currentHand = LeftRight.Left;
            currentLeftSpell = ctx.spawnedScript; 
        }
    }
    public void ClearLeftSpell()
    {
        currentLeftSpell = null;
    }
    public void ClearRightSpell()
    {
        currentRightSpell = null;
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
            if (Physics.Raycast(currentLeftSpell._HandTransform.position, currentLeftSpell._HandTransform.forward, 6, UIMASK, QueryTriggerInteraction.Collide))
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
            if (Physics.Raycast(currentRightSpell._HandTransform.position, currentRightSpell._HandTransform.forward, 6, UIMASK, QueryTriggerInteraction.Collide))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    private void Start()
    {
        leftTrigger.action.started += LeftTriggerPress;
        leftTrigger.action.canceled += LeftTriggerRelease;

        leftGrip.action.started += LeftGripPress;
        leftGrip.action.canceled += LeftGripRelease;

        rightTrigger.action.started += RightTriggerPress;
        rightTrigger.action.canceled += RightTriggerRelease;

        rightGrip.action.started += RightGripPress;
        rightGrip.action.canceled += RightGripRelease;
    }

    private void RightGripRelease(InputAction.CallbackContext obj)
    {
        if(currentRightSpell != null)
        {
            currentRightSpell.GripRelease();
        }
    }
    private void RightGripPress(InputAction.CallbackContext obj)
    {
        if(currentRightSpell != null)
        {
            currentRightSpell.GripPress();
        }
    }
    private void RightTriggerRelease(InputAction.CallbackContext obj)
    {
        if(currentRightSpell != null && rightHandNotPointingAtUI)
        {
            currentRightSpell.TriggerRelease();
            rightHandNotPointingAtUI = false;
        }
    }
    private void RightTriggerPress(InputAction.CallbackContext obj)
    {
        if (currentRightSpell != null)
        {
            rightHandNotPointingAtUI = RaycastCheck(LeftRight.Right);

            if (rightHandNotPointingAtUI)
                currentRightSpell.TriggerPress(); 
        }
    }


    private void LeftGripRelease(InputAction.CallbackContext obj)
    {
        if(currentLeftSpell != null)
        {
            currentLeftSpell.GripRelease();
        }

    }
    private void LeftGripPress(InputAction.CallbackContext obj)
    {
        if (currentLeftSpell != null)
        {
            currentLeftSpell.GripPress();
        }
    }
    private void LeftTriggerRelease(InputAction.CallbackContext obj)
    {
        if(currentLeftSpell != null && leftHandNotPointingAtUI)
        {
            currentLeftSpell.TriggerRelease();
            leftHandNotPointingAtUI = false;
        }
    }
    private void LeftTriggerPress(InputAction.CallbackContext obj)
    {
        if (currentLeftSpell != null)
        {
            leftHandNotPointingAtUI = RaycastCheck(LeftRight.Left);

            if (leftHandNotPointingAtUI)
                currentLeftSpell.TriggerPress();
        }
    }
    private void Update()
    {
        if (currentLeftSpell)
        {
            //Hold events
            if(leftHandNotPointingAtUI)
                currentLeftSpell.TriggerHoldSafe();

            currentLeftSpell.GripHoldSafe();

            currentLeftSpell.gripPressedValue = leftGrip.action.ReadValue<float>();
            currentLeftSpell.triggerPressedValue = leftTrigger.action.ReadValue<float>();
        }
        if (currentRightSpell)
        {   
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
