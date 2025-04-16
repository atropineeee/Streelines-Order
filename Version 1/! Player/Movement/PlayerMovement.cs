using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class PlayerMovement
{
    #region
    protected PlayerController PlayerController;

    public PlayerMovement(PlayerController playerController)
    {
        this.PlayerController = playerController;
    }
    #endregion

    [Header("Player Components")]
    protected Rigidbody RigidBody;
    protected CapsuleCollider Collider;
    protected Transform PlayerTransform;
    protected Transform CameraTransform;

    [Header("Player Movement Stats")]
    [SerializeField] public float PlayerVelocity = 0.0f;

    [SerializeField] public float PlayerAccelaration = 1.25f;
    [SerializeField] public float PlayerAimAccelaration = 3.5f;

    [SerializeField] public float PlayerDecelaration = 1.15f;
    [SerializeField] public float PlayerAimDecelaration = 3.5f;

    [SerializeField] public float PlayerSpeedMultiplier = 2.25f;

    [Header("Player Speeds")]
    [SerializeField] public float PlayerMaxWalkingSpeed = 0.625f;
    [SerializeField] public float PlayerMaxRunningSpeed = 1.25f;
    [SerializeField] public float PlayerMaxSprintingSpeed = 2.25f;

    [Header("Player Rotation")]
    [SerializeField] public Vector2 InputDirection;
    [SerializeField] public Vector3 currentTargetRotation;
    [SerializeField] public Vector3 rotationTime = new Vector3(0f, 0.475f, 0f);
    [SerializeField] public Vector3 dampedRotationVelocity;
    [SerializeField] public Vector3 dampedRotationTime;
    [SerializeField] public float RotationBlend = 0f;
    [SerializeField] public float RotationBlendSpeed = 1.5f;

    [Header("Strafing")]
    [SerializeField] public float StrafeBlend = 0f;
    [SerializeField] public float StrafeBlendSpeed = 10f;
    [SerializeField] public float StrafeForwardBlend = 0f;

    [Header("Player Rotation Values")]
    [SerializeField] public float TargetRotationAngle;
    [SerializeField] public Vector3 MovementDirection;
    [SerializeField] public Vector3 TargetRotationDirection;
    [SerializeField] public Vector3 CurrentPlayerVelocity;
    [SerializeField] public Vector3 CurrentForward;
    [SerializeField] public Vector3 CameraForward;
    [SerializeField] public Vector3 CameraSideward;

    public void Awake()
    {
        this.RigidBody = this.PlayerController.RigidBody;
        this.Collider = this.PlayerController.Collider;
        this.PlayerTransform = this.PlayerController.PlayerTransform;
        this.CameraTransform = this.PlayerController.CameraTransform;
    }
    public void Update()
    {
        
    }

    public void FixedUpdate()
    {
        MovePlayer();
        UpdateRotationBlend();
        UpdateStrafeBlend();
    }

    public void MovePlayer()
    {
        this.InputDirection = GetInputDirection();
        this.MovementDirection = GetMovementDirection();
        this.CurrentPlayerVelocity = GetPlayerHorizontalVelocity();
        this.CameraForward = GetCameraForward();
        this.CameraSideward = GetCameraSideward();

        if (this.InputDirection != Vector2.zero)
        {
            if (this.PlayerController.PlayerState.PlayerAimingState == PlayerAimingState.Aiming)
            {
                if (this.PlayerController.PlayerState.PlayerMovementType == PlayerMovementType.Walking)
                {
                    if (this.PlayerVelocity < this.PlayerMaxWalkingSpeed)
                    {
                        this.PlayerVelocity += Time.deltaTime * this.PlayerAimAccelaration;
                    }

                    if (this.PlayerVelocity > this.PlayerMaxWalkingSpeed)
                    {
                        this.PlayerVelocity -= Time.deltaTime * this.PlayerAimAccelaration;
                    }
                }

                if (this.PlayerController.PlayerState.PlayerMovementType == PlayerMovementType.Running)
                {
                    if (this.PlayerVelocity > this.PlayerMaxRunningSpeed)
                    {
                        this.PlayerVelocity -= Time.deltaTime * this.PlayerAimDecelaration;
                    }

                    if (this.PlayerVelocity < this.PlayerMaxRunningSpeed)
                    {
                        this.PlayerVelocity += Time.deltaTime * this.PlayerAimAccelaration;
                    }
                }

                this.CurrentForward = (this.CameraForward * this.InputDirection.y + this.CameraSideward * this.InputDirection.x).normalized;

                if (this.CameraForward.sqrMagnitude > 0f)
                {
                    this.TargetRotationAngle = Quaternion.LookRotation(this.CameraSideward).eulerAngles.y;
                }

                this.RigidBody.AddForce(this.CurrentForward * (this.PlayerVelocity * this.PlayerSpeedMultiplier) - this.CurrentPlayerVelocity, ForceMode.VelocityChange);
            }
            else
            {

                if (this.PlayerController.PlayerState.PlayerMovementType == PlayerMovementType.Walking)
                {
                    if (this.PlayerVelocity < this.PlayerMaxWalkingSpeed)
                    {
                        this.PlayerVelocity += Time.deltaTime * this.PlayerAccelaration;
                    }
                }

                if (this.PlayerController.PlayerState.PlayerMovementType == PlayerMovementType.Running)
                {
                    if (this.PlayerVelocity > this.PlayerMaxRunningSpeed)
                    {
                        this.PlayerVelocity -= Time.deltaTime * this.PlayerDecelaration;
                    }

                    if (this.PlayerVelocity < this.PlayerMaxRunningSpeed)
                    {
                        this.PlayerVelocity += Time.deltaTime * this.PlayerAccelaration;
                    }
                }

                if (this.PlayerController.PlayerState.PlayerMovementType == PlayerMovementType.Sprinting)
                {
                    if (this.PlayerVelocity < this.PlayerMaxSprintingSpeed)
                    {
                        this.PlayerVelocity += Time.deltaTime * this.PlayerAccelaration;
                    }
                }

                this.TargetRotationAngle = Rotate(MovementDirection);
                this.TargetRotationDirection = GetTargetRotationDirection(TargetRotationAngle);
                this.CurrentForward = this.PlayerTransform.forward;

                this.RigidBody.AddForce(this.CurrentForward * (this.PlayerVelocity * this.PlayerSpeedMultiplier) - this.CurrentPlayerVelocity, ForceMode.VelocityChange);
            }
        }

        if (this.InputDirection == Vector2.zero)
        {
            if (this.PlayerVelocity < 0.0f)
            {
                this.PlayerVelocity = 0.0f;
            }

            if (this.PlayerController.PlayerState.PlayerAimingState == PlayerAimingState.Aiming)
            {
                if (this.PlayerVelocity > 0.0f)
                {
                    this.PlayerVelocity -= Time.deltaTime * this.PlayerAimDecelaration;
                }

                this.CurrentForward = (this.CameraForward * this.InputDirection.y + this.CameraSideward * this.InputDirection.x).normalized;

                if (this.CameraForward.sqrMagnitude > 0f)
                {
                    this.TargetRotationAngle = Quaternion.LookRotation(this.CameraSideward).eulerAngles.y;
                }

                this.RigidBody.AddForce(this.CurrentForward * (this.PlayerVelocity * this.PlayerSpeedMultiplier) - this.CurrentPlayerVelocity, ForceMode.VelocityChange);
            }
            else
            {
                if (this.PlayerVelocity > 0.0f)
                {
                    this.PlayerVelocity -= Time.deltaTime * this.PlayerDecelaration;
                }

                this.CurrentForward = this.PlayerTransform.forward;
                this.RigidBody.AddForce(this.CurrentForward * (this.PlayerVelocity * this.PlayerSpeedMultiplier) - this.CurrentPlayerVelocity, ForceMode.VelocityChange);
            }
        }

        this.PlayerController.Animator.SetFloat("Velocity", this.PlayerVelocity);
    }

    #region
    protected void UpdateStrafeBlend()
    {
        float targetBlend = 0;
        float targetBlend2 = 0;

        if (this.InputDirection.x == 1f)
        {
            targetBlend = 1f;
        }
        else if (this.InputDirection.x == -1f)
        {
            targetBlend = -1f;
        }
        else if (this.InputDirection.y == 1f)
        {
            targetBlend2 = 1f;
        }
        else if (this.InputDirection.y == -1f)
        {
            targetBlend2 = -1f;
        }
        else
        {
            targetBlend = 0f;
            targetBlend2 = 0f;
        }

            

        // Sideward
        //this.StrafeBlend = Mathf.Lerp(this.StrafeBlend, targetBlend, Time.deltaTime * this.StrafeBlendSpeed);
        //if (MathF.Abs(this.StrafeBlend) < 0.01f)
        //{
        //    this.StrafeBlend = 0f;
        //}
        this.PlayerController.Animator.SetFloat("StrafeX", targetBlend);

        // Forward
        //this.StrafeForwardBlend = Mathf.Lerp(this.StrafeForwardBlend, targetBlend2, Time.deltaTime * this.StrafeBlendSpeed);
        //if (MathF.Abs(this.StrafeForwardBlend) < 0.01f)
        //{
        //    this.StrafeForwardBlend = 0f;
        //}
        this.PlayerController.Animator.SetFloat("StrafeZ", targetBlend2);
    }
    #endregion

    #region Camera Aiming Mode
    protected Vector3 GetCameraSideward()
    {
        Vector3 camSide = this.CameraTransform.right;
        camSide.y = 0f;
        camSide.Normalize();
        return camSide;
    }

    protected Vector3 GetCameraForward()
    {
        Vector3 camForward = this.CameraTransform.forward;
        Vector3 camRight = this.CameraTransform.right;
        camForward.y = 0f;
        camForward.Normalize();
        return camForward;
    }
    #endregion

    #region Player Aim Rotation
    protected void UpdateRotationBlend()
    {
        if (this.PlayerController.PlayerState.PlayerAimingState == PlayerAimingState.Aiming) { return; }

        float targetBlend = 0f;

        switch (this.PlayerController.PlayerState.PlayerTurningDirection)
        {
            case PlayerTurningDirection.TurningLeft:
                targetBlend = -2f;
                break;
            case PlayerTurningDirection.TurningRight:
                targetBlend = 2f;
                break;
            case PlayerTurningDirection.None:
            default:
                targetBlend = 0f;
                break;
        }

        this.RotationBlend = Mathf.Lerp(this.RotationBlend, targetBlend, Time.deltaTime * this.RotationBlendSpeed);

        if (Mathf.Abs(this.RotationBlend) < 0.01f)
        {
            this.RotationBlend = 0f;
        }

        this.PlayerController.Animator.SetFloat("Rotation", this.RotationBlend);
    }
    #endregion

    #region Movement Direction
    private Vector3 GetMovementDirection()
    {
        Vector3 movementDirection = new Vector3(this.InputDirection.x, 0f, this.InputDirection.y);
        movementDirection.Normalize();
        return movementDirection;
    }

    private float Rotate(Vector3 direction)
    {
        float DirectionAngle = UpdateTargetRotation(direction);
        RotateTowardsTargetDirection();
        return DirectionAngle;
    }

    private float UpdateTargetRotation(Vector3 direction, bool RotateCamera = true)
    {
        float directionAngle = GetDirectionAngle(direction);

        if (RotateCamera)
        {
            directionAngle = AddCameraRotationAngle(directionAngle);
        }

        if (directionAngle != currentTargetRotation.y)
        {
            UpdateTargetRotationData(directionAngle);
        }

        return directionAngle;
    }

    private float GetDirectionAngle(Vector3 direction)
    {
        float directionAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        if (directionAngle < 0f)
        {
            directionAngle += 360f;
        }

        return directionAngle;
    }

    private float AddCameraRotationAngle(float angle)
    {
        angle += this.PlayerController.MainCamera.eulerAngles.y;

        if (angle > 360f)
        {
            angle -= 360f;
        }

        return angle;
    }

    private void UpdateTargetRotationData(float targetAngle)
    {
        this.currentTargetRotation.y = targetAngle;
        this.dampedRotationTime.y = 0f;
    }

    private void RotateTowardsTargetDirection()
    {
        float currentYAngle = this.RigidBody.rotation.eulerAngles.y;

        if (currentYAngle == currentTargetRotation.y)
        {
            return;
        }

        float smoothedYAngle = Mathf.SmoothDampAngle(currentYAngle, this.currentTargetRotation.y, ref this.dampedRotationVelocity.y, this.rotationTime.y - this.dampedRotationTime.y);

        this.dampedRotationTime.y += Time.deltaTime;

        Quaternion targetRotation = Quaternion.Euler(0f, smoothedYAngle, 0f);

        this.RigidBody.MoveRotation(targetRotation);
    }

    private Vector3 GetTargetRotationDirection(float targetRotationAngle)
    {
        return Quaternion.Euler(0f, targetRotationAngle, 0f) * Vector3.forward;
    }

    private Vector3 GetPlayerHorizontalVelocity()
    {
        Vector3 playerHorizontalVelocity = this.RigidBody.velocity;

        playerHorizontalVelocity.y = 0f;

        return playerHorizontalVelocity;
    }
    #endregion

    #region Player Input
    public Vector2 GetInputDirection()
    {
        Vector2 InputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        return InputDirection;
    }
    #endregion
}
