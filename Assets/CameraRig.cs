using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRig : MonoBehaviour
{
    public Transform FloorOffset;
    public Transform Camera;
    public float SittingOffset = 0, CameraYOffset, PlayerControllerYOffset;
    public InputActionReference headPosition;


    [HideInInspector] public float AdjustedCameraHeight;
    private float _scale = 1;



    // Update is called once per frame
    void Update()
    {
        UpdateFloorOffset();

        AdjustedCameraHeight = FloorOffset.transform.localPosition.y + Camera.localPosition.y * _scale;

        Debug.Log(headPosition.action.ReadValue<Vector3>());
    }

    private void UpdateFloorOffset()
    {
        if (FloorOffset)
        {
            var pos = FloorOffset.transform.localPosition;
            var intendedOffset = GetYOffset();
            FloorOffset.transform.localPosition = new Vector3(pos.x, intendedOffset, pos.z);
        }
    }
    protected virtual float GetYOffset()
    {
        return SittingOffset + CameraYOffset + PlayerControllerYOffset;
    }
}
