using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 子集居中代码
/// 暂时不能用，没有考虑到rectTransform_R.position.x会变动
/// </summary>
public class GUICentering : MonoBehaviour
{
    public bool isUseOnStart = true;

    RectTransform _rectTransform;
    public RectTransform rectTransform
    {
        get
        {
            if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
            return _rectTransform;
        }
        private set
        {
            _rectTransform = value;
        }
    }

    [Header("居中的父级")]
    [SerializeField]
    RectTransform parent;
    [Header("所需要居中的元素")]
    [SerializeField]
    List<RectTransform> childRectTransforms;

    RectTransform rectTransform_L;
    RectTransform rectTransform_R;
    void Start()
    {
        //Centering();
    }

    void Reset()
    {
        childRectTransforms = GetComponentsInChildren<RectTransform>().ToList();
        parent = transform.parent.GetComponent<RectTransform>();
    }


    public void Centering()
    {
        rectTransform_L = null;
        rectTransform_R = null;

        if (childRectTransforms.Count < 2)
        {
            Debug.Log("元素过少，不执行居中");
            return;
        }

        //获取最左、右的元素
        childRectTransforms.Sort((x, y) =>
        {
            if (Utils.ConvertWorldToLocal(x.position, parent).x + x.rect.width > Utils.ConvertWorldToLocal(y.position, parent).x + y.rect.width)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        });

        rectTransform_L = childRectTransforms[0];
        rectTransform_R = childRectTransforms[childRectTransforms.Count - 1];


        var dif = ((Utils.ConvertLocalToWorld(rectTransform_R.localPosition + new Vector3(rectTransform_R.rect.width / 2, 0, 0), parent).x) -
              (Utils.ConvertLocalToWorld(rectTransform_L.localPosition - new Vector3(rectTransform_L.rect.width / 2, 0, 0), parent).x)) / 2;
        rectTransform.position = new Vector3(rectTransform_R.position.x - (dif / 2), rectTransform_R.position.y, rectTransform_R.position.z);
    }


}
