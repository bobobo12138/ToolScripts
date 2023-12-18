using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




/// <summary>
/// UIƽ�ƹ���
/// </summary>
public class Animation_UITranslate
{
    public RectTransform rectTransform;
    /// <summary>�Զ��巽��</summary>
    public Vector2 dir;

    /// <summary>�Ƿ����active״̬�²���</summary>
    public bool playOnlyInActive = false;
    public float duration = 0.5f;

    GameObject mask;
    Vector2 maskPos;
    /// <summary>
    /// ���캯��
    /// </summary>
    /// <param name="rectTransform"></param>
    /// <param name="mask">��ֹ�󴥵�mask</param>
    public Animation_UITranslate(RectTransform rectTransform, GameObject mask = null)
    {
        this.rectTransform = rectTransform;
        dir = Vector2.right;
        if (mask != null)
        {
            this.mask = mask;
            maskPos = mask.transform.position;
        }
    }

    public Animation_UITranslate(RectTransform rectTransform, Vector2 dir, GameObject mask = null)
    {
        this.rectTransform = rectTransform;
        this.dir = dir;
        if (mask != null)
        {
            this.mask = mask;
            maskPos = mask.transform.position;
        }
    }


    /// <summary>
    /// ����
    /// </summary>
    public void Play()
    {
        if (playOnlyInActive)
        {
            if (!rectTransform.gameObject.activeSelf) return;
        }

        rectTransform.anchoredPosition = new Vector2(rectTransform.rect.width, rectTransform.rect.height) * dir;
        if (mask != null)
            mask.transform.position = maskPos;
        rectTransform.DOAnchorPos(Vector2.zero, duration).OnComplete(() =>
        {

        });
    }

    public void Play(Action before, Action after)
    {
        if (playOnlyInActive)
        {
            if (!rectTransform.gameObject.activeSelf) return;
        }

        before?.Invoke();
        rectTransform.anchoredPosition = new Vector2(rectTransform.rect.width, rectTransform.rect.height) * dir;
        if (mask != null)
            mask.transform.position = maskPos;
        rectTransform.DOAnchorPos(Vector2.zero, duration).OnComplete(() =>
        {
            after?.Invoke();
        });
    }


    /// <summary>
    /// �ط�
    /// </summary>
    public void PlayBack()
    {
        if (playOnlyInActive)
        {
            if (!rectTransform.gameObject.activeSelf) return;
        }
        if (mask != null)
            mask.transform.position = maskPos;
        rectTransform.DOAnchorPos(new Vector2(rectTransform.rect.width, rectTransform.rect.height) * dir, duration).OnComplete(() =>
        {
        });
    }

    public void PlayBack(Action before, Action after)
    {
        if (playOnlyInActive)
        {
            if (!rectTransform.gameObject.activeSelf) return;
        }

        before?.Invoke();
        if (mask != null)
            mask.transform.position = maskPos;
        rectTransform.DOAnchorPos(new Vector2(rectTransform.rect.width, rectTransform.rect.height) * dir, duration).OnComplete(() =>
        {
            after?.Invoke();
        });
    }


    public void ResetUserPosition()
    {
        rectTransform.anchoredPosition = Vector2.zero;
    }
}
