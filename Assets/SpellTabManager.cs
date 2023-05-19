using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellTabManager : MonoBehaviour, ITab
{
    [Space(10f)]
    public SpellManager _spellManager;

    [Header("UI Components")]
    [SerializeField] private Image _spellIcon;
    [SerializeField] private TMP_Text _spellName;
    [SerializeField] private TMP_Text _spellDescription;
    [Space(20)]
    [SerializeField] private GameObject _equipButton;
    [Space(10f)]
    public List<FullSpellObject> spellList;

    private int arrayLocation = 0;

    private LeftRight hotbarHand;
    private int hotbarIndex;
    private bool waitingForSpellSelection = false;

    public BookMenuManager Overhead { get; set; }

    private void OnEnable()
    {
        LoadMenu();
    }

    #region Page Turning
    public void ChangeMenu(bool left)
    {
        if (left)
        {
            if (arrayLocation == 0)
            {
                arrayLocation = spellList.Count - 1;
                LoadMenu();
            }
            else
            {
                arrayLocation--;
                LoadMenu();
            }
        }
        else
        {
            if (arrayLocation == spellList.Count - 1)
            {
                arrayLocation = 0;
                LoadMenu();
            }
            else
            {
                arrayLocation++;
                LoadMenu();
            }
        }
    }
    private void LoadMenu()
    {
        if (spellList.Count != 0)
        {
            FullSpellObject currentSpell = spellList[arrayLocation];

            _spellName.text = currentSpell.spellName;
            _spellIcon.sprite = currentSpell.spellIcon;
            _spellDescription.text = currentSpell.spellDescription;
        }
    } 
    #endregion
    #region Hotbar Menu Management
    public void ActiveHotbarRight(int index)
    {
        Overhead.ActivateTab(2);

        _equipButton.SetActive(true);

        waitingForSpellSelection = true;

        hotbarHand = LeftRight.Right;

        hotbarIndex = index;
    }
    public void ActiveHotbarLeft(int index)
    {
        Overhead.ActivateTab(2);

        _equipButton.SetActive(true);

        waitingForSpellSelection = true;

        hotbarHand = LeftRight.Left;

        hotbarIndex = index;
    }
    public void ConfirmSelectionButton()
    {
        if (hotbarHand == 0)
        {
            _spellManager._hotbarManager.ChangeLeftHotbar(hotbarIndex, spellList[arrayLocation].spellCouple);
        }
        else
        {
            _spellManager._hotbarManager.ChangeRightHotbar(hotbarIndex, spellList[arrayLocation].spellCouple);
        }
        _equipButton.SetActive(false);
        waitingForSpellSelection = false;
        Overhead.ActivateTab(1);
    } 

    public void ConfirmButtonVanish()
    {
        if (waitingForSpellSelection)
        {
            _equipButton.SetActive(false);
            waitingForSpellSelection = false;
        }
    }
    #endregion

    #region Adding New Spells
    public void UpdateList()
    {
        spellList.AddRange(Player.Instance._SpellBook.GetSpellList().Except(spellList, new SpellEqualityComparer()));
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
    #endregion
}
[Serializable]
public class FullSpellObject
{
    public Sprite spellIcon;
    public string spellName;
    public CoreSpellComponents spellCouple;
    [TextArea(5, 10)]
    public string spellDescription;

}
[Serializable]
public class CoreSpellComponents
{
    public GameObject spellMechanics;
    public GameObject spellCircle;

    public AnimatorOverrideController RightAnimationController;
    public AnimatorOverrideController LeftAnimationController;

    public CoreSpellComponents(GameObject newMechanics, GameObject newCircle, AnimatorOverrideController newRight, AnimatorOverrideController newLeft)
    {
        spellCircle = newCircle;
        spellMechanics = newMechanics;
        RightAnimationController = newRight;
        LeftAnimationController = newLeft;

    }
}