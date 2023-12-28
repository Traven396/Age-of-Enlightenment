using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantSpin : MonoBehaviour
{
    public float rotationSpeed = 1;
    private enum XYZ
    {
        X,
        Y,
        Z
    }
    [SerializeField] private XYZ RotationAxis;
    private void Update()
    {
        switch (RotationAxis)
        {
            case XYZ.X:
                transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime, Space.Self);
                break;
            case XYZ.Y:
                transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
                break;
            case XYZ.Z:
                transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime, Space.Self);
                break;
        }

    }

    public void SetSpeed(float newRotationSpeed)
    {
        rotationSpeed = newRotationSpeed;
    }
    public void SetNewAxis(string XYZ)
    {
        switch (XYZ) {
            case "X":
                RotationAxis = ConstantSpin.XYZ.X;
                break;
            case "Y":
                RotationAxis = ConstantSpin.XYZ.Y;
                break;
            case "Z":
                RotationAxis = ConstantSpin.XYZ.Z;
                break;
            default:
                Debug.Log("Invalid axis. What da heck");
                break;
        }
        
    }
}
