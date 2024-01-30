using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllableProjectile : MonoBehaviour
{
    private GestureManager RemoteController;
    [SerializeField] private float ControlStrength;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (RemoteController)
        {
            if (!rb.isKinematic)
            {
                rb.AddForce(RemoteController.currVel * ControlStrength, ForceMode.Force); 
            }
        }
    }

    public void SetController(GestureManager newController)
    {
        RemoteController = newController;
    }
}
