using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RectTransform;
/// <summary>
/// ����rect�ߴ����õ���Ŀ����ͬ
/// </summary>
public class GSetSize : MonoBehaviour
{
    [SerializeField]
    RectTransform aim;
    [SerializeField]
    bool isUpdate=false;

    [SerializeField]
    bool isNotAnyDirection = true;//�����ⷽ��

    [SerializeField]
    Axis axis;

    [SerializeField]
    bool isFollowScale = false;
    void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (isNotAnyDirection)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(aim);
            transform.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(axis, axis == Axis.Vertical ? aim.rect.height : aim.rect.width);
        }
        else
        {
            transform.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(Axis.Horizontal, aim.rect.width);
            transform.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(Axis.Vertical, aim.rect.height);
        }

        if (isFollowScale)
        {
            transform.localScale = aim.localScale;
        }
    }

    private void LateUpdate()
    {
        if (!isUpdate)
        {
            return;
        }

        Refresh();
    }
}
