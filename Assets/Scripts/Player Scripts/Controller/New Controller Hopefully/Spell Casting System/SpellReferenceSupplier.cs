using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AgeOfEnlightenment.PlayerController
{
    public class SpellReferenceSupplier : MonoBehaviour
    {
        [Header("Hand-Independent References")]
        public Rigidbody _PlayerRb;
        public PlayerMotionController _PlayerMotion;
        [Space(10)]
        [Header("Left Hand")]
        public Transform _LeftHandTransform;
        public Transform _LeftPalmTransform;
        public Transform _LeftBackOfHandTransform;
        public Transform _LeftCirclePointTransform;

        [Space(10)]
        [Header("Right Hand")]
        public Transform _RightHandTransform;
        public Transform _RightPalmTransform;
        public Transform _RightBackOfHandTransform;
        public Transform _RightCirclePointTransform;

        public void NewParametersRight(SpellSwapCallbackContext ctx)
        {
            if (ctx.spawnedScript != null)
            {
                var spell = ctx.spawnedScript;

                spell._PalmTransform = _RightPalmTransform;
                spell._BackOfHandTransform = _RightBackOfHandTransform;
                spell._HandTransform = _RightHandTransform;
                spell._CircleHolderTransform = _RightCirclePointTransform;

                if (_RightCirclePointTransform.transform.childCount != 0)
                    spell._SpellCircle = _RightCirclePointTransform.transform.GetChild(_RightCirclePointTransform.transform.childCount - 1).gameObject;

                spell._PlayerRb = _PlayerRb;
                spell._MotionController = _PlayerMotion;
            }
        }
        public void NewParametersLeft(SpellSwapCallbackContext ctx)
        {
            if (ctx.spawnedScript != null)
            {
                //Get a temp reference to the current spell
                var spell = ctx.spawnedScript;

                spell._PalmTransform = _LeftPalmTransform;
                spell._BackOfHandTransform = _LeftBackOfHandTransform;
                spell._HandTransform = _LeftHandTransform;
                spell._CircleHolderTransform = _LeftCirclePointTransform;


                if (_LeftCirclePointTransform.transform.childCount != 0)
                    spell._SpellCircle = _LeftCirclePointTransform.transform.GetChild(_LeftCirclePointTransform.transform.childCount - 1).gameObject;

                spell._PlayerRb = _PlayerRb;
                spell._MotionController = _PlayerMotion;
            }
        }
    } 
}
