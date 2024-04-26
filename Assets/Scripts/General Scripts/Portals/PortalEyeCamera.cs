using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PortalEyeCamera : MonoBehaviour
{
    public Camera LeftEyeCamera;
    public Camera RightEyeCamera;
    public Transform TrackedLeftEye, TrackedRightEye;

    private bool headsetInitialized = false;

    private void Awake()
    {
        
    }
    //public void AlignCameras(LeftRight whichEye)
    //{
    //    if(whichEye == LeftRight.Left)
    //        LeftEyeCamera.transform.SetPositionAndRotation(TrackedLeftEye.position, TrackedLeftEye.rotation);
    //    else
    //        RightEyeCamera.transform.SetPositionAndRotation(TrackedRightEye.position, TrackedRightEye.rotation);
    //}
    public void AlignCameras(LeftRight whichEye)
    {
        if (whichEye == LeftRight.Left)
            LeftEyeCamera.transform.rotation = TrackedLeftEye.rotation;
        else
            RightEyeCamera.transform.rotation = TrackedRightEye.rotation;
    }
    private void Update()
    {
        


        //Debug.Log(SHIT);
        if (!headsetInitialized)
        {
            InputDevice headset = InputDevices.GetDeviceAtXRNode(XRNode.Head);
            if (headset.isValid && XRSettings.eyeTextureWidth > 0)
            {
                LeftEyeCamera.transform.SetPositionAndRotation(TrackedLeftEye.position, TrackedLeftEye.rotation);

                RightEyeCamera.transform.SetPositionAndRotation(TrackedRightEye.position, TrackedRightEye.rotation);


                //Quaternion leftEyeRotation, rightEyeRotation;
                //Vector3 leftEyePosition, rightEyePosition, centerEye;

                //headset.TryGetFeatureValue(CommonUsages.rightEyeRotation, out rightEyeRotation);
                //headset.TryGetFeatureValue(CommonUsages.leftEyeRotation, out leftEyeRotation);

                //headset.TryGetFeatureValue(CommonUsages.leftEyePosition, out leftEyePosition);
                //headset.TryGetFeatureValue(CommonUsages.rightEyePosition, out rightEyePosition);

                //headset.TryGetFeatureValue(CommonUsages.devicePosition, out centerEye);

                //transform.position = Camera.main.transform.position;

                //transform.rotation = Camera.main.transform.rotation;

                //Vector3 left = Quaternion.Inverse(leftEyeRotation) * leftEyePosition;
                //Vector3 right = Quaternion.Inverse(rightEyeRotation) * rightEyePosition;

                //Vector3 leftWorld, rightWorld;

                //Vector3 offset = (left - right) * 0.5f;

                //Matrix4x4 m = Camera.main.cameraToWorldMatrix;

                //Debug.Log("Offset: " + offset + "\n"
                //        + "Manual Offset: " + (centerEye - leftEyePosition));

                //leftWorld = m.MultiplyPoint(-offset);
                //rightWorld = m.MultiplyPoint(offset);

                //LeftEyeCamera.transform.position = leftWorld;
                //RightEyeCamera.transform.position = rightWorld;



                //LeftEyeCamera.transform.rotation = leftEyeRotation;
                //RightEyeCamera.transform.rotation = rightEyeRotation;

                headsetInitialized = true;
            } 
        }
    }
}
