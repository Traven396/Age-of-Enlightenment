using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TargetManager : MonoBehaviour
{
    public LayerMask ignoreLayers;

    [HideInInspector]public Transform leftHandPosition;

    [HideInInspector]public Transform rightHandPosition;


    #region GetClosest
    private ITargetable currentClosestL = null;
    private ITargetable previousClosestL = null;

    private ITargetable currentClosestR = null;
    private ITargetable previousClosestR = null;
    #endregion
    #region RaycastFromHand
    private GameObject leftTargetter;
    private GameObject rightTargetter;
    #endregion

    private void Start()
    {
        leftTargetter = new GameObject("Left Targetter");
        rightTargetter = new GameObject("Right Targetter");
    }
    public ITargetable GetClosest(LeftRight hand)
    {
        if (hand == 0)
        {
            RaycastHit hit;
            if (Physics.SphereCast(leftHandPosition.position, .1f, leftHandPosition.forward, out hit, 3f, ignoreLayers))
            {
                if (hit.collider.gameObject.TryGetComponent(out ITargetable target))
                {
                    currentClosestL = target;
                    currentClosestL.OnSelect();

                    if (previousClosestL != currentClosestL)
                    {
                        if (previousClosestL != null)
                            previousClosestL.OnDeselect();
                    }
                }
                else
                {
                    if (previousClosestL != null)
                        previousClosestL.OnDeselect();
                    currentClosestL = null;
                }
            }
            else
            {
                if (previousClosestL != null)
                    previousClosestL.OnDeselect();
                currentClosestL = null;
            }
            previousClosestL = currentClosestL;
            return currentClosestL;
        }
        else
        {
            RaycastHit hit;
            if (Physics.SphereCast(rightHandPosition.position, .1f, rightHandPosition.forward, out hit, 3f, ignoreLayers))
            {
                if (hit.collider.gameObject.TryGetComponent(out ITargetable target))
                {
                    currentClosestR = target;
                    currentClosestR.OnSelect();

                    if (previousClosestR != currentClosestR)
                    {
                        if (previousClosestR != null)
                            previousClosestR.OnDeselect();
                    }
                }
                else
                {
                    if (previousClosestR != null)
                        previousClosestR.OnDeselect();
                    currentClosestR = null;
                }
            }
            else
            {
                if (previousClosestR != null)
                    previousClosestR.OnDeselect();
                currentClosestR = null;
            }
            previousClosestR = currentClosestR;
            return currentClosestR;
        }
    }

    public RaycastHit RaycastFromHand(LeftRight hand, float maxDistance)
    {
        
        if (hand == 0)
        {
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(leftHandPosition.position, leftHandPosition.forward, out hit, maxDistance, ignoreLayers))
            {
                leftTargetter.transform.position = hit.point;
                
                return hit;
            }
            else
            {
                return hit;
            }
        }
        else
        {
            RaycastHit hit = new RaycastHit();
            if(Physics.Raycast(rightHandPosition.position, rightHandPosition.forward, out hit, maxDistance, ignoreLayers))
            {
                rightTargetter.transform.position = hit.point;
                return hit;
            }
            else
            {
                return hit;
            }
        }
    }

    public RaycastHit[] HandSphereCastAll(LeftRight hand, float maxDistance, float sphereRadius)
    {
        RaycastHit[] hits;
        if(hand == 0)
        {
            hits = Physics.SphereCastAll(leftHandPosition.position, sphereRadius, leftHandPosition.forward, maxDistance, ignoreLayers);
        }
        else
        {
            hits = Physics.SphereCastAll(rightHandPosition.position, sphereRadius, rightHandPosition.forward, maxDistance, ignoreLayers);
        }
        return hits;
    }
}
