using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GestureManager : MonoBehaviour
{

    public bool displayInChat = false;
    public bool LeftHand = false;


    private Vector3 prevPos;
    public Vector3 currVel { get; private set; }
    public float dotProdX { get; private set; }
    public float dotProdY { get; private set; }
    public float dotProdZ { get; private set; }

    public float angX { get; private set; }
    public float angY { get; private set; }
    public float angZ { get; private set; }
    private Quaternion previousRotation;
    public Vector3 angularVel { get; private set; }
    public float angularDotX { get; private set; }
    public float angularDotY { get; private set; }
    public float angularDotZ { get; private set; }

    
    private void Update()
    {
        if (displayInChat)
        {
            Debug.Log("Current Velocity: " + currVel + "\n"
                + "DotProdX: " + dotProdX.ToString("#.0") + "\n"
                + "DotProdY: " + dotProdY.ToString("#.0") + "\n"
                + "DotProdZ: " + dotProdZ.ToString("#.0") + "\n"
                + "AngX: " + angX.ToString("#.0") + "\n"
                + "AngY: " + angY.ToString("#.0") + "\n"
                + "AngZ: " + angZ.ToString("#.0") + "\n"
                + "Angular Velocity: " + angularVel + "\n"
                + "DotAngX: " + angularDotX.ToString("#.0") + "\n"
                + "DotAngY: " + angularDotY.ToString("#.0") + "\n"
                + "DotAngZ: " + angularDotZ.ToString("#.0") + "\n");
        }
        #region Angular Velocity
        Quaternion deltaRotation = transform.rotation * Quaternion.Inverse(previousRotation);
        previousRotation = transform.rotation;
        deltaRotation.ToAngleAxis(out var angle, out var axis);
        angle *= Mathf.Deg2Rad;
        angularVel = (1.0f / Time.deltaTime) * angle * axis;

        angularDotX = Vector3.Dot(angularVel, transform.right);
        angularDotY = Vector3.Dot(angularVel, transform.up);
        angularDotZ = Vector3.Dot(angularVel, transform.forward);
        #endregion
        #region Regular Velocity
        if (prevPos != null)
        {
            currVel = (transform.position - prevPos) / Time.deltaTime;

            dotProdX = Vector3.Dot(currVel, transform.right);
            dotProdY = Vector3.Dot(currVel, transform.up);
            dotProdZ = Vector3.Dot(currVel, transform.forward);

            angX = Vector3.Angle(currVel, transform.right);
            angY = Vector3.Angle(currVel, transform.up);
            angZ = Vector3.Angle(currVel, transform.forward);

            if (LeftHand)
            {
                dotProdX = -dotProdX;
            }
        }
        prevPos = transform.position; 
        #endregion
    }

    
}