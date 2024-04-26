namespace AgeOfEnlightenment.Portals
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class PortalObjectBase : MonoBehaviour
    {
        protected static readonly Quaternion halfTurn = Quaternion.Euler(0.0f, 180.0f, 0.0f);

        public float PortalCooldown = .2f;

        protected Portal inPortal;
        protected Portal outPortal;
        protected int inPortalCount = 0;
        public bool justTeleported { get; protected set; } = false;
        public Vector3 previousPortalOffset { get; set; }

        public virtual void SetIsInPortal(Portal inPortal, Portal outPortal)
        {
            this.inPortal = inPortal;
            this.outPortal = outPortal;

            ++inPortalCount;
        }

        public virtual void ExitPortal()
        {
            --inPortalCount;

        }
        public abstract void Warp();
        protected IEnumerator TeleportCooldown()
        {
            yield return new WaitForSeconds(PortalCooldown);
            justTeleported = false;
        }
    }

}