using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RectTransform;

/// <summary>
/// 不规则布局组
/// </summary>
public class GIrregularLayoutGroup : MonoBehaviour, ILayoutGroup
{
    [HideInInspector]
    RectTransform _rectTransform;
    public RectTransform rectTransform
    {
        get
        {
            if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
            return _rectTransform;
        }
    }
    public Axis createAxis = Axis.Horizontal;//创建方向，默认从左到右
    public Axis sortAxis = Axis.Horizontal;  //不规则排序方向，默认从左到右
    public RectOffset padding;
    public float spacing = 10;
    public bool isAutoSize = true;//是否自动计算大小
    //public GridLayoutGroup


    /// <summary>
    /// 计算
    /// </summary>
    public void Calculate()
    {
        List<RectTransform> childs = new List<RectTransform>();
        for (int i = 0; i < rectTransform.childCount; i++)
        {
            childs.Add(rectTransform.GetChild(i) as RectTransform);
        }

        float maxSize = 0;
        Axis maxSizeAxis = Axis.Vertical;

        if (createAxis == Axis.Horizontal && sortAxis == Axis.Horizontal)
        {
            maxSizeAxis = Axis.Vertical;
            //从左到右依次创建，从左到右依次排序，超过边界的换行
            float x = padding.left;
            float y = padding.top;
            float maxHeight = 0;

            for (int i = 0; i < childs.Count; i++)
            {
                RectTransform child = childs[i];
                if (x + child.sizeDelta.x > rectTransform.rect.width - padding.right)
                {
                    x = padding.left;
                    y += maxHeight + spacing;
                    maxHeight = 0;
                }

                child.anchoredPosition = new Vector2(x + (child.rect.width / 2), -y - (child.rect.height / 2));
                x += child.sizeDelta.x + spacing;
                maxHeight = Mathf.Max(maxHeight, child.sizeDelta.y);
                maxSize = Mathf.Abs(child.anchoredPosition.y) + (child.rect.height / 2) + padding.bottom;
            }
        }
        else if (createAxis == Axis.Vertical && sortAxis == Axis.Vertical)
        {
            maxSizeAxis = Axis.Horizontal;
            //从上到下依次创建，从上到下依次排序，超过边界的换行
            float x = padding.left;
            float y = padding.top;
            float maxWidth = 0;

            for (int i = 0; i < childs.Count; i++)
            {
                RectTransform child = childs[i];
                if (y + child.sizeDelta.y > rectTransform.rect.height - padding.bottom)
                {
                    y = padding.top;
                    x += maxWidth + spacing;
                    maxWidth = 0;
                }

                child.anchoredPosition = new Vector2(x + (child.rect.width / 2), -y - (child.rect.height / 2));
                y += child.sizeDelta.y + spacing;
                maxWidth = Mathf.Max(maxWidth, child.sizeDelta.x);
                maxSize = Mathf.Abs(child.anchoredPosition.x) + (child.rect.width / 2) + padding.right;
            }
        }
        else if (createAxis == Axis.Vertical && sortAxis == Axis.Horizontal)
        {
            maxSizeAxis = Axis.Horizontal;
            //从上到下依次创建，从左到右依次排序，超过边界的换行
            float x = padding.left;
            float y = padding.top;
            int verticalItemIndex = 0;

            for (int i = 0; i < childs.Count; i++)
            {
                RectTransform child = childs[i];

                if (y + child.sizeDelta.y > rectTransform.rect.height - padding.bottom)
                {
                    verticalItemIndex = verticalItemIndex == 0 ? i : verticalItemIndex;
                    y = padding.top;
                }

                float lastWidth = verticalItemIndex != 0 ? (childs[i - verticalItemIndex].rect.width / 2) + childs[i - verticalItemIndex].anchoredPosition.x : 0;
                child.anchoredPosition = new Vector2(x + (child.rect.width / 2) + lastWidth + spacing, -y - (child.rect.height / 2));
                y += child.sizeDelta.y + spacing;
                maxSize = Mathf.Max(Mathf.Abs(child.anchoredPosition.x) + (child.rect.width / 2) + padding.right, maxSize);
            }

        }
        else if (createAxis == Axis.Horizontal && sortAxis == Axis.Vertical)
        {
            //从左到右依次创建，从上到下依次排序，超过边界的换行

            Debug.LogError("芜湖，暂不支持");
        }

        if (isAutoSize) rectTransform.SetSizeWithCurrentAnchors(maxSizeAxis, maxSize);
        Debug.Log("计算完毕");
    }

    void ILayoutController.SetLayoutHorizontal()
    {
        Calculate();
    }

    void ILayoutController.SetLayoutVertical()
    {
        Calculate();
    }
}
