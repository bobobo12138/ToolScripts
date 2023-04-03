using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 简易红点
/// </summary>
public class GRedPoint : MonoBehaviour
{
    public bool isRun;
    [SerializeField]
    [Header("红点")]
    GameObject redPoint;
    [SerializeField]
    [Header("红点父级")]
    GRedPoint gRedPointParent;
    [Header("红点子集")]
    [SerializeField]
    List<GRedPoint> childGRedPoints = new List<GRedPoint>();

    /// <summary>
    /// 根据子集列表刷新是否还有红点
    /// </summary>
    public void Refresh()
    {
        foreach (var v in childGRedPoints)
        {
            if (v.isRun)
            {
                TurnOn();
                break;
            }
        }
        TurnOff();
    }

    public void TurnOn()
    {
        if (redPoint == null)
        {
            Debug.LogError("没有红点obj");
            return;
        }

        redPoint.SetActive(true);
        isRun = true;
        gRedPointParent?.Refresh();
    }

    public void TurnOff()
    {
        if (redPoint == null)
        {
            Debug.LogError("没有红点obj");
            return;
        }

        redPoint.SetActive(false);
        isRun = false;
        gRedPointParent?.Refresh();
    }
    /// <summary>
    /// 设置红点父级，自红点会影响父红点
    /// </summary>
    /// <param name="gRedPointParent"></param>
    public void SetParent(GRedPoint gRedPointParent)
    {
        this.gRedPointParent = gRedPointParent;
    }


    public void AddChildRedPoint(GRedPoint gRedPointParent)
    {
        childGRedPoints.Add(gRedPointParent);
    }
}
