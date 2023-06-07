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

    //public Action beforePlay;
    //public Action afterPlay;

    //public Action beforePlayBack;
    //public Action afterPlayBack;

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

        //beforePlay?.Invoke();
        rectTransform.anchoredPosition = new Vector2(rectTransform.rect.width, rectTransform.rect.height) * dir;
        rectTransform.DOAnchorPos(Vector2.zero, duration).OnComplete(() =>
        {
            //afterPlay?.Invoke();
        });
    }

    public void Play(Action before, Action after)
    {
        if (playOnlyInActive)
        {
            if (!rectTransform.gameObject.activeSelf) return;
        }

        //beforePlay?.Invoke();
        before?.Invoke();
        rectTransform.anchoredPosition = new Vector2(rectTransform.rect.width, rectTransform.rect.height) * dir;
        rectTransform.DOAnchorPos(Vector2.zero, duration).OnComplete(() =>
        {
            //afterPlay?.Invoke(); after?.Invoke();
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

        //beforePlayBack?.Invoke();
        rectTransform.DOAnchorPos(new Vector2(rectTransform.rect.width, rectTransform.rect.height) * dir, duration).OnComplete(() =>
        {
            //afterPlayBack?.Invoke();
        });
    }

    public void PlayBack(Action before, Action after)
    {
        if (playOnlyInActive)
        {
            if (!rectTransform.gameObject.activeSelf) return;
        }

        //beforePlayBack?.Invoke();
        before?.Invoke();
        rectTransform.DOAnchorPos(new Vector2(rectTransform.rect.width, rectTransform.rect.height) * dir, duration).OnComplete(() =>
        {
            //afterPlayBack?.Invoke(); after?.Invoke();
        });
    }
}
