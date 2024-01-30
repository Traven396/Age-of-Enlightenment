using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR;

[RequireComponent(typeof(SpellVisualsManager))]

public class SpellManager : MonoBehaviour
{
    public SpellHotbarManager _hotbarManager { get; private set; }

    //All left hand stuff
    [SerializeField] private GameObject leftSpellGO;
    private GameObject _spawnedLeftSpell;
    private SpellBlueprint _spawnedLeftBlueprint;

    //All right hand stuff
    [SerializeField] private GameObject rightSpellGO;
    private GameObject _spawnedRightSpell;
    private SpellBlueprint _spawnedRightBlueprint;

    public UnityEvent<SpellSwapCallbackContext> RightSpellSwap, LeftSpellSwap;

    private void Start()
    {
        _hotbarManager = GetComponent<SpellHotbarManager>();
    }
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

        rightSpellGO = spellToBe.spellMechanics;

        SpawnSpell(LeftRight.Right);

        RightSpellSwap.Invoke(new SpellSwapCallbackContext(_spawnedRightSpell, _spawnedRightBlueprint, spellToBe.RightAnimationController, spellToBe.spellCircle));

        _spawnedRightBlueprint.OnSelect();
    }

    public void ChangeLeftSpell(CoreSpellComponents spellToBe)
    {
        DespawnSpell(LeftRight.Left);

        leftSpellGO = spellToBe.spellMechanics;

        SpawnSpell(LeftRight.Left);

        LeftSpellSwap.Invoke(new SpellSwapCallbackContext(_spawnedLeftSpell, _spawnedLeftBlueprint, spellToBe.LeftAnimationController, spellToBe.spellCircle));

        _spawnedLeftBlueprint.OnSelect();
    }

    private void SpawnSpell(LeftRight side)
    {
        if(side == 0)
        {
            if (leftSpellGO)
            {
                _spawnedLeftSpell = Instantiate(leftSpellGO, transform);
                _spawnedLeftBlueprint = _spawnedLeftSpell.GetComponent<SpellBlueprint>();
            }
        }
        else
        {
            if (rightSpellGO)
            {
                _spawnedRightSpell = Instantiate(rightSpellGO, transform);
                _spawnedRightBlueprint = _spawnedRightSpell.GetComponent<SpellBlueprint>();
            }
        }
    }

    private void DespawnSpell(LeftRight side)
    {
        if(side == 0)
        {
            if (_spawnedLeftSpell)
            {
                _spawnedLeftBlueprint.OnDeselect();

                Destroy(_spawnedLeftSpell);

                _spawnedLeftBlueprint = null;

                LeftSpellSwap.Invoke(new SpellSwapCallbackContext());

            }
        }
        else
        {
            if (_spawnedRightSpell)
            {
                _spawnedRightBlueprint.OnDeselect();

                Destroy(_spawnedRightSpell);

                _spawnedRightBlueprint = null;

                RightSpellSwap.Invoke(new SpellSwapCallbackContext());
            }
        }
    }

    public void ClearRightSpell()
    {
        DespawnSpell(LeftRight.Right);
        rightSpellGO = null;
        _spawnedRightBlueprint = null;
    }
    public void ClearLeftSpell()
    {
        DespawnSpell(LeftRight.Left);
        leftSpellGO = null;
        _spawnedLeftBlueprint = null;
    }

}
public enum LeftRight
{
    Left,
    Right
}
public class SpellSwapCallbackContext{
    public GameObject spawnedSelf;
    public SpellBlueprint spawnedScript;
    public AnimatorOverrideController newAnimator;
    public GameObject circle;

    public SpellSwapCallbackContext(GameObject newSelf, SpellBlueprint newScript, AnimatorOverrideController newerAnimator, GameObject theCircle)
    {
        spawnedSelf = newSelf;
        spawnedScript = newScript;
        newAnimator = newerAnimator;
        circle = theCircle;
    }
    public SpellSwapCallbackContext()
    {
        spawnedScript = null;
        spawnedSelf = null;
        newAnimator = null;
        circle = null;
    }
}
