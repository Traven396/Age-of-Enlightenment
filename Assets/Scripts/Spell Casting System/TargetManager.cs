using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TargetManager : MonoBehaviour
{
    public LayerMask ignoreLayers;

    [HideInInspector]public Transform leftHandPosition;

    [HideInInspector]public Transform rightHandPosition;

    private LayerMask groundLayer;

    #region GetClosest
    private TargettableEntity currentClosestLEntity = null;
    private TargettableEntity previousClosestLEntity = null;

    private TargettableEntity currentClosestREntity = null;
    private TargettableEntity previousClosestREntity = null;

    #endregion
    #region RaycastFromHand
    private GameObject leftTargetter;
    private GameObject rightTargetter;
    #endregion

    private void Start()
    {
        leftTargetter = new GameObject("Left Targetter");
        rightTargetter = new GameObject("Right Targetter");
        ignoreLayers = ~(ignoreLayers);
        groundLayer = LayerMask.NameToLayer("Ground");
    }
    //public ITargetable GetClosest(LeftRight hand)
    //{
    //    if (hand == 0)
    //    {
    //        RaycastHit hit;
    //        if (Physics.SphereCast(leftHandPosition.position, .1f, leftHandPosition.forward, out hit, 3f, ignoreLayers))
    //        {
    //            if (hit.collider.gameObject.TryGetComponent(out ITargetable target))
    //            {
    //                currentClosestL = target;
    //                currentClosestL.OnSelect();

    //                if (previousClosestL != currentClosestL)
    //                {
    //                    if (previousClosestL != null)
    //                        previousClosestL.OnDeselect();
    //                }
    //            }
    //            else
    //            {
    //                if (previousClosestL != null)
    //                    previousClosestL.OnDeselect();
    //                currentClosestL = null;
    //            }
    //        }
    //        else
    //        {
    //            if (previousClosestL != null)
    //                previousClosestL.OnDeselect();
    //            currentClosestL = null;
    //        }
    //        previousClosestL = currentClosestL;
    //        return currentClosestL;
    //    }
    //    else
    //    {
    //        RaycastHit hit;
    //        if (Physics.SphereCast(rightHandPosition.position, .1f, rightHandPosition.forward, out hit, 3f, ignoreLayers))
    //        {
    //            if (hit.collider.gameObject.TryGetComponent(out ITargetable target))
    //            {
    //                currentClosestR = target;
    //                currentClosestR.OnSelect();

    //                if (previousClosestR != currentClosestR)
    //                {
    //                    if (previousClosestR != null)
    //                        previousClosestR.OnDeselect();
    //                }
    //            }
    //            else
    //            {
    //                if (previousClosestR != null)
    //                    previousClosestR.OnDeselect();
    //                currentClosestR = null;
    //            }
    //        }
    //        else
    //        {
    //            if (previousClosestR != null)
    //                previousClosestR.OnDeselect();
    //            currentClosestR = null;
    //        }
    //        previousClosestR = currentClosestR;
    //        return currentClosestR;
    //    }
    //}
    public TargettableEntity GetClosestTeleTarget(LeftRight hand, float maxDistance, float radius)
    {
        if(hand == 0)
        {
            //RaycastHit hit;
            RaycastHit[] allHits;
            allHits = Physics.SphereCastAll(leftHandPosition.position, radius, leftHandPosition.forward, maxDistance, ignoreLayers);
            //if (Physics.SphereCast(rightHandPosition.position, radius, rightHandPosition.forward, out hit, maxDistance, ignoreLayers))
            if (allHits.Length != 0)
            {
                foreach (RaycastHit raycastHit in allHits)
                {
                    if (raycastHit.collider.gameObject.TryGetComponent(out TargettableEntity testTarget))
                    {
                        if (!testTarget.isSelected)
                        {
                            if (!testTarget.TeleTargettable)
                                return null;
                            currentClosestLEntity = testTarget;
                            break;
                        }
                        currentClosestLEntity = null;
                    }
                }
                if (currentClosestLEntity)
                {
                    currentClosestLEntity.Select();

                    if (previousClosestLEntity != currentClosestLEntity)
                    {
                        if (previousClosestLEntity != null)
                            previousClosestLEntity.Deselect();
                    }
                }
                else
                {
                    if (previousClosestLEntity != null)
                        previousClosestLEntity.Deselect();
                    currentClosestLEntity = null;
                }
            }
            else
            {
                if (previousClosestLEntity != null)
                    previousClosestLEntity.Deselect();
                currentClosestLEntity = null;
            }
            previousClosestLEntity = currentClosestLEntity;
            return currentClosestLEntity;
        }
        else
        {
            //RaycastHit hit;
            RaycastHit[] allHits;
            allHits = Physics.SphereCastAll(rightHandPosition.position, radius, rightHandPosition.forward, maxDistance, ignoreLayers);
            //if (Physics.SphereCast(rightHandPosition.position, radius, rightHandPosition.forward, out hit, maxDistance, ignoreLayers))
            if(allHits.Length != 0)
            {
                foreach (RaycastHit raycastHit in allHits)
                {
                    if(raycastHit.collider.gameObject.TryGetComponent(out TargettableEntity testTarget))
                    {
                        if (!testTarget.isSelected)
                        {
                            if (!testTarget.TeleTargettable)
                                return null;
                            currentClosestREntity = testTarget;
                            break;
                        }
                        currentClosestREntity = null;
                    }
                }
                if (currentClosestREntity)
                {
                    currentClosestREntity.Select();

                    if (previousClosestREntity != currentClosestREntity)
                    {
                        if (previousClosestREntity != null)
                            previousClosestREntity.Deselect();
                    }
                }
                else
                {
                    if (previousClosestREntity != null)
                        previousClosestREntity.Deselect();
                    currentClosestREntity = null;
                }
            }
            else
            {
                if (previousClosestREntity != null)
                    previousClosestREntity.Deselect();
                currentClosestREntity = null;
            }
            previousClosestREntity = currentClosestREntity;
            return currentClosestREntity;
        }
    }
    public RaycastHit RaycastFromHand(LeftRight hand, float maxDistance)
    {
        
        if (hand == 0)
        {
            RaycastHit hit;
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
            RaycastHit hit;
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
    public RaycastHit RaycastFromHandToGround(LeftRight hand, float maxDistance)
    {

        if (hand == 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(leftHandPosition.position, leftHandPosition.forward, out hit, maxDistance, groundLayer))
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
            RaycastHit hit;
            if (Physics.Raycast(rightHandPosition.position, rightHandPosition.forward, out hit, maxDistance, groundLayer))
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
