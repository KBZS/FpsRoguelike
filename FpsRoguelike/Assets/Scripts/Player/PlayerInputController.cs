using UnityEngine;
using UnityEngine.InputSystem;

public static class PlayerInputController
{
    // Player actions' const string titles
    private const string MOVE = "Move";
    private const string LOOK = "Look";
    private const string FIRE = "Fire";
    private const string JUMP = "Jump";
    private const string CROUCH = "Crouch";
    private const string RUN = "Run";

    public static Vector2 GetMoveVector(PlayerInput playerInput) => playerInput.actions[MOVE].ReadValue<Vector2>();

    public static Vector2 GetLookVector(PlayerInput playerInput) => playerInput.actions[LOOK].ReadValue<Vector2>();

    public static bool GetFirePressedDown(PlayerInput playerInput) => playerInput.actions[FIRE].triggered && GetFireHold(playerInput);

    public static bool GetFireHold(PlayerInput playerInput) => playerInput.actions[FIRE].IsPressed();

    public static bool GetJumpPressedDown(PlayerInput playerInput) => playerInput.actions[JUMP].triggered && GetJumpHold(playerInput);

    public static bool GetJumpHold(PlayerInput playerInput) => playerInput.actions[JUMP].IsPressed();

    public static bool GetCrouchPressedDown(PlayerInput playerInput) => playerInput.actions[CROUCH].triggered && GetCrouchHold(playerInput);

    public static bool GetCrouchHold(PlayerInput playerInput) => playerInput.actions[CROUCH].IsPressed();

    public static bool GetRunPressedDown(PlayerInput playerInput) => playerInput.actions[RUN].triggered && GetRunHold(playerInput);

    public static bool GetRunHold(PlayerInput playerInput) => playerInput.actions[RUN].IsPressed();
}
