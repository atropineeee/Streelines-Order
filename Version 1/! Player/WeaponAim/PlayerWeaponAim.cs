using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerWeaponAim
{
    #region
    protected PlayerController PlayerController;

    public PlayerWeaponAim(PlayerController playerController)
    {
        this.PlayerController = playerController;
    }

    #endregion

    [Header("Player Components")]
    protected Rigidbody RigidBody;
    protected CapsuleCollider Collider;
    protected Transform PlayerTransform;
    protected Transform CameraTransform;

    public float rotationSpeed = 10f;

    public void Awake()
    {
        this.RigidBody = this.PlayerController.RigidBody;
        this.Collider = this.PlayerController.Collider;
        this.PlayerTransform = this.PlayerController.PlayerTransform;
        this.CameraTransform = this.PlayerController.CameraTransform;
    }

    public void Update()
    {
        if (Input.GetMouseButton(1))
        {
            this.PlayerController.PlayerState.PlayerAimingState = PlayerAimingState.Aiming;

            Vector3 camForward = this.CameraTransform.forward;
            camForward.y = 0f;
            camForward.Normalize();

            if (camForward.sqrMagnitude > 0f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(camForward);
                Vector3 currentRotation = this.PlayerTransform.rotation.eulerAngles;
                Vector3 targetEuler = targetRotation.eulerAngles;

                Quaternion yOnlyRotation = Quaternion.Euler(0f, targetEuler.y, 0f);
                this.PlayerTransform.rotation = Quaternion.Slerp(this.PlayerTransform.rotation, yOnlyRotation, rotationSpeed * Time.deltaTime);
            }
        }
        else
        {
            this.PlayerController.PlayerState.PlayerAimingState = PlayerAimingState.None;
        }
    }
    public void FixedUpdate()
    {

    }
}
