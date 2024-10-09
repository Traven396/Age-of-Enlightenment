using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HotbarTabOverseer : MonoBehaviour, ITab
{
    private SpellListTab _spellTab;

    [Header("UI Components")]
    [SerializeField] private Image arrowGO;
    [SerializeField] private TMP_Text textIndicator;
    [SerializeField] private GameObject reminderMenu;
    [Space(20)]
    [SerializeField] private GameObject _equipButton;

    private bool waitingForSpellSelection = false;

    public BookMenuManager Overhead { get; set; }

    private void OnEnable()
    {
        _spellTab = GetComponent<SpellListTab>();
    }
    #region Hotbar Menu Management
    public void ActiveHotbarRight(int index)
    {
        Overhead.ActivateTab(2);

        _equipButton.SetActive(true);
        waitingForSpellSelection = true;

        _spellTab.hotbarHand = LeftRight.Right;
        _spellTab.hotbarIndex = index;

        reminderMenu.SetActive(true);
        arrowGO.gameObject.transform.localRotation = Quaternion.Euler(0, 0, index * 90);
        textIndicator.text = "R";
    }
    public void ActiveHotbarLeft(int index)
    {
        Overhead.ActivateTab(2);
        
        _equipButton.SetActive(true);
        waitingForSpellSelection = true;

        _spellTab.hotbarHand = LeftRight.Left;
        _spellTab.hotbarIndex = index;

        reminderMenu.SetActive(true);
        arrowGO.gameObject.transform.localRotation = Quaternion.Euler(0, 0, index * 90);
        textIndicator.text = "L";
    }
    public void ConfirmSelectionButton()
    {
        _equipButton.SetActive(false);
        reminderMenu.SetActive(false);
        waitingForSpellSelection = false;
        Overhead.ActivateTab(1);
    } 

    public void ConfirmButtonVanish()
    {
        if (waitingForSpellSelection)
        {
            _equipButton.SetActive(false);
            reminderMenu.SetActive(false);
            waitingForSpellSelection = false;
        }
    }
    #endregion
}