using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerSpellbook : MonoBehaviour
{
    public UnityEvent newSpellLearned;
    private List<FullSpellObject> knownSpells = new List<FullSpellObject>();

    public void AttemptSpellLearn(FullSpellObject inputKnowledge)
    {
        if (!knownSpells.Contains(inputKnowledge))
        {
            knownSpells.Add(inputKnowledge);
            newSpellLearned.Invoke();
        }
    }

    public List<FullSpellObject> GetSpellList()
    {
        return knownSpells;
    }
}
