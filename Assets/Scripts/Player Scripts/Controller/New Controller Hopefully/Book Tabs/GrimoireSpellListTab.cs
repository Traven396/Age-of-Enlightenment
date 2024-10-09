namespace AgeOfEnlightenment.Player
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class GrimoireSpellListTab : MonoBehaviour, GrimoireTab
    {
        public GrimoireManager Overhead { get; set; }

        public InternalPlayerHotbar _HotbarManager;

        [Header("UI Components")]
        [SerializeField] private Image _spellIcon;
        [SerializeField] private TMP_Text _spellName;
        [SerializeField] private TMP_Text _spellDescription;
        [SerializeField] private List<GameObject> spellCategoryTabs;

        public List<MenuGrouping> PotentialSpellLocations;

        private List<SpellbookEntry> currentSelectedSchool;

        private int spellSchoolSpellListArrayLocation = 0;
        private int currentPage = 0;

        [HideInInspector]
        public int hotbarIndex = 0;
        [HideInInspector]
        public LeftRight hotbarHand = LeftRight.Left;

        #region Specific Spell List

        public void ChangeSpellTab(int whichSchool)
        {
            var school = (SpellSchool)whichSchool;

            switch (school)
            {
                case SpellSchool.Pyrokinesis:
                    {
                        currentSelectedSchool = PlayerSingleton.Instance._SpellBook.GetFireSpellList();
                        break;
                    }
                case SpellSchool.Geomancy:
                    {
                        currentSelectedSchool = PlayerSingleton.Instance._SpellBook.GetEarthSpellList();
                        break;
                    }
                case SpellSchool.Tempestia:
                    {
                        currentSelectedSchool = PlayerSingleton.Instance._SpellBook.GetAirSpellList();
                        break;
                    }
                case SpellSchool.Hydromorphy:
                    {
                        currentSelectedSchool = PlayerSingleton.Instance._SpellBook.GetWaterSpellList();
                        break;
                    }
                case SpellSchool.Frostweaving:
                    {
                        currentSelectedSchool = PlayerSingleton.Instance._SpellBook.GetIceSpellList();
                        break;
                    }
                case SpellSchool.Voltcraft:
                    {
                        currentSelectedSchool = PlayerSingleton.Instance._SpellBook.GetLightningSpellList();
                        break;
                    }
                case SpellSchool.Druidic:
                    {
                        currentSelectedSchool = PlayerSingleton.Instance._SpellBook.GetNatureSpellList();
                        break;
                    }
                case SpellSchool.Metallurgy:
                    {
                        currentSelectedSchool = PlayerSingleton.Instance._SpellBook.GetMetalSpellList();
                        break;
                    }
                case SpellSchool.Astral:
                    {
                        currentSelectedSchool = PlayerSingleton.Instance._SpellBook.GetAstralSpellList();
                        break;
                    }
            }

            spellCategoryTabs.ForEach(t => t.transform.localPosition = new Vector3(0, 10, 0.2f));
            spellCategoryTabs[whichSchool].transform.localPosition = new Vector3(0, 0, 0.2f);

            LoadFullList();
        }

        private void LoadFullList()
        {
            PotentialSpellLocations.ForEach(m => m.GameObjectItem.SetActive(false));

            for (int i = 0; i < currentSelectedSchool.Count; i++)
            {
                PotentialSpellLocations[i].GameObjectItem.SetActive(true);
                PotentialSpellLocations[i].Icon.sprite = currentSelectedSchool[i + currentPage].SpellDisplayIcon;
                PotentialSpellLocations[i].Title.text = currentSelectedSchool[i + currentPage].Name;
            }
        }
        public void NewLoad()
        {
            currentSelectedSchool = PlayerSingleton.Instance._SpellBook.GetFireSpellList();
            spellCategoryTabs.ForEach(t => t.transform.localPosition = new Vector3(0, 10, 0.2f));
            spellCategoryTabs[0].transform.localPosition = new Vector3(0, 0, 0.2f);

            LoadFullList();
        }

        public void SpecificMenuPageChange(bool left)
        {
            if (left)
            {
                if (spellSchoolSpellListArrayLocation == 0)
                {
                    spellSchoolSpellListArrayLocation = currentSelectedSchool.Count - 1;
                }
                else
                {
                    spellSchoolSpellListArrayLocation--;
                }
            }
            else
            {
                if (spellSchoolSpellListArrayLocation == currentSelectedSchool.Count - 1)
                {
                    spellSchoolSpellListArrayLocation = 0;
                }
                else
                {
                    spellSchoolSpellListArrayLocation++;
                }
            }
            UpdatePage();
        }
        private void UpdatePage()
        {
            if (currentSelectedSchool.Count != 0)
            {
                SpellbookEntry currentSpell = currentSelectedSchool[spellSchoolSpellListArrayLocation];

                _spellName.text = currentSpell.Name;
                _spellIcon.sprite = currentSpell.SpellDisplayIcon;
                _spellDescription.text = currentSpell.SpellDescription;
            }
        }
        public void SpellCardButton(int num)
        {
            spellSchoolSpellListArrayLocation = num + currentPage;

            UpdatePage();
        }

        public void ConfirmSelection()
        {
            if (hotbarHand == 0)
            {
                _HotbarManager.ChangeLeftHotbar(hotbarIndex, currentSelectedSchool[spellSchoolSpellListArrayLocation].SpellComponents);
            }
            else
            {
                _HotbarManager.ChangeRightHotbar(hotbarIndex, currentSelectedSchool[spellSchoolSpellListArrayLocation].SpellComponents);
            }
        }
        #endregion

        #region Adding New Spells

        //private class SpellEqualityComparer : IEqualityComparer<FullSpellObject>
        //{
        //    public bool Equals(FullSpellObject s1, FullSpellObject s2)
        //    {
        //        if (s2 == null && s1 == null)
        //            return true;
        //        else if (s1 == null || s2 == null)
        //            return false;
        //        else if (s1.spellName == s2.spellName && s1.spellCouple.spellMechanics == s2.spellCouple.spellMechanics)
        //            return true;
        //        else
        //            return false;
        //    }

        //    public int GetHashCode(FullSpellObject obj)
        //    {
        //        return obj.spellName.GetHashCode();
        //    }
        //}

        [Serializable]
        public class MenuGrouping
        {
            public GameObject GameObjectItem;
            public Image Icon;
            public TMP_Text Title;
        }
        #endregion
    }

}