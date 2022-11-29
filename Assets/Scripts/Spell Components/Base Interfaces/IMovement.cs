using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovement
{
    bool GesturePerformed(GestureManager _gestureManager, out Vector3 direction);
}
