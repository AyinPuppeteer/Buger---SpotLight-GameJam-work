using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class gameInput : MonoBehaviour
{
    public static gameInput Instance { get; private set; }

    private InputActions InputActions;

    private void Awake()
    {
        Instance = this;
        InputActions = new InputActions();
        InputActions.Player.Enable();
    }

    public Vector2 GetMovementInput() => InputActions.Player.move.ReadValue<Vector2>();

}
