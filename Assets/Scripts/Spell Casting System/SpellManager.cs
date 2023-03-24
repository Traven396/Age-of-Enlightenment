using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

[RequireComponent(typeof(TargetManager))]
[RequireComponent(typeof(SpellParameterSupplier))]
[RequireComponent(typeof(SpellInputManager))]
[RequireComponent(typeof(SpellVisualsManager))]
public class SpellManager : MonoBehaviour
{
    private TargetManager _targetManager;
    private SpellParameterSupplier _parameterSupplier;
    private SpellInputManager _inputManager;
    private SpellVisualsManager _visualsManager;

    //All left hand stuff
    [SerializeField] private GameObject leftSpell;
    private GameObject _spawnedLeftSpell;
    private SpellBlueprint _spawnedLeftBlueprint;

    //All right hand stuff
    [SerializeField] private GameObject rightSpell;
    private GameObject _spawnedRightSpell;
    private SpellBlueprint _spawnedRightBlueprint;


    private void Start()
    {
        _targetManager = GetComponent<TargetManager>();
        _visualsManager = GetComponent<SpellVisualsManager>();
        _parameterSupplier = GetComponent<SpellParameterSupplier>();
        _inputManager = GetComponent<SpellInputManager>();
        

        _parameterSupplier.SetupTargetManger(_targetManager);
        //_visualsManager.SetupAnimationControllers(_parameterSupplier);



        SpawnSpell(LeftRight.Left);
        SpawnSpell(LeftRight.Right);
    }

    public void ChangeRightSpell(SpellGameObjectCouple spellToBe)
    {
        DespawnSpell(LeftRight.Right);
        rightSpell = spellToBe.spellMechanics;

        _visualsManager.ChangeRightCircle(spellToBe.spellCircle);
        _visualsManager.ChangeAnimatorController(LeftRight.Right, spellToBe.RightAnimationController);

        SpawnSpell(LeftRight.Right);
        
    }

    public void ChangeLeftSpell(SpellGameObjectCouple spellToBe)
    {
        DespawnSpell(LeftRight.Left);
        leftSpell = spellToBe.spellMechanics;

        _visualsManager.ChangeLeftCircle(spellToBe.spellCircle);
        _visualsManager.ChangeAnimatorController(LeftRight.Left, spellToBe.LeftAnimationController);

        SpawnSpell(LeftRight.Left);
        
    }

    private void SpawnSpell(LeftRight side)
    {
        if(side == 0)
        {
            if (leftSpell)
            {
                _spawnedLeftSpell = Instantiate(leftSpell, transform);
                _spawnedLeftBlueprint = _spawnedLeftSpell.GetComponent<SpellBlueprint>();
                _spawnedLeftBlueprint._targetManager = _targetManager;
                _spawnedLeftBlueprint._visualsManager = _visualsManager;

                _parameterSupplier.SetParametersLeft(_spawnedLeftBlueprint);
                _inputManager.SetLeftSpell(_spawnedLeftBlueprint);
            }
        }
        else
        {
            if (rightSpell)
            {
                _spawnedRightSpell = Instantiate(rightSpell, transform);
                _spawnedRightBlueprint = _spawnedRightSpell.GetComponent<SpellBlueprint>();
                _spawnedRightBlueprint._targetManager = _targetManager;
                _spawnedRightBlueprint._visualsManager = _visualsManager;

                _parameterSupplier.SetParametersRight(_spawnedRightBlueprint);
                _inputManager.SetRightSpell(_spawnedRightBlueprint);
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
            }
        }
        else
        {
            if (_spawnedRightSpell)
            {
                _spawnedRightBlueprint.OnDeselect();

                Destroy(_spawnedRightSpell);

                _spawnedRightBlueprint = null;
            }
        }
        _visualsManager.DespawnCircle(side);
    }

    public void ClearSpells()
    {
        DespawnSpell(LeftRight.Left);
        DespawnSpell(LeftRight.Right);
        leftSpell = null;
        rightSpell = null;
        _spawnedLeftBlueprint = null;
        _spawnedRightBlueprint = null;
    }
    
}
