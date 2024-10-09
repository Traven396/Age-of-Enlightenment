using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FoxheadDev.GestureDetection;

public interface IMovement
{
    bool GesturePerformed(HandPhysicsTracker _handPhysicsTracker, out Vector3 direction);
}
