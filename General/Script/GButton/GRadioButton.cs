using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// 单选项,单选框必须和装载同时出现,没有装载时只是个普通按钮
/// 注意按钮的子集有挡住它的图片（将子集图片射线检测关掉即可）
/// 像使用按钮一样使用
/// </summary>
public class GRadioButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerClickHandler, IPointerUpHandler
{
    //+=图省事，后续需要改为addlinsenter
    protected Action onSelected;//切换事件
    protected Action onExit;//退出事件
    protected Action onRepeatedSelected;//当重复选择发生时
    public GRadioButtonLoader gRadioButtonLoader;//指向的组，可拖动可代码赋值

    //单击与取消时的显示
    [Header("焦点时的图片/非焦点时的图片")]
    [Header("为空则代表不会变化")]
    [SerializeField]
    protected GameObject clickTrans;
    [SerializeField]
    protected GameObject disTrans;

    [Header("移动到此处时的遮罩")]
    [SerializeField]
    GameObject grayMask;

    public bool interactable = true;
    public bool isAni = false;//是否小动画
    public bool isSelected;
    [SerializeField]
    bool isAwakeInit = false;//是否awake初始化，建议手动调用init初始化


    [Header("动画细节参数")]
    [SerializeField]
    bool isCustomTrans_Ani = false;
    [Tooltip("自定义动画根节点；未赋值的话，默认会以transform为参数")]
    [ShowIf("isCustomTrans_Ani", true)]
    [SerializeField]
    Transform _trans_Ani;
    Transform trans_Ani
    {
        get
        {
            if (!isCustomTrans_Ani) return transform;
            if (_trans_Ani == null) _trans_Ani = transform;
            return _trans_Ani;
        }
        set
        {
            _trans_Ani = value;
        }
    }
    [SerializeField]
    [Range(0, 1)]
    float aniScaleValue = 0.9f;//形变尺寸
    float inc_AniScaleValue;//形变增量
    const int aniStep = 5;

    protected void Reset()
    {
        trans_Ani = transform;
    }

    protected void Awake()
    {
        if (isAwakeInit) Init();
    }

    /// <summary>
    /// 需要手动代用初始化
    /// </summary>
    public virtual void Init()
    {
        inc_AniScaleValue = (1 - aniScaleValue) / aniStep;
        RemoveAllAction();
        ///初始显示退出
        if (disTrans != null)
            disTrans.SetActive(true);
        if (clickTrans != null)
            clickTrans.SetActive(false);
    }

    public void RemoveAllAction()
    {
        onSelected = null;
        onExit = null;
        onRepeatedSelected = null;
    }

    public void AddListener_OnSelected(Action action,bool isRemoveLastAction = false)
    {
        if (isRemoveLastAction)
        {
            onSelected = action;
        }
        else
        {
            onSelected += action;
        }
    }

    public void AddListener_OnExit(Action action, bool isRemoveLastAction = false)
    {
        if (isRemoveLastAction)
        {
            onExit = action;
        }
        else
        {
            onExit += action;
        }
    }

    public void AddListener_OnRepeatedSelected(Action action, bool isRemoveLastAction = false)
    {
        if (isRemoveLastAction)
        {
            onRepeatedSelected = action;
        }
        else
        {
            onRepeatedSelected += action;
        }
    }




    /// <summary>
    /// 可主动调用的单击事件
    /// </summary>
    public void Click()
    {
        if (gRadioButtonLoader == null)
        {
            onSelected?.Invoke();
            SetSelected();
        }
        else
        {
            if (gRadioButtonLoader.last != this)///值改变时
            {
                if (gRadioButtonLoader.last != null)
                {
                    gRadioButtonLoader.last.onExit?.Invoke();//执行组内上一个的exit
                    gRadioButtonLoader.last.SetExit();
                }
                gRadioButtonLoader.last = this;
                onSelected?.Invoke();
                SetSelected();
            }
            else
            {
                onRepeatedSelected?.Invoke();//执行重复选择
            }
        }
    }
    /// <summary>
    /// 不调用onSelected事件的Click
    /// </summary>
    public void Click_NoOnSelectedAction()
    {
        if (gRadioButtonLoader == null)
        {
            SetSelected();
        }
        else
        {
            if (gRadioButtonLoader.last != this)///值改变时
            {
                if (gRadioButtonLoader.last != null)
                {
                    gRadioButtonLoader.last.onExit?.Invoke();//执行组内上一个的exit
                    gRadioButtonLoader.last.SetExit();
                }
                gRadioButtonLoader.last = this;
                SetSelected();
            }
        }
    }

    /// <summary>
    /// 强制点击
    /// </summary>
    public void Click_Force()
    {
        if (gRadioButtonLoader == null)
        {
            onSelected?.Invoke();
            SetSelected();
        }
        else
        {
            if (gRadioButtonLoader.last != null)
            {
                gRadioButtonLoader.last.onExit?.Invoke();//执行组内上一个的exit
                gRadioButtonLoader.last.SetExit();
            }
            gRadioButtonLoader.last = this;
            onSelected?.Invoke();
            SetSelected();
        }
    }

    protected void SetSelected()
    {
        if (disTrans != null)
            disTrans.SetActive(false);
        if (clickTrans != null)
            clickTrans.SetActive(true);
        isSelected = true;
    }
    protected void SetExit()
    {
        if (disTrans != null)
            disTrans.SetActive(true);
        if (clickTrans != null)
            clickTrans.SetActive(false);

        isSelected = false;
    }
    /// <summary>
    /// 刷新
    /// </summary>
    public void ReFresh()
    {
        if (disTrans != null)
            disTrans.SetActive(true);
        if (clickTrans != null)
            clickTrans.SetActive(false);
        if (grayMask != null)
            grayMask.SetActive(false);

        if (gRadioButtonLoader != null)
        {
            if (gRadioButtonLoader.last != null)
            {
                gRadioButtonLoader.last = null;
            }
        }

        isSelected = false;
    }

    #region 事件接口
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!interactable) return;

        if (grayMask != null)
            grayMask.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!interactable) return;

        if (grayMask != null)
            grayMask.SetActive(false);
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (!interactable) return;
        if (!isAni) return;
        StopAllCoroutines();
        StartCoroutine(Ani_Down());
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!interactable) return;
        Click();
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!interactable) return;
        if (!isAni) return;
        StopAllCoroutines();
        StartCoroutine(Ani_Up());

    }

    /// <summary>
    /// 暂时的协程动画
    /// </summary>
    IEnumerator Ani_Down()
    {
        int count = aniStep;
        trans_Ani.localScale = Vector3.one;
        while (count > 0)
        {
            trans_Ani.localScale -= Vector3.one * inc_AniScaleValue;
            count--;
            yield return 0;
        }
        yield return 0;
    }

    IEnumerator Ani_Up()
    {
        trans_Ani.localScale = Vector3.one * aniScaleValue;
        int count = aniStep;
        while (count > 0)
        {
            trans_Ani.localScale += Vector3.one * inc_AniScaleValue;
            count--;
            yield return 0;
        }
        yield return 0;
    }


    #endregion
}
