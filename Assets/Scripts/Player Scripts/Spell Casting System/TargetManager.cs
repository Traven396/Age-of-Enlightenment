using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TargetManager : MonoBehaviour
{
    public LayerMask ignoreLayers;

    public Transform leftHandPosition;

    public Transform rightHandPosition;

    private LayerMask groundLayer;

    #region GetClosest
    private TargettableEntity currentClosestLEntity = null;
    private TargettableEntity previousClosestLEntity = null;

    private TargettableEntity currentClosestREntity = null;
    private TargettableEntity previousClosestREntity = null;

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
        ignoreLayers = ~(ignoreLayers);
        groundLayer = ~LayerMask.NameToLayer("Ground");
    }

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
            Physics.Raycast(leftHandPosition.position, leftHandPosition.forward, out hit, maxDistance, ignoreLayers);
            return hit;
        }
        else
        {
            RaycastHit hit;
            Physics.Raycast(rightHandPosition.position, rightHandPosition.forward, out hit, maxDistance, ignoreLayers);
            return hit;
        }
    }
    public RaycastHit RaycastFromHandToGround(LeftRight hand, float maxDistance)
    {

        if (hand == 0)
        {
            RaycastHit hit;
            Physics.Raycast(leftHandPosition.position, leftHandPosition.forward, out hit, maxDistance, groundLayer);
            return hit;
        }
        else
        {
            RaycastHit hit;
            Physics.Raycast(rightHandPosition.position, rightHandPosition.forward, out hit, maxDistance, groundLayer);
            return hit;
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
