using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kryz.CharacterStats;

public class ShieldSpell : SpellBlueprint
{
    [Header("Mana Costs")]
    public int initialManaCost = 4;
    public int manaDrainCost;
    private StatModifier manaDrainMod;
    private GameObject shieldMesh;

    private void Start()
    {
        shieldMesh = _CircleHolderTransform.GetComponentInChildren<MeshCollider>().gameObject;
        _SpellCircle = _CircleHolderTransform.transform.GetChild(_CircleHolderTransform.transform.childCount - 1).gameObject;

        manaDrainMod = new StatModifier(-manaDrainCost, StatModType.Flat, this);
    }

    public override void GripPress()
    {
        base.GripPress();
        if (Player.Instance.currentMana >= initialManaCost)
        {
            shieldMesh.SetActive(true);
            iTween.ScaleTo(_SpellCircle, Vector3.one * 4, 1);

            Player.Instance.AddManaRegenModifier(manaDrainMod);
        }
    }
    public override void GripHold()
    {
        if (shieldMesh.activeInHierarchy)
        {
            if (Player.Instance.currentMana < manaDrainMod.Value)
            {
                DeactivateShield();
            }
        }
    }
    public override void GripRelease()
    {
        base.GripRelease();
        DeactivateShield();
    }

    void DeactivateShield()
    {
        if (shieldMesh.activeInHierarchy)
        {
            shieldMesh.SetActive(false);
            iTween.ScaleTo(_SpellCircle, Vector3.one, 1);

            Player.Instance.RemoveManaRegenModifier(manaDrainMod); 
        }
    }
    public override void OnDeselect()
    {
        base.OnDeselect();
        DeactivateShield();
    }
}
