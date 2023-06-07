using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




/// <summary>
/// UI平移工具
/// </summary>
public class Animation_UITranslate
{
    public RectTransform rectTransform;
    /// <summary>自定义方向</summary>
    public Vector2 dir;

    /// <summary>是否仅在active状态下播放</summary>
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
    /// 运行
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
    /// 回放
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
