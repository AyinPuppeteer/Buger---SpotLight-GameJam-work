using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : Key, I_Interacts
{
    public void TakeInteract()
    {
        //����ͼƬ

        SetActive(!IsActive);//��ת����״̬
    }
}