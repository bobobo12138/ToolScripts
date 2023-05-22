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

    public Action beforeAni;
    public Action afterAni;

    public Animation_UITranslate(RectTransform rectTransform)
    {
        this.rectTransform = rectTransform;
    }

    public void Play()
    {
        if (rectTransform.gameObject.activeSelf)//若已经显示则不播放动画
        {
            beforeAni?.Invoke();
            afterAni?.Invoke();
        }
        else
        {
            beforeAni?.Invoke();
            rectTransform.anchoredPosition = new Vector2(rectTransform.rect.width, 0);
            rectTransform.DOAnchorPos(Vector2.zero, 0.5f).OnComplete(() => { afterAni?.Invoke(); });

        }
    }
}
