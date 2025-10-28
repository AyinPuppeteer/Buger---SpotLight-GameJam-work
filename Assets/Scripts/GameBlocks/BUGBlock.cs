using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//管理BUG方块的脚本
public class BUGBlock : Lock
{
    [SerializeField]
    private SpriteRenderer Image;
    [SerializeField]
    private Sprite ShowImg, UnShowImg;

    [SerializeField]
    private BoxCollider2D Collider;

    protected override void SetActive(bool b)
    {
        base.SetActive(b);
        if (b)
        {
            Image.sprite = ShowImg;
            Collider.enabled = true;
        }
        else
        {
            Image.sprite = UnShowImg;
            Collider.enabled = false;
        }
    }
}