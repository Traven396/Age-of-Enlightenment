using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandStrength : MonoBehaviour
{
    [Header("Debug")]
    public JointSettings CurrentSettings;
    public bool LogStrengthChanges;

    [Tooltip("If true will update the joint every update - useful for tweaking HVRJointSettings in play mode.")]
    public bool AlwaysUpdateJoint;

    public JointSettings JointSettings { get; private set; }

    public ConfigurableJoint Joint { get; set; }

    public bool Stopped { get; private set; }

    private JointDrive _stoppedDrive;

    protected virtual void Awake()
    {
        _stoppedDrive = new JointDrive();
        _stoppedDrive.maximumForce = 0f;
        _stoppedDrive.positionSpring = 0f;
        _stoppedDrive.positionDamper = 0f;

        if (AlwaysUpdateJoint)
        {
            Debug.LogWarning($"AlwaysUpdateJoint is enabled on {name}, was this intentional?");
        }
    }

    public void Initialize(JointSettings defaultSettings)
    {
        JointSettings = defaultSettings;
        UpdateJoint();
    }

    protected virtual void FixedUpdate()
    {
        if (AlwaysUpdateJoint)
        {
            UpdateJoint();
        }
    }

    protected virtual void UpdateJoint()
    {
        if (Stopped)
            return;

        UpdateStrength(JointSettings);
    }

    protected virtual void UpdateStrength(JointSettings settings)
    {
        if (settings)
            settings.ApplySettings(Joint);

        CurrentSettings = settings;

        if (LogStrengthChanges && settings)
        {
            Debug.Log($"{settings.name} applied.");
        }
    }

    public virtual void Stop()
    {
        Stopped = true;
        Joint.xDrive = Joint.yDrive = Joint.zDrive = Joint.angularXDrive = Joint.angularYZDrive = Joint.slerpDrive = _stoppedDrive;
    }

    public virtual void Restart()
    {
        Stopped = false;
        UpdateJoint();
    }
}

