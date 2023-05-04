using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpellHotbarManager : MonoBehaviour
{
    private SpellManager _spellManager;

    private CoreSpellComponents[] rightHandHotbar = new CoreSpellComponents[4];
    public GameObject[] rightMenuSections = new GameObject[4];
    private GameObject[] spawnedRightCircles = new GameObject[4];

    private CoreSpellComponents[] leftHandHotbar = new CoreSpellComponents[4];
    public GameObject[] leftMenuSections = new GameObject[4];
    private GameObject[] spawnedLeftCircles = new GameObject[4];

    public UnityEvent<CoreSpellComponents, LeftRight> spellSwap;

    private void Start()
    {
        _spellManager = GetComponent<SpellManager>();
    }

    public void ChangeRightSpell(int index)
    {
        if (rightHandHotbar[index] != null)
        {
            spellSwap.Invoke(rightHandHotbar[index], LeftRight.Right);
            //_spellManager.ChangeRightSpell(rightHandHotbar[index]);
        }
    }

    public void ChangeRightHotbar(int index, CoreSpellComponents spellToBe)
    {
        
        if (spawnedRightCircles[index])
        {
            Destroy(spawnedRightCircles[index]);
        }
        rightHandHotbar[index] = spellToBe;
        if(spellToBe != null)
            spawnedRightCircles[index] = Instantiate(spellToBe.spellCircle, rightMenuSections[index].transform);
    }

    public void ChangeLeftSpell(int index)
    {
        if (leftHandHotbar[index] != null)
        {
            spellSwap.Invoke(leftHandHotbar[index], LeftRight.Left);
            _spellManager.ChangeLeftSpell(leftHandHotbar[index]);
        }
    }

    public void ChangeLeftHotbar(int index, CoreSpellComponents spellToBe)
    {
        leftHandHotbar[index] = spellToBe;
        if (spawnedLeftCircles[index])
        {
            Destroy(spawnedLeftCircles[index]);
        }
        spawnedLeftCircles[index] = Instantiate(spellToBe.spellCircle, leftMenuSections[index].transform);
    }

    public void ClearLeftHotbar()
    {
        foreach (GameObject circle in spawnedLeftCircles)
        {
            if(circle)
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
