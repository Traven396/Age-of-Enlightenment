using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepOfTheWind : SpellBlueprint
{
    [Header("Mana Cost")]
    public int maxManaCost = 20;
    public float increaseAmount = 4;

    private bool wasActuallyPressed = false;


    private void Start()
    {
        
        spellCircle = circleHolder.transform.GetChild(circleHolder.transform.childCount - 1).gameObject;
        if (Player.Instance.stepOfWindOn)
        {
            iTween.ScaleTo(spellCircle, Vector3.one * 1.5f, .2f);
        }
    }
    public override void TriggerPress()
    {
        base.TriggerPress();
        wasActuallyPressed = true;
    }
    public override void TriggerRelease()
    {
        base.TriggerRelease();
        if (wasActuallyPressed)
        {
            if (Player.Instance.stepOfWindOn)
            {
                Player.Instance.SubtractPlayerMoveSpeed(increaseAmount);
                Player.Instance.AddMaximumMana(maxManaCost);
                Player.Instance.stepOfWindOn = false;
                iTween.ScaleTo(spellCircle, Vector3.one, .2f);
            }
            else
            {
                Player.Instance.AddPlayerMoveSpeed(increaseAmount);
                Player.Instance.SubtractMaximumMana(maxManaCost);
                Player.Instance.stepOfWindOn = true;
                iTween.ScaleTo(spellCircle, Vector3.one * 1.5f, .2f);
            }
            wasActuallyPressed = false;
        }
    }
}
