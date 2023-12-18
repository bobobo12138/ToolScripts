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

    GameObject mask;
    Vector2 maskPos;
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="rectTransform"></param>
    /// <param name="mask">防止误触的mask</param>
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
    /// 运行
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
    /// 回放
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
