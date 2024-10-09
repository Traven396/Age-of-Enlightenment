using FoxheadDev.GestureDetection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingUpGesture : MonoBehaviour, IMovement
{
    [SerializeField] private float yVelThreshold = 2;

    public bool GesturePerformed(HandPhysicsTracker _handPhysicsTracker, out Vector3 direction)
    {
        direction = Vector3.zero;
        if (_handPhysicsTracker.ViewSpaceVelocity.y > yVelThreshold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
