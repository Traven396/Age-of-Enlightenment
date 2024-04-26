namespace AgeOfEnlightenment.Portals
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class PortalablePlayerBody : PortalObjectBase
    {
        public PortalablePlayerCamera PortalPlayerCam;
        //Also check the hands

        private FrankensteinCharacterController characterController;

        [NonSerialized] public bool headCurrentlyTeleported;
        [NonSerialized] public bool leftHandCurrentlyTeleported;
        [NonSerialized] public bool rightHandCurrentlyTeleported;

        private void Awake()
        {
            //PortalPlayerCam = GetComponentInChildren<PortalablePlayerCamera>();
            //PortalPlayerCam._playerBody = this;
            
            characterController = GetComponent<FrankensteinCharacterController>();

            
        }
        public override void Warp()
        {
            //if (headCurrentlyTeleported)
            //{
            //    PortalPlayerCam.EnableDisableClone(false);
            //}
            //else
            //{
            //    headCurrentlyTeleported = true;
            //    PortalPlayerCam.EnableDisableClone(true);
            //}

            if (!justTeleported)
            {
                var inTransform = inPortal.transform;
                var outTransform = outPortal.transform;

                // Update position of object.
                Vector3 relativePos = inTransform.InverseTransformPoint(transform.position);
                relativePos = halfTurn * relativePos;
                transform.position = outTransform.TransformPoint(relativePos) + (-outTransform.forward * 0.15f);

                // Update rotation of object.
                Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * transform.rotation;
                relativeRot = halfTurn * relativeRot;
                transform.rotation = outTransform.rotation * relativeRot;

                // Update velocity of rigidbody.
                Vector3 relativeVel = inTransform.InverseTransformDirection(characterController.GetVelocity());
                relativeVel = halfTurn * relativeVel;
                characterController.SetMomentum(outTransform.TransformDirection(relativeVel));

                // Swap portal references.
                var tmp = inPortal;
                inPortal = outPortal;
                outPortal = tmp;

                justTeleported = true;
                StartCoroutine(TeleportCooldown()); 
            }
        }

        private void Update()
        {
            //Debug.Log(headCurrentlyTeleported);
        }
    }

}