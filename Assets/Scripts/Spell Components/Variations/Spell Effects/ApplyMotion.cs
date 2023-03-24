using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyMotion : MonoBehaviour
{
    public ForceMode forceType;
    public float forceMultiplier = 1;
    public bool canReflect = false;

    private LayerMask playerLayer, enemyLayer;
    private void Start()
    {
        playerLayer = LayerMask.NameToLayer("PlayerProjectile");
        enemyLayer = LayerMask.NameToLayer("PlayerProjectile");
    }
    public void Cast(Rigidbody target, Vector3 direction) {
        target.AddForce(direction * forceMultiplier, forceType);
        if (canReflect)
        {
            if(target.gameObject.layer == enemyLayer)
            {
                target.gameObject.layer = playerLayer;
            }
        }
    }

    public void ChangeMotion(Transform target, Vector3 linerVelToBe, Vector3 angularVelToBe)
    {
        Rigidbody targetRb = target.GetComponent<Rigidbody>();
        if (targetRb == null)
        {
            return;
        }
        targetRb.velocity = linerVelToBe;
        targetRb.angularVelocity = angularVelToBe;
    }

}
