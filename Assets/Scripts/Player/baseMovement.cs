using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class BaseMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float originSpeed = 5.0f;
    [SerializeField] private float sneakSpeed = 2.5f;   //����
    [SerializeField] private float climbSpeed = 2.0f;   //����
    [Header("Collision Settings")]
    [SerializeField] private float playerWidth = .35f;
    [SerializeField] private float playerHeight = .5f;
    [SerializeField] private float deadZone = 0.1f;   //��������
    [Header("Force Settings")]
    [SerializeField] private float gravity = 9.8f;
    [SerializeField] private float jumpForce = 5.0f;

    private Rigidbody2D rb;

    private float speed;
    private bool isWalking = false;
    private bool isClimbing = false;
    private bool isSneaking = false;
    private bool isGrounded = false;
    private int isJumping = 0;   //���Ը��������ʾ��Ծ״̬������0Ϊδ������������Ծ�˼��Σ�-1����
    private bool toRight = true;
    private bool canMove = false;
    private bool canClimb = false;
    private bool canJump = false;

    private float horizontal = 0f;
    private float vertical = 0f;

    private Vector2 inputVector = Vector2.zero;
    private Vector2 moveDir = Vector2.zero;

    private float verticalVelocity = 0f; // �Զ�����ֱ�ٶ�
    private bool jumpRequested = false;

    // ����������¶����
    public bool IsGrounded_ { get => isGrounded; }
    public bool IsClimbing_ { get => isClimbing; }
    public bool IsSneaking_ { get => isSneaking; }
    public bool CanClimb_ { get => canClimb; }
    public bool CanJump_ { get => canJump; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; // ȷ������������������
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
        // ������Ծ��ť
        if (gameInput.Instance.GetJumpInputDown())
        {
            jumpRequested = true;
        }
        // ��������ť�߼�
        if (gameInput.Instance.InteractInputDown())
        {
            // ��������ӽ����߼�
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

        // ���������ƶ�
        Vector2 move = new Vector2(horizontal, 0) * speed * Time.fixedDeltaTime;

        // ��ֱ�������Զ����ٶȿ���
        move.y += verticalVelocity * Time.fixedDeltaTime;

        // ����ʱ��ֱ�������������
        if (isClimbing)
        {
            move.y = vertical * climbSpeed * Time.fixedDeltaTime;
            verticalVelocity = 0f; // ����ʱ��������Ӱ��
        }

        CheckCollision(move, move.magnitude);
        if (canMove)
        {
            rb.MovePosition(rb.position + move);
        }
    }

    private void HandleForce()
    {
        // ��Ծ�߼�
        if (isGrounded && jumpRequested)
        {
            verticalVelocity += jumpForce;
            isJumping = 1; //Ŀǰ��һ����
            isGrounded = false;
            jumpRequested = false; // ������Ծ����
        }

        // �����������
        if (!isGrounded && !isClimbing)
        {
            verticalVelocity -= gravity * Time.fixedDeltaTime;
        }

        // ���ʱ������ֱ�ٶ�
        if (isGrounded && !isClimbing)
        {
            verticalVelocity = 0f;
        }
    }

    private void HandleVisualLayer()
    {
        // �����Ӿ��㼶
        // ������
        Vector3 localScale = transform.localScale;
        if (toRight)
            localScale.x = Mathf.Abs(localScale.x);
        else
            localScale.x = -Mathf.Abs(localScale.x);
        transform.localScale = localScale;
    }

    private void CheckCollision(Vector2 moveDir, float moveDistance)
    {//��Ҫ���������ײ
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
