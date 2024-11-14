using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AgeOfEnlightenment.Player
{
    [CreateAssetMenu(menuName = "MyAssets/Spellbook Entry")]
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
        public GameObject SpellMechanics;
        public GameObject SpellCircle;

        public CoreSpellComponents(GameObject newMechanics, GameObject newCircle)
        {
            SpellCircle = newCircle;
            SpellMechanics = newMechanics;
        }
    } 
}