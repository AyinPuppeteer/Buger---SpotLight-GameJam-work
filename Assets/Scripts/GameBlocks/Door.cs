using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Lock
{
    [SerializeField]
    private SpriteRenderer Image;
    [SerializeField]
    private Sprite OpenImg, CloseImg;

    [SerializeField]
    private BoxCollider2D Collider;

    protected override void SetActive(bool b)
    {
        base.SetActive(b);

        if (b)
        {
            Image.sprite = OpenImg;
        }
        else
        {
            Image.sprite = CloseImg;
        }
        Collider.enabled = !b;
    }
}
