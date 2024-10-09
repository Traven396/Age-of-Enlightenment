using FoxheadDev.GestureDetection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiseGesture : MonoBehaviour, IMovement
{
    [SerializeField] private float yVelThreshold = 1;
    [SerializeField] private float xDotThreshold = 1;

    public bool GesturePerformed(HandPhysicsTracker _handPhysicsTracker, out Vector3 direction)
    {
        direction = Vector3.zero;
        if (_handPhysicsTracker.Velocity.y > yVelThreshold && _handPhysicsTracker.SelfSpaceVelocity.x < -xDotThreshold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
