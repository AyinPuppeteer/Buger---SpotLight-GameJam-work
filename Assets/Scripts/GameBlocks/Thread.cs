using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Thread : MonoBehaviour, I_PickItem
{
    [SerializeField]
    private SpriteRenderer Image;
    [SerializeField]
    private Sprite GainedImg;//已获取时图片

    private string SceneName;//所在场景的名字

    [SerializeField]
    [Min(1)]
    private int ID;//所在场景内的编号

    private bool Gained;//是否被获取

    private void Awake()
    {
        SceneName = SceneManager.GetActiveScene().name;
        if(GameSave.Instance.CheckThread(SceneName, ID))
        {
            //设置为透明状态
            Image.sprite = GainedImg;
            Gained = true;
        }
    }

    public void Pick()
    {
        if (Gained) return;
        GameSave.Instance.GainThread(SceneName, ID);
        //切换为透明状态
        Image.sprite = GainedImg;
        Gained = true;
    }
}