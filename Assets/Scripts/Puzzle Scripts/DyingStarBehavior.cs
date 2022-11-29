using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
/*
 * Aidyn Reis
 * 10/6/22
 * Advanced Technologies 2nd project
 */

public class DyingStarBehavior : MonoBehaviour
{
    private ConfigurableJoint selfJoint;
    public GameObject anchorPoint;

    XRBaseInteractor currentInteractor = null;

    float maxDistance = 1f;


    void Start()
    {
        //Set the joint variable to itself
        selfJoint = GetComponent<ConfigurableJoint>();
        Rigidbody rb;
        if (anchorPoint == null)
        {
            //Make an anchor point right when the star spawns
            anchorPoint = new GameObject("Anchor");

            //Move it to the star object
            anchorPoint.transform.position = transform.position;

            //Provide it with a rigidbody component
            rb = anchorPoint.AddComponent<Rigidbody>();

            //Apply the required settings for it
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
        else
        {
            rb = anchorPoint.GetComponent<Rigidbody>();
        }
        //Then attach it to the joint
        selfJoint.connectedBody = rb;

        //Make the breaking distance dependent on the joint, which allows me to make various sized star lengths
        maxDistance = GetComponent<ConfigurableJoint>().linearLimit.limit * 1.05f;
       
    }
    private void FixedUpdate()
    {
        CheckDistance();
    }

    public void CheckDistance()
    {
        if (currentInteractor != null)
        {
            //Calculate the distance
            float distanceDeficit = Vector3.Distance(anchorPoint.transform.position, currentInteractor.transform.position);
            
            if (distanceDeficit > maxDistance)
            {
               
                currentInteractor.allowSelect = false;
            }
        }
     
    }

    public void SelectEnter(SelectEnterEventArgs args)
    {
        //Get a reference to the current thing grabbing the star
        currentInteractor = (XRBaseInteractor)args.interactorObject;

    }
    public void SelectExit()
    {
        //Make sure to turn back on the selection, then remove the reference
        currentInteractor.allowSelect = true;
        currentInteractor = null;
        
    }

    private void OnDestroy()
    {
        //A simple way to make sure the hierarchy stays clean
        Destroy(anchorPoint);
    }
}
