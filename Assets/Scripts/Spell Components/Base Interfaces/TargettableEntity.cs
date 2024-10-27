using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class TargettableEntity : MonoBehaviour
{
    public bool TelekinesisTargettable = false, PyroTargettable = false, MetalTargettable = false,
        AquaTargettable = false, GeneralTargettable = false, EarthTargettable = false;
    private Rigidbody targetRB;
    private Outline selfOutline;
    public bool isSelected = false;
    void Start()
    {
        targetRB = GetComponent<Rigidbody>();
        selfOutline = GetComponent<Outline>();

        selfOutline.OutlineWidth = 0;
    }
    public Rigidbody GetTargetRB()
    {
        return targetRB;
    }

    public void Select()
    {
        selfOutline.OutlineWidth = 5;
        isSelected = true;
    }
    public void Deselect()
    {
        selfOutline.OutlineWidth = 0;
        isSelected = false;
    }
}
