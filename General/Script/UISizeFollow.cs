using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RectTransform;

/// <summary>
/// ¸úËæÄ³¸öUIµÄsize
/// </summary>
public class UISizeFollow : MonoBehaviour
{
    [SerializeField]
    RectTransform follow;
    Rect followerRect;

    RectTransform rectTransform;
    private void Awake()
    {
        rectTransform= GetComponent<RectTransform>();
        followerRect = new Rect(Vector2.zero, Vector2.zero);
    }

    void Update()
    {
        if (followerRect.size == follow.rect.size) return;

        followerRect = follow.rect;
        rectTransform.SetSizeWithCurrentAnchors(Axis.Horizontal, followerRect.width);
        rectTransform.SetSizeWithCurrentAnchors(Axis.Vertical, followerRect.height);
    }
}
