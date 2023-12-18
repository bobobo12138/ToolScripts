using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// 暂时的单选项,单选框必须和装载同时出现,没有装载时只是个普通按钮
/// 注意按钮的子集有挡住它的图片（将子集图片射线检测关掉即可）
/// 像使用按钮一样使用
/// </summary>
public class GRadioButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerClickHandler, IPointerUpHandler
{
    //+=图省事，后续需要改为addlinsenter
    public Action onSelected;//切换事件,使用+=
    public Action onExit;//退出事件,使用+=
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
    Transform aniTrans;

    protected void Awake()
    {
        if (isAwakeInit) Init();
    }

    /// <summary>
    /// 需要手动代用初始化
    /// </summary>
    public virtual void Init()
    {
        aniTrans = GetComponent<Transform>();

        /// 本项目中单选框已经特化
        /// 不再是+=，注意初始化问题
        /// 主要原因是图片历史记录，建议将图片历史记录的radioButton独立出来，防止位置bug
        onSelected += () =>
        {
            SetSelected();
        };
        onExit += () =>
        {
            SetExit();
        };

        ///初始显示退出
        if (disTrans != null)
            disTrans.SetActive(true);
        if (clickTrans != null)
            clickTrans.SetActive(false);
    }

    public void RemoveAllAction()
    {
        onSelected = () =>
        {
            SetSelected();
        };
        onExit = () =>
        {
            SetExit();
        };
    }

    /// <summary>
    /// 可主动调用的单击事件
    /// </summary>
    public void Click()
    {
        if (gRadioButtonLoader == null)
        {
            onSelected();
        }
        else
        {
            if (gRadioButtonLoader.last != this)///值改变时
            {
                if (gRadioButtonLoader.last != null)
                {
                    gRadioButtonLoader.last.onExit();//执行组内上一个的exit
                }
                gRadioButtonLoader.last = this;
                onSelected();
            }
        }
    }
    /// <summary>
    /// 不调用onSelected事件的Click
    /// 特殊方法，不应该放在这里边，后续采用继承
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
                    gRadioButtonLoader.last.onExit();//执行组内上一个的exit
                }
                gRadioButtonLoader.last = this;
                SetSelected();
            }
        }
    }

    /// <summary>
    /// </summary>
    public void Click_Force()
    {
        if (gRadioButtonLoader == null)
        {
            onSelected();
        }
        else
        {
            if (gRadioButtonLoader.last != null)
            {
                gRadioButtonLoader.last.onExit();//执行组内上一个的exit
            }
            gRadioButtonLoader.last = this;
            onSelected();
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
        int count = 5;
        transform.localScale = Vector3.one;
        while (count > 0)
        {
            transform.localScale -= Vector3.one * 0.02f;
            count--;
            yield return 0;
        }
        yield return 0;
    }

    IEnumerator Ani_Up()
    {
        transform.localScale = Vector3.one * 0.9f;
        int count = 5;
        while (count > 0)
        {
            transform.localScale += Vector3.one * 0.02f;
            count--;
            yield return 0;
        }
        yield return 0;
    }


    #endregion
}
