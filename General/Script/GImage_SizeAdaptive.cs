using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RectTransform;

public class GImage_SizeAdaptive : MonoBehaviour
{
    [Header("基于父级的缩放")]
    public float scale = 0.95f;

    Image image;
    RectTransform parent;
    public void SetSprite(Sprite s)
    {
        if (parent == null) parent = transform.parent.GetComponent<RectTransform>();
        if (image == null) image = GetComponent<Image>();

        Canvas.ForceUpdateCanvases();

        image.sprite = s;
        SizeAdaptive();
    }

    public void SetNativeSize()
    {
        image.SetNativeSize();
    }

    public void Refresh()
    {
        if (image.sprite != null)
        {
            SizeAdaptive();
        }
    }

    private void Reset()
    {
        if (GetComponent<Image>() == null) { AprilDebug.LogError("添加Image组件！"); }
    }

    void SizeAdaptive()
    {
        image.SetNativeSize();
        //先等比例扩大平铺防止图片过小（获得平铺倍数）
        float maxTiled = Utils.MaxTiled(new Vector2(image.rectTransform.rect.width, image.rectTransform.rect.height), new Vector2(parent.rect.width, parent.rect.height));
        //然后进行尺寸限定
        var size = Utils.WHSizelimit(new Vector2(image.rectTransform.rect.width * maxTiled, image.rectTransform.rect.height * maxTiled), new Vector2(parent.rect.width, parent.rect.height));
        size *= scale;
        image.rectTransform.SetSizeWithCurrentAnchors(Axis.Horizontal, size.x);
        image.rectTransform.SetSizeWithCurrentAnchors(Axis.Vertical, size.y);
    }
}
