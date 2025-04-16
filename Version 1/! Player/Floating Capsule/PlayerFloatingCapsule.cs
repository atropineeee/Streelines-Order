using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerFloatingCapsule
{
    #region
    protected PlayerController PlayerController;

    public PlayerFloatingCapsule(PlayerController playerController)
    {
        this.PlayerController = playerController;
    }

    #endregion

    [Header("Player Components")]
    protected Rigidbody RigidBody;
    protected CapsuleCollider Collider;
    protected Transform PlayerTransform;

    [Header("Capsule Size")]
    public float Default_Height = 1.7f;
    public float Default_CenterY = 0.85f;
    public float Default_Radius = 0.15f;

    [Header("Capsule Dampener")]
    [Range(0f, 1f)] public float Default_SlopeRange = 0.35f;
    [Range(0f, 5f)] public float Default_FloatRayCast = 1.45f;
    [Range(0f, 50f)] public float Default_FloatStrength = 12;

    [Header("Floating Capsule Slope Speed Modifier")]
    public float onSlopeSpeedModifier;
    public float _SlopeSpeedModifier;
    public float _SlopeAngle;
    public float GroundAngle;
    public Vector3 ColliderLocalSpace;
    public LayerMask LayerMask;

    public void Awake()
    {
        this.RigidBody = this.PlayerController.RigidBody;
        this.Collider = this.PlayerController.Collider;
        this.PlayerTransform = this.PlayerController.PlayerTransform;

        this.LayerMask = 1;
    }

    public void Update()
    {
        SetColliderRadius(this.Default_Radius);
        SetColliderHeight(this.Default_Height * (1f - this.Default_SlopeRange));
        RecenterCollider();

        this.ColliderLocalSpace = Collider.center;
        float ColliderHeight = this.Collider.height / 2f;

        if (ColliderHeight < this.Collider.radius)
        {
            SetColliderRadius(ColliderHeight);
        }
    }

    public void FixedUpdate()
    {
        Vector3 ColliderCenterSpace = this.Collider.bounds.center;
        Ray DownRay = new Ray(ColliderCenterSpace, Vector3.down);

        Debug.DrawRay(DownRay.origin, DownRay.direction * this.Default_FloatRayCast, Color.red);

        if (Physics.Raycast(DownRay, out RaycastHit hit, this.Default_FloatRayCast, this.LayerMask, QueryTriggerInteraction.Ignore))
        {
            this.GroundAngle = Vector3.Angle(hit.normal, -DownRay.direction);

            float DistanceToFloatingPoint = this.ColliderLocalSpace.y * this.PlayerTransform.transform.localScale.y - hit.distance;

            if (DistanceToFloatingPoint == 0f) { return; }

            float FloatingAmount = DistanceToFloatingPoint * this.Default_FloatStrength - GetPlayerVelocity().y;
            Vector3 LiftForce = new Vector3(0f, FloatingAmount, 0f);

            this.RigidBody.AddForce(LiftForce, ForceMode.VelocityChange);
        }
    }

    private void RecenterCollider()
    {
        float ColliderHeight = this.Default_Height - this.Collider.height;
        Vector3 newCenterY = new Vector3(0f, this.Default_CenterY + (ColliderHeight / 2f), 0f);
        this.Collider.center = newCenterY;
    }

    private void SetColliderHeight(float useable_Height)
    {
        this.Collider.height = useable_Height;
    }

    private void SetColliderRadius(float useable_Radius)
    {
        this.Collider.radius = useable_Radius;
    }

    private Vector3 GetPlayerVelocity()
    {
        return new Vector3(0f, this.RigidBody.velocity.y, 0f);
    }
}
