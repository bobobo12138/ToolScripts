using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// ��ʱ�ĵ�ѡ��,��ѡ������װ��ͬʱ����,û��װ��ʱֻ�Ǹ���ͨ��ť
/// ע�ⰴť���Ӽ��е�ס����ͼƬ�����Ӽ�ͼƬ���߼��ص����ɣ�
/// ��ʹ�ð�ťһ��ʹ��
/// </summary>
public class GRadioButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerClickHandler, IPointerUpHandler
{
    //+=ͼʡ�£�������Ҫ��Ϊaddlinsenter
    public Action onSelected;//�л��¼�,ʹ��+=
    public Action onExit;//�˳��¼�,ʹ��+=
    public GRadioButtonLoader gRadioButtonLoader;//ָ����飬���϶��ɴ��븳ֵ

    public bool interactable = true;
    public bool isAlwaysCanBeClick = false;//���ǿ��Ա�����������ӵ�ѡ��ť��ѡ�к󲻿��ٴε���Ĺ���
    public bool isAni = false;//�Ƿ�С����
    public bool isSelected;
    [SerializeField]
    bool isAwakeInit = false;//�Ƿ�awake��ʼ���������ֶ�����init��ʼ��


    //������ȡ��ʱ����ʾ
    [Header("����ʱ��ͼƬ/�ǽ���ʱ��ͼƬ")]
    [Header("Ϊ���������仯")]
    [SerializeField]
    protected GameObject clickTrans;
    [SerializeField]
    protected GameObject disTrans;

    [Header("�ƶ����˴�ʱ������")]
    [SerializeField]
    GameObject grayMask;


    Transform aniTrans;

    protected void Awake()
    {
        if (isAwakeInit) Init();
    }

    private void OnEnable()
    {
        transform.localScale = Vector3.one;
    }

    /// <summary>
    /// ��Ҫ�ֶ����ó�ʼ��
    /// </summary>
    public virtual void Init()
    {
        aniTrans = GetComponent<Transform>();

        /// ����Ŀ�е�ѡ���Ѿ��ػ�
        /// ������+=��ע���ʼ������
        /// ��Ҫԭ����ͼƬ��ʷ��¼�����齫ͼƬ��ʷ��¼��radioButton������������ֹλ��bug
        onSelected += () =>
        {
            SetSelected();
        };
        onExit += () =>
        {
            SetExit();
        };

        ///��ʼ��ʾ�˳�
        if (disTrans != null)
            disTrans.SetActive(true);
        if (clickTrans != null)
            clickTrans.SetActive(false);
    }
    /// <summary>
    /// ���������õĵ����¼�
    /// </summary>
    public void Click()
    {
        if (isAlwaysCanBeClick)
        {
            //����ִ�в���
            onSelected();
            if (gRadioButtonLoader.last != this)
            {
                if (gRadioButtonLoader.last != null)
                {
                    gRadioButtonLoader.last.onExit();
                }
                gRadioButtonLoader.last = this;
            }
            return;
        }

        if (gRadioButtonLoader == null)
        {
            onSelected();
        }
        else
        {
            //����һ����ť���Լ��������κβ���
            if (gRadioButtonLoader.last != this)
            {
                if (gRadioButtonLoader.last != null)
                {
                    gRadioButtonLoader.last.onExit();
                }
                gRadioButtonLoader.last = this;
                onSelected();
            }
        }
    }
    /// <summary>
    /// ������onSelected�¼���Click
    /// ���ⷽ������Ӧ�÷�������ߣ��������ü̳�
    /// </summary>
    public void Click_NoOnSelectedAction()
    {
        if (isAlwaysCanBeClick)
        {
            //����ִ�в���
            SetSelected();
            if (gRadioButtonLoader.last != this)
            {
                if (gRadioButtonLoader.last != null)
                {
                    gRadioButtonLoader.last.onExit();
                }
                gRadioButtonLoader.last = this;
            }
            return;
        }

        if (gRadioButtonLoader == null)
        {
            SetSelected();
        }
        else
        {
            if (gRadioButtonLoader.last != this)///ֵ�ı�ʱ
            {
                if (gRadioButtonLoader.last != null)
                {
                    gRadioButtonLoader.last.onExit();//ִ��������һ����exit
                }
                gRadioButtonLoader.last = this;
                SetSelected();
            }
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
    /// ˢ��
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

    #region �¼��ӿ�
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
    /// ��ʱ��Э�̶���
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
