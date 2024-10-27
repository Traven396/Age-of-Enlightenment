namespace AgeOfEnlightenment.Portals
{
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

        private List<PortalObjectBase> portalObjects = new List<PortalObjectBase>();

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
        private void LateUpdate()
        {
            InitializeCameras();

            for (int i = 0; i < portalObjects.Count; ++i)
            {
                Transform currentPortalObjectT = portalObjects[i].transform;
                Vector3 localForward = transform.TransformDirection(Vector3.forward);


                Vector3 portalToObject = currentPortalObjectT.position - transform.position;
                float dotProduct = Vector3.Dot(localForward, portalToObject);

                //Debug.Log(dotProduct);

                // If this is true: The player has moved across the portal
                if (dotProduct < 0f)
                {
                    portalObjects[i].Warp();

                    _destinationPortal.OnObjectEnterPortal(portalObjects[i]);

                    portalObjects.RemoveAt(i);
                    
                    i--;
                }

            //    Vector3 offsetFromPortal = currentPortalObjectT.position - transform.position;
            //    

            //    int portalSide = Math.Sign(Vector3.Dot(offsetFromPortal, localForward));
            //    int portalSideOld = Math.Sign(Vector3.Dot(portalObjects[i].previousPortalOffset, localForward));

            //    if(portalSide != portalSideOld)
            //    {
                    
            //        portalObjects[i].Warp();

            //        //The players camera is  removed when the players body goes through in the TriggerExit method
            //        //If you "peeking" through the portal you arent actually teleporting, so you need to be able to go back through and stop "peeking"

            //            Debug.Log("Teleported somethign. And removed it from the list");
            //            _destinationPortal.OnObjectEnterPortal(portalObjects[i]);
            //            portalObjects.RemoveAt(i);
            //            i--;


            //        portalObjects[i].previousPortalOffset = offsetFromPortal;
            //    }
            //    else
            //    {
            //        portalObjects[i].previousPortalOffset = offsetFromPortal;
            //    }
            //    Debug.DrawLine(portalObjects[i].transform.position, portalObjects[i].transform.position + (Vector3.up * .1f), Color.red);
            //    Debug.DrawLine(transform.TransformVector(offsetFromPortal), transform.position, Color.blue);
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

        int SideOfPortal(Vector3 pos)
        {
            return Math.Sign(Vector3.Dot(pos - transform.position, transform.forward));
        }

        public void OnObjectEnterPortal(PortalObjectBase portalObject)
        {
            if (!portalObjects.Contains(portalObject))
            {
                portalObject.SetIsInPortal(this, _destinationPortal);
                //portalObject.previousPortalOffset = portalObject.transform.position - transform.position;
                portalObjects.Add(portalObject);
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            var obj = other.GetComponent<PortalObjectBase>();
            if (obj)
            {
                OnObjectEnterPortal(obj);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            var obj = other.GetComponent<PortalObjectBase>();

            if (obj && portalObjects.Contains(obj))
            {
                portalObjects.Remove(obj);
                obj.ExitPortal();
            }
        }

        public void RemovePortalObject(PortalObjectBase targetToRemove)
        {
            if (portalObjects.Contains(targetToRemove))
                portalObjects.Remove(targetToRemove);
        }

        private void RenderBothEyes(ScriptableRenderContext context, List<Camera> thisOne)
        {
            if (Active)
            {
                if (selfRenderer.isVisible)
                {
                    if (_destinationPortal)
                    {
                        RenderPortalNewStyle(LeftRight.Left, context);
                        RenderPortalNewStyle(LeftRight.Right, context);
                    }
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

        private void RenderPortalNewStyle(LeftRight whichEye, ScriptableRenderContext SRC)
        {
            OrientCamerasMatrixStyle(whichEye);

            Transform outTransform = _destinationPortal.transform;
            Vector3 portalToPlayerVector = (VrHeadset.transform.position - transform.position).normalized;
            Plane plane;

            if (Vector3.Dot(portalToPlayerVector, transform.forward) > 0)
                plane = new Plane(outTransform.forward, outTransform.transform.position);
            else
                plane = new Plane(-outTransform.forward, outTransform.transform.position);

            Vector4 clipPlaneWorldSpace = new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance);

            //float distanceToPortal = Vector3.Dot(VrHeadset.transform.position - transform.position, transform.forward);
            float distanceToPortal = 1;
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

}