using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//梯子的脚本
public class Ladder : MonoBehaviour
{
    [Min(1)]
    public int Length;//长度（单位为格）

    public SpriteRenderer SpR;
    public BoxCollider2D Collider;

    private void OnValidate()
    {
        SpR.size = new(0.16f, Length * 0.16f);
        Collider.offset = new(0.08f, Length * 0.08f);
        Collider.size = new(0.16f, Length * 0.16f);
    }
}