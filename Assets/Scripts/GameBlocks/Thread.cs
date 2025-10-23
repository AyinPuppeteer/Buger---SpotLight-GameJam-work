using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Thread : MonoBehaviour, I_PickItem
{
    [SerializeField]
    private SpriteRenderer Image;
    [SerializeField]
    private Sprite GainedImg;//�ѻ�ȡʱͼƬ

    private string SceneName;//���ڳ���������

    [SerializeField]
    [Min(1)]
    private int ID;//���ڳ����ڵı��

    private bool Gained;//�Ƿ񱻻�ȡ

    private void Awake()
    {
        SceneName = SceneManager.GetActiveScene().name;
        if(GameSave.Instance.CheckThread(SceneName, ID))
        {
            //����Ϊ͸��״̬
            Image.sprite = GainedImg;
            Gained = true;
        }
    }

    public void Pick()
    {
        if (Gained) return;
        GameSave.Instance.GainThread(SceneName, ID);
        //�л�Ϊ͸��״̬
        Image.sprite = GainedImg;
        Gained = true;
    }
}