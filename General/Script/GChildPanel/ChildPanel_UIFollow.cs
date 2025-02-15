using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 子面板基类
/// 或许需要修改，拥有重写的虚方法，同时采用委托
/// </summary>
public abstract class ChildPanel_UIFollow : GBaseMono
{
    //public bool isHaveRadioButton = false;

    ////本界面界面对应按钮，某些情况下会出现面板-对应-按钮的情况，调用按钮事件或许比强制show更合理
    ////可以手动设置值
    //[ShowIf("isHaveRadioButton",true)]
    //public GRadioButton clickRadioButton;

    [HideInInspector]
    public RectTransform rectTransform;
    [HideInInspector]
    public bool isShow;

    public bool isPullDownPanel = false;
    [EnableIf("isPullDownPanel", true)]
    [SerializeField]
    public UIFollowMouse uIFollowMouse;//跟随区域，要使用的话需要在Init中赋值,动画都是依托与此对象
    [EnableIf("isPullDownPanel", true)]
    [SerializeField]
    protected GRadioButton deChooseBG_Button;//背景按钮

    //子面板列表，用于切换子面板，子面板可无限嵌套，但是每次查找都要在子面板中遍历，还没做优化
    [SerializeField]
    protected List<ChildPanelBase> childPanels = new List<ChildPanelBase>();

    public sealed override void Init()
    {
        Canvas.ForceUpdateCanvases();
        rectTransform = GetComponent<RectTransform>();

        if (uIFollowMouse != null)
        {
            ///子类需要为uIFollowMouse初始化，若需要使用的话
            deChooseBG_Button.AddListener_OnSelected(uIFollowMouse.PlaySetMin);
            uIFollowMouse.AfterOffsetMax += AfterShowAni;
            uIFollowMouse.AfterOffsetMin += AfterHideAni;
            uIFollowMouse.AfterOffsetMin += () => { SetActive(false); };//若是 uIFollowMouse存在，动画会自动隐藏
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

    /// <summary>
    /// 获取当前childpanelbase下的子面板，和getcomponent思想一致
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetChildPanel<T>() where T : ChildPanelBase
    {
        foreach (var v in childPanels)
        {
            if (v is T) return v as T;
        }
        Debug.LogError("未找到子面板" + nameof(T));
        return null;
    }
    public void AddChildPanel(ChildPanelBase _childPanelBase)
    {
        if (!childPanels.Contains(_childPanelBase))
            childPanels.Add(_childPanelBase);
    }

    public void RemoveChildPanel(ChildPanelBase _childPanelBase)
    {
        if (childPanels.Contains(_childPanelBase))
            childPanels.Remove(_childPanelBase);
    }

    public virtual void Recycle() { }

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
