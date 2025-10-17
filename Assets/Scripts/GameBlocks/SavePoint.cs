using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SavePoint : MonoBehaviour, I_PickItem
{
    private SpriteRenderer Image;

    public Vector3 Position { get => transform.position; }
    
    public void Pick()
    {
        //�л�ͼƬ�򲥷Ŷ���

        GameManager.Instance.SetSavePoint(this);
    }
}