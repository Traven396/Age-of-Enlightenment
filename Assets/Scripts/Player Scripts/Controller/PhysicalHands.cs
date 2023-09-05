using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicalHands : MonoBehaviour
{
    [Header("Required Components")]
    //The target the hand will move towards
    public Transform Target;

    //More locations for more accurate travel
    public Transform Shoulder;
    public Rigidbody ParentBody;
    [Header("Offsets")]
    public Vector3 positionalOffset;
    public Vector3 rotationalOffset;

    [Header("Settings")]
    public JointSettings JointSettings;
    //The maximum length the hand goes forward
    [HideInInspector] public float ArmLength = .75f;
    public int SolverIterations = 10;
    public int SolverVelocityIterations = 10;
    public float ReturnSpeed = 5;
    public float MaxTargetDistance = .8f;
    public float InertiaTensor = .02f;

    private bool isReturning;
    private Vector3 previousPosition;
    private Quaternion previousRotation;
    public Rigidbody rb { get; private set; }
    public ConfigurableJoint Joint { get; set; }
    public HandStrength StrengthHandler;
    private Quaternion _jointOffset;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        SetupJoint();

        rb.inertiaTensorRotation = Quaternion.identity;
        rb.inertiaTensor = new Vector3(InertiaTensor, InertiaTensor, InertiaTensor);
        rb.maxAngularVelocity = 150f;
        rb.solverIterations = SolverIterations;
        rb.solverVelocityIterations = SolverVelocityIterations;

        if (ReturnSpeed < .1f)
            ReturnSpeed = 5f;

        StrengthHandler.Joint = Joint;
        StrengthHandler.Initialize(JointSettings);
    }

    private void Start()
    {
        previousPosition = Target.position;
        previousRotation = Target.rotation;
    }
    private void OnEnable()
    {
        _jointOffset = Quaternion.Inverse(Quaternion.Inverse(ParentBody.rotation) * transform.rotation);
    }
    private void SetupJoint()
    {
        Joint = ParentBody.transform.gameObject.AddComponent<ConfigurableJoint>();
        Joint.autoConfigureConnectedAnchor = false;
        Joint.connectedBody = rb;
        Joint.connectedAnchor = Vector3.zero;
        Joint.anchor = Vector3.zero;
        Joint.rotationDriveMode = RotationDriveMode.Slerp;
    }
    private void FixedUpdate()
    {
        UpdateAnchor();

        Joint.targetRotation = Quaternion.Inverse(ParentBody.rotation) * Target.rotation * _jointOffset * Quaternion.Euler(rotationalOffset);

        UpdateTargetVelocity();
    }
    public void UpdateTargetVelocity()
    {
        var local = ParentBody.transform.InverseTransformPoint(Target.position);

        var velocity = (local - previousPosition) / Time.fixedDeltaTime;
        previousPosition = local;
        Joint.targetVelocity = velocity;

        var angularVelocity = AngularVelocity(Target.rotation, previousRotation);
        Joint.targetAngularVelocity = Quaternion.Inverse(ParentBody.transform.rotation) * angularVelocity;

        previousRotation = Target.rotation;
    }
    private void UpdateAnchor()
    {
        //Moves the joint anchor every frame
        var localTargetPosition = ParentBody.transform.InverseTransformPoint(Target.position);

        if (Shoulder)
        {
            var localAnchor = ParentBody.transform.InverseTransformPoint(Shoulder.position);
            var dir = localTargetPosition - localAnchor;
            dir = Vector3.ClampMagnitude(dir, ArmLength);

            var point = localAnchor + dir + positionalOffset;
            Joint.targetPosition = point;
        }
        else
        {
            Joint.targetPosition = localTargetPosition + positionalOffset;
        }
    }
    public Vector3 AngularVelocity(Quaternion current, Quaternion previous)
    {
        var deltaRotation = current * Quaternion.Inverse(previous);
        if (deltaRotation.w < 0)
        {
            deltaRotation.x = -deltaRotation.x;
            deltaRotation.y = -deltaRotation.y;
            deltaRotation.z = -deltaRotation.z;
            deltaRotation.w = -deltaRotation.w;
        }

        deltaRotation.ToAngleAxis(out var angle, out var axis);
        angle *= Mathf.Deg2Rad;
        return axis * (angle / Time.fixedDeltaTime);
    }

}
