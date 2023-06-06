using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum ChildPanelType
//{
//    normal,             //常规类型，无动画
//    bottomToCenterAni,  //从下往上的上拉框类型，从下往上的动画
//}

/// <summary>
/// 子面板基类
/// 或许需要修改，拥有重写的虚方法，同时采用委托
/// </summary>
public abstract class ChildPanelBase : GBaseMono
{
    //[SerializeField]
    //float aniSpeed = 1;
    //public ChildPanelType type;

    [HideInInspector]
    public RectTransform rectTransform;
    [HideInInspector]
    public bool isShow;
    [Header("是否总是显示")]
    public bool isAlwaysShow = false;


    [SerializeField]
    public UIFollowMouse uIFollowMouse;//跟随区域，要使用的话需要在Init中赋值,动画都是依托与此对象
    [SerializeField]
    protected GRadioButton deChooseBG_Button;//背景按钮

    public sealed override void Init()
    {
        Canvas.ForceUpdateCanvases();
        rectTransform = GetComponent<RectTransform>();

        if (uIFollowMouse != null)
        {
            ///子类需要为uIFollowMouse初始化，若需要使用的话
            deChooseBG_Button.onSelected += () => { uIFollowMouse.PlaySetMin(); };
            uIFollowMouse.AfterOffsetMax += AfterShowAni;
            uIFollowMouse.AfterOffsetMax += () =>
            {
                deChooseBG_Button.gameObject.SetActive(true);//设置挡板

            };//设置挡板}

            uIFollowMouse.AfterOffsetMin += AfterHideAni;
            uIFollowMouse.AfterOffsetMin += () =>
            {
                SetActive(false);
                deChooseBG_Button.gameObject.SetActive(false);//设置挡板
            };//若是 uIFollowMouse存在，动画会自动隐藏
        }

        OnInit();
    }
    /// <summary>
    /// 强制显示、隐藏
    /// 只有此接口会调用子面板的OnShow、OnHide
    /// 注意此方法会触发动画（若带有UIFollow组件的话），请勿在动画回调中触发此方法
    /// </summary>
    /// <param name="_isShow"></param>
    public void Show(bool _isShow = true)
    {
        ///根据_isShow调用OnShow或OnHide
        if (_isShow)
        {
            OnShow();
            if (uIFollowMouse != null)
                uIFollowMouse.PlaySetMax();
        }
        else
        {
            OnHide();
            if (uIFollowMouse != null)
            {
                uIFollowMouse.PlaySetMin();
                return;
            }
        }
        ///设置显示
        SetActive(_isShow);
    }

    /// <summary>
    /// 强制设置显示
    /// 此方法不会调动动画
    /// </summary>
    /// <param name="_isShow"></param>
    public void SetActive(bool _isShow = true)
    {
        isShow = _isShow;
        gameObject.SetActive(_isShow);
        if (deChooseBG_Button != null)
            deChooseBG_Button.gameObject.SetActive(_isShow);//设置挡板
    }

    ///可以在此处指定该面板的type
    protected abstract void OnInit();
    public sealed override void Refresh()
    {
        OnRefresh();
    }
    protected abstract void OnRefresh();
    protected virtual void OnShow() { }
    protected virtual void OnHide() { }
    protected virtual void OnShowAni() { }
    protected virtual void AfterShowAni() { }
    protected virtual void OnHideAni() { }
    protected virtual void AfterHideAni() { }



}
