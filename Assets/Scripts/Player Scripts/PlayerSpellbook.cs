using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerSpellbook : MonoBehaviour
{
    public UnityEvent newSpellLearned;
    private List<FullSpellObject> knownSpells = new List<FullSpellObject>();

    public List<FullSpellObject> knownFireSpells, knownEarthSpells, knownWindSpells, knownWaterSpells, knownIceSpells, knownLightningSpells, knownMetalSpells, knownPlantSpells, knownAstralSpells;

    public void AttemptSpellLearn(FullSpellObject inputKnowledge)
    {
        if (!knownSpells.Contains(inputKnowledge))
        {
            knownSpells.Add(inputKnowledge);
            newSpellLearned.Invoke();
        }
    }

    #region Getters
    public List<FullSpellObject> GetSpellList()
    {
        return knownSpells;
    }
    public List<FullSpellObject> GetFireSpellList()
    {
        return knownFireSpells;
    }
    public List<FullSpellObject> GetEarthSpellList()
    {
        return knownEarthSpells;
    }
    public List<FullSpellObject> GetWindSpellList()
    {
        return knownWindSpells;
    }
    public List<FullSpellObject> GetWaterSpellList()
    {
        return knownWaterSpells;
    }
    public List<FullSpellObject> GetIceSpellList()
    {
        return knownIceSpells;
    }
    public List<FullSpellObject> GetLightningSpellList()
    {
        return knownLightningSpells;
    }
    public List<FullSpellObject> GetPlantSpellList()
    {
        return knownPlantSpells;
    }
    public List<FullSpellObject> GetMetalSpellList()
    {
        return knownMetalSpells;
    }
    public List<FullSpellObject> GetAstralSpellList()
    {
        return knownAstralSpells;
    } 
    #endregion
}
