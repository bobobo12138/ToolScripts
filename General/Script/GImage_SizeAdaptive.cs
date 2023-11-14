using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RectTransform;

public class GImage_SizeAdaptive : MonoBehaviour
{

    [Header("���ڸ���������")]
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

    private void Reset()
    {
        if (GetComponent<Image>() == null) { Debug.LogError("���Image�����"); }
    }

    void SizeAdaptive()
    {
        image.SetNativeSize();
        //�ȵȱ�������ƽ�̷�ֹͼƬ��С�����ƽ�̱�����
        float maxTiled = Utils.MaxTiled(new Vector2(image.rectTransform.rect.width, image.rectTransform.rect.height), new Vector2(parent.rect.width, parent.rect.height));
        //Ȼ����гߴ��޶�
        var size = Utils.WHSizelimit(new Vector2(image.rectTransform.rect.width * maxTiled, image.rectTransform.rect.height * maxTiled), new Vector2(parent.rect.width, parent.rect.height));
        size *= scale;
        image.rectTransform.SetSizeWithCurrentAnchors(Axis.Horizontal, size.x);
        image.rectTransform.SetSizeWithCurrentAnchors(Axis.Vertical, size.y);
    }
}
