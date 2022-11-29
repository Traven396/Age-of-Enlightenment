using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellParameterSupplier : MonoBehaviour
{
    public Rigidbody playerRb;
    [Space(10)]
    public Transform leftPalm;
    public Transform leftBackOfHand;
    public Transform leftHand;
    public GestureManager leftGesture;
    [Space(10)]
    public Transform rightPalm;
    public Transform rightBackOfHand;
    public Transform rightHand;
    public GestureManager rightGesture;

    public void SetParametersLeft(SpellBlueprint spell)
    {
        spell._palmLocation = leftPalm;
        spell._backOfHandLocation = leftBackOfHand;
        spell._handLocation = leftHand;
        spell._gestureManager = leftGesture;
        spell.playerRb = playerRb;
    }
    public void SetParametersRight(SpellBlueprint spell)
    {
        spell._palmLocation = rightPalm;
        spell._backOfHandLocation = rightBackOfHand;
        spell._handLocation = rightHand;
        spell._gestureManager = rightGesture;
        spell.playerRb = playerRb;
    }

    public void SetupTargetManger(TargetManager manager)
    {
        manager.leftHandPosition = leftHand;
        manager.rightHandPosition = rightHand;
    }
}
