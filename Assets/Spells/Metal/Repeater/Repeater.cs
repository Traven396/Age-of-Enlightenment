using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repeater : SpellBlueprint
{
    [SerializeField] private float LaunchForce = 14;
    [SerializeField] private GameObject BladePrefab;


    private List<GameObject> Blades = new List<GameObject>();

    public override void TriggerPress()
    {
        base.TriggerPress();

        Blades.Add(Instantiate(BladePrefab, _handLocation.TransformPoint(Vector3.up * .1f), Quaternion.identity));
        Blades.ForEach(blade => blade.GetComponent<Rigidbody>().AddForce(_handLocation.transform.forward * LaunchForce, ForceMode.VelocityChange));
    }
}
