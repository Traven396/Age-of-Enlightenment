using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "JointSettings")]
public class JointSettings : ScriptableObject
{
    [Header("Most Used Settings")]

    [Tooltip("If true then X,Y,Z will all use X values.")]
    public bool XMaster = true;
    public AOEJointDrive XDrive;
    public AOEAngularJointDrive SlerpDrive;
    public JointApply ApplyMode = JointApply.PositionAndRotation;


    [Header("Other Settings")]
    public bool EnableCollision = false;
    public bool EnablePreprocessing = true;

    public float BreakForce = float.PositiveInfinity;
    public float BreakTorque = float.PositiveInfinity;

    public float MassScale = 1f;
    public float ConnectedMassScale = 1f;

    public JointProjectionMode ProjectionMode = JointProjectionMode.None;
    public float ProjectionDistance = .1f;
    public float ProjectionAngle = 180f;


    [Header("Tweaks")]
    [Tooltip("If true, Damper is ignored and calculated to be critical damped by mass")]
    public bool CriticalDampPosition;
    [Tooltip("If using critical damping, do we use the connected body or the anchor body.")]
    public bool DampConnectedBody;

    public RotationDriveMode RotationDriveMode = RotationDriveMode.Slerp;

    [Header("Linear Drives")]
    public AOEJointDrive YDrive;
    public AOEJointDrive ZDrive;


    [Header("Angular Drives")]
    public AOEAngularJointDrive AngularXDrive;
    public AOEAngularJointDrive AngularYZDrive;


    [Header("Linear Limits")]
    public AOESoftJointLimit LinearLimit;
    public AOESoftJointLimitSpring LinearLimitSpring;

    public ConfigurableJointMotion XMotion = ConfigurableJointMotion.Free;
    public ConfigurableJointMotion YMotion = ConfigurableJointMotion.Free;
    public ConfigurableJointMotion ZMotion = ConfigurableJointMotion.Free;

    [Header("Angular Limits")]
    public ConfigurableJointMotion AngularXMotion = ConfigurableJointMotion.Free;
    public ConfigurableJointMotion AngularYMotion = ConfigurableJointMotion.Free;
    public ConfigurableJointMotion AngularZMotion = ConfigurableJointMotion.Free;

    public AOESoftJointLimit LowAngularXLimit;
    public AOESoftJointLimit HighAngularXLimit;
    public AOESoftJointLimitSpring AngularXLimitSpring;

    public AOESoftJointLimit AngularYLimit;
    public AOESoftJointLimit AngularZLimit;
    public AOESoftJointLimitSpring AngularYZLimitSpring;




    public void ApplySettings(ConfigurableJoint joint)
    {
        joint.xMotion = XMotion;
        joint.yMotion = YMotion;
        joint.zMotion = ZMotion;

        joint.angularXMotion = AngularXMotion;
        joint.angularYMotion = AngularYMotion;
        joint.angularZMotion = AngularZMotion;

        joint.linearLimitSpring = LinearLimitSpring.CreateSpring();
        joint.linearLimit = LinearLimit.CreateJointLimit();

        joint.angularXLimitSpring = AngularXLimitSpring.CreateSpring();
        joint.lowAngularXLimit = LowAngularXLimit.CreateJointLimit();
        joint.highAngularXLimit = HighAngularXLimit.CreateJointLimit();

        joint.angularYZLimitSpring = AngularYZLimitSpring.CreateSpring();
        joint.angularYLimit = AngularYLimit.CreateJointLimit();
        joint.angularZLimit = AngularZLimit.CreateJointLimit();

        var mass = 0f;
        if (CriticalDampPosition)
        {
            if (DampConnectedBody)
            {
                mass = joint.connectedBody.mass;
            }
            else
            {
                mass = joint.gameObject.GetComponent<Rigidbody>().mass;
            }
        }

        if (ApplyMode == JointApply.PositionAndRotation || ApplyMode == JointApply.Position)
        {
            joint.xDrive = XDrive.CreateJointDrive(CriticalDampPosition, mass);
            if (XMaster)
            {
                joint.yDrive = joint.xDrive;
                joint.zDrive = joint.xDrive;
            }
            else
            {
                joint.yDrive = YDrive.CreateJointDrive(CriticalDampPosition, mass);
                joint.zDrive = ZDrive.CreateJointDrive(CriticalDampPosition, mass);
            }
        }

        if (ApplyMode == JointApply.Rotation || ApplyMode == JointApply.PositionAndRotation)
        {
            joint.rotationDriveMode = RotationDriveMode;
            joint.slerpDrive = SlerpDrive.CreateJointDrive();
            joint.angularXDrive = AngularXDrive.CreateJointDrive();
            joint.angularYZDrive = AngularYZDrive.CreateJointDrive();
        }

        joint.enableCollision = EnableCollision;
        joint.enablePreprocessing = EnablePreprocessing;
        joint.breakForce = BreakForce;
        joint.breakTorque = BreakTorque;
        joint.massScale = MassScale;
        joint.connectedMassScale = ConnectedMassScale;
        joint.projectionMode = ProjectionMode;
        joint.projectionDistance = ProjectionDistance;
        joint.projectionAngle = ProjectionAngle;
    }
}

[Serializable]
public class AOEJointDrive
{
    public float Spring = 1500;
    public float Damper = 100;
    public float MaxForce = 750;

    public JointDrive CreateJointDrive(bool criticalDamp, float mass)
    {
        var drive = new JointDrive();
        drive.positionSpring = Spring;
        if (criticalDamp)
        {
            drive.positionDamper = 2 * Mathf.Sqrt(Spring * mass);
        }
        else
        {
            drive.positionDamper = Damper;
        }
        drive.maximumForce = MaxForce;
        return drive;
    }
}

[Serializable]
public class AOEAngularJointDrive
{
    public float Spring = 200;
    public float Damper = 10;
    public float MaxForce = 50;

    public JointDrive CreateJointDrive()
    {
        var drive = new JointDrive();
        drive.positionSpring = Spring;
        drive.positionDamper = Damper;
        drive.maximumForce = MaxForce;
        return drive;
    }
}

[Serializable]
public class AOESoftJointLimit
{
    public float Limit;
    public float Bounciness;
    public float ContactDistance;

    public SoftJointLimit CreateJointLimit()
    {
        var limit = new SoftJointLimit();

        limit.limit = Limit;
        limit.bounciness = Bounciness;
        limit.contactDistance = ContactDistance;
        return limit;
    }
}

[Serializable]
public class AOESoftJointLimitSpring
{
    public float Spring;
    public float Damper;

    public SoftJointLimitSpring CreateSpring()
    {
        var spring = new SoftJointLimitSpring();
        spring.spring = Spring;
        spring.damper = Damper;
        return spring;
    }
}

public enum JointApply
{
    PositionAndRotation,
    Position,
    Rotation
}
