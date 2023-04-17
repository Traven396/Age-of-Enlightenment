using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class CustomPlayerController : MonoBehaviour
{
    [Header("Behaviour Options")]

    [SerializeField]
    private float speed = 10.0f;

    [SerializeField]
    private float jumpForce = 250.0f;
    [Space(10f)]

    [SerializeField]
    private InputActionReference MoveJoystick;
    [SerializeField]
    private InputActionReference JumpButton;
    [Space(10f)]
    [SerializeField]
    private bool checkForGroundOnJump = true;
    [SerializeField]
    private LayerMask ignoreLayers;

    [Header("Capsule Collider Options")]
    [SerializeField]
    private Vector3 capsuleCenter = new Vector3(0, 1, 0);

    [SerializeField]
    private float capsuleRadius = 0.3f;

    [Header("Height Stuff")]
    //[SerializeField]
    //private float capsuleHeight = 1.6f;
    [SerializeField]
    private float minHeight = 0;
    [SerializeField]
    private float maxHeight = 2f;


    [SerializeField]
    private CapsuleDirection capsuleDirection = CapsuleDirection.YAxis;


    private bool isGrounded;

    private bool buttonPressed;

    private Rigidbody rigidBodyComponent;

    private CapsuleCollider capsuleCollider;

    private XROrigin xrOrigin;


    public enum CapsuleDirection
    {
        XAxis,
        YAxis,
        ZAxis
    }

    void OnEnable()
    {
        rigidBodyComponent = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        xrOrigin = GetComponent<XROrigin>();

        rigidBodyComponent.constraints = RigidbodyConstraints.FreezeRotation;
        capsuleCollider.direction = (int)capsuleDirection;

        capsuleCollider.radius = capsuleRadius;
        capsuleCollider.center = capsuleCenter;
        //capsuleCollider.height = capsuleHeight;

        speed = Player.Instance.playerMoveSpeed;
        

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


    void Update()
    {
        UpdateCollider();

        UpdateMovement();

        UpdateJump();

        SafetyNet();
    }

    private void UpdateMovement()
    {
        Vector2 primary2dValue = MoveJoystick.action.ReadValue<Vector2>();

        if (primary2dValue != Vector2.zero)
        {
            var xAxis = primary2dValue.x * speed * Time.deltaTime;
            var zAxis = primary2dValue.y * speed * Time.deltaTime;

            Vector3 right = Camera.main.transform.TransformDirection(Vector3.right).normalized;
            Vector3 forward = Camera.main.transform.TransformDirection(Vector3.forward).normalized;
            
            forward = Vector3.ProjectOnPlane(forward, transform.up);
            right = Vector3.ProjectOnPlane(right, transform.up);

            transform.position += right * xAxis;
            transform.position += forward * zAxis;
        }
    }

    private void UpdateJump()
    {
        isGrounded = (Physics.Raycast(transform.TransformPoint(capsuleCollider.center), Vector3.down, capsuleCollider.height + .04f, ignoreLayers));
        if (!isGrounded && checkForGroundOnJump)
            return;

        if (JumpButton.action.WasPressedThisFrame())
        {
            if (!buttonPressed)
            {
                buttonPressed = true;
                rigidBodyComponent.AddForce(Vector3.up * jumpForce);
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

        Vector3 center = xrOrigin.CameraInOriginSpacePos;
        center.y = height / 2f + (capsuleCollider.radius *.1f);

        capsuleCollider.height = height;
        capsuleCollider.center = center;
    }

    private void SafetyNet()
    {
        if(transform.position.y < -100)
        {
            transform.position = Vector3.zero;
            rigidBodyComponent.velocity = Vector3.zero;
        }
        if(transform.position.x > 200 || transform.position.x < -200)
        {
            transform.position = Vector3.zero;
            rigidBodyComponent.velocity = Vector3.zero;
        }
        if (transform.position.z > 200 || transform.position.z < -200)
        {
            transform.position = Vector3.zero;
            rigidBodyComponent.velocity = Vector3.zero;
        }
    }
}
