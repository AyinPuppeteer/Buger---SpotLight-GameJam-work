using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//管理场景切换的脚本
public class FadeEvent : MonoBehaviour
{
    private Animator Anim;

    private string AimScene;//目标场景

    public static FadeEvent Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        Anim = GetComponent<Animator>();
    }

    //动画中调用的用于切换场景的函数
    private void LoadScene()
    {
        if(AimScene != null)
        {
            SceneManager.LoadScene(AimScene);
            AimScene = null;
        }
    }

    //切换至指定场景
    public void Fadeto(string scene)
    {
        AimScene = scene;
        Anim.SetTrigger("Anim");
    }
   
    //虚假切换（只有特效，不改变场景）
    public void FakeFade()
    {
        Anim.SetTrigger("Anim");
    }
}