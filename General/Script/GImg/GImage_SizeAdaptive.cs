using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RectTransform;

public class GImage_SizeAdaptive : MonoBehaviour
{
    [Header("基于父级的缩放")]
    public float scale = 0.95f;
    [Header("是否SetNativeSize超出父级后才自适配")]
    public bool isSizeExceeds_SetNativeSize = false;

    Image image;
    RectTransform parent;
    RectTransform rectTransform;
    public void SetSprite(Sprite s)
    {
        if (parent == null) parent = transform.parent.GetComponent<RectTransform>();
        if (image == null) GetImage();
        rectTransform = transform.GetComponent<RectTransform>();
        //Canvas.ForceUpdateCanvases();

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

    public Image GetImage()
    {
        if (image == null) image = GetComponent<Image>();
        return image;
    }

    private void Reset()
    {
        if (GetComponent<Image>() == null) { AprilDebug.LogError("添加Image组件！"); }
    }

    void SizeAdaptive()
    {
        image.SetNativeSize();
        rectTransform.ForceUpdateRectTransforms();


        if (isSizeExceeds_SetNativeSize)
        {
            if (rectTransform.rect.width > parent.rect.width || rectTransform.rect.height > parent.rect.height)
            {
                Method();
            }
        }
        else
        {
            Method();
        }

        void Method()
        {

            //先等比例扩大平铺防止图片过小（获得平铺倍数）
            float maxTiled = Utils.MaxTiled(new Vector2(image.rectTransform.rect.width, image.rectTransform.rect.height), new Vector2(parent.rect.width, parent.rect.height));
            //然后进行尺寸限定
            var size = Utils.WHSizelimit(new Vector2(image.rectTransform.rect.width * maxTiled, image.rectTransform.rect.height * maxTiled), new Vector2(parent.rect.width, parent.rect.height));
            size *= scale;
            image.rectTransform.SetSizeWithCurrentAnchors(Axis.Horizontal, size.x);
            image.rectTransform.SetSizeWithCurrentAnchors(Axis.Vertical, size.y);
        }
    }



}
