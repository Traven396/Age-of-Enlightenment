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
    public Camera VRHeadset;

    public NewPortalBehavior FirstPortal;
    public NewPortalBehavior SecondPortal;

    //Render textures for each eye. Saved for reference, and setup inside awake to be screen size
    RenderTexture _leftEyeRenderTexture;
    RenderTexture _rightEyeRenderTexture;

    //This is the camera that is rendering the actual other side of the portal. We can just set it to be on the actual camera
    //We manually are rendering this camera, as it is disabled, and we are calling Render every frame
    public Camera PortalCameraObject;

    //A variable that is set each time an eye is rendered. It gets set to the SteamVR.instance.eyes[1 or 0].pos. Then its z is set to 0
    //the local position of this GameObject is then set to this
    Quaternion leftEyeRotation;
    Quaternion rightEyeRotation;
    Vector3 leftEyePosition, rightEyePosition;

    private Material firstPortalMaterial, secondPortalMaterial;
    protected void Awake()
    {
        PortalCameraObject.enabled = false;

        _leftEyeRenderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        _rightEyeRenderTexture = new RenderTexture(Screen.width, Screen.height, 24);

        int aa = QualitySettings.antiAliasing == 0 ? 1 : QualitySettings.antiAliasing;
        _leftEyeRenderTexture.antiAliasing = aa;
        _rightEyeRenderTexture.antiAliasing = aa;

        firstPortalMaterial = FirstPortal.GetComponent<MeshRenderer>().material;
        secondPortalMaterial = SecondPortal.GetComponent<MeshRenderer>().material;
    }

    private void Update()
    {
        RenderPortal();
    }

    void RenderPortal()
    {
        PositionCameraByPortal(FirstPortal, SecondPortal, GetEyeWorldPosition(LeftRight.Left));
        RenderNewCamera(FirstPortal.transform, SecondPortal.transform, LeftRight.Left);

        PositionCameraByPortal(FirstPortal, SecondPortal, GetEyeWorldPosition(LeftRight.Right));
        RenderNewCamera(FirstPortal.transform, SecondPortal.transform, LeftRight.Right);
    }

    public Vector3 GetEyeWorldPosition(LeftRight LR)
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(XRNode.Head);
        if (device.isValid)
        {
            device.TryGetFeatureValue(CommonUsages.rightEyeRotation, out rightEyeRotation);
            device.TryGetFeatureValue(CommonUsages.leftEyeRotation, out leftEyeRotation);
            device.TryGetFeatureValue(CommonUsages.leftEyePosition, out leftEyePosition);
            device.TryGetFeatureValue(CommonUsages.rightEyePosition, out rightEyePosition);
        }

        Vector3 left = Quaternion.Inverse(leftEyeRotation) * leftEyePosition;
        Vector3 right = Quaternion.Inverse(rightEyeRotation) * rightEyePosition;

        Vector3 leftWorld, rightWorld;

        Vector3 offset = (left - right) * 0.5f;

        Matrix4x4 m = VRHeadset.cameraToWorldMatrix;

        leftWorld = m.MultiplyPoint(-offset);
        rightWorld = m.MultiplyPoint(offset);

        if (LR == LeftRight.Left)
            return leftWorld;
        else
            return rightWorld;
    }

    public void RenderIntoMaterial(Material material)
    {
        //Vector3 leftWorldAgain, rightWorldAgain;
        //leftWorldAgain = VRHeadset.transform.TransformPoint(leftEyePosition);
        //rightWorldAgain = VRHeadset.transform.parent.TransformPoint(rightEyePosition);

        Matrix4x4 leftMatrix = VRHeadset.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);

        PortalCameraObject.projectionMatrix = leftMatrix;
        PortalCameraObject.targetTexture = _leftEyeRenderTexture;
        PortalCameraObject.Render();
        material.SetTexture("_LeftEyeTexture", _leftEyeRenderTexture);

        Matrix4x4 rightMatrix = VRHeadset.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);

        PortalCameraObject.projectionMatrix = rightMatrix;
        PortalCameraObject.targetTexture = _rightEyeRenderTexture;
        PortalCameraObject.Render();
        material.SetTexture("_RightEyeTexture", _rightEyeRenderTexture);
    }

    void PositionCameraByPortal(NewPortalBehavior firstTarget, NewPortalBehavior secondTarget, Vector3 EyePosition)
    {
        //Set the position of the portal camera to be the same as the eye, and rotate it to the headset.
        //I dont think there would be a way to rotate one eye and have it be different than the other eye? It would be the same as the headset?
        PortalCameraObject.transform.SetPositionAndRotation(EyePosition, VRHeadset.transform.rotation);
        Debug.DrawLine(VRHeadset.transform.position, firstTarget.transform.position, Color.red);
        

        Transform inTransform = firstTarget.transform;
        Transform outTransform = secondTarget.transform;

        // Position the camera behind the other portal.
        Vector3 relativePos = inTransform.InverseTransformPoint(PortalCameraObject.transform.position);
        relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
        PortalCameraObject.transform.position = outTransform.TransformPoint(relativePos);

        Debug.DrawLine(outTransform.position, outTransform.TransformPoint(relativePos), Color.blue);

        // Rotate the camera to look through the other portal.
        Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * PortalCameraObject.transform.rotation;
        relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeRot;
        PortalCameraObject.transform.rotation = outTransform.rotation * relativeRot;
    }

    void RenderNewCamera(Transform inTransform, Transform outTransform, LeftRight LR)
    {
        // Set the camera's oblique view frustum.
        Plane p = new Plane(-outTransform.forward, outTransform.position);
        Vector4 clipPlaneWorldSpace = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        Vector4 clipPlaneCameraSpace =
            Matrix4x4.Transpose(Matrix4x4.Inverse(PortalCameraObject.worldToCameraMatrix)) * clipPlaneWorldSpace;

        var newMatrix = VRHeadset.CalculateObliqueMatrix(clipPlaneCameraSpace);
        PortalCameraObject.projectionMatrix = newMatrix;

        if(LR == LeftRight.Left)
            PortalCameraObject.targetTexture = _leftEyeRenderTexture;
        else
            PortalCameraObject.targetTexture = _rightEyeRenderTexture;

        PortalCameraObject.Render();

        if (LR == LeftRight.Left)
        {
            firstPortalMaterial.SetTexture("_LeftEyeTexture", _leftEyeRenderTexture);
            secondPortalMaterial.SetTexture("_LeftEyeTexture", _leftEyeRenderTexture);
        } else
        {
            firstPortalMaterial.SetTexture("_RightEyeTexture", _rightEyeRenderTexture);
            secondPortalMaterial.SetTexture("_RightEyeTexture", _rightEyeRenderTexture);
        }
    }

}