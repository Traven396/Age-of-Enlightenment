using AgeOfEnlightenment.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AgeOfEnlightenment.PlayerController
{
    [RequireComponent(typeof(PlayerMotionController))]
    public class PlayerInputManager : MonoBehaviour
    {
        #region Input References
        [Header("Motion Buttons")]
        public InputActionReference MoveJoystick;
        public InputActionReference RotationJoystick;
        public InputActionReference JumpButton;
        public InputActionReference BookMenuButton;

        [Header("Left Hand Controls")]
        public InputActionReference LeftTriggerButton;
        public InputActionReference LeftGripButton;
        public InputActionReference LeftAButton;
        public InputActionReference LeftMenuButton;


        [Header("Right Hand Controls")]
        public InputActionReference RightTriggerButton;
        public InputActionReference RightGripButton;
        public InputActionReference RightAButton;
        public InputActionReference RightMenuButton;
        #endregion

        #region UI Management
        [Header("Raycast Testing Shit, god i hate my life")]
        public LayerMask UIMASK;
        

        private bool leftHandNotPointingAtUI = true;
        private bool rightHandNotPointingAtUI = true;
        #endregion

        #region Input Settings

        [Header("Settings")]
        [Range(0, 1)]
        public float TurnThreshold;
        #endregion
        //Current spell references
        private SpellManager _SpellManager;

        [HideInInspector] public bool jumpKeyIsPressed, jumpButtonWasPressed;

        private PlayerMotionController _MotionController;
        private GrimoireManager _GrimoireMenu;
        private InternalPlayerHotbar _InternalHotbar;
        private void Awake()
        {
            _MotionController = GetComponent<PlayerMotionController>();
            _GrimoireMenu = GetComponentInChildren<GrimoireManager>();

            _InternalHotbar = GetComponentInChildren<InternalPlayerHotbar>();
            _SpellManager = GetComponentInChildren<SpellManager>();
        }
        private void OnEnable()
        {
            JumpButton.action.started += JumpStart;
            JumpButton.action.canceled += JumpEnd;

            LeftTriggerButton.action.started += LeftTriggerPress;
            LeftTriggerButton.action.canceled += LeftTriggerRelease;

            LeftGripButton.action.started += LeftGripPress;
            LeftGripButton.action.canceled += LeftGripRelease;

            RightTriggerButton.action.started += RightTriggerPress;
            RightTriggerButton.action.canceled += RightTriggerRelease;

            RightGripButton.action.started += RightGripPress;
            RightGripButton.action.canceled += RightGripRelease;

            BookMenuButton.action.started += ctx => _GrimoireMenu.BookAnimation();


            LeftMenuButton.action.started += ctx => _InternalHotbar.LeftHandMenu.ShowMenu();
            LeftMenuButton.action.canceled += ctx => _InternalHotbar.LeftHandMenu.HideMenu();

            RightMenuButton.action.started += ctx => _InternalHotbar.RightHandMenu.ShowMenu();
            RightMenuButton.action.canceled += ctx => _InternalHotbar.RightHandMenu.HideMenu();
        }
        private void OnDisable()
        {
            JumpButton.action.started -= JumpStart;
            JumpButton.action.canceled -= JumpEnd;

            LeftTriggerButton.action.started -= LeftTriggerPress;
            LeftTriggerButton.action.canceled -= LeftTriggerRelease;

            LeftGripButton.action.started -= LeftGripPress;
            LeftGripButton.action.canceled -= LeftGripRelease;

            RightTriggerButton.action.started -= RightTriggerPress;
            RightTriggerButton.action.canceled -= RightTriggerRelease;

            RightGripButton.action.started -= RightGripPress;
            RightGripButton.action.canceled -= RightGripRelease;

            BookMenuButton.action.started -= ctx => _GrimoireMenu.BookAnimation();

            
        }

        private void Update()
        {
            InputUpdate();
        }
        private void FixedUpdate()
        {
            InputFixedUpdate();
        }
        private void InputUpdate()
        {
            //HandleJumpKey();
            if (_SpellManager.SpawnedLeftBlueprint)
            {
                //Hold events
                if (leftHandNotPointingAtUI)
                    _SpellManager.SpawnedLeftBlueprint.TriggerHoldSafe();

                _SpellManager.SpawnedLeftBlueprint.GripHoldSafe();

                _SpellManager.SpawnedLeftBlueprint.gripPressedValue = LeftGripButton.action.ReadValue<float>();
                _SpellManager.SpawnedLeftBlueprint.triggerPressedValue = LeftTriggerButton.action.ReadValue<float>();
            }
            if (_SpellManager.SpawnedRightBlueprint)
            {
                //Hold events
                if (rightHandNotPointingAtUI)
                    _SpellManager.SpawnedRightBlueprint.TriggerHoldSafe();

                _SpellManager.SpawnedRightBlueprint.GripHoldSafe();

                //Button pressed values
                _SpellManager.SpawnedRightBlueprint.gripPressedValue = RightGripButton.action.ReadValue<float>();
                _SpellManager.SpawnedRightBlueprint.triggerPressedValue = RightTriggerButton.action.ReadValue<float>();

            }
        }

        private void InputFixedUpdate()
        {
            var _direction = GetMovementDirection();

            HandleTurnInput();

            _MotionController.SetInputVelocity(_direction);



            if (_SpellManager.SpawnedLeftBlueprint)
            {
                _SpellManager.SpawnedLeftBlueprint.GripHoldFixed();

                if (leftHandNotPointingAtUI)
                    _SpellManager.SpawnedLeftBlueprint.TriggerHoldFixed();
            }

            if (_SpellManager.SpawnedRightBlueprint)
            {
                if (RaycastCheck(LeftRight.Right))
                {
                    _SpellManager.SpawnedRightBlueprint.GripHoldFixed();

                    if (rightHandNotPointingAtUI)
                        _SpellManager.SpawnedRightBlueprint.TriggerHoldFixed();
                }
            }
        }

        #region Spell Inputs


        private bool RaycastCheck(LeftRight whichHand)
        {
            if (whichHand == 0)
            {
                return !Physics.Raycast(_SpellManager.SpawnedLeftBlueprint._HandTransform.position, _SpellManager.SpawnedLeftBlueprint._HandTransform.forward, 6, UIMASK, QueryTriggerInteraction.Collide);
                
            }
            else
            {
                return !Physics.Raycast(_SpellManager.SpawnedRightBlueprint._HandTransform.position, _SpellManager.SpawnedRightBlueprint._HandTransform.forward, 6, UIMASK, QueryTriggerInteraction.Collide);
            }
        }

        private void RightGripRelease(InputAction.CallbackContext obj)
        {
            if (_SpellManager.SpawnedRightBlueprint)
            {
                _SpellManager.SpawnedRightBlueprint.GripRelease();
            }
        }
        private void RightGripPress(InputAction.CallbackContext obj)
        {
            if (_SpellManager.SpawnedRightBlueprint)
            {
                _SpellManager.SpawnedRightBlueprint.GripPress();
            }
        }
        private void RightTriggerRelease(InputAction.CallbackContext obj)
        {
            if (_SpellManager.SpawnedRightBlueprint && rightHandNotPointingAtUI)
            {
                _SpellManager.SpawnedRightBlueprint.TriggerRelease();
                rightHandNotPointingAtUI = false;
            }
        }
        private void RightTriggerPress(InputAction.CallbackContext obj)
        {
            if (_SpellManager.SpawnedRightBlueprint)
            {
                rightHandNotPointingAtUI = RaycastCheck(LeftRight.Right);

                if (rightHandNotPointingAtUI)
                    _SpellManager.SpawnedRightBlueprint.TriggerPress();
            }
        }


        private void LeftGripRelease(InputAction.CallbackContext obj)
        {
            if (_SpellManager.SpawnedLeftBlueprint)
            {
                _SpellManager.SpawnedLeftBlueprint.GripRelease();
            }

        }
        private void LeftGripPress(InputAction.CallbackContext obj)
        {
            if (_SpellManager.SpawnedLeftBlueprint)
            {
                _SpellManager.SpawnedLeftBlueprint.GripPress();
            }
        }
        private void LeftTriggerRelease(InputAction.CallbackContext obj)
        {
            if (_SpellManager.SpawnedLeftBlueprint && leftHandNotPointingAtUI)
            {
                _SpellManager.SpawnedLeftBlueprint.TriggerRelease();
                leftHandNotPointingAtUI = false;
            }
        }
        private void LeftTriggerPress(InputAction.CallbackContext obj)
        {
            if (_SpellManager.SpawnedLeftBlueprint)
            {
                leftHandNotPointingAtUI = RaycastCheck(LeftRight.Left);

                if (leftHandNotPointingAtUI)
                    _SpellManager.SpawnedLeftBlueprint.TriggerPress();
            }
        }
        #endregion


        #region Helper Functions

        private void JumpStart(InputAction.CallbackContext obj)
        {
            jumpKeyIsPressed = true;

            HandleJumpInput();
        }
        private void JumpEnd(InputAction.CallbackContext obj)
        {
            jumpKeyIsPressed = false;
        }
        private Vector3 GetMovementDirection()
        {
            var _worldDirection = Vector3.zero;
            var _inputDirection = MoveJoystick.action.ReadValue<Vector2>().normalized;

            _worldDirection += Vector3.ProjectOnPlane(Camera.main.transform.right, transform.up).normalized * _inputDirection.x;
            _worldDirection += Vector3.ProjectOnPlane(Camera.main.transform.forward, transform.up).normalized * _inputDirection.y;

            if (_worldDirection.sqrMagnitude > 1)
                _worldDirection.Normalize();

            return _worldDirection;
        }

        void HandleJumpInput()
        {
            if (_MotionController.CurrentControllerState == PlayerMotionController.PlayerControllerState.Grounded)
            {
                if ((jumpKeyIsPressed))
                {

                    //Call methods on Motion Controller;
                    _MotionController.OnLoseGroundContact();
                    _MotionController.StartJump();
                }
            }
        }

        void HandleTurnInput()
        {
            float inputValue = RotationJoystick.action.ReadValue<Vector2>().x;

            if (inputValue * inputValue >= TurnThreshold)
            {
                _MotionController.RotatePlayer(inputValue);
            }
        }
        #endregion
    } 
}
