using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEditor.XR.LegacyInputHelpers;
using UnityEngine.XR;
using UnityEngine.InputSystem;
using Unity.XR.CoreUtils;

public class HexaBodyScript : MonoBehaviour
{
    [Header("XR Toolkit Parts")]
    public XROrigin XRRig;
    public GameObject XRCamera;

    [Header("Actionbased Controller")]
    public ActionBasedController CameraController;
    public ActionBasedController RightHandController;
    public ActionBasedController LeftHandController;

    [Space(10f)]

    public InputActionReference RightTrackPadPosition;
    public InputActionReference RightTrackPadTouch;

    [Space(10f)]

    public InputActionReference LeftTrackPadPosition;
    public InputActionReference LeftTrackPadTouch;

    [Space(10f)]

    public InputActionReference JumpButton;

    [Header("Hexabody Parts")]
    public GameObject Head;
    public GameObject Chest;
    public GameObject Fender;
    public GameObject Monoball;
    private Rigidbody monoBallRb;

    public ConfigurableJoint RightHandJoint;
    public ConfigurableJoint LeftHandJoint;
    public ConfigurableJoint Spine;

    [Header("Hexabody Movespeed")]
    public float moveForceCrouch;
    public float moveForceWalk;
    public float moveForceSprint;

    [Header("Hexabody Drag")]
    public float angularDragOnMove;
    public float angularBreakDrag;

    [Header("Hexabody Croch & Jump")]
    bool jumping = false;

    public float crouchSpeed;
    public float lowesCrouch;
    public float highestCrouch;
    private float additionalHight;

    private float rotationOffset = 0;

    Vector3 CrouchTarget;

    //---------Input Values---------------------------------------------------------------------------------------------------------------//

    private Quaternion headYaw;
    private Vector3 moveDirection;
    private Vector3 monoballTorque;

    private Vector3 CameraControllerPos;

    private Vector3 RightHandControllerPos;
    private Vector3 LeftHandControllerPos;

    private Quaternion RightHandControllerRotation;
    private Quaternion LeftHandControllerRotation;

    private Vector2 RightTrackpadValue;
    private Vector2 LeftTrackpadValue;

    private float JumpButtonPressed;

    private float RightTrackpadTouched;
    private float LeftTrackpadTouched;

    void Start()
    {
        additionalHight = (0.5f * Monoball.transform.lossyScale.y) + (0.5f * Fender.transform.lossyScale.y) + (Head.transform.position.y - Chest.transform.position.y);
        monoBallRb = Monoball.GetComponent<Rigidbody>();
    }

    
    void Update()
    {
        CameraToPlayer();
        XRRigToPlayer();

        GetContollerInputValues();
    }

    private void FixedUpdate() 
    {
        MovePlayerViaController();
        Jump();

        if (!jumping)
        {
            SpineContractionOnRealWorldCrouch();
        }

        RotatePlayer();
        MoveAndRotateHand();
    }

    private void GetContollerInputValues()
    {
        LeftTrackpadValue = LeftHandController.translateAnchorAction.action.ReadValue<Vector2>();
        //Right Controller
        //Position & Rotation
        RightHandControllerPos = RightHandController.positionAction.action.ReadValue<Vector3>();
        RightHandControllerRotation = RightHandController.rotationAction.action.ReadValue<Quaternion>();

        //Trackpad
        RightTrackpadValue = RightTrackPadPosition.action.ReadValue<Vector2>();
        RightTrackpadTouched = RightTrackPadTouch.action.ReadValue<float>();


        //Left Contoller
        //Position & Rotation
        LeftHandControllerPos = LeftHandController.positionAction.action.ReadValue<Vector3>();
        LeftHandControllerRotation = LeftHandController.rotationAction.action.ReadValue<Quaternion>();

        //Trackpad
        
        LeftTrackpadTouched = LeftTrackPadTouch.action.ReadValue<float>();

        //Jump Button
        JumpButtonPressed = JumpButton.action.ReadValue<float>();

        //Camera Inputs
        CameraControllerPos = CameraController.positionAction.action.ReadValue<Vector3>();

        headYaw = Quaternion.Euler(0, XRRig.Camera.transform.eulerAngles.y, 0);
        moveDirection = headYaw * new Vector3(LeftTrackpadValue.x, 0, LeftTrackpadValue.y);
        monoballTorque = new Vector3(moveDirection.z, 0, -moveDirection.x);
    }

    //------Transforms---------------------------------------------------------------------------------------
    private void CameraToPlayer()
    {
        XRCamera.transform.position = Head.transform.position;
    }
    private void XRRigToPlayer()
    {
        XRRig.transform.position = new Vector3(Fender.transform.position.x, Fender.transform.position.y - (0.5f * Fender.transform.localScale.y + 0.5f * Monoball.transform.localScale.y), Fender.transform.position.z);
    }
    private void RotatePlayer()
    {
        //if(RightTrackpadTouched == 1)
        //{
        //    rotationOffset += RightTrackpadValue.x;
        //    if (rotationOffset >= 360)
        //        rotationOffset -= 360;
        //    if (rotationOffset <= -360)
        //        rotationOffset += 360;
        //}
        //Quaternion offset = Quaternion.Euler(0f, rotationOffset, 0f);
        Chest.transform.rotation = headYaw;// * Quaternion.Inverse(offset);
    }
    //-----HexaBody Movement---------------------------------------------------------------------------------
    private void MovePlayerViaController()
    {
        if (!jumping)
        {
            if (LeftTrackpadTouched == 0)
            {
                StopMonoball();
            }

            else if (LeftTrackpadTouched == 1)
            {
                MoveMonoball(moveForceWalk);
            }
        }

        else if (jumping)
        {
            if (LeftTrackpadTouched == 0)
            {
                StopMonoball();
            }

            else if (LeftTrackpadTouched == 1)
            {
                MoveMonoball(moveForceCrouch);
            }
        }

    }
    private void MoveMonoball(float force)
    {
        monoBallRb.freezeRotation = false;
        monoBallRb.angularDrag = angularDragOnMove;
        monoBallRb.AddTorque(monoballTorque.normalized * force, ForceMode.Force);
    }
    private void StopMonoball()
    {
        monoBallRb.angularDrag = angularBreakDrag;

        if (monoBallRb.velocity == Vector3.zero)
        {
            monoBallRb.freezeRotation = true;
        }

    }

    //------Jumping------------------------------------------------------------------------------------------
    private void Jump()
    {
        if (JumpButtonPressed == 1 && LeftTrackpadValue.y < 0)
        {
            jumping = true;
            JumpSitDown();
        }

        else if ((JumpButtonPressed == 0) && jumping == true)
        {
            jumping = false;
            JumpSitUp();
        }

    }
    private void JumpSitDown()
    {
        if (CrouchTarget.y >= lowesCrouch)
        {
            CrouchTarget.y -= crouchSpeed * Time.fixedDeltaTime;
            Spine.targetPosition = new Vector3(0, CrouchTarget.y, 0);
        }
    }
    private void JumpSitUp()
    {
        CrouchTarget = new Vector3(0, highestCrouch - additionalHight, 0);
        Spine.targetPosition = CrouchTarget;
    }

    //------Joint Controll-----------------------------------------------------------------------------------
    private void SpineContractionOnRealWorldCrouch()
    {
        CrouchTarget.y = Mathf.Clamp(CameraControllerPos.y - additionalHight, lowesCrouch, highestCrouch - additionalHight);
        Spine.targetPosition = new Vector3(0, CrouchTarget.y, 0);

    }
    private void MoveAndRotateHand()
    {
        RightHandJoint.targetPosition = RightHandControllerPos - CameraControllerPos;
        LeftHandJoint.targetPosition = LeftHandControllerPos - CameraControllerPos;

        RightHandJoint.targetRotation = RightHandControllerRotation;
        LeftHandJoint.targetRotation = LeftHandControllerRotation;
    }
}
