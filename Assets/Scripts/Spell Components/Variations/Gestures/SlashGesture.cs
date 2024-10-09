using FoxheadDev.GestureDetection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashGesture : MonoBehaviour, IMovement
{
    private Vector3 startPos;
    [SerializeField] private float distanceRequired = .3f;
     

    public bool GesturePerformed(HandPhysicsTracker _handPhysicsTracker, out Vector3 direction)
    {
        direction = Vector3.zero;
        if (_handPhysicsTracker.SelfSpaceVelocity.y < -1 && _handPhysicsTracker.SelfSpaceVelocity.y > -1.5)
        {
            startPos = _handPhysicsTracker.SelfHandTransform.position;
        }
        if (_handPhysicsTracker.SelfSpaceVelocity.y < -1.5 && _handPhysicsTracker.AngularVelocity.x > 1.5 && Vector3.Distance(_handPhysicsTracker.SelfHandTransform.position, startPos) > distanceRequired)
        {

            return true;
        }
        return false;
    }
}
