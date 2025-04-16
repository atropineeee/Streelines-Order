using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Components")]
    [SerializeField] public Transform PlayerTransform;
    [SerializeField] public Transform CameraTransform;
    [SerializeField] public CapsuleCollider Collider;
    [SerializeField] public Rigidbody RigidBody;
    [SerializeField] public Animator Animator;

    [Header("Camera Components")]
    [SerializeField] public Transform MainCamera;

    [Header("Sub Scripts")]
    public PlayerState PlayerState;
    public PlayerMovement PlayerMovement;
    public PlayerWeaponAim PlayerWeaponAim;
    public PlayerStateChanger PlayerStateChanger;
    public PlayerFloatingCapsule PlayerFloatingCapsule;

    private void Awake()
    {
        GetPlayerComponents();
        GetRequredSubScripts();
        AwakePlayerScripts();
    }

    private void GetPlayerComponents()
    {
        this.MainCamera = GameObject.Find("PlayerMCam").GetComponent<Transform>();
        this.PlayerTransform = GetComponent<Transform>();
        this.CameraTransform = GameObject.Find("PlayerVCam").GetComponent<Transform>();
        this.Collider = GetComponent<CapsuleCollider>();
        this.RigidBody = GetComponent<Rigidbody>();
        this.Animator = GetComponent<Animator>();

    }

    private void GetRequredSubScripts()
    {
        this.PlayerState = new PlayerState(this);
        this.PlayerMovement = new PlayerMovement(this);
        this.PlayerWeaponAim = new PlayerWeaponAim(this);
        this.PlayerStateChanger = new PlayerStateChanger(this);
        this.PlayerFloatingCapsule = new PlayerFloatingCapsule(this);   
    }

    private void AwakePlayerScripts()
    {
        this.PlayerMovement.Awake();
        this.PlayerWeaponAim.Awake();
        this.PlayerStateChanger.Awake();
        this.PlayerFloatingCapsule.Awake();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    private void Update()
    {
        this.PlayerMovement.Update();
        this.PlayerWeaponAim.Update();
        this.PlayerStateChanger.Update();
        this.PlayerFloatingCapsule.Update();
    }

    private void FixedUpdate()
    {
        this.PlayerMovement.FixedUpdate();
        this.PlayerWeaponAim.FixedUpdate();
        this.PlayerStateChanger.FixedUpdate();
        this.PlayerFloatingCapsule.FixedUpdate();
        
    }
}
