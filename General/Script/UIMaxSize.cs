using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RectTransform;

public class UIMaxSize : MonoBehaviour
{
    public int max;
    RectTransform rectTransform;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        if (rectTransform.rect.width > max)
        {
            Destroy(rectTransform.GetComponent<ContentSizeFitter>());
            rectTransform.SetSizeWithCurrentAnchors(Axis.Horizontal, max);
        }
    }

}
