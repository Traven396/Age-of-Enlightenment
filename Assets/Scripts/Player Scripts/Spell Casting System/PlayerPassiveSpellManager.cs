using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kryz.CharacterStats;
using System;
using UnityEngine.Events;

public class PlayerPassiveSpellManager : MonoBehaviour
{
    private Dictionary<string, GameObject> physicalPassiveSpells = new Dictionary<string, GameObject>();
    private Dictionary<string, PassiveSpellEffect> etherealPassiveSpells = new Dictionary<string, PassiveSpellEffect>();

    #region Ethereal Spell Stuff
    /// <summary>
    /// Turns on or off a passive stat buff on the player. Firing an event either way. Returns true when a modifier is added, and false when it is removed
    /// </summary>
    /// <param name="source">A string that will tells the code what spell is adding the effect</param>
    /// <param name="mod">The stat modifier to be toggled</param>
    /// <param name="stat">What stat to add the modifier to</param>
    /// <returns></returns>
    public bool TogglePassiveEtherealSpell(string source, StatModifier mod, CharacterStat stat)
    {
        PassiveSpellEffect effect = new PassiveSpellEffect(stat, mod);

        bool added = etherealPassiveSpells.TryAdd(source, effect);
        if (added)
        {
            effect.AffectedStat.AddModifierWithEvent(effect.Modifier);
        }
        else
        {
            etherealPassiveSpells.Remove(source);
            effect.AffectedStat.RemoveModifierWithEvent(effect.Modifier);
        }

        return added;
    }
    public bool QueryEtherealSpell(string source)
    {
        return etherealPassiveSpells.ContainsKey(source);
    }
    public void AddEtherealSpell(string source, StatModifier mod, CharacterStat stat)
    {
        PassiveSpellEffect addition = new PassiveSpellEffect(stat, mod);

        etherealPassiveSpells.Add(source, addition);
        stat.AddModifierWithEvent(mod);
    }
    public void RemoveEtherealSpell(string source)
    {
        PassiveSpellEffect removal = etherealPassiveSpells[source];

        removal.AffectedStat.RemoveModifierWithEvent(removal.Modifier);

        etherealPassiveSpells.Remove(source);
    }
    #endregion
    #region Physical Spell Stuff
    public bool TogglePassivePhysicalSpell(string source, GameObject spellPrefab)
    {
        bool added = true;

        if (!physicalPassiveSpells.ContainsKey(source))
        {
            GameObject spawnedPrefab = Instantiate(spellPrefab, Player.Instance.transform.GetChild(0));
            physicalPassiveSpells.Add(source, spawnedPrefab);
        }
        else
        {
            added = false;
            Destroy(physicalPassiveSpells[source]);
            physicalPassiveSpells.Remove(source);
        }

        return added;
    }
    public bool QueryPhysicalSpell(string source)
    {
        return physicalPassiveSpells.ContainsKey(source);
    }

    #endregion

    private class PassiveSpellEffect
    {
        public CharacterStat AffectedStat;
        public StatModifier Modifier;

        public PassiveSpellEffect(CharacterStat _stat, StatModifier _mod)
        {
            AffectedStat = _stat;
            Modifier = _mod;
        }
    }
}

