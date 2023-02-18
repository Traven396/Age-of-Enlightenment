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


    public void ChangeAnimatorController(LeftRight side, AnimatorOverrideController newController)
    {
        if(side == 0)
        {
            leftHandController.runtimeAnimatorController = newController != null ? newController : defaultLeftController;
        }
        else
        {
            rightHandController.runtimeAnimatorController = newController != null ? 
                newController : defaultRightController;
        }
    }
    public void ChangeRightCircle(GameObject circleToBe)
    {
        DespawnCircle(LeftRight.Right);
        if(circleToBe)
            SpawnNewCircle(circleToBe, LeftRight.Right);
    }

    public void ChangeLeftCircle(GameObject circleToBe)
    {
        DespawnCircle(LeftRight.Left);
        if(circleToBe)
            SpawnNewCircle(circleToBe, LeftRight.Left);
    }

    public void DespawnCircle(LeftRight side)
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
}
