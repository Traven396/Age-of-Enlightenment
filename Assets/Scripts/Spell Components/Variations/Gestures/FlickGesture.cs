using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickGesture : MonoBehaviour, IMovement
{
    public float requiredFlickSpeed = 2f;
    private Vector3 flickStartPos;
    public bool GesturePerformed(GestureManager _gestureManager, out Vector3 direction)
    {
        //if(_gestureManager.GetVelocity().magnitude < requiredFlickSpeed && _gestureManager.GetVelocity().magnitude > (requiredFlickSpeed * .9) && flickStartPos == Vector3.zero)
        //{
        //    flickStartPos = _gestureManager.transform.position;
        //}
        //if(_gestureManager.GetVelocity().magnitude > requiredFlickSpeed)
        //{
        //    direction = (_gestureManager.transform.position - flickStartPos).normalized;
        //    flickStartPos = Vector3.zero;
        //    return true;
        //}
        if(_gestureManager.currVel.magnitude > requiredFlickSpeed)
        {
            direction = _gestureManager.currVel.normalized;
            return true;
        }
        direction = Vector3.zero;
        return false;
    }
}
