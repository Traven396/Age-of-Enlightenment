using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class TargetManager : MonoBehaviour
{
    public LayerMask ignoredLayers;

    public Transform LeftHandTransform;

    public Transform RightHandTransform;

    public Transform BodyPosition;

    private LayerMask groundLayer;

    #region GetClosest
    private TargettableEntity currentClosestLEntity = null;
    private TargettableEntity previousClosestLEntity = null;

    private TargettableEntity currentClosestREntity = null;
    private TargettableEntity previousClosestREntity = null;

    private RaycastHit[] allHits;
    #endregion

    public void NewSpell(SpellSwapCallbackContext ctx)
    {
        if (ctx.spawnedScript != null)
        {
            ctx.spawnedScript._TargetManager = this; 
        }
    }

    private void Start()
    {
        ignoredLayers = ~(ignoredLayers);
        groundLayer = ~LayerMask.NameToLayer("Ground");
    }
    public void ClearCurrentTarget(LeftRight whichHand)
    {
        if (whichHand == 0)
        {
            currentClosestLEntity.Deselect();
            currentClosestLEntity = null;
            previousClosestLEntity = null;
        }
        else
        {
            currentClosestREntity.Deselect();
            currentClosestREntity = null;
            previousClosestREntity = null;
        }
    }

    public TargettableEntity ReturnClosestTelekinesisTargetEntity(LeftRight hand, float maxDistance, float radius, Vector3 raycastDirection)
    {
        //Left Hand
        if(hand == 0)
        {
            Debug.Log("LEFT HAND TARGETING NOT SETUP");
            return null;
        }
        //Right Hand
        else
        {
            allHits = Physics.SphereCastAll(RightHandTransform.position, radius, raycastDirection, maxDistance, ignoredLayers);

            //If we dont hit anything with the raycast. Deselect the old one, and stop checking
            if (allHits.Length == 0)
            {
                ResetClosestTarget(hand);
                return null;
            }

            currentClosestREntity = DetermineClosestTelekinesisTarget(allHits).collider.gameObject.GetComponent<TargettableEntity>();

            //If the new closest object is not the same, reset stuff
            if(previousClosestREntity != currentClosestREntity)
            {
                ResetClosestTarget(hand);

                previousClosestREntity = currentClosestREntity;

                currentClosestREntity.Select();

            }

            if (currentClosestREntity)
            {
                if (currentClosestREntity.isSelected == false)
                {
                    currentClosestREntity.Select();
                } 
            }

            return currentClosestREntity;


        }
    }

    private void ResetClosestTarget(LeftRight whichSide)
    {
        if(whichSide == LeftRight.Left)
        {
            if (previousClosestLEntity)
            {
                previousClosestLEntity.Deselect();

                previousClosestLEntity = null;
            }
        }
        else
        {
            if (previousClosestREntity)
            {
                previousClosestREntity.Deselect();

                previousClosestREntity = null;
            }
        }

    }

    RaycastHit DetermineClosestTelekinesisTarget(RaycastHit[] hits)
    {
        RaycastHit closestHit = hits[0];
        var closestDistance = closestHit.distance;
        foreach (RaycastHit hit in hits)
        {
            if(hit.collider.gameObject.TryGetComponent<TargettableEntity>(out var tempObject))
            {
                if (tempObject.TelekinesisTargettable)
                {
                    if (hit.distance < closestDistance)
                    {
                        closestDistance = hit.distance;
                        closestHit = hit;
                    } 
                }
            }
        }

        return closestHit;
    }

    public TargettableEntity GetClosestTeleTarget(LeftRight hand, float maxDistance, float radius)
    {
        if(hand == 0)
        {
            //RaycastHit hit;
            RaycastHit[] allHits;
            allHits = Physics.SphereCastAll(LeftHandTransform.position, radius, LeftHandTransform.forward, maxDistance, ignoredLayers);
            //if (Physics.SphereCast(rightHandPosition.position, radius, rightHandPosition.forward, out hit, maxDistance, ignoreLayers))
            if (allHits.Length != 0)
            {
                foreach (RaycastHit raycastHit in allHits)
                {
                    if (raycastHit.collider.gameObject.TryGetComponent(out TargettableEntity testTarget))
                    {
                        if (!testTarget.isSelected)
                        {
                            if (!testTarget.TelekinesisTargettable)
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
            allHits = Physics.SphereCastAll(RightHandTransform.position, radius, RightHandTransform.forward, maxDistance, ignoredLayers);
            //if (Physics.SphereCast(rightHandPosition.position, radius, rightHandPosition.forward, out hit, maxDistance, ignoreLayers))
            if(allHits.Length != 0)
            {
                foreach (RaycastHit raycastHit in allHits)
                {
                    if(raycastHit.collider.gameObject.TryGetComponent(out TargettableEntity testTarget))
                    {
                        if (!testTarget.isSelected)
                        {
                            if (!testTarget.TelekinesisTargettable)
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
            Physics.Raycast(LeftHandTransform.position, LeftHandTransform.forward, out hit, maxDistance, ignoredLayers);
            return hit;
        }
        else
        {
            RaycastHit hit;
            Physics.Raycast(RightHandTransform.position, RightHandTransform.forward, out hit, maxDistance, ignoredLayers);
            return hit;
        }
    }
    public RaycastHit RaycastFromHandToGround(LeftRight hand, float maxDistance)
    {

        if (hand == 0)
        {
            Physics.Raycast(LeftHandTransform.position, LeftHandTransform.forward, out RaycastHit hit, maxDistance, groundLayer);
            return hit;
        }
        else
        {
            Physics.Raycast(RightHandTransform.position, RightHandTransform.forward, out RaycastHit hit, maxDistance, groundLayer);
            return hit;
        }
    }

    public RaycastHit[] HandSphereCastAll(LeftRight hand, float maxDistance, float sphereRadius)
    {
        RaycastHit[] hits;
        if(hand == 0)
        {
            hits = Physics.SphereCastAll(LeftHandTransform.position, sphereRadius, LeftHandTransform.forward, maxDistance, ignoredLayers);
        }
        else
        {
            hits = Physics.SphereCastAll(RightHandTransform.position, sphereRadius, RightHandTransform.forward, maxDistance, ignoredLayers);
        }
        return hits;
    }
    public RaycastHit[] BodyBoxCastAll(float maxDistance, Vector3 boxSize)
    {
        RaycastHit[] PreFiltereedHits = Physics.BoxCastAll(BodyPosition.position, boxSize, Camera.main.gameObject.transform.forward, Quaternion.identity, maxDistance, ignoredLayers);

        //foreach (RaycastHit hit in PreFiltereedHits)
        //{
        //    if(Physics.Linecast(hit.transform.position, ))
        //}
        //I could filter and see if the targeted objects are actually within sight of the player, but the center point of an object might be around the corner yet still be visible
        return PreFiltereedHits;
    }
}
