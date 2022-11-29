using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowGesture : MonoBehaviour, IMovement
{
    public bool GestureOverride = false;
    public float requiredSpeed = 1f;
    public bool GesturePerformed(GestureManager _gestureManager, out Vector3 direction)
    {
        direction = Vector3.zero;
        if (GestureOverride)
            return true;
        
        if (_gestureManager.angX > 145 && _gestureManager.dotProdX < -requiredSpeed)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
