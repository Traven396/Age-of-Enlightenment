using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashGesture : MonoBehaviour, IMovement
{
    private Vector3 startPos;
    [SerializeField] private float distanceRequired = .3f;
    public bool GesturePerformed(GestureManager _gestureManager, out Vector3 direction)
    {
        direction = Vector3.zero;
        if (_gestureManager.dotProdY < -1 && _gestureManager.dotProdY > -1.5)
        {
            startPos = _gestureManager.transform.position;
        }
        if (_gestureManager.dotProdY < -1.5 && _gestureManager.angularDotX > 1.5 && Vector3.Distance(_gestureManager.transform.position, startPos) > distanceRequired)
        {
            
            return true;
        }
        return false;
    }
}
