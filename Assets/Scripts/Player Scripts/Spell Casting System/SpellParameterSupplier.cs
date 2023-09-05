using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellParameterSupplier : MonoBehaviour
{
    public Rigidbody playerRb;
    public FrankensteinCharacterController playerPhys;
    [Space(10)]
    public Transform leftPalm;
    public Transform leftBackOfHand;
    public Transform leftHand;
    public GameObject leftCircleHolder;

    [Space(10)]
    public GestureManager leftGesture;

    [Space(10)]
    public Transform rightPalm;
    public Transform rightBackOfHand;
    public Transform rightHand;
    public GameObject rightCircleHolder;

    [Space(10)]
    public GestureManager rightGesture;


    public void NewParametersRight(SpellSwapCallbackContext ctx)
    {
        if (ctx.spawnedScript != null)
        {
            var spell = ctx.spawnedScript;

            spell._palmLocation = rightPalm;
            spell._backOfHandLocation = rightBackOfHand;
            spell._handLocation = rightHand;
            spell.circleHolder = rightCircleHolder;
            if (rightCircleHolder.transform.childCount != 0)
                spell.spellCircle = rightCircleHolder.transform.GetChild(rightCircleHolder.transform.childCount - 1).gameObject;

            spell._gestureManager = rightGesture;
            spell.playerRb = playerRb;
            spell.playerPhys = playerPhys;
        }
    }
    public void NewParametersLeft(SpellSwapCallbackContext ctx)
    {
        if (ctx.spawnedScript != null)
        {
            var spell = ctx.spawnedScript;

            spell._palmLocation = leftPalm;
            spell._backOfHandLocation = leftBackOfHand;
            spell._handLocation = leftHand;
            spell.circleHolder = leftCircleHolder;
            if (leftCircleHolder.transform.childCount != 0)
                spell.spellCircle = leftCircleHolder.transform.GetChild(leftCircleHolder.transform.childCount - 1).gameObject;

            spell._gestureManager = leftGesture;
            spell.playerRb = playerRb;
            spell.playerPhys = playerPhys;
        }
    }
}
