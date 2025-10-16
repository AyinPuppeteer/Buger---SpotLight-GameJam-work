using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Animator AlertAnim;

    private void Start()
    {
        //开始时，生成角色
    }

    //触发BUG时播放警告动画
    public void BugAlert()
    {
        AlertAnim.Play("BUG触发效果");
    }
}
