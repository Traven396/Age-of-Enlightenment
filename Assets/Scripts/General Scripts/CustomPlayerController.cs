using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class CustomPlayerController : MonoBehaviour
{
    [Header("Behaviour Options")]

    public float movementThreshold = 0.1f;
    public float jumpForce = 250.0f;
    public bool checkForGroundOnJump = true;
    public LayerMask groundLayers;
    public float slopeCheckerThreshold = .5f;
    public float groundCheckerThreshold = .2f;
    [Range(1, 89)]
    public float maxSlopeAngle = 53;
    [Range(0, 1)]
    public float frictionAgainstFloor = .3f;
    [Tooltip("Speed multiplier based on slope angle")]
    public AnimationCurve speedMultiplierOnAngle = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    public float canSlideMultiplierCurve = 0.06f, cantSlideMultiplierCurve = .04f, climbingStairsMultiplierCurve = .6f;
    public float dampSpeedDown = 0.1f, dampSpeedUp = .2f;

    [Space(10f)]

    [Header("Input Buttons")]
    public InputActionReference MoveJoystick;
    public InputActionReference JumpButton;

    [Space(10f)]

    [Header("Physical Body Parts")]
    public XROrigin xrOrigin;
    public Vector3 capsuleCenter = new Vector3(0, 1, 0);
    public CapsuleCollider MainBody;
    public Rigidbody BodyRB;
    public Transform NeckPivot, LeftShoulder, RightShoulder;
    public Transform CameraRig;
    public Transform LeftController;
    public Transform RightController;
    public PhysicalHands LeftHandCode;
    public PhysicalHands RightHandCode;

    [Space(10f)]    

    [Header("Body Collider Options")]
    public float minHeight = 0;
    public float maxHeight = 2f;
    public float capsuleRadius = 0.3f;
    public CapsuleDirection capsuleDirection = CapsuleDirection.YAxis;

    [Space(10f)]
    [Header("Player Size Options")]
    public float EyeLevel = 1.66f;
    public float PlayerWaistHeight = 1f;
    public float MaxLean = .3f;
    public float DefaultArmLength = .75f;

    private bool isGrounded;
    private bool buttonPressed;
    private Vector3 _cameraStartingPosition;
    private Vector3 prevPos, previousVelocity;
    private ConfigurableJoint _leftJoint, _rightJoint;
    

    //Gets immediately set to the stat in Player file
    private float speed;

    private bool _waitingForCameraMovement, isTouchingSlope, isTouchingStep;
    private Vector3 groundNormal, prevGroundNormal, forward, globalForward, reactionForward, down, globalDown, reactionGlobalDown, currVelocity;
    private float targetAngle, currentSurfaceAngle;


    public enum CapsuleDirection
    {
        XAxis,
        YAxis,
        ZAxis
    }

    void OnEnable()
    {
        BodyRB.constraints = RigidbodyConstraints.FreezeRotation;
        MainBody.direction = (int)capsuleDirection;

        MainBody.radius = capsuleRadius;
        MainBody.center = capsuleCenter;
        //MainBody.height = capsuleHeight;

        speed = Player.Instance.playerMoveSpeed;

        _cameraStartingPosition = xrOrigin.Camera.transform.localPosition;


        Player.StatsChanged += SetSpeed;
    }
    private void OnDisable()
    {
        Player.StatsChanged -= SetSpeed;
    }

    void SetSpeed()
    {
        speed = Player.Instance.playerMoveSpeed;
    }
    private void Awake()
    {
        SetupArms();
    }

    private void SetupArms()
    {
        if (LeftShoulder && LeftHandCode.rb)
        {
            if (!_leftJoint)
            {
                _leftJoint = BodyRB.gameObject.AddComponent<ConfigurableJoint>();
                _leftJoint.autoConfigureConnectedAnchor = false;
                _leftJoint.connectedAnchor = Vector3.zero;
                _leftJoint.connectedBody = LeftHandCode.rb;
                _leftJoint.anchor = BodyRB.transform.InverseTransformPoint(LeftShoulder.position);
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
                _rightJoint = BodyRB.gameObject.AddComponent<ConfigurableJoint>();

                _rightJoint.autoConfigureConnectedAnchor = false;
                _rightJoint.connectedAnchor = Vector3.zero;
                _rightJoint.connectedBody = RightHandCode.rb;
                _rightJoint.anchor = BodyRB.transform.InverseTransformPoint(RightShoulder.position);
                _rightJoint.xMotion = ConfigurableJointMotion.Limited;
                _rightJoint.yMotion = ConfigurableJointMotion.Limited;
                _rightJoint.zMotion = ConfigurableJointMotion.Limited;
            }


            var limit = _rightJoint.linearLimit;
            limit.limit = DefaultArmLength;
            _rightJoint.linearLimit = limit;

        }
    }

    void FixedUpdate()
    {
        CheckSlopeAndDirections();


        if (_waitingForCameraMovement)
            CheckCameraMovement();
        UpdateCollider();
        
        UpdateMovement();
        UpdateJump();
        SafetyNet();

        prevPos = transform.position;
    }

    private void CheckCameraMovement()
    {
        if (Vector3.Distance(_cameraStartingPosition, xrOrigin.Camera.transform.localPosition) < .05f)
        {
            return;
        }

        var delta = xrOrigin.Camera.transform.position - MainBody.transform.position;
        delta.y = 0f;
        CameraRig.transform.position -= delta;
        _waitingForCameraMovement = false;
    }
    private void UpdateMovement()
    {
        Vector2 primary2dValue = MoveJoystick.action.ReadValue<Vector2>();

        //if (primary2dValue != Vector2.zero)
        //{

        //    Vector3 right = Camera.main.transform.TransformDirection(Vector3.right);
        //    Vector3 forward = Camera.main.transform.TransformDirection(Vector3.forward);

        //    //forward = Vector3.ProjectOnPlane(forward, transform.up);
        //    //right = Vector3.ProjectOnPlane(right, transform.up);
        //    forward.y = 0;
        //    right.y = 0;

        //    var movementAxis = right * primary2dValue.x + forward * primary2dValue.y;
        //    movementAxis.Normalize();
        //    movementAxis *= speed * Time.deltaTime;
        //    AddVelocity(movementAxis);
        //}
        if(primary2dValue.magnitude > movementThreshold)
        {
            targetAngle = Mathf.Atan2(primary2dValue.x, primary2dValue.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;

            BodyRB.velocity = Vector3.SmoothDamp(BodyRB.velocity, forward * speed, ref currVelocity, dampSpeedUp);
        }
        else
        {
            BodyRB.velocity = Vector3.SmoothDamp(BodyRB.velocity, Vector3.zero, ref currVelocity, dampSpeedDown);
        }
    }
    private void UpdateJump()
    {
        isGrounded = (Physics.Raycast(transform.TransformPoint(MainBody.center), Vector3.down, MainBody.height + .04f, groundLayers));
        if (!isGrounded && checkForGroundOnJump)
            return;

        if (JumpButton.action.WasPressedThisFrame())
        {
            if (!buttonPressed)
            {
                buttonPressed = true;
                BodyRB.AddForce(Vector3.up * jumpForce);
            }
        }
        else if (buttonPressed)
        {
            
            buttonPressed = false;
        }
    }

    private void UpdateCollider()
    {
        var height = Mathf.Clamp(xrOrigin.CameraInOriginSpaceHeight, minHeight, maxHeight);
        MainBody.height = height;

        var pelvisOffset = .15f;

        var dir = -xrOrigin.Camera.transform.forward;
        dir.y = 0f;
        dir.Normalize();

        var pelvisTarget = BodyRB.transform.InverseTransformDirection(dir * pelvisOffset);

        //This has potential to cause issues since im hard setting it. But from my testing they are very rare
        //pelvisTarget.y = Mathf.Max(LocoBall.position.y - BodyRB.position.y, 0f);
        pelvisTarget.y = 0;
        MainBody.center = Vector3.MoveTowards(MainBody.center, pelvisTarget, Time.deltaTime);


        //MoveHead();
        LimitHead();
    }
    private void LimitHead()
    {
        var delta = NeckPivot.transform.position - MainBody.transform.position;
        delta.y = 0;

        if (delta.sqrMagnitude < .01f || delta.magnitude < MaxLean) return;

        var allowedPosition = MainBody.transform.position + delta.normalized * MaxLean;
        var difference = allowedPosition - NeckPivot.transform.position;
        difference.y = 0f;
        CameraRig.transform.position += difference;
    }
    private void SafetyNet()
    {
        if(BodyRB.position.y < -100)
        {
            BodyRB.position = Vector3.zero;
            BodyRB.velocity = Vector3.zero;
        }
        if(BodyRB.position.x > 200 || BodyRB.position.x < -200)
        {
            BodyRB.position = Vector3.zero;
            BodyRB.velocity = Vector3.zero;
        }
        if (BodyRB.position.z > 200 || BodyRB.position.z < -200)
        {
            BodyRB.position = Vector3.zero;
            BodyRB.velocity = Vector3.zero;
        }
    }
    private void CheckSlopeAndDirections()
    {
        prevGroundNormal = groundNormal;

        RaycastHit slopeHit;
        if (Physics.SphereCast(MainBody.transform.position, slopeCheckerThreshold, Vector3.down, out slopeHit, MainBody.height / 2f + 0.5f, groundLayers))
        {
            groundNormal = slopeHit.normal;

            if (slopeHit.normal.y == 1)
            {

                forward = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                globalForward = forward;
                reactionForward = forward;

                SetFriction(frictionAgainstFloor, true);

                currentSurfaceAngle = 0f;
                isTouchingSlope = true;
            }
            else
            {
                //set forward
                Vector3 tmpGlobalForward = transform.forward.normalized;
                Vector3 tmpForward = new Vector3(tmpGlobalForward.x, Vector3.ProjectOnPlane(transform.forward.normalized, slopeHit.normal).normalized.y, tmpGlobalForward.z);
                Vector3 tmpReactionForward = new Vector3(tmpForward.x, tmpGlobalForward.y - tmpForward.y, tmpForward.z);

                if (currentSurfaceAngle <= maxSlopeAngle && !isTouchingStep)
                {
                    //set forward
                    forward = tmpForward * ((speedMultiplierOnAngle.Evaluate(currentSurfaceAngle / 90f) * canSlideMultiplierCurve) + 1f);
                    globalForward = tmpGlobalForward * ((speedMultiplierOnAngle.Evaluate(currentSurfaceAngle / 90f) * canSlideMultiplierCurve) + 1f);
                    reactionForward = tmpReactionForward * ((speedMultiplierOnAngle.Evaluate(currentSurfaceAngle / 90f) * canSlideMultiplierCurve) + 1f);

                    SetFriction(frictionAgainstFloor, true);
                }
                else if (isTouchingStep)
                {
                    //set forward
                    forward = tmpForward * ((speedMultiplierOnAngle.Evaluate(currentSurfaceAngle / 90f) * climbingStairsMultiplierCurve) + 1f);
                    globalForward = tmpGlobalForward * ((speedMultiplierOnAngle.Evaluate(currentSurfaceAngle / 90f) * climbingStairsMultiplierCurve) + 1f);
                    reactionForward = tmpReactionForward * ((speedMultiplierOnAngle.Evaluate(currentSurfaceAngle / 90f) * climbingStairsMultiplierCurve) + 1f);

                    SetFriction(frictionAgainstFloor, true);
                }
                else
                {
                    //set forward
                    forward = tmpForward * ((speedMultiplierOnAngle.Evaluate(currentSurfaceAngle / 90f) * cantSlideMultiplierCurve) + 1f);
                    globalForward = tmpGlobalForward * ((speedMultiplierOnAngle.Evaluate(currentSurfaceAngle / 90f) * cantSlideMultiplierCurve) + 1f);
                    reactionForward = tmpReactionForward * ((speedMultiplierOnAngle.Evaluate(currentSurfaceAngle / 90f) * cantSlideMultiplierCurve) + 1f);

                    SetFriction(0f, true);
                }

                currentSurfaceAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
            }

            //set down
            down = Vector3.Project(Vector3.down, slopeHit.normal);
            globalDown = Vector3.down.normalized;
            reactionGlobalDown = Vector3.up.normalized;
        }
        else
        {
            groundNormal = Vector3.zero;

            forward = Vector3.ProjectOnPlane(transform.forward, slopeHit.normal).normalized;
            globalForward = forward;
            reactionForward = forward;

            //set down
            down = Vector3.down.normalized;
            globalDown = Vector3.down.normalized;
            reactionGlobalDown = Vector3.up.normalized;

            SetFriction(frictionAgainstFloor, true);
        }
    }

    private void SetFriction(float _frictionWall, bool _isMinimum)
    {
        MainBody.material.dynamicFriction = 0.6f * _frictionWall;
        MainBody.material.staticFriction = 0.6f * _frictionWall;

        if (_isMinimum) MainBody.material.frictionCombine = PhysicMaterialCombine.Minimum;
        else MainBody.material.frictionCombine = PhysicMaterialCombine.Maximum;
    }

    public void AddVelocity(Vector3 speed)
    {
        BodyRB.AddForce(speed, ForceMode.VelocityChange);
        //LeftHandCode.rb.velocity += speed;
        //RightHandCode.rb.velocity += speed;
    }
}
