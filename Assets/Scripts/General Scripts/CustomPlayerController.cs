using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class CustomPlayerController : MonoBehaviour
{
    [Header("Behaviour Options")]
    
    public float jumpForce = 250.0f;
    public bool checkForGroundOnJump = true;
    public LayerMask ignoreLayers;

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
    private bool _waitingForCameraMovement;

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
        if (_waitingForCameraMovement)
            CheckCameraMovement();
        //MoveHands();
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

        if (primary2dValue != Vector2.zero)
        {

            Vector3 right = Camera.main.transform.TransformDirection(Vector3.right);
            Vector3 forward = Camera.main.transform.TransformDirection(Vector3.forward);

            //forward = Vector3.ProjectOnPlane(forward, transform.up);
            //right = Vector3.ProjectOnPlane(right, transform.up);
            forward.y = 0;
            right.y = 0;

            var movementAxis = right * primary2dValue.x + forward * primary2dValue.y;
            movementAxis.Normalize();
            movementAxis *= speed * Time.deltaTime;
            Debug.Log(movementAxis);
            AddVelocity(movementAxis);
        }

        //MoveHands();
    }
    private void MoveHands()
    {
        var v = (transform.position - prevPos) / Time.deltaTime;
        var acceler = (v - previousVelocity) / Time.deltaTime;
        previousVelocity = v;

        LeftHandCode.rb.AddForce(acceler * LeftHandCode.rb.mass, ForceMode.Force);
        RightHandCode.rb.AddForce(acceler * RightHandCode.rb.mass, ForceMode.Force);
    }
    private void UpdateJump()
    {
        isGrounded = (Physics.Raycast(transform.TransformPoint(MainBody.center), Vector3.down, MainBody.height + .04f, ignoreLayers));
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

    public void AddVelocity(Vector3 speed)
    {
        BodyRB.velocity += speed;
        //LeftHandCode.rb.velocity += speed;
        //RightHandCode.rb.velocity += speed;
    }
}
