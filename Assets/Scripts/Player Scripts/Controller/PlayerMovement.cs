using UnityEngine;
using System.Collections;
using Unity.XR.CoreUtils;

public class PlayerMovement : MonoBehaviour {

	//Collider variables;
	[Header("Options :")]
	[Range(0f, 1f)] public float stepHeightRatio = 0.25f;
    public CapsuleCollider col;
    public Rigidbody rig;
    public Transform NeckPivot;
	public Transform CameraRig;
    public CameraRig _CameraRigCode;
    public Transform LeftShoulder, RightShoulder;
    public Transform LeftController;
    public Transform RightController;
    public PhysicalHands LeftHandCode;
    public PhysicalHands RightHandCode;
    [Header("Size Options :")]
	[SerializeField] public float colliderHeight = 2f;
	[SerializeField] public float minColliderHeight = 0;
	[SerializeField] public float maxColliderHeight = 2;
	[SerializeField] public float colliderThickness = 1f;
	[SerializeField] public Vector3 colliderOffset = Vector3.zero;
	[SerializeField] public float MaxLean = .3f;
    public float DefaultArmLength = .75f;

    //Sensor variables;
    [Header("Sensor Options :")]
	private int currentLayer;
	[SerializeField] public bool isInDebugMode = false;

	bool isGrounded = false;
	bool IsUsingExtendedSensorRange  = true;
	float baseSensorRange = 0f;
    bool isCrouching = false;
	Vector3 currentGroundAdjustmentVelocity = Vector3.zero;

	
	Sensor sensor;
	XROrigin xrOrigin;

    private ConfigurableJoint _leftJoint, _rightJoint;

    //Changed from AWAKE to OnENABLE
    void OnEnable()
	{
		Setup();
        //SetupArms();

		sensor = new Sensor(this.transform, col);
		RecalculateColliderDimensions();
		RecalibrateSensor();
	}

	void Reset () {
		Setup();
	}

	void OnValidate()
	{
		if(gameObject.activeInHierarchy)
			RecalculateColliderDimensions();
	}

    #region Setup Stuff

    void Setup()
    {
        col = GetComponent<CapsuleCollider>();
        rig = GetComponent<Rigidbody>();
        xrOrigin = FindObjectOfType<XROrigin>();

        rig.freezeRotation = true;
        rig.useGravity = false;
    }
    private void SetupArms()
    {
        if (LeftShoulder && LeftHandCode.rb)
        {
            if (!_leftJoint)
            {
                _leftJoint = rig.gameObject.AddComponent<ConfigurableJoint>();
                _leftJoint.autoConfigureConnectedAnchor = false;
                _leftJoint.connectedAnchor = Vector3.zero;
                _leftJoint.connectedBody = LeftHandCode.rb;
                _leftJoint.anchor = rig.transform.InverseTransformPoint(LeftShoulder.position);
                _leftJoint.xMotion = ConfigurableJointMotion.Limited;
                _leftJoint.yMotion = ConfigurableJointMotion.Limited;
                _leftJoint.zMotion = ConfigurableJointMotion.Limited;
            }

            var limit = _leftJoint.linearLimit;
            limit.limit = DefaultArmLength;
            _leftJoint.linearLimit = limit;

        }

        if (RightShoulder && RightHandCode.rb)
        {
            if (!_rightJoint)
            {
                _rightJoint = rig.gameObject.AddComponent<ConfigurableJoint>();

                _rightJoint.autoConfigureConnectedAnchor = false;
                _rightJoint.connectedAnchor = Vector3.zero;
                _rightJoint.connectedBody = RightHandCode.rb;
                _rightJoint.anchor = rig.transform.InverseTransformPoint(RightShoulder.position);
                _rightJoint.xMotion = ConfigurableJointMotion.Limited;
                _rightJoint.yMotion = ConfigurableJointMotion.Limited;
                _rightJoint.zMotion = ConfigurableJointMotion.Limited;
            }


            var limit = _rightJoint.linearLimit;
            limit.limit = DefaultArmLength;
            _rightJoint.linearLimit = limit;

        }

        SetArmLength(DefaultArmLength);
    } 
    #endregion

    void LateUpdate()
	{
		if(isInDebugMode)
			sensor.DrawDebug();
	}

    #region Miscellaneous
    
    public void UpdatePlayerHeight()
    {

        /*
         * 
         * For right now I wont be updating the players height because it is causing a lot of issues and 
         * I would rather keep my sanity than spend another 3 weeks trying to get everything working
         * 
         */

        //var height = Mathf.Clamp(xrOrigin.CameraInOriginSpaceHeight, minColliderHeight, maxColliderHeight);
        //col.height = height;

        //var pelvisOffset = .15f;

        //var dir = -xrOrigin.Camera.transform.forward;
        //dir.y = 0f;
        //dir.Normalize();

        //var pelvisTarget = rig.transform.InverseTransformDirection(dir * pelvisOffset);

        ////This has potential to cause issues since im hard setting it. But from my testing they are very rare
        ////pelvisTarget.y = Mathf.Max(LocoBall.position.y - BodyRB.position.y, 0f);
        //pelvisTarget.y = 0;
        //col.center = Vector3.MoveTowards(col.center, pelvisTarget, Time.deltaTime);

        //var playerHeight = xrOrigin.Camera.transform.localPosition.y;
        //if(playerHeight <= -.3f && !isCrouching)
        //{
        //    col.height = (maxColliderHeight / 2f);
        //    col.center = col.center + new Vector3(0f, stepHeightRatio * col.height / 2f, 0f);
        //    isCrouching = true;
        //}
        //else if(playerHeight >= -.3f && isCrouching)
        //{
        //    col.height = (maxColliderHeight);
        //    col.center = col.center + new Vector3(0f, stepHeightRatio * col.height / 2f, 0f);
        //    isCrouching = false;
        //}
    }
    public void LimitHead()
    {
        var delta = NeckPivot.transform.position - col.transform.position;
        delta.y = 0;

        if (delta.sqrMagnitude < .01f || delta.magnitude < MaxLean) return;

        var allowedPosition = col.transform.position + delta.normalized * MaxLean;
        var difference = allowedPosition - NeckPivot.transform.position;
        difference.y = 0f;
        CameraRig.transform.position += difference;
    }
    Vector3 GetColliderCenter()
    {
        if (col == null)
            Setup();

        return col.bounds.center;
    }
    private void SetArmLength(float _length)
    {
        LeftHandCode.ArmLength = _length;
        RightHandCode.ArmLength = _length;
    }
    public void RotatePlayer(float angleAmount)
    {
        Quaternion currentRot = rig.rotation;
        Quaternion q = Quaternion.AngleAxis(angleAmount, Vector3.up);
        rig.MoveRotation(currentRot * q);
    }
    #endregion

    #region Recalibration Stuff
    public void RecalculateColliderDimensions()
    {
        col.height = colliderHeight;
        col.center = colliderOffset * colliderHeight;
        col.radius = colliderThickness / 2f;

        col.center = col.center + new Vector3(0f, stepHeightRatio * col.height / 2f, 0f);
        col.height *= (1f - stepHeightRatio);

        if (col.height / 2f < col.radius)
            col.radius = col.height / 2f;

        if (sensor != null)
            RecalibrateSensor();
    }
    void RecalibrateSensor()
    {
        sensor.SetCastDirection(Sensor.CastDirection.Down);

        RecalculateSensorLayerMask();

        RecalculateLength();

        sensor.isInDebugMode = isInDebugMode;
    }

    public void RecalculateLength()
    {
        sensor.SetCastOrigin(GetColliderCenter());

        //Calculate and set sensor length;
        float _length = 0f;
        _length += (col.height * (1f - stepHeightRatio)) * 0.5f;
        _length += col.height * stepHeightRatio;
        baseSensorRange = _length * (1f + .001f) * transform.localScale.x;
        sensor.castLength = _length * transform.localScale.x;
    }

    //Recalculate sensor layermask based on current physics settings;
    void RecalculateSensorLayerMask()
    {
        int _layerMask = 0;
        int _objectLayer = this.gameObject.layer;

        //Calculate layermask;
        for (int i = 0; i < 32; i++)
        {
            if (!Physics.GetIgnoreLayerCollision(_objectLayer, i))
                _layerMask = _layerMask | (1 << i);
        }

        //Make sure that the calculated layermask does not include the 'Ignore Raycast' layer;
        if (_layerMask == (_layerMask | (1 << LayerMask.NameToLayer("Ignore Raycast"))))
        {
            _layerMask ^= (1 << LayerMask.NameToLayer("Ignore Raycast"));
        }

        //Set sensor layermask;
        sensor.layermask = _layerMask;

        //Save current layer;
        currentLayer = _objectLayer;
    }
    #endregion

    #region Height Adjustment
    //Check if mover is grounded;
    //Store all relevant collision information for later;
    //Calculate necessary adjustment velocity to keep the correct distance to the ground;
    void Check()
    {
        //Reset ground adjustment velocity;
        currentGroundAdjustmentVelocity = Vector3.zero;

        //Set sensor length;
        if (IsUsingExtendedSensorRange)
            sensor.castLength = baseSensorRange + (col.height * transform.localScale.x) * stepHeightRatio;
        else
            sensor.castLength = baseSensorRange;

        sensor.Cast();

        //If sensor has not detected anything, set flags and return;
        if (!sensor.HasDetectedHit())
        {
            isGrounded = false;
            return;
        }

        //Set flags for ground detection;
        isGrounded = true;

        //Get distance that sensor ray reached;
        float _distance = sensor.GetDistance();



        //THIS MIGHT BE WHERE I HAVE SOME ISSUES. IF EVER THE PLAYER IS WALKING UP THINGS THEY SHOULD THIS COULD BE THE ISSUE
        //I AM SETTING THE MAX HEIGHT IN THE CODE, MEANING IT ASSUMES THE PLAYER IS STANDING UP ALWAYS


        //Calculate how much mover needs to be moved up or down;
        float _upperLimit = ((maxColliderHeight * transform.localScale.x) * (1f - stepHeightRatio)) * 0.5f;
        float _middle = _upperLimit + (maxColliderHeight * transform.localScale.x) * stepHeightRatio;
        float _distanceToGo = _middle - _distance;

        //Set new ground adjustment velocity for the next frame;
        currentGroundAdjustmentVelocity = transform.up * (_distanceToGo / Time.fixedDeltaTime);
    }

    //Check if mover is grounded;
    public void CheckForGround()
    {
        //Check if object layer has been changed since last frame;
        //If so, recalculate sensor layer mask;
        if (currentLayer != this.gameObject.layer)
            RecalculateSensorLayerMask();

        Check();
    }

    //Set mover velocity;
    public void SetVelocity(Vector3 _velocity)
    {
        rig.velocity = _velocity + currentGroundAdjustmentVelocity;
    }

    //Returns 'true' if mover is touching ground and the angle between hte 'up' vector and ground normal is not too steep (e.g., angle < slope_limit);
    public bool IsGrounded()
    {
        return isGrounded;
    } 
    #endregion

    #region Setters
    //Setters;

    //Set whether sensor range should be extended;
    public void SetExtendSensorRange(bool _isExtended)
    {
        IsUsingExtendedSensorRange = _isExtended;
    }

    //Set height of collider;
    public void SetColliderHeight(float _newColliderHeight)
    {
        if (col.height == _newColliderHeight)
            return;

        col.height = _newColliderHeight;
        RecalculateColliderDimensions();
    }

    //Set acceptable step height;
    public void SetStepHeightRatio(float _newStepHeightRatio)
    {
        _newStepHeightRatio = Mathf.Clamp(_newStepHeightRatio, 0f, 1f);
        stepHeightRatio = _newStepHeightRatio;
        RecalculateColliderDimensions();
    }
    #endregion

    #region Getters
    //Getters;

    public Vector3 GetGroundNormal()
    {
        return sensor.GetNormal();
    }

    public Vector3 GetGroundPoint()
    {
        return sensor.GetPosition();
    }

    public Collider GetGroundCollider()
    {
        return sensor.GetCollider();
    }

    #endregion
}
