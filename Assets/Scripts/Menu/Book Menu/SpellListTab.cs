using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellListTab : MonoBehaviour, ITab
{
    public SpellHotbarManager _hotbar;
    public BookMenuManager Overhead { get; set; }

    [Header("UI Components")]
    [SerializeField] private Image _spellIcon;
    [SerializeField] private TMP_Text _spellName;
    [SerializeField] private TMP_Text _spellDescription;
    [SerializeField] private List<GameObject> spellCateogryTabs;

    public List<MenuGrouping> potentialSpells;

    private List<FullSpellObject> currentSelectedSchool;
    
    private int arrayLocation = 0;
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
                    currentSelectedSchool = Player.Instance._SpellBook.GetFireSpellList();
                    break;
                }
            case SpellSchool.Geomancy:
                {
                    currentSelectedSchool = Player.Instance._SpellBook.GetEarthSpellList();
                    break;
                }
            case SpellSchool.Tempestia:
                {
                    currentSelectedSchool = Player.Instance._SpellBook.GetWindSpellList();
                    break;
                }
            case SpellSchool.Hydromorphy:
                {
                    currentSelectedSchool = Player.Instance._SpellBook.GetWaterSpellList();
                    break;
                }
            case SpellSchool.Frostweaving:
                {
                    currentSelectedSchool = Player.Instance._SpellBook.GetIceSpellList();
                    break;
                }
            case SpellSchool.Voltcraft:
                {
                    currentSelectedSchool = Player.Instance._SpellBook.GetLightningSpellList();
                    break;
                }
            case SpellSchool.Druidic:
                {
                    currentSelectedSchool = Player.Instance._SpellBook.GetPlantSpellList();
                    break;
                }
            case SpellSchool.Metallurgy:
                {
                    currentSelectedSchool = Player.Instance._SpellBook.GetMetalSpellList();
                    break;
                }
            case SpellSchool.Astral:
                {
                    currentSelectedSchool = Player.Instance._SpellBook.GetAstralSpellList();
                    break;
                }
        }

        spellCateogryTabs.ForEach(t => t.transform.localPosition = new Vector3(0, 10, 0.2f));
        spellCateogryTabs[whichSchool].transform.localPosition = new Vector3(0, 0, 0.2f);

        LoadFullList();
    }

    private void LoadFullList()
    {
        potentialSpells.ForEach(m => m.actualItem.SetActive(false));

        for (int i = 0; i < currentSelectedSchool.Count; i++)
        {
            potentialSpells[i].actualItem.SetActive(true);
            potentialSpells[i].icon.sprite = currentSelectedSchool[i + currentPage].spellDisplayIcon;
            potentialSpells[i].title.text = currentSelectedSchool[i + currentPage].spellName;
        }
    }
    public void NewLoad()
    {
        currentSelectedSchool = Player.Instance._SpellBook.GetFireSpellList();
        spellCateogryTabs.ForEach(t => t.transform.localPosition = new Vector3(0, 10, 0.2f));
        spellCateogryTabs[0].transform.localPosition = new Vector3(0, 0, 0.2f);

        LoadFullList();
    }

    public void SpecificMenuPageChange(bool left)
    {
        if (left)
        {
            if (arrayLocation == 0)
            {
                arrayLocation = currentSelectedSchool.Count - 1;
                UpdatePage();
            }
            else
            {
                arrayLocation--;
                UpdatePage();
            }
        }
        else
        {
            if (arrayLocation == currentSelectedSchool.Count - 1)
            {
                arrayLocation = 0;
                UpdatePage();
            }
            else
            {
                arrayLocation++;
                UpdatePage();
            }
        }
    }
    private void UpdatePage()
    {
        if (currentSelectedSchool.Count != 0)
        {
            FullSpellObject currentSpell = currentSelectedSchool[arrayLocation];

            _spellName.text = currentSpell.spellName;
            _spellIcon.sprite = currentSpell.spellDisplayIcon;
            _spellDescription.text = currentSpell.spellDescription;
        }
    }
    public void SpellCardButton(int num)
    {
        arrayLocation = num + currentPage;

        UpdatePage();
    }

    public void ConfirmSelection()
    {
        if (hotbarHand == 0)
        {
            _hotbar.ChangeLeftHotbar(hotbarIndex, currentSelectedSchool[arrayLocation].spellCouple);
        }
        else
        {
            _hotbar.ChangeRightHotbar(hotbarIndex, currentSelectedSchool[arrayLocation].spellCouple);
        }
    }
    #endregion

    #region Adding New Spells
    public void UpdateList(SpellSchool newAndImprovedSchool)
    {
        //spellList.AddRange(Player.Instance._SpellBook.GetSpellList().Except(spellList, new SpellEqualityComparer()));
        //switch(newAndImprovedSchool)
        //{
        //    case SpellSchool.Pyrokinesis:
        //    {
        //      fireSpellList.AddRange(Player.Instance._SpellBook.GetFireSpellList().Except(fireSpellList, new SpellEqualityComparer()));
        //      break;
        //    }
        //    case SpellSchool.Geomancy:
        //    {
        //      earthSpellList.AddRange(Player.Instance._SpellBook.GetEarthSpellList().Except(fireSpellList, new SpellEqualityComparer()));
        //      break;
        //    }
        //    case SpellSchool.Tempestia:
        //    {
        //      windSpellList.AddRange(Player.Instance._SpellBook.GetWindSpellList().Except(fireSpellList, new SpellEqualityComparer()));
        //      break;
        //    }
        //    case SpellSchool.Hydromorphy:
        //    {
        //      waterSpellList.AddRange(Player.Instance._SpellBook.GetIceSpellList().Except(fireSpellList, new SpellEqualityComparer()));
        //      break;
        //    }
        //    default:
        //    {
        //      //This should never be reached. Its an enum and I am covering every case.
        //      //but everywhere I read said it HAD to have a default so here we go
        //      Debug.Log("You have done the impossible");
        //      break;
        //    }
        //}
    }

    private class SpellEqualityComparer : IEqualityComparer<FullSpellObject>
    {
        public bool Equals(FullSpellObject s1, FullSpellObject s2)
        {
            if (s2 == null && s1 == null)
                return true;
            else if (s1 == null || s2 == null)
                return false;
            else if (s1.spellName == s2.spellName && s1.spellCouple.spellMechanics == s2.spellCouple.spellMechanics)
                return true;
            else
                return false;
        }

        public int GetHashCode(FullSpellObject obj)
        {
            return obj.spellName.GetHashCode();
        }
    }

    [Serializable]
    public class MenuGrouping
    {
        public GameObject actualItem;
        public Image icon;
        public TMP_Text title;
    }
    #endregion

}
