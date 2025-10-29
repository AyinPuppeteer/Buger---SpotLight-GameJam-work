using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WoveThread : MonoBehaviour, I_PickItem
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

    private void Start()
    {
        SceneName = SceneManager.GetActiveScene().name;
        Debug.Log(GameManager.Instance);
        if(GameSave.Instance.CheckWove(GameManager.Instance.LevelID, ID))
        {
            //����Ϊ͸��״̬
            Image.sprite = GainedImg;
            Gained = true;
        }
    }

    public void Pick()
    {
        if (Gained) return;
        GameSave.Instance.GainWove(GameManager.Instance.LevelID, ID);
        //�л�Ϊ͸��״̬
        Image.sprite = GainedImg;
        Gained = true;
    }
}