using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour, I_Interacts
{
    [SerializeField]
    private Teleport Outport;//����

    [SerializeField]
    private Transform EffTransform;//��Ч�������transform
    [SerializeField]
    private SpriteRenderer Image;

    public void TakeInteract()
    {
        if(Outport == null)
        {
            Destroy(gameObject);
        }
        else
        {
            BaseMovement.Instance.transform.position = Outport.transform.position;//����

            GameManager.Instance.DoTrans();
        }
    }

    private void OnTriggerEnter2D(Collider2D another)
    {
        if(another.GetComponent<BaseMovement>() != null)
        {
            EffTransform.DOScale(new Vector3(0.1f, 0.1f, 0.1f), 0.2f);
            Image.material.SetFloat("_RotateSpeed", 2);
        }
    }

    private void OnTriggerExit2D(Collider2D another)
    {
        if (another.GetComponent<BaseMovement>() != null)
        {
            EffTransform.DOScale(new Vector3(0.06f, 0.06f, 0.06f), 0.2f);
            Image.material.SetFloat("_RotateSpeed", 1);
        }
    }
}