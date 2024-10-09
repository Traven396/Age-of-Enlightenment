using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AgeOfEnlightenment.PlayerController
{
    public class PlayerMotionController : MonoBehaviour
    {
        public enum CharacterTurnType
        {
            Continuous,
            Snap
        }
        [Header("Movement Settings")]
        [SerializeField] private float MovementSpeed = 7f;
        public CharacterTurnType TurnMethod;
        [SerializeField] private float JumpSpeed = 10f;
        [SerializeField] private float JumpDuration = 0.2f;
        [SerializeField] private float SlideGravity;
        [SerializeField] private float AirFriction = 0.5f;
        [SerializeField] private float GroundFriction = 100f;
        [SerializeField] private float Gravity = 30f;
        [SerializeField] private float MaxGravity = 30f;
        //'Aircontrol' determines to what degree the player is able to move while in the air;
        [SerializeField] [Range(0f, 1f)] private float AirControl = 0.4f;


        [Header("Collider Settings")]
        [SerializeField] private float colliderHeight = 2f;
        [SerializeField] private float minColliderHeight = 0;
        [SerializeField] private float maxColliderHeight = 2;
        [Range(0f, 1f)] public float stepHeightRatio = 0.25f;
        [SerializeField] private float MaxLean = .3f;
        [SerializeField] private Transform NeckPivot;
        [SerializeField] private Transform CameraRig;

        [Header("Events")] 
        public VectorEvent OnJump;
        public VectorEvent OnLand;
        public delegate void VectorEvent(Vector3 v);
        public enum PlayerControllerState
        {
            Grounded,
            Midair,
            Sliding,
            Jumping
        }

        [HideInInspector] public PlayerControllerState CurrentControllerState;

        private Vector3 currentPlayerVelocity;
        //Saved velocity from last frame;
        Vector3 inputVelocity = Vector3.zero;
        //Saved horizontal movement velocity from last frame;

        Vector3 savedMovementVelocity = Vector3.zero;
        Vector3 currentGroundAdjustmentVelocity;
        Vector3 currentMomentum = Vector3.zero;

        private float jumpStartTime;


        //Acceptable slope angle limit;
        public float slopeLimit = 80f;

        //private float currentJumpStartTime;
        float baseSensorRange = 0f;

        private bool isGrounded;
        private bool IsUsingExtendedSensorRange;
        private bool isCrouching;

        private Rigidbody _Rigidbody;
        private Sensor _Sensor;
        private CapsuleCollider _Collider;
        private void Awake()
        {
            _Collider = GetComponent<CapsuleCollider>();
            _Rigidbody = GetComponent<Rigidbody>();

            _Sensor = new Sensor(transform, _Collider);


            RecalibrateSensor();
        }

        private void FixedUpdate()
        {
            MotionUpdate();

            SafetyNet();
        }

        private void MotionUpdate()
        {
            CheckForGround();

            ApplyGravityFriction();

            CurrentControllerState = DeterminePlayerState();

            IsUsingExtendedSensorRange = isGrounded;

            ApplyVelocity();

            LimitHead();

            ChangePlayerHeight();
        }

        private void LimitHead()
        {
            var delta = NeckPivot.transform.position - _Collider.transform.position;
            delta.y = 0;

            if (delta.sqrMagnitude < .01f || delta.magnitude < MaxLean) return;

            var allowedPosition = _Collider.transform.position + delta.normalized * MaxLean;
            var difference = allowedPosition - NeckPivot.transform.position;
            difference.y = 0f;

            CameraRig.position += difference;
        }
        private void ApplyGravityFriction()
        {
            Vector3 _verticalMomentum = Vector3.zero;
            Vector3 _horizontalMomentum = Vector3.zero;

            //Split momentum into vertical and horizontal components;
            if (currentMomentum != Vector3.zero)
            {
                _verticalMomentum = VectorMath.ExtractDotVector(currentMomentum, transform.up);
                _horizontalMomentum = currentMomentum - _verticalMomentum;
            }

            //Add gravity to vertical momentum;
            _verticalMomentum -= Gravity * Time.deltaTime * transform.up;

            //Remove any downward force if the controller is grounded;
            if (CurrentControllerState == PlayerControllerState.Grounded)
                _verticalMomentum = Vector3.zero;

            //Apply friction to horizontal momentum based on whether the controller is grounded;
            if (CurrentControllerState == PlayerControllerState.Grounded)
                _horizontalMomentum = VectorMath.IncrementVectorTowardTargetVector(_horizontalMomentum, GroundFriction, Time.deltaTime, Vector3.zero);
            else
                _horizontalMomentum = VectorMath.IncrementVectorTowardTargetVector(_horizontalMomentum, AirFriction, Time.deltaTime, Vector3.zero);

            //Add horizontal and vertical momentum back together;
            currentMomentum = _horizontalMomentum + _verticalMomentum;

            //Project the current momentum onto the current ground normal if the controller is sliding down a slope;
            if (CurrentControllerState == PlayerControllerState.Sliding)
            {
                currentMomentum = Vector3.ProjectOnPlane(currentMomentum, GetGroundNormal());
            
                Vector3 _slideDirection = Vector3.ProjectOnPlane(-transform.up, GetGroundNormal()).normalized;
                currentMomentum += SlideGravity * Time.deltaTime * _slideDirection;
            }

            //If controller is jumping, override vertical velocity with jumpSpeed;
            if (CurrentControllerState == PlayerControllerState.Jumping)
            {
                currentMomentum = VectorMath.RemoveDotVector(currentMomentum, transform.up);
                currentMomentum += transform.up * JumpSpeed;
            }
        }
        private void ApplyVelocity()
        {
            _Rigidbody.velocity = inputVelocity + currentMomentum;// + currentGroundAdjustmentVelocity;
        }

        public void SafetyNet()
        {
            var safeV3 = new Vector3(0, 1, 0);
            if (transform.position.y < -100)
            {
                transform.position = safeV3;
                SetMomentum(Vector3.zero);
            }
            if (Mathf.Abs(transform.position.x) > 200)
            {
                transform.position = safeV3;
                SetMomentum(Vector3.zero);
            }
            if (Mathf.Abs(transform.position.z) > 200)
            {
                transform.position = safeV3;
                SetMomentum(Vector3.zero);
            }
        }
        private PlayerControllerState DeterminePlayerState()
        {
            //Check if vertical momentum is pointing upwards;
            //bool _isRising = IsRisingOrFalling() && (VectorMath.GetDotProduct(currentMomentum, transform.up) > 0f);
            //Check if controller is sliding;
            bool _isSliding = isGrounded && IsGroundTooSteep();

            //Debug.Log(isGrounded + " " + IsGroundTooSteep());

            switch (CurrentControllerState)
            {
                case PlayerControllerState.Grounded:

                    if (!isGrounded)
                    {
                        OnLoseGroundContact();
                        return PlayerControllerState.Midair;
                    }
                    if (_isSliding)
                        return PlayerControllerState.Sliding;

                    return PlayerControllerState.Grounded;
                    
                case PlayerControllerState.Midair:

                    //If the player is on the ground and NOT sliding, then we have fully landed
                    if(isGrounded && !_isSliding)
                    {
                        OnGroundContactRegained(currentMomentum);
                        return PlayerControllerState.Grounded;
                    }
                    //If the player is already sliding then we technically arent on the ground, but rather sliding
                    if (_isSliding)
                    {
                        OnGroundContactRegained(currentMomentum);
                        return PlayerControllerState.Sliding;
                    }

                    return PlayerControllerState.Midair;

                case PlayerControllerState.Sliding:

                    if(!isGrounded)
                        return PlayerControllerState.Midair;
                    if (isGrounded && !_isSliding)
                    {
                        OnGroundContactRegained(currentMomentum);
                        return PlayerControllerState.Grounded;
                    }

                    return PlayerControllerState.Sliding;

                case PlayerControllerState.Jumping:
                    if((Time.time - jumpStartTime) > JumpDuration)
                        return PlayerControllerState.Midair;

                    return PlayerControllerState.Jumping;
            }


            return PlayerControllerState.Grounded;
        }
        private void ChangePlayerHeight()
        {
            var playerHeight = Camera.main.transform.localPosition.y;

            //When the player pushes their head down far enough the colldier with half in height
            //This also creates boolean that I can check for other stuff maybe? Some kind of stealth?

            if (playerHeight <= -.3f && !isCrouching)
            {
                _Collider.height = (maxColliderHeight / 2f);

                isCrouching = true;
            }
            else if (playerHeight >= -.3f && isCrouching)
            {
                _Collider.height = (maxColliderHeight);

                isCrouching = false;
            }
        }

        #region Public Methods
        public void OnLoseGroundContact()
        {
            //Calculate current velocity;
            //If velocity would exceed the controller's movement speed, decrease movement velocity appropriately;
            //This prevents unwanted accumulation of velocity;
            float _horizontalMomentumSpeed = VectorMath.RemoveDotVector(currentMomentum, transform.up).magnitude;
            Vector3 _currentVelocity = currentMomentum + Vector3.ClampMagnitude(savedMovementVelocity, Mathf.Clamp(MovementSpeed - _horizontalMomentumSpeed, 0f, MovementSpeed));

            //Calculate length and direction from '_currentVelocity';
            float _length = _currentVelocity.magnitude;

            //Calculate velocity direction;
            Vector3 _velocityDirection = Vector3.zero;
            if (_length != 0f)
                _velocityDirection = _currentVelocity / _length;

            //Subtract from '_length', based on 'movementSpeed' and 'airControl', check for overshooting;
            if (_length >= MovementSpeed * AirControl)
                _length -= MovementSpeed * AirControl;
            else
                _length = 0f;

            currentMomentum = _velocityDirection * _length;
        }

        //This function is called when the controller has fully landed on a surface after being in the air;
        void OnGroundContactRegained(Vector3 _collisionVelocity)
        {
            OnLand?.Invoke(_collisionVelocity);
        }
        //This method is called by the InputController when the Jump Key is pressed
        public void StartJump()
        {
            
            //Add jump force to momentum;
            currentMomentum += transform.up * JumpSpeed;
            //Set jump start time;

            jumpStartTime = Time.time;

            //All the jumping physics stuff will be put here

            CurrentControllerState = PlayerControllerState.Jumping;
        }
        public void SetInputVelocity(Vector3 _inputVelocity)
        {
            inputVelocity = _inputVelocity * MovementSpeed;
        }

        public void RotatePlayer(float inputValue)
        {
            //Need to perform a switch statement depending on if its on Snap Turn or Smooth turning
            if(TurnMethod == CharacterTurnType.Continuous)
            {
                Quaternion currentRot = _Rigidbody.rotation;
                Quaternion q = Quaternion.AngleAxis(inputValue, Vector3.up);

                _Rigidbody.MoveRotation(currentRot * q);
            }
            else
            {
                Debug.Log("Snap turn hasnt been implemented yet. Sorry");
            }
        }
        public void AddMomentum(Vector3 additionalMomentum)
        {
            currentMomentum += additionalMomentum;
        }
        public void SetMomentum(Vector3 _newMomentum)
        {
            currentMomentum = _newMomentum;
        }
        public Vector3 GetMomentum()
        {
            return currentMomentum;
        }
        public void ChangeGravity(float modifierValue, bool allowNegative)
        {
            if (allowNegative)
            {
                Gravity += modifierValue;
            }
            else if (Gravity >= 0 && !allowNegative)
            {
                Gravity = Mathf.Max(Gravity + modifierValue, 0);
            }
            if (Gravity > MaxGravity)
                Gravity = MaxGravity;
        }
        #endregion

        //Check if the player is grounded;
        //Store all relevant collision information for later;
        //Calculate necessary adjustment velocity to keep the correct distance to the ground;
        void CheckForGround()
        {
            //Reset ground adjustment velocity;
            currentGroundAdjustmentVelocity = Vector3.zero;

            //Set sensor length;
            if (IsUsingExtendedSensorRange)
                _Sensor.castLength = baseSensorRange + (_Collider.height * transform.localScale.x) * stepHeightRatio;
            else
                _Sensor.castLength = baseSensorRange;

            _Sensor.Cast();

            //If sensor has not detected anything, set flags and return;
            if (!_Sensor.HasDetectedHit())
            {
                isGrounded = false;
                return;
            }

            //Set flags for ground detection;
            isGrounded = true;

            //Get distance that sensor ray reached;
            float _distance = _Sensor.GetDistance();



            //THIS MIGHT BE WHERE I HAVE SOME ISSUES. IF EVER THE PLAYER IS WALKING UP THINGS THEY SHOULD THIS COULD BE THE ISSUE
            //I AM SETTING THE MAX HEIGHT IN THE CODE, MEANING IT ASSUMES THE PLAYER IS STANDING UP ALWAYS


            //Calculate how much mover needs to be moved up or down;
            float _upperLimit = ((maxColliderHeight * transform.localScale.x) * (1f - stepHeightRatio)) * 0.5f;
            float _middle = _upperLimit + (maxColliderHeight * transform.localScale.x) * stepHeightRatio;
            float _distanceToGo = _middle - _distance;

            //Set new ground adjustment velocity for the next frame;
            currentGroundAdjustmentVelocity = transform.up * (_distanceToGo / Time.fixedDeltaTime);
        }

        void RecalibrateSensor()
        {
            _Sensor.SetCastDirection(Sensor.CastDirection.Down);

            RecalculateSensorLayerMask();

            RecalculateLength();

            //_Sensor.isInDebugMode = isInDebugMode;
        }

        public void RecalculateLength()
        {
            _Sensor.SetCastOrigin(_Collider.bounds.center);

            //Calculate and set sensor length;
            float _length = 0f;
            _length += (_Collider.height * (1f - stepHeightRatio)) * 0.5f;
            _length += _Collider.height * stepHeightRatio;
            baseSensorRange = _length * (1f + .001f) * transform.localScale.x;


            _Sensor.castLength = _length * transform.localScale.x;
        }

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
            _Sensor.layermask = _layerMask;
        }

        public Vector3 GetGroundNormal()
        {
            return _Sensor.GetNormal();
        }

        private bool IsGroundTooSteep()
        {
            if (!isGrounded)
                return true;

            return Vector3.Angle(GetGroundNormal(), transform.up) > slopeLimit;
        }
    } 
}
