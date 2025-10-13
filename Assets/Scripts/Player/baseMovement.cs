using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class baseMovement : MonoBehaviour
{//函数名驼峰命名，变量小写
    [Header("Movement Settings")]
    [SerializeField] private float originspeed = 5.0f;
    [SerializeField] private float sneakspeed = 1.0f;
    [Header("Collision Settings")]
    [SerializeField] private float playerwidth = .35f;
    [SerializeField] private float playerheight = .5f;

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
        if (movedir != Vector2.zero)
        {
            iswalking = true;
            speed = originspeed;
            if (movedir.x < 0) toright = false;
            else if (movedir.x > 0) toright = true;
        }
        else
        {
            iswalking = false;
            speed = 0;
        }
        float horizontal = inputvector.x;
        float movedistance = speed * Time.deltaTime;
        checkCollision(new Vector2(horizontal, 0), movedistance);
        if (canmove)
        {
            rb.MovePosition(rb.position + horizontal * movedistance);
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

        canmove = hit.collider.CompareTag("Obstacle");
    }
}
