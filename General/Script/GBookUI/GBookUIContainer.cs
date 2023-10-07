using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// page容器
/// 目前不支持页面类内部调用跳转
/// 目前不支持中途改变事件
/// </summary>
[Serializable]
public class GBookUIContainer
{
    public int nowOrder { get; private set; }

    Action<object> onPageEnd_Head;   //页面结束时_从首页结束的回调，参数就是当前页data
    Action<object> onPageEnd_End;    //页面结束时_从尾页结束的回调，参数就是当前页data
    Action<int> onPageChange;        //参数是order
    Action<int> afterPageChange;     //参数是order

    [SerializeField]
    protected GameObject mask;//防止连续点击的遮罩
    [SerializeField]
    protected List<GBookUIPage> gBookUIPages = new List<GBookUIPage>();


    public void InitSet(object data = null, Action<object> onPageEnd_Head = null, Action<object> onPageEnd_End = null)
    {
        this.onPageEnd_Head += (data) => { };
        this.onPageEnd_End += (data) => { };
        this.onPageChange += (order) => { mask.SetActive(true); nowOrder = order; };
        this.afterPageChange += (order) => { mask.SetActive(false); nowOrder = order; };

        this.onPageEnd_Head += onPageEnd_Head;
        this.onPageEnd_End += onPageEnd_End;
        this.onPageChange += onPageChange;
        this.afterPageChange += afterPageChange;

        for (int i = 0; i < gBookUIPages.Count; i++)
        {
            var last = i > 0 ? gBookUIPages[i - 1] : null;
            var next = i < gBookUIPages.Count - 1 ? gBookUIPages[i + 1] : null;
            gBookUIPages[i].InitSet(i, this.onPageEnd_Head, this.onPageEnd_End, last, next);
            gBookUIPages[i].ResetGroup();
        }
        gBookUIPages[0].Show(data);
    }

    public void Refresh()
    {
        foreach (var v in gBookUIPages)
        {
            v.Recycle();
            v.Hide();
        }
    }

    public void JumpToPage(int order, object data = null, bool isAnime = true)
    {
        if (order < 0 || order >= gBookUIPages.Count)
        {
            Debug.LogError("跳转页数超出范围");
            return;
        }
        gBookUIPages[order].Show(data, isAnime ? 0 : order);
    }
}
