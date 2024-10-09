using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR;
using TMPro;
using FoxheadDev.GestureDetection;

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

    private Step newStep;

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
            }
        }
        else
        {
            rightHand.TryGetFeatureValue(CommonUsages.deviceVelocity, out var vel);
        }

        var movementAmount = new Vector3((playerHand.localAngularVel / 20).y, -(playerHand.localAngularVel / 20).x, playerHand.localAngularVel.z);
        

        if(movementAmount.x >= detectionThreshold)
        {
            iTween.PunchScale(rightIndicator, Vector3.one * .2f, 1f);
        }
        if(movementAmount.x <= -detectionThreshold)
        {
            iTween.PunchScale(leftIndicator, Vector3.one * .2f, 1f);
        }
        if(movementAmount.y >= detectionThreshold)
        {
            iTween.PunchScale(upIndicator, Vector3.one * .2f, 1f);
        }
        if (movementAmount.y <= -detectionThreshold)
        {
            iTween.PunchScale(downIndicator, Vector3.one * .2f, 1f);

        }

        //Debug.Log("Regular First: "+ playerHand.angularVel + "\n" 
        //    + "Inverse First: " + playerHand.localAngularVel + "\n" 
        //    + "Rigidbody Reading: " + handRB.angularVelocity);
    }
}