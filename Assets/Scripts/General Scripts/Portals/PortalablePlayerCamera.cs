namespace AgeOfEnlightenment.Portals
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class PortalablePlayerCamera : PortalObjectBase
    {
        private bool UsingMainCamera { get { return !cloneCam.enabled; } }

        private GameObject cloneCamObject;
        private Camera cloneCam;

        public PortalablePlayerBody _playerBody { get; set; }
        private void Awake()
        {
            cloneCamObject = new GameObject("Camera Portal Clone");

            cloneCam = cloneCamObject.AddComponent<Camera>();
            cloneCam.CopyFrom(Camera.main);

            cloneCam.depth = 1;

            EnableDisableClone(false);
        }
        public override void Warp()
        {
            if (!_playerBody.headCurrentlyTeleported)
            {
                EnableDisableClone(true);
                _playerBody.headCurrentlyTeleported = true;
            }
            else
            {
                EnableDisableClone(false);
                _playerBody.headCurrentlyTeleported = false;
            }
        }
        private void Update()
        {
            if (inPortal == null || outPortal == null)
            {
                return;
            }

            if (cloneCamObject.activeSelf)
            {
                var inTransform = inPortal.transform;
                var outTransform = outPortal.transform;

                // Update position of clone.
                Vector3 relativePos = inTransform.InverseTransformPoint(transform.position);
                relativePos = halfTurn * relativePos;
                cloneCamObject.transform.position = outTransform.TransformPoint(relativePos);

                // Update rotation of clone.
                Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * transform.rotation;
                relativeRot = halfTurn * relativeRot;
                cloneCamObject.transform.rotation = outTransform.rotation * relativeRot;
            }
        }
        public void EnableDisableClone(bool changeTo)
        {
            cloneCam.enabled = changeTo;
            cloneCamObject.SetActive(changeTo);

            //Camera.main.enabled = !changeTo;
        }
    }

}