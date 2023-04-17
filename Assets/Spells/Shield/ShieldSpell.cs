using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSpell : SpellBlueprint
{
    [Header("Mana Costs")]
    public int initialManaCost = 4;
    public int manaDrain = 1;
    private GameObject shieldMesh;

    private void Start()
    {
        shieldMesh = circleHolder.GetComponentInChildren<MeshCollider>().gameObject;
        spellCircle = circleHolder.transform.GetChild(circleHolder.transform.childCount - 1).gameObject;
    }

    public override void GripPress()
    {
        base.GripPress();
        if (Player.Instance.currentMana >= initialManaCost)
        {
            shieldMesh.SetActive(true);
            iTween.ScaleTo(spellCircle, Vector3.one * 4, 1);

            Player.Instance.SubtractManaRegen(manaDrain);
        }
    }
    public override void GripHold()
    {
        if (shieldMesh.activeInHierarchy)
        {
            if (Player.Instance.currentMana < manaDrain)
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
            iTween.ScaleTo(spellCircle, Vector3.one, 1);

            Player.Instance.AddManaRegen(manaDrain); 
        }
    }
    public override void OnDeselect()
    {
        base.OnDeselect();
        DeactivateShield();
    }
}
