using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kryz.CharacterStats;

public class StepOfTheWind : SpellBlueprint
{
    const string MOVEMENT_MODIFIER_NAME = "StepOfTheWind_Speed";
    const string MANA_MODIFIER_NAME = "StepOfTheWind_Mana";

    [Header("Mana Cost")]
    public int _maxManaCost;
    [Range(0, 1)]
    public float _movementIncreaseAmount;

    private StatModifier MaxManaMod;
    private static StatModifier MovementMod;

    private bool validTriggerPress = false;


    private void Start()
    {
        _SpellCircle = _CircleHolderTransform.transform.GetChild(_CircleHolderTransform.transform.childCount - 1).gameObject;
        if (Player.Instance._PassiveManager.QueryEtherealSpell(MOVEMENT_MODIFIER_NAME))
        {
            iTween.ScaleTo(_SpellCircle, Vector3.one * 1.5f, .2f);
        }

        MaxManaMod = new StatModifier(-_maxManaCost, StatModType.Flat);
        MovementMod = new StatModifier(_movementIncreaseAmount, StatModType.PercentAdd);
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
            if (Player.Instance._PassiveManager.TogglePassiveEtherealSpell(MOVEMENT_MODIFIER_NAME, MovementMod, Player.Instance.movementSpeed))
            {
                //Player.Instance._PassiveManager.AddEtherealSpell(MOVEMENT_MODIFIER_NAME, MovementMod, Player.Instance.movementSpeed);
                Player.Instance.AddMaximumManaModifier(MaxManaMod);

                iTween.ScaleTo(_SpellCircle, Vector3.one * 1.5f, .2f);
            }
            else
            {
                //Player.Instance._PassiveManager.RemoveEtherealSpell(MOVEMENT_MODIFIER_NAME);
                Player.Instance.RemoveMaxiumumManaModifier(MaxManaMod);

                iTween.ScaleTo(_SpellCircle, Vector3.one, .2f);
            }
            validTriggerPress = false;
        }
    }
}
