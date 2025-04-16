using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerState
{
    #region
    protected PlayerController PlayerController;

    public PlayerState(PlayerController playerController)
    {
        this.PlayerController = playerController;
    }
    #endregion

    public PlayerDirection PlayerDirection = PlayerDirection.None;
    public PlayerTurningDirection PlayerTurningDirection = PlayerTurningDirection.None;
    public PlayerGroundedState PlayerGroundedState = PlayerGroundedState.Grounded;
    public PlayerMovementType PlayerMovementType = PlayerMovementType.Running;
    public PlayerMovementState PlayerMovementState = PlayerMovementState.Idle;
    public PlayerAimingState PlayerAimingState = PlayerAimingState.None;
    public PlayerExtraActions PlayerExtraActions = PlayerExtraActions.None;
    public PlayerWeaponState PlayerWeaponState = PlayerWeaponState.Hand;
}

public enum PlayerDirection { None, Forward, Backward, Leftward, Rightward }
public enum PlayerTurningDirection { None, TurningLeft, TurningRight, TurningLeft180, TurningRight180 }
public enum PlayerGroundedState { Grounded, Swimming, Falling }
public enum PlayerMovementState { Idle, Walking, Running, Sprinting, Jumping, Sneaking, Driving }
public enum PlayerMovementType { Walking, Running, Sprinting }
public enum PlayerAimingState { None, Aiming }
public enum PlayerExtraActions { None, GetInCar, GetOutCar, DiveDown, HangUp, HangDown }
public enum PlayerWeaponState { Hand, Glock, DEagle, AK47, M4A1, Knife }
