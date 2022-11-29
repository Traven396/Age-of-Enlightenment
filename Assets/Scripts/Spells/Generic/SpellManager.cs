using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

[RequireComponent(typeof(TargetManager))]
[RequireComponent(typeof(SpellParameterSupplier))]
[RequireComponent(typeof(SpellInputManager))]
public class SpellManager : MonoBehaviour
{
    private TargetManager _targetManager;
    private SpellParameterSupplier _parameterSupplier;
    private SpellInputManager _inputManager;

    //All left hand stuff
    [SerializeField] private GameObject leftSpell;
    private GameObject _spawnedLeftSpell;
    private SpellBlueprint _spawnedLeftBlueprint;

    //All right hand stuff
    [SerializeField] private GameObject rightSpell;
    private GameObject _spawnedRightSpell;
    private SpellBlueprint _spawnedRightBlueprint;


    private UnityEngine.XR.InputDevice _rightController;
    private void Start()
    {
        _targetManager = GetComponent<TargetManager>();
        _parameterSupplier = GetComponent<SpellParameterSupplier>();
        _inputManager = GetComponent<SpellInputManager>();

        _parameterSupplier.SetupTargetManger(_targetManager);

        SpawnSpell(LeftRight.Left);
        SpawnSpell(LeftRight.Right);
    }

    private void Update()
    {

        
        
    }
    public void ChangeRightSpell(GameObject spellToBe)
    {
        DespawnSpell(LeftRight.Right);
        rightSpell = spellToBe;
        SpawnSpell(LeftRight.Right);
    }

    public void ChangeLeftSpell(GameObject spellToBe)
    {
        DespawnSpell(LeftRight.Left);
        leftSpell = spellToBe;
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
                _parameterSupplier.SetParametersLeft(_spawnedLeftBlueprint);
                _inputManager.SetControlsLeft(_spawnedLeftBlueprint);
            }
        }
        else
        {
            if (rightSpell)
            {
                _spawnedRightSpell = Instantiate(rightSpell, transform);
                _spawnedRightBlueprint = _spawnedRightSpell.GetComponent<SpellBlueprint>();
                _spawnedRightBlueprint._targetManager = _targetManager;

                _parameterSupplier.SetParametersRight(_spawnedRightBlueprint);
                _inputManager.SetControlsRight(_spawnedRightBlueprint);
            }
        }
    }

    private void DespawnSpell(LeftRight side)
    {
        if(side == 0)
        {
            if (_spawnedLeftSpell)
            {
                Destroy(_spawnedLeftSpell);
                _spawnedLeftBlueprint = null;
            }
        }
        else
        {
            if (_spawnedRightSpell)
            {
                Destroy(_spawnedRightSpell);
                _spawnedRightBlueprint = null;
            }
        }
    }


    
}
