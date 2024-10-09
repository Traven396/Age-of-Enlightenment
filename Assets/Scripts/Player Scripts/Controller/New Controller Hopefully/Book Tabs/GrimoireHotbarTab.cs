using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfEnlightenment.Player
{
    public class GrimoireHotbarTab : MonoBehaviour, GrimoireTab
    {
        public GrimoireManager Overhead { get; set; }

        private GrimoireSpellListTab _SpellTab;

        [Header("UI Components")]
        [SerializeField] private Image ArrowGO;
        [SerializeField] private TMP_Text TextIndicator;
        [SerializeField] private GameObject ReminderMenu;
        [Space(20)]
        [SerializeField] private GameObject EquipButton;

        private bool waitingForSpellSelection = false;

        private void OnEnable()
        {
            _SpellTab = GetComponent<GrimoireSpellListTab>();
        }
        #region Hotbar Menu Management
        public void ActiveHotbarRight(int index)
        {
            Overhead.ActivateTab(2);

            EquipButton.SetActive(true);
            waitingForSpellSelection = true;

            _SpellTab.hotbarHand = LeftRight.Right;
            _SpellTab.hotbarIndex = index;

            ReminderMenu.SetActive(true);
            ArrowGO.gameObject.transform.localRotation = Quaternion.Euler(0, 0, index * 90);
            TextIndicator.text = "R";
        }
        public void ActiveHotbarLeft(int index)
        {
            Overhead.ActivateTab(2);

            EquipButton.SetActive(true);
            waitingForSpellSelection = true;

            _SpellTab.hotbarHand = LeftRight.Left;
            _SpellTab.hotbarIndex = index;

            ReminderMenu.SetActive(true);
            ArrowGO.gameObject.transform.localRotation = Quaternion.Euler(0, 0, index * 90);
            TextIndicator.text = "L";
        }
        public void ConfirmSelectionButton()
        {
            EquipButton.SetActive(false);
            ReminderMenu.SetActive(false);
            waitingForSpellSelection = false;
            Overhead.ActivateTab(1);
        }

        public void ConfirmButtonVanish()
        {
            if (waitingForSpellSelection)
            {
                EquipButton.SetActive(false);
                ReminderMenu.SetActive(false);
                waitingForSpellSelection = false;
            }
        }
        #endregion
    }
}
