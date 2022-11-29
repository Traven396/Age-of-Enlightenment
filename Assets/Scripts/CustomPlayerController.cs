using System.Collections;
using System.Collections.Generic;
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

    [SerializeField]
    private float capsuleHeight = 1.6f;

    [SerializeField]
    private CapsuleDirection capsuleDirection = CapsuleDirection.YAxis;


    private bool isGrounded;

    private bool buttonPressed;

    private Rigidbody rigidBodyComponent;

    private CapsuleCollider capsuleCollider;


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

        rigidBodyComponent.constraints = RigidbodyConstraints.FreezeRotation;
        capsuleCollider.direction = (int)capsuleDirection;
        capsuleCollider.radius = capsuleRadius;
        capsuleCollider.center = capsuleCenter;
        capsuleCollider.height = capsuleHeight;
    }



    void Update()
    {

        UpdateMovement();

        UpdateJump();
    }

    private void UpdateMovement()
    {
        Vector2 primary2dValue = MoveJoystick.action.ReadValue<Vector2>();


        if (primary2dValue != Vector2.zero)
        {

            var xAxis = primary2dValue.x * speed * Time.deltaTime;
            var zAxis = primary2dValue.y * speed * Time.deltaTime;

            Vector3 right = transform.TransformDirection(Vector3.right);
            Vector3 forward = transform.TransformDirection(Vector3.forward);

            transform.position += right * xAxis;
            transform.position += forward * zAxis;
        }
    }

    private void UpdateJump()
    {
        isGrounded = (Physics.Raycast((new Vector2(transform.position.x, transform.position.y + 2.0f)), Vector3.down, 5.0f, ignoreLayers));

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
}
