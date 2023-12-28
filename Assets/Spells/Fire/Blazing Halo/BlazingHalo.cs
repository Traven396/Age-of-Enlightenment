using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kryz.CharacterStats;
public class BlazingHalo : SpellBlueprint
{
    const string PASSIVE_NAME = "BlazingHalo";

    public GameObject _haloPrefab;
    public float _manaCost;

    private bool validTriggerPress = false;
    private StatModifier ManaModifier;
    private void Start()
    {
        ManaModifier = new StatModifier(-_manaCost, StatModType.Flat);

        spellCircle = circleHolder.transform.GetChild(circleHolder.transform.childCount - 1).gameObject;

        if (Player.Instance._PassiveManager.QueryPhysicalSpell(PASSIVE_NAME))
        {
            iTween.ScaleTo(spellCircle, Vector3.one * 1.5f, .1f);
        }
    }
    public override void TriggerPress()
    {
        base.TriggerPress();
        validTriggerPress = true;
    }

    public override void TriggerRelease()
    {
        base.TriggerRelease();
        if (validTriggerPress)
        {
            if(Player.Instance._PassiveManager.TogglePassivePhysicalSpell(PASSIVE_NAME, _haloPrefab))
            {
                Player.Instance.AddMaximumManaModifier(ManaModifier);
                iTween.ScaleTo(spellCircle, Vector3.one * 1.5f, .2f);
            }
            else
            {
                Player.Instance.RemoveMaxiumumManaModifier(ManaModifier);
                iTween.ScaleTo(spellCircle, Vector3.one, .2f);
            }

        }
        validTriggerPress = false;
    }
}
