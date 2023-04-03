using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ���׺��
/// </summary>
public class GRedPoint : MonoBehaviour
{
    public bool isRun;
    [SerializeField]
    [Header("���")]
    GameObject redPoint;
    [SerializeField]
    [Header("��㸸��")]
    GRedPoint gRedPointParent;
    [Header("����Ӽ�")]
    [SerializeField]
    List<GRedPoint> childGRedPoints = new List<GRedPoint>();

    /// <summary>
    /// �����Ӽ��б�ˢ���Ƿ��к��
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
            Debug.LogError("û�к��obj");
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
            Debug.LogError("û�к��obj");
            return;
        }

        redPoint.SetActive(false);
        isRun = false;
        gRedPointParent?.Refresh();
    }
    /// <summary>
    /// ���ú�㸸�����Ժ���Ӱ�츸���
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
