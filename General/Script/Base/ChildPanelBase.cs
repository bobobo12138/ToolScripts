using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum ChildPanelType
//{
//    normal,             //�������ͣ��޶���
//    bottomToCenterAni,  //�������ϵ����������ͣ��������ϵĶ���
//}

/// <summary>
/// ��������
/// ������Ҫ�޸ģ�ӵ����д���鷽����ͬʱ����ί��
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

    [SerializeField]
    public UIFollowMouse uIFollowMouse;//��������Ҫʹ�õĻ���Ҫ��Init�и�ֵ
    [SerializeField]
    protected GRadioButton deChooseBG_Button;//������ť

    public sealed override void Init()
    {
        Canvas.ForceUpdateCanvases();
        rectTransform = GetComponent<RectTransform>();

        if (uIFollowMouse != null)
        {
            ///������ҪΪuIFollowMouse��ʼ��������Ҫʹ�õĻ�
            deChooseBG_Button.onSelected += () => { uIFollowMouse.PlaySetMin(); };
            uIFollowMouse.AfterOffsetMax += AfterShowAni;
            uIFollowMouse.AfterOffsetMin += AfterHideAni;
        }

        OnInit();
    }
    /// <summary>
    /// ǿ����ʾ������
    /// ֻ�д˽ӿڻ����������OnShow��OnHide
    /// </summary>
    /// <param name="_isShow"></param>
    public void Show(bool _isShow=true)
    {
        ///����_isShow����OnShow��OnHide
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
                uIFollowMouse.PlaySetMin();
        }
        ///������ʾ
        isShow = _isShow;
        gameObject.SetActive(_isShow);
        if (deChooseBG_Button != null)
            deChooseBG_Button.gameObject.SetActive(_isShow);//���õ���
    }
    ///�����ڴ˴�ָ��������type
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
