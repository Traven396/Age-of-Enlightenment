using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSpell : SpellBlueprint
{
    private GameObject shieldMesh;
    private GameObject spellCircle;
    private void Start()
    {
        shieldMesh = circleHolder.GetComponentInChildren<MeshCollider>().gameObject;
        spellCircle = circleHolder.transform.GetChild(0).gameObject;
    }

    public override void GripPress()
    {
        base.GripPress();
        shieldMesh.SetActive(true);
        iTween.ScaleTo(spellCircle, Vector3.one * 4, 1);
    }
    public override void GripRelease()
    {
        base.GripRelease();
        shieldMesh.SetActive(false);
        iTween.ScaleTo(spellCircle, Vector3.one, 1);
    }
}
