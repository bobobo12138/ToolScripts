using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// page容器
/// 目前不支持页面类内部调用跳转，需要思考JumpToPage逻辑
/// 目前不支持中途改变事件
/// </summary>
[Serializable]
public class GBookUIContainer : MonoBehaviour
{
    public int nowOrder { get; private set; }
    public int pageCount { get { return gBookUIPages.Count; } }

    Action<object> onPageEnd_Head;   //页面结束时_从首页结束的回调，参数就是当前页data
    Action<object> onPageEnd_End;    //页面结束时_从尾页结束的回调，参数就是当前页data
    Action<int> onPageChange;        //参数是order
    Action<int> afterPageChange;     //参数是order

    [SerializeField]
    protected GameObject mask;//防止连续点击的遮罩
    [SerializeField]
    protected List<GBookUIPage> gBookUIPages = new List<GBookUIPage>();

    public void InitSet(Action<object> onPageEnd_Head = null, Action<object> onPageEnd_End = null, Action<int> onPageChange = null, Action<int> afterPageChange = null)
    {
        this.onPageEnd_Head += onPageEnd_Head;
        this.onPageEnd_End += onPageEnd_End;

        this.onPageChange += onPageChange;
        this.afterPageChange += afterPageChange;

        for (int i = 0; i < gBookUIPages.Count; i++)
        {
            var last = i > 0 ? gBookUIPages[i - 1] : null;
            var next = i < gBookUIPages.Count - 1 ? gBookUIPages[i + 1] : null;
            gBookUIPages[i].InitSet(i, this, this.onPageEnd_Head, this.onPageEnd_End, onPageChange, afterPageChange, last, next);
            gBookUIPages[i].ResetGroup();
        }
    }

    public void Refresh(object data = null)
    {
        foreach (var v in gBookUIPages)
        {
            v.Recycle();
            v.Hide();
        }
        mask.SetActive(false);
        gBookUIPages[0].Show(data);
    }

    /// <summary>
    /// 页面跳转暂时有问题，不支持页面中跳转，只能一开始设置
    /// </summary>
    /// <param name="order"></param>
    /// <param name="data"></param>
    /// <param name="isAnime"></param>
    public void JumpToPage(int order, object data = null, bool isAnime = true)
    {
        if (order < 0 || order >= gBookUIPages.Count)
        {
            Debug.LogError("跳转页数超出范围");
            return;
        }
        gBookUIPages[order].Show(data, isAnime ? 0 : order);
    }

    public void ShowMask(bool isShow = true)
    {
        mask.SetActive(isShow);
    }
}
