using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingUpGesture : MonoBehaviour, IMovement
{
    [SerializeField] private float yVelThreshold = 2;
    public bool GesturePerformed(GestureManager _gestureManager, out Vector3 direction)
    {
        direction = Vector3.zero;
        if (_gestureManager.dotProdWorldY > yVelThreshold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}