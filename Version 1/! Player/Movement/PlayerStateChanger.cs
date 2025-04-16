using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerStateChanger
{
    #region
    protected PlayerController PlayerController;

    public PlayerStateChanger(PlayerController playerController)
    {
        this.PlayerController = playerController;
    }

    #endregion

    [Header("Player Components")]
    protected Rigidbody RigidBody;
    protected CapsuleCollider Collider;
    protected Transform PlayerTransform;

    protected Vector2 InputDirection;
    protected float PlayerVelocity;

    protected float TargetRotation;
    protected float CurrentRotation;

    protected PlayerMovementType currentMovementType = PlayerMovementType.Running;

    public void Awake()
    {
        this.RigidBody = this.PlayerController.RigidBody;
        this.Collider = this.PlayerController.Collider;
        this.PlayerTransform = this.PlayerController.PlayerTransform;
    }

    public void Update()
    {
        UpdatePlayerDirection();
        UpdatePlayerTurningDirection();
        UpdatePlayerMovementState();
        UpdatePlayerMovementType();
    }

    private void UpdatePlayerMovementState()
    {
        if (this.PlayerVelocity < 0.0f)
        {
            this.PlayerController.PlayerState.PlayerMovementState = PlayerMovementState.Idle;
            return;
        }

        if (this.PlayerVelocity >= 2.25f) 
        {
            this.PlayerController.PlayerState.PlayerMovementState = PlayerMovementState.Sprinting;
            return;
        }

        if (this.PlayerVelocity >= 1.25f)
        {
            this.PlayerController.PlayerState.PlayerMovementState = PlayerMovementState.Running;
            return;
        }

        if (this.PlayerVelocity > 0.625f)
        {
            this.PlayerController.PlayerState.PlayerMovementState = PlayerMovementState.Walking;
            return;
        }
    }

    private void UpdatePlayerMovementType()
    {   
        // If the Player is Sprinting
        if (Input.GetKey(KeyCode.LeftShift) && this.PlayerController.PlayerState.PlayerAimingState != PlayerAimingState.Aiming) 
        {
            this.PlayerController.PlayerState.PlayerMovementType = PlayerMovementType.Sprinting;
            this.PlayerController.Animator.SetBool("IsAiming", false);
        }
        else
        {
            if (this.PlayerController.PlayerState.PlayerAimingState == PlayerAimingState.Aiming)
            {
                this.PlayerController.PlayerState.PlayerMovementType = PlayerMovementType.Walking;
                this.PlayerController.Animator.SetBool("IsAiming", true);
            }
            else
            {
                this.PlayerController.PlayerState.PlayerMovementType = this.currentMovementType;
                this.PlayerController.Animator.SetBool("IsAiming", false);
            }
        }


        // Toggle Walk/Run
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (this.PlayerController.PlayerState.PlayerMovementType == PlayerMovementType.Walking) 
            {
                this.PlayerController.PlayerState.PlayerMovementType = PlayerMovementType.Running;
                this.currentMovementType = PlayerMovementType.Running;
            }
            else if (this.PlayerController.PlayerState.PlayerMovementType == PlayerMovementType.Running)
            {
                this.PlayerController.PlayerState.PlayerMovementType = PlayerMovementType.Walking;
                this.currentMovementType = PlayerMovementType.Walking;
            }
        }
    }

    private void UpdatePlayerTurningDirection()
    {
        this.TargetRotation = this.RigidBody.rotation.eulerAngles.y;
        this.CurrentRotation = this.PlayerController.PlayerMovement.currentTargetRotation.y;
        
        float angleDiff = Mathf.DeltaAngle(this.TargetRotation, this.CurrentRotation);
        bool isCloseToTurnTarget = Mathf.Abs(angleDiff) < 15f;


        if (this.InputDirection == Vector2.zero) 
        {
            this.PlayerController.PlayerState.PlayerTurningDirection = PlayerTurningDirection.None;
            return;
        }

        if (isCloseToTurnTarget) 
        {
            this.PlayerController.PlayerState.PlayerTurningDirection = PlayerTurningDirection.None;
            return;
        }

        if (angleDiff > 0f && Mathf.Abs(angleDiff) < 170f)
        {
            this.PlayerController.PlayerState.PlayerTurningDirection = PlayerTurningDirection.TurningRight;
        }
        
        if (angleDiff < 0f && Mathf.Abs(angleDiff) < 170f)
        {
            this.PlayerController.PlayerState.PlayerTurningDirection = PlayerTurningDirection.TurningLeft;
        }
       
        if (angleDiff > 0f && Mathf.Abs(angleDiff) >= 170f)
        {
            this.PlayerController.PlayerState.PlayerTurningDirection = PlayerTurningDirection.TurningRight;
            //this.PlayerController.PlayerState.PlayerTurningDirection = PlayerTurningDirection.TurningRight180;
        }
        
        if (angleDiff < 0f && Mathf.Abs(angleDiff) >= 170f)
        {
            this.PlayerController.PlayerState.PlayerTurningDirection = PlayerTurningDirection.TurningLeft;
            //this.PlayerController.PlayerState.PlayerTurningDirection = PlayerTurningDirection.TurningLeft180;
        }
    }

    private void UpdatePlayerDirection()
    {
        this.InputDirection = this.PlayerController.PlayerMovement.InputDirection;
        this.PlayerVelocity = this.PlayerController.PlayerMovement.PlayerVelocity;

        if (this.PlayerVelocity == 0f)
        {
            this.PlayerController.PlayerState.PlayerDirection = PlayerDirection.None;
            return;
        }

        if (this.InputDirection.y > 0 && this.InputDirection.x == 0)
        {
            this.PlayerController.PlayerState.PlayerDirection = PlayerDirection.Forward;
        }

        if (this.InputDirection.y < 0 && this.InputDirection.x == 0)
        {
            this.PlayerController.PlayerState.PlayerDirection = PlayerDirection.Backward;
        }

        if (this.InputDirection.x > 0 && this.InputDirection.y == 0)
        {
            this.PlayerController.PlayerState.PlayerDirection = PlayerDirection.Rightward;
        }

        if (this.InputDirection.x < 0 && this.InputDirection.y == 0)
        {
            this.PlayerController.PlayerState.PlayerDirection = PlayerDirection.Leftward;
        }
    }

    public void FixedUpdate()
    {

    }
}
