using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AgeOfEnlightenment.Spellbook
{
    [CreateAssetMenu(menuName = "Spellbook/Spellbook Entry")]
    public class SpellbookEntry : ScriptableObject
    {
        public string Name;
        public Sprite SpellDisplayIcon;
        public CoreSpellComponents SpellComponents;
        [TextArea(5, 10)]
        public string SpellDescription;
        public SpellSchool School;
    }
    [Serializable]
    public class CoreSpellComponents
    {
        public GameObject spellMechanics;
        public GameObject spellCircle;

        public AnimatorOverrideController RightAnimationController;
        public AnimatorOverrideController LeftAnimationController;

        public CoreSpellComponents(GameObject newMechanics, GameObject newCircle, AnimatorOverrideController newRight, AnimatorOverrideController newLeft)
        {
            spellCircle = newCircle;
            spellMechanics = newMechanics;
            RightAnimationController = newRight;
            LeftAnimationController = newLeft;
        }
    } 
}