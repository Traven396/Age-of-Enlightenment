using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AgeOfEnlightenment.Player
{
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

                spell._PalmTransform = rightPalm;
                spell._BackOfHandTransform = rightBackOfHand;
                spell._HandTransform = rightHand;
                spell._CircleHolderTransform = rightCircleHolder.transform;
                if (rightCircleHolder.transform.childCount != 0)
                    spell._SpellCircle = rightCircleHolder.transform.GetChild(rightCircleHolder.transform.childCount - 1).gameObject;

                spell._GestureManager = rightGesture;
                spell._PlayerRb = playerRb;
                spell.playerPhys = playerPhys;
            }
        }
        public void NewParametersLeft(SpellSwapCallbackContext ctx)
        {
            if (ctx.spawnedScript != null)
            {
                var spell = ctx.spawnedScript;

                spell._PalmTransform = leftPalm;
                spell._BackOfHandTransform = leftBackOfHand;
                spell._HandTransform = leftHand;
                spell._CircleHolderTransform = leftCircleHolder.transform;
                if (leftCircleHolder.transform.childCount != 0)
                    spell._SpellCircle = leftCircleHolder.transform.GetChild(leftCircleHolder.transform.childCount - 1).gameObject;

                spell._GestureManager = leftGesture;
                spell._PlayerRb = playerRb;
                spell.playerPhys = playerPhys;
            }
        }
    }

}