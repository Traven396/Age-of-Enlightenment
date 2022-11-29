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

    public List<SpellMenuItem> spellList;

    private int arrayLocation = 0;

    // Start is called before the first frame update
    void Start()
    {
        _animation = GetComponentInChildren<Animator>();
        menuShowing = displayedMenu.activeInHierarchy;

        LoadMenu();
        button.action.started += ctx => ToggleMenu();
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

    public void SpellHandButton(bool left)
    {
        if(left)
        {
            _spellManager.ChangeLeftSpell(spellList[arrayLocation].spellGO);
        }
        else
        {
            _spellManager.ChangeRightSpell(spellList[arrayLocation].spellGO);
        }
    }
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
}

[System.Serializable]
public class SpellMenuItem
{
    public Sprite spellIcon;
    public string spellName;
    public GameObject spellGO;
    public string spellDescription;

    public static SpellMenuItem CreateFromJSON(string JsonString)
    {
        return JsonUtility.FromJson<SpellMenuItem>(JsonString);
    }

    public string CreateJSON()
    {
        return JsonUtility.ToJson(this);
    }

}
