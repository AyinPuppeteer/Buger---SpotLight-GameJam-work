using UnityEngine;

public class BaseMovement : CharacterBase
{
    protected override void Awake()
    {
        base.Awake();
        canJump = true; // 玩家可以跳跃
    }

    protected override void Update()
    {
        HandleTriggerButton();
        base.Update();
    }

    protected override void GetInput()
    {
        inputVector = GameInput.Instance.GetMovementInput();
        horizontal = inputVector.x;
        vertical = inputVector.y;
    }

    private void HandleTriggerButton()
    {
        // 接收跳跃按钮
        if (GameInput.Instance.GetJumpInputDown())
        {
            jumpRequested = true;
        }

        // 处理交互按钮逻辑
        if (GameInput.Instance.InteractInputDown())
        {
            Debug.Log("Interact button pressed");
        }
    }

}