using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR;

namespace AgeOfEnlightenment.Player
{

    public class SpellManager : MonoBehaviour
    {
        //All left hand stuff
        [SerializeField] private GameObject _LeftSpellGO;
        private GameObject SpawnedLeftSpell;
        [HideInInspector] public SpellBlueprint SpawnedLeftBlueprint { get; private set; }

        //All right hand stuff
        [SerializeField] private GameObject _RightSpellGO;
        private GameObject SpawnedRightSpell;
        [HideInInspector] public SpellBlueprint SpawnedRightBlueprint { get; private set; }

        public UnityEvent<SpellSwapCallbackContext> RightSpellSwap, LeftSpellSwap;

        public void NewSpellSwap(CoreSpellComponents spellToBe, LeftRight whichHand)
        {
            if (whichHand == 0)
                ChangeLeftSpell(spellToBe);
            else
                ChangeRightSpell(spellToBe);
        }
        public void ChangeRightSpell(CoreSpellComponents spellToBe)
        {
            DespawnSpell(LeftRight.Right);

            _RightSpellGO = spellToBe.SpellMechanics;

            SpawnSpell(LeftRight.Right);

            SpawnedRightBlueprint.currentHand = LeftRight.Right;

            RightSpellSwap.Invoke(new SpellSwapCallbackContext(SpawnedRightSpell, SpawnedRightBlueprint, spellToBe.SpellCircle));

            //Fire the selection event for the spell. This is for setting up initial things in the spell before any other events fire
            SpawnedRightBlueprint.OnSelect();
        }

        public void ChangeLeftSpell(CoreSpellComponents spellToBe)
        {
            DespawnSpell(LeftRight.Left);

            _LeftSpellGO = spellToBe.SpellMechanics;

            SpawnSpell(LeftRight.Left);

            SpawnedLeftBlueprint.currentHand = LeftRight.Left;

            LeftSpellSwap.Invoke(new SpellSwapCallbackContext(SpawnedLeftSpell, SpawnedLeftBlueprint, spellToBe.SpellCircle));

            SpawnedLeftBlueprint.OnSelect();
        }

        private void SpawnSpell(LeftRight side)
        {
            if (side == 0)
            {
                if (_LeftSpellGO)
                {
                    SpawnedLeftSpell = Instantiate(_LeftSpellGO, transform);
                    SpawnedLeftBlueprint = SpawnedLeftSpell.GetComponent<SpellBlueprint>();
                }
            }
            else
            {
                if (_RightSpellGO)
                {
                    SpawnedRightSpell = Instantiate(_RightSpellGO, transform);
                    SpawnedRightBlueprint = SpawnedRightSpell.GetComponent<SpellBlueprint>();
                }
            }
        }

        private void DespawnSpell(LeftRight side)
        {
            if (side == 0)
            {
                if (SpawnedLeftSpell)
                {
                    SpawnedLeftBlueprint.OnDeselect();

                    Destroy(SpawnedLeftSpell);

                    SpawnedLeftBlueprint = null;

                    LeftSpellSwap.Invoke(new SpellSwapCallbackContext());

                }
            }
            else
            {
                if (SpawnedRightSpell)
                {
                    SpawnedRightBlueprint.OnDeselect();

                    Destroy(SpawnedRightSpell);

                    SpawnedRightBlueprint = null;

                    RightSpellSwap.Invoke(new SpellSwapCallbackContext());
                }
            }
        }

        public void ClearRightSpell()
        {
            DespawnSpell(LeftRight.Right);
            _RightSpellGO = null;
            SpawnedRightBlueprint = null;
        }
        public void ClearLeftSpell()
        {
            DespawnSpell(LeftRight.Left);
            _LeftSpellGO = null;
            SpawnedLeftBlueprint = null;
        }

    } 
}

