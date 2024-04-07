using General;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace General
{
    public enum UISpriteLoaderState
    {
        Max,//最大适配，会超出parent
        Min,//最小适配，不会超出parent
    }


}


public class UISpriteLoader : MonoBehaviour
{
    public UISpriteLoaderState spriteLoaderState = UISpriteLoaderState.Min;

    RectTransform _rectTransform;
    public RectTransform rectTransform
    {
        get
        {
            if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
            return _rectTransform;
        }
        protected set
        {
            _rectTransform = value;
        }
    }
    [SerializeField]
    Text txt_NumShow;

    [SerializeField]
    Image image;
    [SerializeField]
    [Tooltip("img的尺寸参考父级")]
    RectTransform trans_ImgSizeLimit;



    private void Reset()
    {
        trans_ImgSizeLimit = GetComponent<RectTransform>();

        if (trans_ImgSizeLimit == null)
        {
            Debug.LogError("trans_ImgSizeLimit没有rectTransform");
        }
    }


    public void SetSprite(Sprite sprite, string num = null,int fontSize = 0)
    {
        image.sprite = sprite;
        image.SetNativeSize();

        switch (spriteLoaderState)
        {
            case UISpriteLoaderState.Max:
                Utils.MaxTiled(
                    userRectTransform: image.rectTransform,
                    user: new Vector2(image.rectTransform.rect.width, image.rectTransform.rect.height),
                    max: new Vector2(trans_ImgSizeLimit.rect.width, trans_ImgSizeLimit.rect.height));//进行最大平铺
                break;
            case UISpriteLoaderState.Min:
                Utils.MinTiled(
                    userRectTransform: image.rectTransform,
                    user: new Vector2(image.rectTransform.rect.width, image.rectTransform.rect.height),
                    max: new Vector2(trans_ImgSizeLimit.rect.width, trans_ImgSizeLimit.rect.height));//进行最小平铺
                break;
        }
        image.transform.localPosition = Vector2.zero;
        if (txt_NumShow != null && num !=null)
        {
            txt_NumShow.text = "X" + num;
            txt_NumShow.fontSize = fontSize;
            txt_NumShow.gameObject.SetActive(true);
        }
        else if (txt_NumShow != null)
        {
            txt_NumShow.gameObject.SetActive(true);
        }
    }

}
