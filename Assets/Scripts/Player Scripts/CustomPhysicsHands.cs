using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPhysicsHands : MonoBehaviour
{
    public ConfigurableJoint joint;
    public Transform target;
    private Quaternion startRotation;

    private void Start()
    {
        startRotation = transform.rotation;
        joint.configuredInWorldSpace = true;
    }
    void Update()
    {
        
        SetRotation(joint, target.rotation, startRotation);
        joint.connectedAnchor = target.position - joint.connectedBody.position;
    }
    void SetRotation(ConfigurableJoint joint, Quaternion targetRotation, Quaternion startRotation)
    {
        var right = joint.axis;
        var forward = Vector3.Cross(joint.axis, joint.secondaryAxis).normalized;
        var up = Vector3.Cross(forward, right).normalized;
        Quaternion worldToJointSpace = Quaternion.LookRotation(forward, up);
        Quaternion resultRotation = Quaternion.Inverse(worldToJointSpace);

        resultRotation *= startRotation * Quaternion.Inverse(targetRotation);
        resultRotation *= worldToJointSpace;
        joint.targetRotation = resultRotation;
    }
}
