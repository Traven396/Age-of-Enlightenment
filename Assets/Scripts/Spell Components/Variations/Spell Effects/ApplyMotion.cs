using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyMotion : MonoBehaviour
{
    public ForceMode forceType;
    public float forceMultiplier = 1;
    public virtual void Cast(Rigidbody target, Vector3 force) {
        target.AddForce(force * forceMultiplier, forceType);
    }

}
