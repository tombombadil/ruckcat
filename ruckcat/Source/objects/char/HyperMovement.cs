using UnityEngine;
using CMF;
using Sirenix.OdinInspector;

namespace Ruckcat
{
    public class HyperMovement : Char
    {
        [TitleGroup("Main"), PropertyOrder(-1)] public FloatField BaseSpeed;

        [TitleGroup("Main"), PropertyOrder(-1)] public float LeftRightSpeed = 20f;

        [TitleGroup("Main"), PropertyOrder(-1)] public float RotateSpeed = 500f;

        [TitleGroup("Main"), PropertyOrder(-1)] public float GroundFriction = 100f;

        [FoldoutGroup("HyperMovement (Left | Right Limits)", expanded: false), PropertyOrder(98)] 
        [Tooltip("player'in yürüyüşünü kısıtlayacak sol ve sağdaki engellerin layerları.")] public string[] LeftRightBlocksLayers;

        [FoldoutGroup("HyperMovement (Left | Right Limits)", expanded: false), PropertyOrder(98)]
        [Tooltip(
            "sağ ve sol duvar kontrol raylerinin start pointi için offset  (transform.position + BlockRayStartPositionOffset).")] public Vector3 LeftRightRayStartPositionOffset = Vector3.zero;

        [FoldoutGroup("HyperMovement (Left | Right Limits)", expanded: false), PropertyOrder(98)] public bool LeftRightRayDebugMode = false;

        [FoldoutGroup("HyperMovement (Left | Right Limits)", expanded: false), PropertyOrder(98)] public bool LockInput = false;

        [FoldoutGroup("HyperMovement (Jump)", expanded: false), PropertyOrder(98)] [Range(0f, 1f)] public float airControl = 0.4f;

        [FoldoutGroup("HyperMovement (Jump)", expanded: false), PropertyOrder(98)] public float jumpSpeed = 10f;

        [FoldoutGroup("HyperMovement (Jump)", expanded: false), PropertyOrder(98)] public float jumpDuration = 0.2f;

        [FoldoutGroup("HyperMovement (Jump)", expanded: false), PropertyOrder(98)] float currentJumpStartTime = 0f;

        [FoldoutGroup("HyperMovement (Jump)", expanded: false), PropertyOrder(98)] public float airFriction = 0.5f;

        [FoldoutGroup("HyperMovement (Jump)", expanded: false), PropertyOrder(98)] protected Vector3 momentum = Vector3.zero;

        [FoldoutGroup("HyperMovement (Jump)", expanded: false), PropertyOrder(98)] Vector3 savedVelocity = Vector3.zero;

        [FoldoutGroup("HyperMovement (Jump)", expanded: false), PropertyOrder(98)] Vector3 savedMovementVelocity = Vector3.zero;

        [FoldoutGroup("HyperMovement (Jump)", expanded: false), PropertyOrder(98)] public float gravity = 30f;

        [FoldoutGroup("HyperMovement (Jump)", expanded: false), PropertyOrder(98)]
        [Tooltip("How fast the character will slide down steep slopes.")] public float slideGravity = 5f;

        [FoldoutGroup("HyperMovement (Jump)", expanded: false), PropertyOrder(98)] public float slopeLimit = 80f;

        [FoldoutGroup("HyperMovement (Jump)", expanded: false), PropertyOrder(98)]
        [Tooltip("Whether to calculate and apply momentum relative to the controller's transform.")] public bool useLocalMomentum = false;

        private Transform rotatedMesh; //rotate edilecek child obje
        private float currentYRotation = 0f;
        private float stepHeightRatio = 0.25f;
        private float colliderHeight = 2f;
        private float colliderThickness = 1f;
        private Vector3 colliderOffset = Vector3.zero;
        [HideInInspector] public float defaultSpeed;

        [FoldoutGroup("HyperMovement (Sensor)", expanded: false), PropertyOrder(98)] [SerializeField]
        public Sensor.CastType sensorType = Sensor.CastType.Raycast;

        [FoldoutGroup("HyperMovement (Sensor)", expanded: false), PropertyOrder(98)] [SerializeField] [Range(1, 5)]
        public int sensorArrayRows = 1;

        [FoldoutGroup("HyperMovement (Sensor)", expanded: false), PropertyOrder(98)] [SerializeField] [Range(3, 10)]
        public int sensorArrayRayCount = 6;

        [FoldoutGroup("HyperMovement (Sensor)", expanded: false), PropertyOrder(98)] [SerializeField]
        public bool sensorArrayRowsAreOffset = false;

        [FoldoutGroup("HyperMovement (Sensor)", expanded: false), PropertyOrder(98)] [SerializeField]
        public bool SensorDebugMode = false;

        private float sensorRadiusModifier = 0.8f;
        private int currentLayer;
        [HideInInspector] public bool StopPlayerWhenGameFailed = false;
        [HideInInspector] public bool StopPlayerWhenGameSuccessed = false;
        [HideInInspector] public Vector3[] raycastArrayPreviewPositions;

        [HideInInspector]
        public delegate void VectorEvent(Vector3 v);

        [HideInInspector] public VectorEvent OnJump;
        [HideInInspector] public VectorEvent OnLand;
        bool isGrounded = false;
        bool IsUsingExtendedSensorRange = true;
        float baseSensorRange = 0f;
        Vector3 currentGroundAdjustmentVelocity = Vector3.zero;
        Collider col;
        Sensor sensor;
        BoxCollider boxCollider;
        SphereCollider sphereCollider;

        CapsuleCollider capsuleCollider;

        //References to attached components;
        protected Transform tr;
        //protected Mover mover;

        protected CeilingDetector ceilingDetector;

        //Jump key variables;
        bool jumpKeyWasPressed = false;
        bool jumpKeyWasLetGo = false;

        bool jumpKeyIsPressed = false;

        //Enum describing basic controller states; 
        public enum ControllerState
        {
            Grounded,
            Sliding,
            Falling,
            Rising,
            Jumping
        }

        protected ControllerState currentControllerState = ControllerState.Falling;
        protected Transform cameraTransform;
        private bool isFirstTouch = true;
        private Vector2 startPoint;
        private Vector3 tempPos;
        private Vector2 updatePoint;
        private float dir = 1;
        RaycastHit _raycastHit;
        private bool jumpCont = false;
        private Vector3 playerBoundingBox;
        void OnDisable()
        {
            if (rb) rb.velocity = Vector3.zero;
            if (CoreInputCont.Instance) CoreInputCont.Instance.EventTouch.RemoveListener(onTouch);
            if (HyperLevelCont.Instance) HyperLevelCont.Instance.EventGameStatus.RemoveListener(onGameOverResult);
        }

        void OnEnable()
        {
            if (CoreInputCont.Instance) CoreInputCont.Instance.EventTouch.AddListener(onTouch);
            if (HyperLevelCont.Instance) HyperLevelCont.Instance.EventGameStatus.AddListener(onGameOverResult);
        }

        public void SetTouchEventListener(bool status)
        {
            if (status)
            {
                if (CoreInputCont.Instance) CoreInputCont.Instance.EventTouch.AddListener(onTouch);
                if (HyperLevelCont.Instance) HyperLevelCont.Instance.EventGameStatus.AddListener(onGameOverResult);
            }
            else
            {
                if (CoreInputCont.Instance) CoreInputCont.Instance.EventTouch.RemoveListener(onTouch);
                if (HyperLevelCont.Instance) HyperLevelCont.Instance.EventGameStatus.RemoveListener(onGameOverResult);
            }
        }

        public override void Init()
        {
            base.Init();

            defaultSpeed = BaseSpeed.GetValue();
            playerBoundingBox = calculateBounds();
            tr = transform;
            ceilingDetector = GetComponent<CeilingDetector>();

            if (GetComponentInChildren<Animator>())
                rotatedMesh = GetComponentInChildren<Animator>().transform;

            Setup();

            sensor = new Sensor(this.tr, col);
            RecalculateColliderDimensions();
            RecalibrateSensor();

            CoreInputCont.Instance.EventTouch.AddListener(onTouch);
            HyperLevelCont.Instance.EventGameStatus.AddListener(onGameOverResult);
        }


        public virtual void onTouch(Ruckcat.Touch _touch)
        {
            if (LockInput) return;
            if (GameStatus == GameStatus.STARTGAME && !isFirstTouch)
            {
                if (_touch.Phase == TouchPhase.Began)
                {
                    startPoint = _touch.GetPoint(0);
                    tempPos = transform.position;
                }

                if (_touch.Phase == TouchPhase.Moved)
                {
                    if (tempPos != Vector3.zero)
                    {
                        updatePoint = _touch.GetPoint(1);

                        if ((updatePoint.x - startPoint.x) < 0) dir = -1;
                        else dir = 1;

                        Vector3 _lenght =
                            new Vector3(
                                ((playerBoundingBox.x / 2) * dir) + ((updatePoint.x - startPoint.x) * LeftRightSpeed),
                                0, 0);
                        if (!Physics.Raycast(transform.position + LeftRightRayStartPositionOffset, _lenght.normalized,
                            out _raycastHit, _lenght.magnitude, LayerMask.GetMask(LeftRightBlocksLayers),
                            QueryTriggerInteraction.UseGlobal))
                        {
                            transform.position =
                                new Vector3(tempPos.x + (updatePoint.x - startPoint.x) * LeftRightSpeed,
                                    transform.position.y, transform.position.z);
                            startPoint = _touch.GetPoint(1);
                            tempPos = transform.position;
                        }
                        else
                        {
                            transform.position =
                                new Vector3(_raycastHit.point.x - Vector3.right.x * (playerBoundingBox.x / 2) * dir,
                                    transform.position.y, transform.position.z);
                            startPoint = _touch.GetPoint(1);
                            tempPos = transform.position;
                        }
                    }
                    else
                    {
                        tempPos = transform.position;
                        startPoint = _touch.GetPoint(0);
                    }
                }

                if (_touch.Phase == TouchPhase.Ended)
                {
                }
            }

            isFirstTouch = false;
        }

        private Vector3 calculateBounds()
        {
            Quaternion currentRotation = this.transform.rotation;
            this.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            Bounds bounds = new Bounds(this.transform.position, Vector3.zero);

            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            {
                bounds.Encapsulate(renderer.bounds);
            }

            Vector3 localCenter = bounds.center - this.transform.position;
            bounds.center = localCenter;

            this.transform.rotation = currentRotation;
            return new Vector3(bounds.size.x - (bounds.center.x * 2), bounds.size.y - (bounds.center.y * 2),
                bounds.size.z - (bounds.center.z * 2));
        }


        void Reset()
        {
            Setup();
        }

        void OnValidate()
        {
            if (this.gameObject.activeInHierarchy)
                RecalculateColliderDimensions();

            if (sensorType == Sensor.CastType.RaycastArray)
                raycastArrayPreviewPositions =
                    Sensor.GetRaycastStartPositions(sensorArrayRows, sensorArrayRayCount, sensorArrayRowsAreOffset, 1f);
        }

        void Setup()
        {
            tr = transform;
            col = GetComponent<Collider>();

            //If no collider is attached to this gameobject, add a collider;
            if (col == null)
            {
                tr.gameObject.AddComponent<CapsuleCollider>();
                col = GetComponent<Collider>();
            }


            if (gameObject.GetComponent<Rigidbody>())
            {
                gameObject.AddComponent<Rigidbody>();
                rb = GetComponent<Rigidbody>();
            }

            /*boxCollider = GetComponent<BoxCollider>();
            sphereCollider = GetComponent<SphereCollider>();*/
            capsuleCollider = GetComponent<CapsuleCollider>();

            //Freeze rigidbody rotation and disable rigidbody gravity;
            rb.freezeRotation = true;
            rb.useGravity = false;
        }

        void LateUpdate()
        {
            if (GameStatus == GameStatus.STARTGAME)
            {
                if (SensorDebugMode)
                {
                    sensor.DrawDebug();
                }

                if (LeftRightRayDebugMode)
                {
                    Debug.DrawLine(
                        new Vector3((transform.position.x + (playerBoundingBox.x / 2)), transform.position.y,
                            transform.position.z) + LeftRightRayStartPositionOffset,
                        new Vector3((transform.position.x - (playerBoundingBox.x / 2)), transform.position.y,
                            transform.position.z) + LeftRightRayStartPositionOffset, Color.red,
                        Time.deltaTime); // boundingBox alanı debug
                    Debug.DrawLine(
                        new Vector3((transform.position.x + (playerBoundingBox.x / 2)), transform.position.y,
                            transform.position.z) + LeftRightRayStartPositionOffset,
                        new Vector3((transform.position.x + (playerBoundingBox.x / 2) + 1), transform.position.y,
                            transform.position.z) + LeftRightRayStartPositionOffset, Color.green,
                        Time.deltaTime); // sağ ray debug
                    Debug.DrawLine(
                        new Vector3((transform.position.x - (playerBoundingBox.x / 2)), transform.position.y,
                            transform.position.z) + LeftRightRayStartPositionOffset,
                        new Vector3((transform.position.x - (playerBoundingBox.x / 2) - 1), transform.position.y,
                            transform.position.z) + LeftRightRayStartPositionOffset, Color.green,
                        Time.deltaTime); // sol ray debug
                }

                if (rotatedMesh)
                    handleRotation();
            }
        }

        public override void Update()
        {
            base.Update();
            if (GameStatus == GameStatus.STARTGAME)
            {
                Jump();
            }
        }


        public void onGameOverResult(GameStatus gameStatus)
        {
            if (gameStatus == GameStatus.GAMEOVER_FAILED && StopPlayerWhenGameFailed)
            {
                rb.velocity = Vector3.zero;
            }
            else if (gameStatus == GameStatus.GAMEOVER_SUCCEED && StopPlayerWhenGameSuccessed)
            {
                rb.velocity = Vector3.zero;
            }
        }

        public void TryJump()
        {
            jumpCont = true;
        }

        void Jump()
        {
            bool _newJumpKeyPressedState = jumpCont;

            if (jumpKeyIsPressed == false && _newJumpKeyPressedState == true)
                jumpKeyWasPressed = true;

            if (jumpKeyIsPressed == false && _newJumpKeyPressedState == false)
                jumpKeyWasLetGo = true;

            jumpKeyIsPressed = _newJumpKeyPressedState;
        }

        //Handle jump booleans for later use in FixedUpdate;
        void HandleJumpKeyInput()
        {
            bool _newJumpKeyPressedState = IsJumpKeyPressed();

            if (jumpKeyIsPressed == false && _newJumpKeyPressedState == true)
                jumpKeyWasPressed = true;

            if (jumpKeyIsPressed == false && _newJumpKeyPressedState == false)
                jumpKeyWasLetGo = true;

            jumpKeyIsPressed = _newJumpKeyPressedState;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (GameStatus == GameStatus.STARTGAME)
            {
                CheckForGround();
                currentControllerState = DetermineControllerState();
                HandleMomentum();
                HandleJumping();
                Vector3 _velocity = CalculateMovementVelocity();
                Vector3 _worldMomentum = momentum;
                if (useLocalMomentum)
                    _worldMomentum = tr.localToWorldMatrix * momentum;
                _velocity += _worldMomentum;
                SetExtendSensorRange(IsGrounded());
                SetVelocity(_velocity);
                savedVelocity = _velocity;
                savedMovementVelocity = _velocity - _worldMomentum;
                jumpKeyWasLetGo = false;
                jumpKeyWasPressed = false;
                jumpCont = false;
                if (ceilingDetector != null)
                    ceilingDetector.ResetFlags();
            }
        }

        protected virtual Vector3 CalculateMovementDirection()
        {
            Vector3 _velocity = Vector3.zero;

            //If no camera transform has been assigned, use the character's transform axes to calculate the movement direction;
            if (cameraTransform == null)
            {
                _velocity += tr.right * GetHorizontalMovementInput();
                _velocity += tr.forward * GetVerticalMovementInput();
                // _velocity += tr.forward  /** characterInput.GetVerticalMovementInput()*/;
            }
            /*else
            {
                //If a camera transform has been assigned, use the assigned transform's axes for movement direction;
                //Project movement direction so movement stays parallel to the ground;
                _velocity += Vector3.ProjectOnPlane(cameraTransform.right, tr.up).normalized * characterInput.GetHorizontalMovementInput();
                _velocity += Vector3.ProjectOnPlane(cameraTransform.forward, tr.up).normalized * characterInput.GetVerticalMovementInput();
            }*/

            //If necessary, clamp movement vector to magnitude of 1f;
            if (_velocity.magnitude > 1f)
                _velocity.Normalize();

            return _velocity;
        }

        protected virtual Vector3 CalculateMovementVelocity()
        {
            Vector3 _velocity = CalculateMovementDirection();
            Vector3 _velocityDirection = _velocity;
            _velocity *= BaseSpeed.GetValue();
            // if (!(currentControllerState == ControllerState.Grounded))
                // _velocity = _velocityDirection * BaseSpeed.GetValue() * airControl;
            return _velocity;
        }


        ControllerState DetermineControllerState()
        {
            bool _isRising = IsRisingOrFalling() && (VectorMath.GetDotProduct(GetMomentum(), tr.up) > 0f);
            bool _isSliding = IsGroundedMover() && IsGroundTooSteep();
            if (currentControllerState == ControllerState.Grounded)
            {
                if (_isRising)
                {
                    OnGroundContactLost();
                    return ControllerState.Rising;
                }

                if (!IsGroundedMover())
                {
                    OnGroundContactLost();
                    return ControllerState.Falling;
                }

                if (_isSliding)
                {
                    return ControllerState.Sliding;
                }

                return ControllerState.Grounded;
            }

            if (currentControllerState == ControllerState.Falling)
            {
                if (_isRising)
                {
                    return ControllerState.Rising;
                }

                if (IsGroundedMover() && !_isSliding)
                {
                    OnGroundContactRegained(momentum);
                    return ControllerState.Grounded;
                }

                if (_isSliding)
                {
                    OnGroundContactRegained(momentum);
                    return ControllerState.Sliding;
                }

                return ControllerState.Falling;
            }

            if (currentControllerState == ControllerState.Sliding)
            {
                if (_isRising)
                {
                    OnGroundContactLost();
                    return ControllerState.Rising;
                }

                if (!IsGroundedMover())
                {
                    return ControllerState.Falling;
                }

                if (IsGroundedMover() && !_isSliding)
                {
                    OnGroundContactRegained(momentum);
                    return ControllerState.Grounded;
                }

                return ControllerState.Sliding;
            }

            if (currentControllerState == ControllerState.Rising)
            {
                if (!_isRising)
                {
                    if (IsGroundedMover() && !_isSliding)
                    {
                        OnGroundContactRegained(momentum);
                        return ControllerState.Grounded;
                    }

                    if (_isSliding)
                    {
                        return ControllerState.Sliding;
                    }

                    if (!IsGroundedMover())
                    {
                        return ControllerState.Falling;
                    }
                }

                if (ceilingDetector != null)
                {
                    if (ceilingDetector.HitCeiling())
                    {
                        OnCeilingContact();
                        return ControllerState.Falling;
                    }
                }

                return ControllerState.Rising;
            }

            if (currentControllerState == ControllerState.Jumping)
            {
                if ((Time.time - currentJumpStartTime) > jumpDuration)
                    return ControllerState.Rising;

                if (jumpKeyWasLetGo)
                    return ControllerState.Rising;

                if (ceilingDetector != null)
                {
                    if (ceilingDetector.HitCeiling())
                    {
                        OnCeilingContact();
                        return ControllerState.Falling;
                    }
                }

                return ControllerState.Jumping;
            }

            return ControllerState.Falling;
        }


        private void handleRotation()
        {
            if (!rotatedMesh || RotateSpeed == 0) return;

            Vector3 _velocity;
            // if(ignoreControllerMomentum)
            // _velocity = controller.GetMovementVelocity();
            // else
            // _velocity = controller.GetVelocity();
            _velocity = GetVelocity();
            _velocity = Vector3.ProjectOnPlane(_velocity, transform.up);
            float _magnitudeThreshold = 0.001f;
            if (_velocity.magnitude < _magnitudeThreshold)
                return;
            _velocity.Normalize();
            Vector3 _currentForward = rotatedMesh.forward;
            float _angleDifference = VectorMath.GetAngle(_currentForward, _velocity, transform.up);
            float fallOffAngle = 90f;
            float _factor = Mathf.InverseLerp(0f, fallOffAngle, Mathf.Abs(_angleDifference));
            float _step = Mathf.Sign(_angleDifference) * _factor * Time.deltaTime * RotateSpeed;
            if (_angleDifference < 0f && _step < _angleDifference)
                _step = _angleDifference;
            else if (_angleDifference > 0f && _step > _angleDifference)
                _step = _angleDifference;
            currentYRotation += _step;
            if (currentYRotation > 360f)
                currentYRotation -= 360f;
            if (currentYRotation < -360f)
                currentYRotation += 360f;
            rotatedMesh.localRotation = Quaternion.Euler(0f, currentYRotation, 0f);
        }


        void HandleJumping()
        {
            if (currentControllerState == ControllerState.Grounded)
            {
                if (jumpKeyIsPressed == true || jumpKeyWasPressed)
                {
                    //Call events;
                    OnGroundContactLost();
                    OnJumpStart();

                    currentControllerState = ControllerState.Jumping;
                }
            }
        }

        void HandleMomentum()
        {
            if (useLocalMomentum)
                momentum = tr.localToWorldMatrix * momentum;

            Vector3 _verticalMomentum = Vector3.zero;
            Vector3 _horizontalMomentum = Vector3.zero;

            if (momentum != Vector3.zero)
            {
                _verticalMomentum = VectorMath.ExtractDotVector(momentum, tr.up);
                _horizontalMomentum = momentum - _verticalMomentum;
            }

            _verticalMomentum -= tr.up * gravity * Time.deltaTime;
            if (currentControllerState == ControllerState.Grounded)
                _verticalMomentum = Vector3.zero;
            if (currentControllerState == ControllerState.Grounded)
                _horizontalMomentum =
                    VectorMath.IncrementVectorTowardTargetVector(_horizontalMomentum, GroundFriction,
                        Time.deltaTime, Vector3.zero);
            else
                _horizontalMomentum =
                    VectorMath.IncrementVectorTowardTargetVector(_horizontalMomentum, airFriction, Time.deltaTime,
                        Vector3.zero);

            momentum = _horizontalMomentum + _verticalMomentum;

            if (currentControllerState == ControllerState.Sliding)
            {
                momentum = Vector3.ProjectOnPlane(momentum, GetGroundNormal());
            }

            if (currentControllerState == ControllerState.Sliding)
            {
                Vector3 _slideDirection = Vector3.ProjectOnPlane(-tr.up, GetGroundNormal()).normalized;
                momentum += _slideDirection * slideGravity * Time.deltaTime;
            }

            if (currentControllerState == ControllerState.Jumping)
            {
                momentum = VectorMath.RemoveDotVector(momentum, tr.up);
                momentum += tr.up * jumpSpeed;
            }

            if (useLocalMomentum)
                momentum = tr.worldToLocalMatrix * momentum;
        }


        void OnJumpStart()
        {
            if (useLocalMomentum)
                momentum = tr.localToWorldMatrix * momentum;
            momentum += tr.up * jumpSpeed;
            currentJumpStartTime = Time.time;
            if (OnJump != null)
                OnJump(momentum);

            if (useLocalMomentum)
                momentum = tr.worldToLocalMatrix * momentum;
        }

        void OnGroundContactLost()
        {
            float _horizontalMomentumSpeed = VectorMath.RemoveDotVector(GetMomentum(), tr.up).magnitude;
            Vector3 _currentVelocity = GetMomentum() + Vector3.ClampMagnitude(savedMovementVelocity,
                Mathf.Clamp(BaseSpeed.GetValue() - _horizontalMomentumSpeed, 0f, BaseSpeed.GetValue()));

            float _length = _currentVelocity.magnitude;

            Vector3 _velocityDirection = Vector3.zero;
            if (_length != 0f)
                _velocityDirection = _currentVelocity / _length;

            if (_length >= BaseSpeed.GetValue() * airControl)
                _length -= BaseSpeed.GetValue() * airControl;
            else
                _length = 0f;

            if (useLocalMomentum)
                momentum = tr.localToWorldMatrix * momentum;

            momentum = _velocityDirection * _length;

            if (useLocalMomentum)
                momentum = tr.worldToLocalMatrix * momentum;
        }

        void OnGroundContactRegained(Vector3 _collisionVelocity)
        {
            if (OnLand != null)
                OnLand(_collisionVelocity);
        }

        void OnCeilingContact()
        {
            if (useLocalMomentum)
                momentum = tr.localToWorldMatrix * momentum;

            momentum = VectorMath.RemoveDotVector(momentum, tr.up);

            if (useLocalMomentum)
                momentum = tr.worldToLocalMatrix * momentum;
        }


        private bool IsRisingOrFalling()
        {
            Vector3 _verticalMomentum = VectorMath.ExtractDotVector(GetMomentum(), tr.up);

            float _limit = 0.001f;

            return (_verticalMomentum.magnitude > _limit);
        }

        private bool IsGroundTooSteep()
        {
            if (!IsGroundedMover())
                return true;

            return (Vector3.Angle(GetGroundNormal(), tr.up) > slopeLimit);
        }


        public override Vector3 GetVelocity()
        {
            return savedVelocity;
        }

        public Vector3 GetMovementVelocity()
        {
            return savedMovementVelocity;
        }

        public Vector3 GetMomentum()
        {
            Vector3 _worldMomentum = momentum;
            if (useLocalMomentum)
                _worldMomentum = tr.localToWorldMatrix * momentum;

            return _worldMomentum;
        }

        public bool IsGrounded()
        {
            return (currentControllerState == ControllerState.Grounded ||
                    currentControllerState == ControllerState.Sliding);
        }

        public bool IsSliding()
        {
            return (currentControllerState == ControllerState.Sliding);
        }

        public void AddMomentum(Vector3 _momentum)
        {
            if (useLocalMomentum)
                momentum = tr.localToWorldMatrix * momentum;

            momentum += _momentum;

            if (useLocalMomentum)
                momentum = tr.worldToLocalMatrix * momentum;
        }


        public void RecalculateColliderDimensions()
        {
            if (col == null)
            {
                Setup();
                if (col == null)
                {
                    Debug.LogWarning("There is no collider attached to " + this.gameObject.name + "!");
                    return;
                }
            }

            //Set collider dimensions based on collider variables;
            // if (boxCollider)
            // {
            //     Vector3 _size = Vector3.zero;
            //     _size.x = colliderThickness;
            //     _size.z = colliderThickness;

            //     // boxCollider.center = colliderOffset * colliderHeight;

            //     _size.y = colliderHeight * (1f - stepHeightRatio);
            //     boxCollider.size = _size;

            //     boxCollider.center = boxCollider.center + new Vector3(0f, stepHeightRatio * colliderHeight / 2f, 0f);
            // }
            // else if (sphereCollider)
            // {
            //     sphereCollider.radius = colliderHeight / 2f;
            //     sphereCollider.center = colliderOffset * colliderHeight;

            //     sphereCollider.center = sphereCollider.center + new Vector3(0f, stepHeightRatio * sphereCollider.radius, 0f);
            //     sphereCollider.radius *= (1f - stepHeightRatio);
            // }
            // else if (capsuleCollider)
            // {
            //     capsuleCollider.height = colliderHeight;
            //     capsuleCollider.center = colliderOffset * colliderHeight;
            //     capsuleCollider.radius = colliderThickness / 2f;

            //     capsuleCollider.center = capsuleCollider.center + new Vector3(0f, stepHeightRatio * capsuleCollider.height / 2f, 0f);
            //     capsuleCollider.height *= (1f - stepHeightRatio);

            //     if (capsuleCollider.height / 2f < capsuleCollider.radius)
            //         capsuleCollider.radius = capsuleCollider.height / 2f;
            // }

            //Recalibrate sensor variables to fit new collider dimensions;
            if (sensor != null)
                RecalibrateSensor();
        }

        void RecalibrateSensor()
        {
            sensor.SetCastOrigin(GetColliderCenter());
            sensor.SetCastDirection(Sensor.CastDirection.Down);
            RecalculateSensorLayerMask();
            sensor.castType = sensorType;
            float _radius = colliderThickness / 2f * sensorRadiusModifier;
            float _safetyDistanceFactor = 0.001f;
            if (boxCollider)
                _radius = Mathf.Clamp(_radius, _safetyDistanceFactor,
                    (boxCollider.size.y / 2f) * (1f - _safetyDistanceFactor));
            else if (sphereCollider)
                _radius = Mathf.Clamp(_radius, _safetyDistanceFactor,
                    sphereCollider.radius * (1f - _safetyDistanceFactor));
            else if (capsuleCollider)
                _radius = Mathf.Clamp(_radius, _safetyDistanceFactor,
                    (capsuleCollider.height / 2f) * (1f - _safetyDistanceFactor));
            sensor.sphereCastRadius = _radius;
            float _length = 0f;
            _length += (colliderHeight * (1f - stepHeightRatio)) * 0.5f;
            _length += colliderHeight * stepHeightRatio;
            baseSensorRange = _length * (1f + _safetyDistanceFactor);
            sensor.castLength = _length;
            sensor.ArrayRows = sensorArrayRows;
            sensor.arrayRayCount = sensorArrayRayCount;
            sensor.offsetArrayRows = sensorArrayRowsAreOffset;
            sensor.isInDebugMode = SensorDebugMode;
            sensor.calculateRealDistance = true;
            sensor.calculateRealSurfaceNormal = true;
            sensor.RecalibrateRaycastArrayPositions();
        }

        //Recalculate sensor layermask based on current physics settings;
        void RecalculateSensorLayerMask()
        {
            int _layerMask = 0;
            int _objectLayer = this.gameObject.layer;

            for (int i = 0; i < 32; i++)
            {
                if (!Physics.GetIgnoreLayerCollision(_objectLayer, i))
                    _layerMask = _layerMask | (1 << i);
            }

            if (_layerMask == (_layerMask | (1 << LayerMask.NameToLayer("Ignore Raycast"))))
            {
                _layerMask ^= (1 << LayerMask.NameToLayer("Ignore Raycast"));
            }
            sensor.layermask = _layerMask;
            currentLayer = _objectLayer;
        }

        Vector3 GetColliderCenter()
        {
            if (col == null)
                Setup();

            return col.bounds.center;
        }

        void Check()
        {
            currentGroundAdjustmentVelocity = Vector3.zero;
            if (IsUsingExtendedSensorRange)
                sensor.castLength = baseSensorRange + colliderHeight * stepHeightRatio;
            else
                sensor.castLength = baseSensorRange;

            sensor.Cast();
            if (!sensor.HasDetectedHit())
            {
                isGrounded = false;
                return;
            }

            isGrounded = true;
            float _distance = sensor.GetDistance();
            float _upperLimit = (colliderHeight * (1f - stepHeightRatio)) * 0.5f;
            float _middle = _upperLimit + colliderHeight * stepHeightRatio;
            float _distanceToGo = _middle - _distance;
            currentGroundAdjustmentVelocity = tr.up * (_distanceToGo / Time.fixedDeltaTime);
        }

        public void CheckForGround()
        {
            if (currentLayer != this.gameObject.layer)
                RecalculateSensorLayerMask();

            Check();
        }

        public virtual void SetVelocity(Vector3 _velocity)
        {
            rb.velocity = _velocity + currentGroundAdjustmentVelocity;
        }

        public bool IsGroundedMover()
        {
            return isGrounded;
        }


        public void SetExtendSensorRange(bool _isExtended)
        {
            IsUsingExtendedSensorRange = _isExtended;
        }

        //Set height of collider;
        public void SetColliderHeight(float _newColliderHeight)
        {
            if (colliderHeight == _newColliderHeight)
                return;

            colliderHeight = _newColliderHeight;
            RecalculateColliderDimensions();
        }

        //Set acceptable step height;
        public void SetStepHeightRatio(float _newStepHeightRatio)
        {
            _newStepHeightRatio = Mathf.Clamp(_newStepHeightRatio, 0f, 1f);
            stepHeightRatio = _newStepHeightRatio;
            RecalculateColliderDimensions();
        }

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


        /*---------------- INPUT ----------------- */
        protected virtual float GetHorizontalMovementInput()
        {
            return 0;
        }

        protected virtual float GetVerticalMovementInput()
        {
            return 1;
        }

        protected virtual bool IsJumpKeyPressed()
        {
            return false;
        }
    }
}