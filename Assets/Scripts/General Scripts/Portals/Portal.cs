using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR;
using RenderPipeline = UnityEngine.Rendering.RenderPipelineManager;

public class Portal : MonoBehaviour
{
    [SerializeField] private PortalEyeCamera _portalCamera;
    [SerializeField] private Portal _destinationPortal;
    [SerializeField] private int recursionLimit = 3;
    public bool Active = true;

    private List<PortalableObject> portalObjects = new List<PortalableObject>();

    private Camera VrHeadset;
    private Renderer selfRenderer;

    RenderTexture leftEyeRenderTexture, rightEyeRenderTexture;

    private bool renderTexturesInitialized = false;
    private void Awake()
    {
        VrHeadset = Camera.main;

        selfRenderer = GetComponent<Renderer>();

        if (!_portalCamera)
            _portalCamera = FindObjectOfType<PortalEyeCamera>();
    }
    private void OnDestroy()
    {
        Destroy(selfRenderer.material);

        if (leftEyeRenderTexture && rightEyeRenderTexture)
        {
            leftEyeRenderTexture.Release();
            rightEyeRenderTexture.Release();
        }
    }
    private void Update()
    {
        InitializeCameras();

        for (int i = 0; i < portalObjects.Count; ++i)
        {
            Vector3 objPos = transform.InverseTransformPoint(portalObjects[i].transform.position);

            if (objPos.z < 0.0f)
            {
                portalObjects[i].Warp();
            }
        }
    }
    private void OnEnable()
    {
        RenderPipeline.beginContextRendering += RenderBothEyes;
    }
    private void OnDisable()
    {
        RenderPipeline.beginContextRendering -= RenderBothEyes;
    }


    private void OnTriggerEnter(Collider other)
    {
        var obj = other.GetComponent<PortalableObject>();
        if (obj != null)
        {
            portalObjects.Add(obj);
            obj.SetIsInPortal(this, _destinationPortal);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        var obj = other.GetComponent<PortalableObject>();

        if (portalObjects.Contains(obj))
        {
            portalObjects.Remove(obj);
            obj.ExitPortal();
        }
    }
    private void RenderBothEyes(ScriptableRenderContext context, List<Camera> thisOne)
    {
        if (HandyHelperScript.VisibleFromCamera(selfRenderer, VrHeadset))
        {
            if (_destinationPortal)
            {
                RenderPortal(LeftRight.Left, context);
                RenderPortal(LeftRight.Right, context);
            } 
        }
    }
    
    
    private void OrientCamerasMatrixStyle(LeftRight WhichEye)
    {
        Transform outTransform = _destinationPortal.transform;

        if (WhichEye == LeftRight.Left)
        {
            Matrix4x4[] recordedMatrices = new Matrix4x4[recursionLimit];
            Matrix4x4 portalMatrix = outTransform.localToWorldMatrix * Matrix4x4.Rotate(Quaternion.Euler(0f, 180f, 0f)) * transform.worldToLocalMatrix;
            Matrix4x4 localToWorldMatrix = _portalCamera.TrackedLeftEye.localToWorldMatrix;
            for (int i = 0; i < recursionLimit; i++)
            {
                localToWorldMatrix = portalMatrix * localToWorldMatrix;
                recordedMatrices[i] = localToWorldMatrix;
            }

            for (int i = recordedMatrices.Length - 1; i >= 0; i--)
            {
                _portalCamera.LeftEyeCamera.transform.SetPositionAndRotation(recordedMatrices[i].GetColumn(3), recordedMatrices[i].rotation);
            }
        }
        else
        {
            Matrix4x4[] recordedMatrices = new Matrix4x4[recursionLimit];
            Matrix4x4 portalMatrix = outTransform.localToWorldMatrix * Matrix4x4.Rotate(Quaternion.Euler(0f, 180f, 0f)) * transform.worldToLocalMatrix;
            Matrix4x4 localToWorldMatrix = _portalCamera.TrackedRightEye.localToWorldMatrix;
            for (int i = 0; i < recursionLimit; i++)
            {
                localToWorldMatrix = portalMatrix * localToWorldMatrix;
                recordedMatrices[i] = localToWorldMatrix;
            }

            for (int i = recordedMatrices.Length - 1; i >= 0; i--)
            {
                _portalCamera.RightEyeCamera.transform.SetPositionAndRotation(recordedMatrices[i].GetColumn(3), recordedMatrices[i].rotation);
            }
        }
    }
    private void InitializeCameras()
    {
        if (!renderTexturesInitialized && XRSettings.eyeTextureWidth > 0)
        {
            leftEyeRenderTexture = new RenderTexture(XRSettings.eyeTextureWidth, XRSettings.eyeTextureHeight, 24, RenderTextureFormat.ARGB32);
            rightEyeRenderTexture = new RenderTexture(XRSettings.eyeTextureWidth, XRSettings.eyeTextureHeight, 24, RenderTextureFormat.ARGB32);

            leftEyeRenderTexture.vrUsage = VRTextureUsage.OneEye;
            rightEyeRenderTexture.vrUsage = VRTextureUsage.OneEye;

            _portalCamera.LeftEyeCamera.targetTexture = leftEyeRenderTexture;

            _portalCamera.RightEyeCamera.targetTexture = rightEyeRenderTexture;


            selfRenderer.material.SetTexture("_LeftEyeTexture", leftEyeRenderTexture);
            selfRenderer.material.SetTexture("_RightEyeTexture", rightEyeRenderTexture);

            renderTexturesInitialized = true;
        }
    }
    private void RenderPortal(LeftRight whichEye, ScriptableRenderContext SRC)
    {
        OrientCamerasMatrixStyle(whichEye);

        Transform outTransform = _destinationPortal.transform;

        Plane plane = new Plane(outTransform.forward, outTransform.position);
        Vector4 clipPlaneWorldSpace = new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance);

        float distanceToPortal = Vector3.Dot(VrHeadset.transform.position - transform.position, transform.forward);

        //LEFT EYE
        if (whichEye == LeftRight.Left)
        {
            _portalCamera.LeftEyeCamera.targetTexture = leftEyeRenderTexture;

            if (MathF.Abs(distanceToPortal) > 0.1f)
            {
                _portalCamera.LeftEyeCamera.projectionMatrix = VrHeadset.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);

                Vector4 clipPlaneCameraSpaceLeft = Matrix4x4.Transpose(Matrix4x4.Inverse(_portalCamera.LeftEyeCamera.worldToCameraMatrix)) * clipPlaneWorldSpace;

                var newLeftMatrix = _portalCamera.LeftEyeCamera.CalculateObliqueMatrix(clipPlaneCameraSpaceLeft);

                _portalCamera.LeftEyeCamera.projectionMatrix = newLeftMatrix; 
            }

            UniversalRenderPipeline.RenderSingleCamera(SRC, _portalCamera.LeftEyeCamera);
        }
        else
        {
            _portalCamera.RightEyeCamera.targetTexture = rightEyeRenderTexture;

            if (MathF.Abs(distanceToPortal) > 0.1f)
            {
                _portalCamera.RightEyeCamera.projectionMatrix = VrHeadset.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);

                Vector4 clipPlaneCameraSpaceRight = Matrix4x4.Transpose(Matrix4x4.Inverse(_portalCamera.RightEyeCamera.worldToCameraMatrix)) * clipPlaneWorldSpace;

                var newRightMatrix = _portalCamera.RightEyeCamera.CalculateObliqueMatrix(clipPlaneCameraSpaceRight);

                _portalCamera.RightEyeCamera.projectionMatrix = newRightMatrix; 
            }

            UniversalRenderPipeline.RenderSingleCamera(SRC, _portalCamera.RightEyeCamera);
        }
    }
}
