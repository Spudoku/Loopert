using UnityEngine;
using UnityEngine.InputSystem;
// Code based on https://www.youtube.com/watch?v=zHSWG05byEc
public class InputManager : MonoBehaviour
{
    public static PlayerInput playerInput;

    public static Vector2 Movement;
    public static bool JumpWasPressed;
    public static bool JumpIsHeld;
    public static bool JumpWasReleased;
    public static bool RunIsHeld;

    public static bool SlideIsHeld;
    public static bool SlideWasReleased;


    private InputAction _moveAction;
    private InputAction _jumpAction;

    private InputAction slideAction;
    // private InputAction _runAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        _moveAction = playerInput.actions["Move"];
        _jumpAction = playerInput.actions["Jump"];
        slideAction = playerInput.actions["Crouch"];
        // _runAction = playerInput.actions["Run"];
    }

    private void Update()
    {
        Movement = _moveAction.ReadValue<Vector2>();
        // Debug.Log($"[InputManager.Update] {Movement}");

        JumpWasPressed = _jumpAction.WasPerformedThisFrame();
        JumpIsHeld = _jumpAction.IsPressed();
        JumpWasReleased = _jumpAction.WasReleasedThisFrame();

        SlideIsHeld = slideAction.IsPressed();
        SlideWasReleased = slideAction.WasReleasedThisFrame();

    }
}
