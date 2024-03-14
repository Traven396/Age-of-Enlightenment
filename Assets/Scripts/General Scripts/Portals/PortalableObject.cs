using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalableObject : MonoBehaviour
{
    public float TeleCooldown = .2f;
    private GameObject cloneObject;

    private int inPortalCount = 0;
    
    private Portal inPortal;
    private Portal outPortal;

    private new Rigidbody rigidbody;
    protected new Collider collider;

    private static readonly Quaternion halfTurn = Quaternion.Euler(0.0f, 180.0f, 0.0f);

    public bool justTeleported { get; private set; } = false;

    protected virtual void Awake()
    {

        MeshFilter selfMeshFilter = GetComponent<MeshFilter>();
        if (!selfMeshFilter)
            selfMeshFilter = GetComponentInChildren<MeshFilter>();

        MeshRenderer selfMeshRenderer = GetComponent<MeshRenderer>();
        if (!selfMeshRenderer)
            selfMeshRenderer = GetComponentInChildren<MeshRenderer>();


        if (selfMeshFilter && selfMeshRenderer)
        {
            cloneObject = new GameObject();
            cloneObject.SetActive(false);
            var meshFilter = cloneObject.AddComponent<MeshFilter>();
            var meshRenderer = cloneObject.AddComponent<MeshRenderer>();



            meshFilter.mesh = selfMeshFilter.mesh;
            meshRenderer.materials = selfMeshRenderer.materials;
            cloneObject.transform.localScale = transform.localScale; 
        }

        rigidbody = GetComponent<Rigidbody>();
        if (!rigidbody)
            rigidbody = GetComponentInChildren<Rigidbody>();

        collider = GetComponent<Collider>();
        if (!collider)
            collider = GetComponentInChildren<Collider>();
    }

    private void LateUpdate()
    {
        if(inPortal == null || outPortal == null)
        {
            return;
        }

        if (cloneObject)
        {
            if (cloneObject.activeSelf && inPortal.Active && outPortal.Active)
            {
                var inTransform = inPortal.transform;
                var outTransform = outPortal.transform;

                // Update position of clone.
                Vector3 relativePos = inTransform.InverseTransformPoint(transform.position);
                relativePos = halfTurn * relativePos;
                cloneObject.transform.position = outTransform.TransformPoint(relativePos);

                // Update rotation of clone.
                Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * transform.rotation;
                relativeRot = halfTurn * relativeRot;
                cloneObject.transform.rotation = outTransform.rotation * relativeRot;
            }
            else
            {
                cloneObject.transform.position = new Vector3(-1000.0f, 1000.0f, -1000.0f);
            } 
        }
    }

    public void SetIsInPortal(Portal inPortal, Portal outPortal)
    {
        this.inPortal = inPortal;
        this.outPortal = outPortal;


        if(cloneObject)
            cloneObject.SetActive(false);

        ++inPortalCount;
    }

    public void ExitPortal()
    {
        --inPortalCount;

        if (inPortalCount == 0 && cloneObject)
        {
            cloneObject.SetActive(false);
        }
    }

    public virtual void Warp()
    {
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
            Vector3 relativeVel = inTransform.InverseTransformDirection(rigidbody.velocity);
            relativeVel = halfTurn * relativeVel;
            rigidbody.velocity = outTransform.TransformDirection(relativeVel);

            // Swap portal references.
            var tmp = inPortal;
            inPortal = outPortal;
            outPortal = tmp;

            justTeleported = true;
            StartCoroutine(TeleportCooldown());
        }
    }

    private IEnumerator TeleportCooldown()
    {
        yield return new WaitForSeconds(TeleCooldown);
        justTeleported = false;
    }
}
