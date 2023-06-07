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

    public Animation_UITranslate(RectTransform rectTransform)
    {
        this.rectTransform = rectTransform;
        dir = Vector2.right;
    }

    public Animation_UITranslate(RectTransform rectTransform, Vector2 dir)
    {
        this.rectTransform = rectTransform;
        this.dir = dir;
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
        rectTransform.DOAnchorPos(new Vector2(rectTransform.rect.width, rectTransform.rect.height) * dir, duration).OnComplete(() =>
        {
            after?.Invoke();
        });
    }
}
