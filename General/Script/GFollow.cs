using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGFollow
{
    public RectTransform GetFollow();
}

/// <summary>
/// 简易跟随脚本
/// 暂时只支持UI
/// </summary>
public class GFollow : MonoBehaviour
{
    [SerializeField]
    RectTransform followTrans;
    RectTransform rectTransform;

    Vector2 temp;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        temp = rectTransform.anchoredPosition - followTrans.pivot;
    }

    public void ReSetFollowTrans(RectTransform _followTrans)
    {
        followTrans = _followTrans;
    }

    public void ReSetFollowTrans(IGFollow gFollow)
    {
        followTrans = gFollow.GetFollow();
    }

    private void LateUpdate()
    {
        rectTransform.anchoredPosition = followTrans.anchoredPosition + temp;
    }

}
