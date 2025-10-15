using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    private InputActions InputActions;

    private void Awake()
    {
        Instance = this;
        InputActions = new InputActions();
        InputActions.Player.Enable();
    }

    public Vector2 GetMovementInput() =>
        InputActions.Player.move.ReadValue<Vector2>();

    public bool GetJumpInputDown() =>
        InputActions.Player.jump.triggered;

    public bool InteractInputDown() =>
        InputActions.Player.interact.triggered;
}
