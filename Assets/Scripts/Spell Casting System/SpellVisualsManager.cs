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

    private GameObject currentLeftCircle;
    private GameObject currentRightCircle;

    public void NewRightSpell(SpellSwapCallbackContext ctx)
    {
        DespawnCircle(LeftRight.Right);
        if (ctx.circle)
            SpawnNewCircle(ctx.circle, LeftRight.Right);

        rightHandController.runtimeAnimatorController = ctx.newAnimator != null ? ctx.newAnimator : defaultRightController;

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

    //Add more methods here to easily allow the moving of the spell circle to a variety of offsets
    //make a persistent offsetAmount and then every frame be trying to move the circle there?
}
