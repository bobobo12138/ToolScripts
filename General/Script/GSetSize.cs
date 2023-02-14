using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RectTransform;
/// <summary>
/// 将本rect尺寸设置的与目标相同
/// </summary>
public class GSetSize : MonoBehaviour
{
    [SerializeField]
    RectTransform aim;
    [SerializeField]
    Axis axis;

    void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(aim);
        transform.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(axis, axis == Axis.Vertical? aim.rect.height: aim.rect.width);
    }

}
