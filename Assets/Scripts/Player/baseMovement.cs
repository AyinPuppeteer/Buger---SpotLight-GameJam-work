using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class baseMovement : MonoBehaviour
{//函数名驼峰命名，变量小写
    [Header("Movement Settings")]
    [SerializeField] private float originspeed = 5.0f;
    [SerializeField] private float sneakspeed = 2.5f;   //蹲速
    [SerializeField] private float climbspeed = 2.0f;   //爬速
    [Header("Collision Settings")]
    [SerializeField] private float playerwidth = .35f;
    [SerializeField] private float playerheight = .5f;
    [SerializeField] private float deadzone = 0.1f;   //设置死区

    Rigidbody2D rb;

    float speed;
    private bool iswalking = false;
    private bool isclimbing = false;
    private bool issneaking = false;
    //private bool isjump[] = false;
    private bool toright = true;
    private bool canmove = false;
    private bool canclimb = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        handleMovement();
        handleVisualLayer();
    }

    private void handleMovement()
    {
        Vector2 inputvector = gameInput.Instance.GetMovementInput();
        Vector2 movedir = inputvector.normalized;
        float horizontal = inputvector.x;
        float vertical = inputvector.y;
        if (movedir != Vector2.zero)
        {
            if (horizontal < -deadzone) toright = false;
            else if (horizontal > deadzone) toright = true;
            if (vertical > deadzone && canclimb) isclimbing = true;
            else if (vertical < -0.5f) issneaking = true;
            else if (vertical > -deadzone) issneaking = false;
            speed = isclimbing ? climbspeed :
                issneaking ? sneakspeed : originspeed;
        }
        else
        {
            if (iswalking) iswalking = false;
            if (isclimbing) isclimbing = false;
            if (issneaking) issneaking = false;
            speed = 0;
        }
        if (!isclimbing) movedir = new Vector2(horizontal, 0); 
        float movedistance = speed * Time.deltaTime;
        checkCollision(movedir, movedistance);
        if (canmove)
        {
            Debug.Log(movedir);
            rb.MovePosition(rb.position + movedir * movedistance);
        }
    }

    private void handleVisualLayer()
    {
        // 处理视觉层级
        // 处理朝向
        Vector3 localscale = transform.localScale;
        if (toright)
            localscale.x = Mathf.Abs(localscale.x);
        else
            localscale.x = -Mathf.Abs(localscale.x);
        transform.localScale = localscale;
    }

    private void checkCollision(Vector2 movedir, float movedistance)
    {//主要解决前方碰撞
        RaycastHit2D hit = Physics2D.BoxCast(transform.position,
            new Vector2(playerwidth, playerheight),
            0,
            movedir,
            movedistance);
        if (hit.collider == null){ canmove = true; return; }

        canmove = !hit.collider.CompareTag("Obstacle");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Climber"))
        {
            canclimb = true;
        }
    }
}
