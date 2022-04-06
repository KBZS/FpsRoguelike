using UnityEngine;
using UnityEngine.InputSystem;

public static class PlayerInputController
{
    public static Vector2 GetMoveVector(PlayerInput playerInput) => 
        playerInput.actions[StringsHelper.MOVE].ReadValue<Vector2>();

    public static Vector2 GetLookVector(PlayerInput playerInput) => 
        playerInput.actions[StringsHelper.LOOK].ReadValue<Vector2>();

    public static bool GetFirePressedDown(PlayerInput playerInput) => 
        playerInput.actions[StringsHelper.FIRE].triggered && GetFireHold(playerInput);

    public static bool GetFireHold(PlayerInput playerInput) => 
        playerInput.actions[StringsHelper.FIRE].IsPressed();

    public static bool GetJumpPressedDown(PlayerInput playerInput) => 
        playerInput.actions[StringsHelper.JUMP].triggered && GetJumpHold(playerInput);

    public static bool GetJumpHold(PlayerInput playerInput) => 
        playerInput.actions[StringsHelper.JUMP].IsPressed();

    public static bool GetCrouchPressedDown(PlayerInput playerInput) => 
        playerInput.actions[StringsHelper.CROUCH].triggered && GetCrouchHold(playerInput);

    public static bool GetCrouchHold(PlayerInput playerInput) => 
        playerInput.actions[StringsHelper.CROUCH].IsPressed();

    public static bool GetRunPressedDown(PlayerInput playerInput) => 
        playerInput.actions[StringsHelper.RUN].triggered && GetRunHold(playerInput);

    public static bool GetRunHold(PlayerInput playerInput) => 
        playerInput.actions[StringsHelper.RUN].IsPressed();
}
