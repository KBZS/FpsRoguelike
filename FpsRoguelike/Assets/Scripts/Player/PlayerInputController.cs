using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;

    // Player actions' const string titles
    private const string MOVE = "Move";
    private const string LOOK = "Look";
    private const string FIRE = "Fire";
    private const string JUMP = "Jump";
    private const string CROUCH = "Crouch";
    private const string RUN = "Run";

    public Vector2 GetMoveVector() => _playerInput.actions[MOVE].ReadValue<Vector2>();

    public Vector2 GetLookVector() => _playerInput.actions[LOOK].ReadValue<Vector2>();

    public bool GetFirePressedDown() => _playerInput.actions[FIRE].triggered && GetFireHold();

    public bool GetFireHold() => _playerInput.actions[FIRE].IsPressed();

    public bool GetJumpPressedDown() => _playerInput.actions[JUMP].triggered && GetJumpHold();

    public bool GetJumpHold() => _playerInput.actions[JUMP].IsPressed();

    public bool GetCrouchPressedDown() => _playerInput.actions[CROUCH].triggered && GetCrouchHold();

    public bool GetCrouchHold() => _playerInput.actions[CROUCH].IsPressed();

    public bool GetRunPressedDown() => _playerInput.actions[RUN].triggered && GetRunHold();

    public bool GetRunHold() => _playerInput.actions[RUN].IsPressed();
}
