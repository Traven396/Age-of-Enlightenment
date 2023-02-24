using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class TargettableEntity : MonoBehaviour
{
    public bool TeleTargettable = false, PyroTargettable = false, MetalTargettable = false,
        AquaTargettable = false, GeneralTargettable = false;
    private Rigidbody targetRB;
    private Outline selfOutline;
    public bool isSelected = false;
    void Start()
    {
        targetRB = GetComponent<Rigidbody>();
        selfOutline = GetComponent<Outline>();
    }
    public Rigidbody GetTargetRB()
    {
        return this.targetRB;
    }

    public void Select()
    {
        selfOutline.OutlineWidth = 3;
    }
    public void Deselect()
    {
        selfOutline.OutlineWidth = 0;
    }
}
