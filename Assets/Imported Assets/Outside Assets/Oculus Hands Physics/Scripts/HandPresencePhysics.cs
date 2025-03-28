using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPresencePhysics : MonoBehaviour
{

    public Transform target;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Position
        rb.velocity = (target.position - transform.position) / Time.fixedDeltaTime;

        //Rotation
        Quaternion rotationDifference = target.rotation * Quaternion.Inverse(transform.rotation);
        rotationDifference.ToAngleAxis(out float angleInDegrees, out Vector3 rotationAxis);

        Vector3 rotationDifferenceInDegrees = angleInDegrees * rotationAxis;

        rb.angularVelocity = (rotationDifferenceInDegrees * Mathf.Deg2Rad / Time.fixedDeltaTime);
    }
}
