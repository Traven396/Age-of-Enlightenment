namespace AgeOfEnlightenment.Player
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using System.Linq;

    public class InternalPlayerSpellbook : MonoBehaviour
    {
        public UnityEvent NewSpellLearned;

        public List<SpellbookEntry> KnownSpells = new();

        public void AttemptSpellLearn(SpellbookEntry inputKnowledge)
        {
            if (!KnownSpells.Contains(inputKnowledge))
            {
                KnownSpells.Add(inputKnowledge);

                NewSpellLearned.Invoke();
            }
        }

        public List<SpellbookEntry> GetFullSpellList()
        {
            return KnownSpells;
        }

        public List<SpellbookEntry> GetFireSpellList()
        {
            var FilteredList = KnownSpells.Where(so => so.School == SpellSchool.Pyrokinesis).ToList();

            return FilteredList;
        }

        public List<SpellbookEntry> GetEarthSpellList()
        {
            var FilteredList = KnownSpells.Where(so => so.School == SpellSchool.Geomancy).ToList();

            return FilteredList;
        }

        public List<SpellbookEntry> GetWaterSpellList()
        {
            var FilteredList = KnownSpells.Where(so => so.School == SpellSchool.Hydromorphy).ToList();

            return FilteredList;
        }

        public List<SpellbookEntry> GetAirSpellList()
        {
            var FilteredList = KnownSpells.Where(so => so.School == SpellSchool.Tempestia).ToList();

            return FilteredList;
        }

        public List<SpellbookEntry> GetAstralSpellList()
        {
            var FilteredList = KnownSpells.Where(so => so.School == SpellSchool.Astral).ToList();

            return FilteredList;
        }
        public List<SpellbookEntry> GetMetalSpellList()
        {
            var FilteredList = KnownSpells.Where(so => so.School == SpellSchool.Metallurgy).ToList();

            return FilteredList;
        }
        public List<SpellbookEntry> GetNatureSpellList()
        {
            var FilteredList = KnownSpells.Where(so => so.School == SpellSchool.Druidic).ToList();

            return FilteredList;
        }
        public List<SpellbookEntry> GetIceSpellList()
        {
            var FilteredList = KnownSpells.Where(so => so.School == SpellSchool.Frostweaving).ToList();

            return FilteredList;
        }
        public List<SpellbookEntry> GetLightningSpellList()
        {
            var FilteredList = KnownSpells.Where(so => so.School == SpellSchool.Voltcraft).ToList();

            return FilteredList;
        }
    }

}