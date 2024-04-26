using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR;
using TMPro;

public class TestingScript : MonoBehaviour
{
    [Range(0,1)]
    public float hapticStrenght;



    public Rigidbody handRB;
    public GestureManager playerHand;
    public Vector3 displayedAng;
    [Space(10)]
    public float detectionThreshold = 2;

    private InputDevice rightHand;
    private InputDevice headset;
    private bool headsetInitialized;

    public GameObject leftIndicator, upIndicator, rightIndicator, downIndicator;

    private void Start()
    {
        
        
        
    }
    private void Update()
    {

        if (!headsetInitialized)
        {
            headset = InputDevices.GetDeviceAtXRNode(XRNode.Head);
            if (headset.isValid && XRSettings.eyeTextureWidth > 0) 
            {
                rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
                headsetInitialized = true;
                Debug.Log("Right Hand located");
            }
        }
        else
        {
            rightHand.TryGetFeatureValue(CommonUsages.deviceVelocity, out var vel);
            Debug.Log(vel);

            
            Debug.DrawLine(Vector3.zero, vel, Color.blue);
        }

        var movementAmount = new Vector3((playerHand.localAngularVel / 20).y, -(playerHand.localAngularVel / 20).x, playerHand.localAngularVel.z);
        

        if(movementAmount.x >= detectionThreshold)
        {
            iTween.PunchScale(rightIndicator, Vector3.one * .2f, 1f);
            Debug.Log("Right Flick");
        }
        if(movementAmount.x <= -detectionThreshold)
        {
            iTween.PunchScale(leftIndicator, Vector3.one * .2f, 1f);
            Debug.Log("Left Flick");
        }
        if(movementAmount.y >= detectionThreshold)
        {
            iTween.PunchScale(upIndicator, Vector3.one * .2f, 1f);
            Debug.Log("Upward Flick");
        }
        if (movementAmount.y <= -detectionThreshold)
        {
            iTween.PunchScale(downIndicator, Vector3.one * .2f, 1f);
            Debug.Log("Downward Flick");
        }

        var dirVector = movementAmount.normalized;
        dirVector.z = 0;
        if (dirVector.magnitude > .1f)
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(dirVector), Color.red); 
        }

        displayedAng = dirVector;

        //Debug.Log("Regular First: "+ playerHand.angularVel + "\n" 
        //    + "Inverse First: " + playerHand.localAngularVel + "\n" 
        //    + "Rigidbody Reading: " + handRB.angularVelocity);
    }
}