using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

public class GamepadCursor : MonoBehaviour
{
    [SerializeField]
    private PlayerInput playerInput;
    [SerializeField]
    private RectTransform cursorTransform;
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private RectTransform canvasTransform;
    [SerializeField]
    private float cursorSpeed = 1000;
    [SerializeField]
    private float padding = 50f;

    private bool previousMouseState;
    private Mouse virtualMouse;
    private Mouse currentMouse;
    private Camera cam;

    private string previousControlScheme = "";

    private const string gamepadScheme = "Gamepad";
    private const string mouseScheme = "Keyboard&Mouse";

    private void OnEnable()
    {
        cam = Camera.main;
        currentMouse = Mouse.current;

        if (virtualMouse == null)
            virtualMouse = (Mouse) InputSystem.AddDevice("VirtualMouse");
        else if (!virtualMouse.added)
            InputSystem.AddDevice(virtualMouse);

        // Pair device to the user
        InputUser.PerformPairingWithDevice(virtualMouse, playerInput.user);

        if (cursorTransform != null)
        {
            Vector2 pos = cursorTransform.anchoredPosition;
            InputState.Change(virtualMouse.position, pos);
        }

        InputSystem.onAfterUpdate += UpdateMotion;

        playerInput.onControlsChanged += OnControlsChanged;
    }

    private void OnDisable()
    {
        InputSystem.RemoveDevice(virtualMouse);
        InputSystem.onAfterUpdate -= UpdateMotion;
        playerInput.onControlsChanged -= OnControlsChanged;
    }

    private void UpdateMotion()
    {
        if (virtualMouse == null || Gamepad.current == null)
            return;

        Vector2 deltaValue = Gamepad.current.leftStick.ReadValue();
        deltaValue *= cursorSpeed * Time.deltaTime;

        Vector2 currentPos = virtualMouse.position.ReadValue();
        Vector2 newPos = currentPos + deltaValue;

        newPos.x = Mathf.Clamp(newPos.x, padding, Screen.width - padding);
        newPos.y = Mathf.Clamp(newPos.y, padding, Screen.height - padding);

        InputState.Change(virtualMouse.position, newPos);
        InputState.Change(virtualMouse.delta, deltaValue);

        bool aButtonPressed = Gamepad.current.aButton.IsPressed();
        if (previousMouseState != aButtonPressed)
        {
            virtualMouse.CopyState<MouseState>(out var mouseState);
            mouseState.WithButton(MouseButton.Left, aButtonPressed);
            InputState.Change(virtualMouse, mouseState);
            previousMouseState = aButtonPressed;
        }

        AnchorCursor(newPos);
    }

    private void AnchorCursor(Vector2 pos)
    {
        Vector2 anchoredPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTransform, pos, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : cam, out anchoredPos);
        cursorTransform.anchoredPosition = anchoredPos;
    }

    private void OnControlsChanged(PlayerInput input)
    {
        if (playerInput.currentControlScheme == mouseScheme && previousControlScheme != mouseScheme)
        {
            cursorTransform.gameObject.SetActive(false);
            Cursor.visible = true;
            currentMouse.WarpCursorPosition(virtualMouse.position.ReadValue());
            previousControlScheme = mouseScheme;
        }
        else if (playerInput.currentControlScheme == gamepadScheme && previousControlScheme != gamepadScheme)
        {
            cursorTransform.gameObject.SetActive(true);
            Cursor.visible = false;
            InputState.Change(virtualMouse.position, currentMouse.position.ReadValue());
            AnchorCursor(currentMouse.position.ReadValue());
            previousControlScheme = gamepadScheme;
        }
    }
}
