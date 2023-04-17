using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System;

public class BookMenuManager : MonoBehaviour
{
    public InputActionReference button;
    public GameObject displayedMenu;

    private Animator _animation;
    private bool menuShowing;

    [Space(10f)]
    public SpellManager _spellManager;

    [SerializeField] private Image _spellIcon;
    [SerializeField] private TMP_Text _spellName;
    [SerializeField] private TMP_Text _spellDescription;
    [Space(20)]
    [SerializeField] private GameObject hotbarMenu;

    public List<SpellMenuItem> spellList;

    private int arrayLocation = 0;

    private LeftRight hotbarHand;

    // Start is called before the first frame update
    void Start()
    {
        _animation = GetComponentInChildren<Animator>();
        menuShowing = displayedMenu.activeInHierarchy;

        LoadMenu();
        button.action.started += ctx => ToggleMenu();
    }

    public void ClearSpells(bool left)
    {
        if (left)
            _spellManager._hotbarManager.ClearLeftHotbar();
        else
            _spellManager._hotbarManager.ClearRightHotbar();
    }

    public void ChangeMenu(bool left)
    {
        if (left)
        {
            if(arrayLocation == 0)
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
            if(arrayLocation == spellList.Count - 1)
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

    //public void SpellHandButton(bool left)
    //{
    //    if(left)
    //    {
    //        _spellManager.ChangeLeftSpell(spellList[arrayLocation].spellCouple);
    //    }
    //    else
    //    {
    //        _spellManager._hotbarManager.ChangeRightHotbar(1, spellList[arrayLocation].spellCouple);
    //        //_spellManager.ChangeRightSpell(spellList[arrayLocation].spellCouple);
    //    }
    //}
    private void LoadMenu()
    {
        if (spellList.Count != 0)
        {
            SpellMenuItem currentSpell = spellList[arrayLocation];
            _spellName.text = currentSpell.spellName;
            _spellIcon.sprite = currentSpell.spellIcon;
            _spellDescription.text = currentSpell.spellDescription; 
        }
    }

    private void ToggleMenu()
    {
        if (menuShowing)
        {
            _animation.Play("Book Closing");
            menuShowing = false;
        }
        else
        {
            _animation.Play("Book Opening");
            menuShowing = true;
        }
    }


    public void ActiveHotbarMenu(bool left)
    {
        hotbarMenu.SetActive(true);
        if (left)
            hotbarHand = LeftRight.Left;
        else
            hotbarHand = LeftRight.Right;
    }

    public void HotbarButton(int index)
    {
        if(hotbarHand == 0)
        {
            _spellManager._hotbarManager.ChangeLeftHotbar(index, spellList[arrayLocation].spellCouple);
        }
        else
        {
            _spellManager._hotbarManager.ChangeRightHotbar(index, spellList[arrayLocation].spellCouple);
        }
        hotbarMenu.SetActive(false);
    }
}

[Serializable]
public class SpellMenuItem
{
    public Sprite spellIcon;
    public string spellName;
    public SpellGameObjectCouple spellCouple;
    [TextArea(5, 10)]
    public string spellDescription;
    
}
[Serializable]
public class SpellGameObjectCouple
{
    public GameObject spellMechanics;
    public GameObject spellCircle;
    public AnimatorOverrideController RightAnimationController;
    public AnimatorOverrideController LeftAnimationController;
}
