using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FoxheadDev.GestureDetection;

public class ThrowGesture : MonoBehaviour, IMovement
{
    public bool GestureOverride = false;
    public float requiredSpeed = 1f;
    public bool GesturePerformed(HandPhysicsTracker _handPhysicsTracker, out Vector3 direction)
    {
        direction = Vector3.zero;
        if (GestureOverride)
            return true;
        
        if (_handPhysicsTracker.SelfSpaceVelocity.x < -requiredSpeed)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
