using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AgeOfEnlightenment.Player
{
    public class InternalPlayerHotbar : MonoBehaviour
    {
        [Header("Right Hand")]
        public RadialMenu RightHandMenu;
        private CoreSpellComponents[] rightHandHotbar = new CoreSpellComponents[4];
        [Space(10)]
        public GameObject[] rightMenuSections = new GameObject[4];
        private GameObject[] spawnedRightCircles = new GameObject[4];

        [Header("Left Hand")]
        public RadialMenu LeftHandMenu;
        
        private CoreSpellComponents[] leftHandHotbar = new CoreSpellComponents[4];
        [Space(10)]
        public GameObject[] leftMenuSections = new GameObject[4];
        private GameObject[] spawnedLeftCircles = new GameObject[4];

        public UnityEvent<CoreSpellComponents, LeftRight> SpellSwapEvent;

        public void ChangeRightSpell(int index)
        {
            if (rightHandHotbar[index] != null)
            {
                SpellSwapEvent.Invoke(rightHandHotbar[index], LeftRight.Right);
            }
        }

        public void ChangeRightHotbar(int index, CoreSpellComponents spellToBe)
        {
            if (spawnedRightCircles[index])
            {
                Destroy(spawnedRightCircles[index]);
            }
            rightHandHotbar[index] = spellToBe;
            if (spellToBe != null)
                spawnedRightCircles[index] = Instantiate(spellToBe.SpellCircle, rightMenuSections[index].transform);
        }

        public void ChangeLeftSpell(int index)
        {
            if (leftHandHotbar[index] != null)
            {
                SpellSwapEvent.Invoke(leftHandHotbar[index], LeftRight.Left);
            }
        }

        public void ChangeLeftHotbar(int index, CoreSpellComponents spellToBe)
        {

            if (spawnedLeftCircles[index])
            {
                Destroy(spawnedLeftCircles[index]);
            }
            leftHandHotbar[index] = spellToBe;
            if (spellToBe != null)
                spawnedLeftCircles[index] = Instantiate(spellToBe.SpellCircle, leftMenuSections[index].transform);
        }

        public void ClearLeftHotbar()
        {
            foreach (GameObject circle in spawnedLeftCircles)
            {
                if (circle)
                    Destroy(circle);
            }
            leftHandHotbar = new CoreSpellComponents[4];
        }
        public void ClearRightHotbar()
        {
            foreach (GameObject circle in spawnedRightCircles)
            {
                if (circle)
                    Destroy(circle);
            }
            rightHandHotbar = new CoreSpellComponents[4];
        }
    } 
}
