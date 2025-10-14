using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class BaseMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float originSpeed = 5.0f;
    [SerializeField] private float sneakSpeed = 2.5f;   //蹲速
    [SerializeField] private float climbSpeed = 2.0f;   //爬速
    [Header("Collision Settings")]
    [SerializeField] private float playerWidth = .35f;
    [SerializeField] private float playerHeight = .5f;
    [SerializeField] private float deadZone = 0.1f;   //设置死区
    [Header("Force Settings")]
    [SerializeField] private float gravity = 9.8f;
    [SerializeField] private float jumpForce = 5.0f;

    private Rigidbody2D rb;

    private float speed;
    private bool isWalking = false;
    private bool isClimbing = false;
    private bool isSneaking = false;
    private bool isGrounded = false;
    private int isJumping = 0;   //可以根据情况表示跳跃状态，比如0为未跳，正数是跳跃了几次，-1下落
    private bool toRight = true;
    private bool canMove = false;
    private bool canClimb = false;
    private bool canJump = false;

    private float horizontal = 0f;
    private float vertical = 0f;

    private Vector2 inputVector = Vector2.zero;
    private Vector2 moveDir = Vector2.zero;

    private float verticalVelocity = 0f; // 自定义竖直速度
    private bool jumpRequested = false;

    // 保护变量暴露属性
    public bool IsGrounded_ { get => isGrounded; }
    public bool IsClimbing_ { get => isClimbing; }
    public bool IsSneaking_ { get => isSneaking; }
    public bool CanClimb_ { get => canClimb; }
    public bool CanJump_ { get => canJump; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; // 确保禁用物理引擎重力
    }

    private void Update()
    {
        HandleTriggerButton();
        HandleVisualLayer();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleForce();
    }

    private void HandleTriggerButton()
    {
        // 接收跳跃按钮
        if (gameInput.Instance.GetJumpInputDown())
        {
            jumpRequested = true;
        }
        // 处理交互按钮逻辑
        if (gameInput.Instance.InteractInputDown())
        {
            // 在这里添加交互逻辑
            Debug.Log("Interact button pressed");
        }
    }

    private void HandleMovement()
    {
        inputVector = gameInput.Instance.GetMovementInput();
        horizontal = inputVector.x;
        vertical = inputVector.y;

        if (inputVector != Vector2.zero)
        {
            if (horizontal < -deadZone) toRight = false;
            else if (horizontal > deadZone) toRight = true;
            if (Mathf.Abs(vertical) > deadZone && canClimb) isClimbing = true;
            else if (vertical < -0.5f && isGrounded) isSneaking = true;
            else if (vertical > -deadZone || !isGrounded) isSneaking = false;
            if (!canClimb || Mathf.Abs(vertical) < deadZone) isClimbing = false;
            speed = isClimbing ? climbSpeed :
                isSneaking ? sneakSpeed : originSpeed;
        }
        else
        {
            if (isWalking) isWalking = false;
            if (isClimbing) isClimbing = false;
            if (isSneaking) isSneaking = false;
            speed = 0;
        }

        // 地面或空中移动
        Vector2 move = new Vector2(horizontal, 0) * speed * Time.fixedDeltaTime;

        // 竖直方向由自定义速度控制
        move.y += verticalVelocity * Time.fixedDeltaTime;

        // 爬梯时竖直方向由输入控制
        if (isClimbing)
        {
            move.y = vertical * climbSpeed * Time.fixedDeltaTime;
            verticalVelocity = 0f; // 爬梯时不受重力影响
        }

        CheckCollision(move, move.magnitude);
        if (canMove)
        {
            rb.MovePosition(rb.position + move);
        }
    }

    private void HandleForce()
    {
        // 跳跃逻辑
        if (isGrounded && jumpRequested)
        {
            verticalVelocity += jumpForce;
            isJumping = 1; //目前是一段跳
            isGrounded = false;
            jumpRequested = false; // 消耗跳跃请求
        }

        // 空中下落加速
        if (!isGrounded && !isClimbing)
        {
            verticalVelocity -= gravity * Time.fixedDeltaTime;
        }

        // 落地时重置竖直速度
        if (isGrounded && !isClimbing)
        {
            verticalVelocity = 0f;
        }
    }

    private void HandleVisualLayer()
    {
        // 处理视觉层级
        // 处理朝向
        Vector3 localScale = transform.localScale;
        if (toRight)
            localScale.x = Mathf.Abs(localScale.x);
        else
            localScale.x = -Mathf.Abs(localScale.x);
        transform.localScale = localScale;
    }

    private void CheckCollision(Vector2 moveDir, float moveDistance)
    {//主要解决各方碰撞
        RaycastHit2D hit = Physics2D.BoxCast(transform.position,
            new Vector2(playerWidth, playerHeight),
            0,
            moveDir,
            moveDistance);
        if (hit.collider == null) { canMove = true; return; }

        canMove = !hit.collider.CompareTag("Obstacle");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            isJumping = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Climber"))
        {
            canClimb = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Climber"))
        {
            canClimb = false;
        }
    }

}
