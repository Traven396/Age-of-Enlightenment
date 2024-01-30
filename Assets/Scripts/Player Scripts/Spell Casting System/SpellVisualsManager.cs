using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellVisualsManager : MonoBehaviour
{
    [Header("Spell Circle Stuff")]
    public GameObject leftCircleHolder;
    public GameObject rightCircleHolder;
    [Space(20f)]
    [Header("Animation Stuff")]
    
    public RuntimeAnimatorController defaultLeftController;

    
    public RuntimeAnimatorController defaultRightController;

    public Animator leftHandController;
    public Animator rightHandController;

    [Space(20f)]
    [Header("Reticles")]
    public Reticle leftReticle;
    public Reticle rightReticle;

    private GameObject currentLeftCircle;
    private GameObject currentRightCircle;

    private Vector3 _leftPositionOffset, _leftRotationOffset;
    private Vector3 _rightPositionOffset, _rightRotationOffset;

    public void NewRightSpell(SpellSwapCallbackContext ctx)
    {
        DespawnCircle(LeftRight.Right);
        if (ctx.circle)
            SpawnNewCircle(ctx.circle, LeftRight.Right);

        rightHandController.runtimeAnimatorController = ctx.newAnimator != null ? ctx.newAnimator : defaultRightController;

        _rightPositionOffset = Vector3.zero;
        _rightRotationOffset = Vector3.zero;

        if (ctx.spawnedScript != null)
        {
            ctx.spawnedScript._visualsManager = this; 
        }
    }
    public void NewLeftSpell(SpellSwapCallbackContext ctx)
    {
        DespawnCircle(LeftRight.Left);
        if (ctx.circle)
            SpawnNewCircle(ctx.circle, LeftRight.Left);

        leftHandController.runtimeAnimatorController = ctx.newAnimator != null ? ctx.newAnimator : defaultLeftController;

        _leftRotationOffset = Vector3.zero;
        _leftPositionOffset = Vector3.zero;

        if (ctx.spawnedScript != null)
            ctx.spawnedScript._visualsManager = this;
    }

    private void DespawnCircle(LeftRight side)
    {
        if (side == 0)
        {
            Destroy(currentLeftCircle);
        }
        else
        {
            Destroy(currentRightCircle);
        }
    }
    private void SpawnNewCircle(GameObject circleToBe, LeftRight side)
    {
        if (side == 0)
        {
            currentLeftCircle = Instantiate(circleToBe, leftCircleHolder.transform);
        }
        else
        {
            currentRightCircle = Instantiate(circleToBe, rightCircleHolder.transform);
        }
    }

    public void ReturnCircleToHolder(LeftRight whichHand)
    {
        if (whichHand == 0)
        {
            if (Quaternion.Angle(currentLeftCircle.transform.rotation, leftCircleHolder.transform.rotation) > .1)
            {
                iTween.RotateUpdate(currentLeftCircle, leftCircleHolder.transform.rotation.eulerAngles, .1f);

            }
            if (Vector3.Distance(leftCircleHolder.transform.position, currentLeftCircle.transform.position) > .001f)
            {
                iTween.MoveUpdate(currentLeftCircle, leftCircleHolder.transform.position, .1f);
            } 
        }
        else
        {
            if(Quaternion.Angle(currentRightCircle.transform.rotation, rightCircleHolder.transform.rotation) > .1)
            {
                iTween.RotateUpdate(currentRightCircle, rightCircleHolder.transform.rotation.eulerAngles, .1f);

            }
            if (Vector3.Distance(rightCircleHolder.transform.position, currentRightCircle.transform.position) > .001f)
            {
                iTween.MoveUpdate(currentRightCircle, rightCircleHolder.transform.position, .1f);
            }
        }
    }

    public void ToggleReticle(LeftRight whichHand, bool disableOrEnable)
    {
        if(whichHand == LeftRight.Left)
        {
            if (!disableOrEnable)
                leftReticle.Hide();
            else
                leftReticle.Show();
        }
        else
        {
            if (disableOrEnable)
                rightReticle.Show();
            else
                rightReticle.Hide();
        }
    }

    //Add more methods here to easily allow the moving of the spell circle to a variety of offsets
    //make a persistent offsetAmount and then every frame be trying to move the circle there?

    private void Update()
    {
        if(currentLeftCircle)
        {
            if(_leftPositionOffset != Vector3.zero)
                iTween.MoveUpdate(currentLeftCircle, leftCircleHolder.transform.TransformPoint(new Vector3(-_leftPositionOffset.x, _leftPositionOffset.y, _leftPositionOffset.z)), .8f);
            
            if(_leftRotationOffset != Vector3.zero)
                iTween.RotateUpdate(currentLeftCircle, (leftCircleHolder.transform.rotation * Quaternion.Euler(_leftRotationOffset)).eulerAngles, .4f);

        }
        if (currentRightCircle)
        {
            if (_rightPositionOffset != Vector3.zero)
                iTween.MoveUpdate(currentRightCircle, rightCircleHolder.transform.TransformPoint(_rightPositionOffset), .8f);

            if (_rightRotationOffset != Vector3.zero)
                iTween.RotateUpdate(currentRightCircle, (rightCircleHolder.transform.rotation * Quaternion.Euler(_rightRotationOffset)).eulerAngles, .4f);

        }
    }
    public void IncreasePositionOffset(Vector3 changeAmount, LeftRight whichHand)
    {
        if (whichHand == LeftRight.Left)
            _leftPositionOffset += changeAmount;
        else
            _rightPositionOffset += changeAmount;
    }
    public void ReducePositionOffset(Vector3 changeAmount, LeftRight whichHand)
    {
        if (whichHand == LeftRight.Left)
            _leftPositionOffset -= changeAmount;
        else
            _rightPositionOffset -= changeAmount;
    }
    public void SetPositionOffset(Vector3 changeAmount, LeftRight whichHand)
    {
        if (whichHand == LeftRight.Left)
            _leftPositionOffset = changeAmount;
        else
            _rightPositionOffset = changeAmount;
    }
    public void ResetPositionOffset(LeftRight whichHand)
    {
        if (whichHand == LeftRight.Left)
            _leftPositionOffset = Vector3.zero;
        else
            _rightPositionOffset = Vector3.zero;
    }

    public void IncreaseRotationOffset(Vector3 changeAmount, LeftRight whichHand)
    {
        if (whichHand == LeftRight.Left)
            _leftRotationOffset += changeAmount;
        else
            _rightRotationOffset += changeAmount;
    }
    public void ReduceRotationOffset(Vector3 changeAmount, LeftRight whichHand)
    {
        if (whichHand == LeftRight.Left)
            _leftRotationOffset -= changeAmount;
        else
            _rightRotationOffset -= changeAmount;
    }
    public void SetRotationOffset(Vector3 changeAmount, LeftRight whichHand)
    {
        if(whichHand == LeftRight.Left)
            _leftRotationOffset = changeAmount;
        else
            _rightRotationOffset = changeAmount;
    }
    public void ResetRotationOffset(LeftRight whichHand)
    {
        if (whichHand == LeftRight.Left)
            _leftRotationOffset = Vector3.zero;
        else
            _rightRotationOffset = Vector3.zero;
    }
}
