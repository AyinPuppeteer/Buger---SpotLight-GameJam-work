using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���ӵĽű�
public class Ladder : MonoBehaviour
{
    [Min(1)]
    public int Length;//���ȣ���λΪ��

    public SpriteRenderer SpR;
    public BoxCollider2D Collider;

    private void OnValidate()
    {
        SpR.size = new(0.16f, Length * 0.16f);
        Collider.offset = new(0.08f, Length * 0.08f);
        Collider.size = new(0.16f, Length * 0.16f);
    }
}