using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiseGesture : MonoBehaviour, IMovement
{
    [SerializeField] private float yVelThreshold = 1;
    [SerializeField] private float xDotThreshold = 1;
    public bool GesturePerformed(GestureManager _gestureManager, out Vector3 direction)
    {
        direction = Vector3.zero;
        if (_gestureManager.currVel.y > yVelThreshold && _gestureManager.dotProdX < -xDotThreshold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
