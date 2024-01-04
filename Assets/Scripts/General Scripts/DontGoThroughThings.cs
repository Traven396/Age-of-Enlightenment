using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontGoThroughThings : MonoBehaviour
{
    public LayerMask _nonCollidableLayers;

    private bool Enabled = true;

    private Rigidbody _rb;
    private float minimumExtent;
    private float partialExtent;
    private float sqrMinimumExtent;
    private Vector3 previousPosition;
    private Collider selfCollider;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        selfCollider = GetComponent<Collider>();
        if (!selfCollider)
        {
            selfCollider = GetComponentInChildren<Collider>();
        }

        minimumExtent = Mathf.Min(Mathf.Min(selfCollider.bounds.extents.x, selfCollider.bounds.extents.y), selfCollider.bounds.extents.z);
        partialExtent = minimumExtent * (1.0f - .1f);
        sqrMinimumExtent = minimumExtent * minimumExtent;

        previousPosition = _rb.position;
    }

    private void FixedUpdate()
    {
        if (Enabled)
        {
            if (!_rb.isKinematic)
            {
                Vector3 movementThisStep = _rb.position - previousPosition;


                float movementSqrMagnitude = movementThisStep.sqrMagnitude;

                if (movementSqrMagnitude > sqrMinimumExtent)
                {
                    float movementMagnitude = Mathf.Sqrt(movementSqrMagnitude);
                    RaycastHit hitInfo;

                    //check for obstructions we might have missed 
                    if (Physics.Raycast(previousPosition, movementThisStep, out hitInfo, movementMagnitude, ~_nonCollidableLayers))
                    {
                        _rb.position = hitInfo.point - (movementThisStep / movementMagnitude) * partialExtent;
                    }
                }

                previousPosition = _rb.position;
            } 
        }
    }

    public void ChangeEnabled(bool newStatus)
    {
        Enabled = newStatus;
        if(_rb)
        previousPosition = _rb.position;
    }
}
