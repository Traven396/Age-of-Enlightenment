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

    public Portal FirstPortal;
    public Portal SecondPortal;

    public float nearClipOffset = 0.05f;
    public float nearClipLimit = 0.2f;
    

    //Render textures for each eye. Saved for reference, and setup inside awake to be screen size
    RenderTexture _leftEyeRenderTexture;
    RenderTexture _rightEyeRenderTexture;
    [Space(20f)]
    //This is the camera that is rendering the actual other side of the portal. We can just set it to be on the actual camera
    //We manually are rendering this camera, as it is disabled, and we are calling Render every frame
    public Camera PortalCam;

    //A variable that is set each time an eye is rendered. It gets set to the SteamVR.instance.eyes[1 or 0].pos. Then its z is set to 0
    //the local position of this GameObject is then set to this
    Quaternion leftEyeRotation;
    Quaternion rightEyeRotation;
    Vector3 leftEyePosition, rightEyePosition;

    private Material firstPortalMaterial, secondPortalMaterial;
    //protected void Awake()
    //{
    //    PortalCam.enabled = false;

    //    _leftEyeRenderTexture = new RenderTexture(Screen.width, Screen.height, 24);
    //    _rightEyeRenderTexture = new RenderTexture(Screen.width, Screen.height, 24);

    //    int aa = QualitySettings.antiAliasing == 0 ? 1 : QualitySettings.antiAliasing;
    //    _leftEyeRenderTexture.antiAliasing = aa;
    //    _rightEyeRenderTexture.antiAliasing = aa;

    //    firstPortalMaterial = FirstPortal.GetComponent<MeshRenderer>().material;
    //    secondPortalMaterial = SecondPortal.GetComponent<MeshRenderer>().material;

    //    //Debug.Log(VRHeadset.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left));
    //}

    private void LateUpdate()
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

        PortalCam.projectionMatrix = leftMatrix;
        PortalCam.targetTexture = _leftEyeRenderTexture;
        PortalCam.Render();
        material.SetTexture("_LeftEyeTexture", _leftEyeRenderTexture);

        Matrix4x4 rightMatrix = VRHeadset.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);

        PortalCam.projectionMatrix = rightMatrix;
        PortalCam.targetTexture = _rightEyeRenderTexture;
        PortalCam.Render();
        material.SetTexture("_RightEyeTexture", _rightEyeRenderTexture);
    }

    void PositionCameraByPortal(Portal firstTarget, Portal secondTarget, Vector3 EyePosition)
    {
        //Set the position of the portal camera to be the same as the eye, and rotate it to the headset.
        //I dont think there would be a way to rotate one eye and have it be different than the other eye? It would be the same as the headset?
        PortalCam.transform.SetPositionAndRotation(EyePosition, VRHeadset.transform.rotation);
        Debug.DrawLine(VRHeadset.transform.position, firstTarget.transform.position, Color.red);
        

        Transform inTransform = firstTarget.transform;
        Transform outTransform = secondTarget.transform;

        // Position the camera behind the other portal.
        Vector3 relativePos = inTransform.InverseTransformPoint(PortalCam.transform.position);
        relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
        PortalCam.transform.position = outTransform.TransformPoint(relativePos);

        Debug.DrawLine(outTransform.position, outTransform.TransformPoint(relativePos), Color.blue);

        // Rotate the camera to look through the other portal.
        Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * PortalCam.transform.rotation;
        relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeRot;
        PortalCam.transform.rotation = outTransform.rotation * relativeRot;
    }

    void RenderNewCamera(Transform inTransform, Transform outTransform, LeftRight LR)
    {
        //Set the camera's oblique view frustum.
        Plane p = new Plane(outTransform.forward, outTransform.position);

        Vector4 clipPlaneWorldSpace = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);

        Vector4 clipPlaneCameraSpace;

        //if (LR == LeftRight.Left)
        //    clipPlaneCameraSpace = Matrix4x4.Transpose(Matrix4x4.Inverse(PortalCam.GetStereoViewMatrix(Camera.StereoscopicEye.Left))) * clipPlaneWorldSpace;
        //if (LR == LeftRight.Right)
        //    clipPlaneCameraSpace = Matrix4x4.Transpose(Matrix4x4.Inverse(PortalCam.GetStereoViewMatrix(Camera.StereoscopicEye.Right))) * clipPlaneWorldSpace;
        //else

            clipPlaneCameraSpace = Matrix4x4.Transpose(Matrix4x4.Inverse(PortalCam.worldToCameraMatrix)) * clipPlaneWorldSpace;

        var newMatrix = VRHeadset.CalculateObliqueMatrix(clipPlaneCameraSpace);
        PortalCam.projectionMatrix = newMatrix;

        //Transform clipPlane = inTransform;
        //int dot = System.Math.Sign(Vector3.Dot(clipPlane.forward, transform.position - PortalCam.transform.position));

        //Vector3 camSpacePos = PortalCam.worldToCameraMatrix.MultiplyPoint(clipPlane.position);
        //Vector3 camSpaceNormal = PortalCam.worldToCameraMatrix.MultiplyVector(clipPlane.forward) * dot;
        //float camSpaceDst = -Vector3.Dot(camSpacePos, camSpaceNormal) + nearClipOffset;

        //// Don't use oblique clip plane if very close to portal as it seems this can cause some visual artifacts
        //if (Mathf.Abs(camSpaceDst) > nearClipLimit)
        //{
        //    Vector4 clipPlaneCameraSpace = new Vector4(camSpaceNormal.x, camSpaceNormal.y, camSpaceNormal.z, camSpaceDst);

        //    // Update projection based on new clip plane
        //    // Calculate matrix with player cam so that player camera settings (fov, etc) are used
        //    PortalCam.projectionMatrix = VRHeadset.CalculateObliqueMatrix(clipPlaneCameraSpace);
        //}
        //else
        //{
        //    PortalCam.projectionMatrix = VRHeadset.projectionMatrix;
        //}

        if (LR == LeftRight.Left)
            PortalCam.targetTexture = _leftEyeRenderTexture;
        else
            PortalCam.targetTexture = _rightEyeRenderTexture;

        PortalCam.Render();

        if (LR == LeftRight.Left)
        {
            firstPortalMaterial.SetTexture("_LeftEyeTexture", _leftEyeRenderTexture);
            //secondPortalMaterial.SetTexture("_LeftEyeTexture", _leftEyeRenderTexture);
        } else
        {
            firstPortalMaterial.SetTexture("_RightEyeTexture", _rightEyeRenderTexture);
            secondPortalMaterial.SetTexture("_RightEyeTexture", _rightEyeRenderTexture);
        }
    }

}