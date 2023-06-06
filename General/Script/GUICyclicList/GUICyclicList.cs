using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// ���ѭ���б�
/// �������ڴ�ֱ����
/// </summary>
/// <typeparam name="T">�б��е�Ԫ��</typeparam>
public abstract class GUICyclicList<T> : MonoBehaviour where T : MonoBehaviour, ICyclicItem
{
    public bool interactable;
    public bool autoInit=false;
    public int count { get { return num_H * (num_V + catchNum_V); } }//��ǰ����
    [HideInInspector]
    public Dictionary<int, T> itemDic;//������0��ʼ

    #region ��Ҫ����
    public T prefab_Item;
    public int maxIndex;//�������

    [Header("����")]
    [SerializeField]
    int num_H = 2;
    [SerializeField]
    int num_V = 2;
    [Header("�л�����������1-3")]
    [SerializeField]
    int catchNum_V = 1;//�ݴ�����

    [Header("�������Ҽ��")]
    [SerializeField]
    Vector2 LR = Vector2.zero;
    [SerializeField]
    Vector2 TB = Vector2.zero;
    [SerializeField]
    int spacing;
    [Header("��Ԫ���С")]
    [SerializeField]
    Vector2 itemSize;
    #endregion;

    /// <summary> �����ͨ����β�н��м��� </summary>
    int headIndex;  //��
    int endIndex;   //β
    float xincrement { get { return ((rectTransform.rect.width - LR.x - LR.y) - (num_H - 1) * spacing) / (num_H); } }//x����
    float yincrement { get { return ((rectTransform.rect.height - TB.x - TB.y) - (num_V - 1) * spacing) / (num_V); } }//y����

    RectTransform rectTransform;
    ScrollRect scrollRect;

    /// <summary>
    /// ��ͨ��SetData������inspector�и�ֵ
    /// </summary>
    protected void SetData(T prefab_Item, int maxIndex, int num_H, int num_V, int catchNum_V, Vector2 lR, Vector2 tB, int spacing, Vector2 itemSize)
    {
        this.prefab_Item = prefab_Item;
        this.maxIndex = maxIndex;
        this.num_H = num_H;
        this.num_V = num_V;
        this.catchNum_V = catchNum_V;
        LR = lR;
        TB = tB;
        this.spacing = spacing;
        this.itemSize = itemSize;
    }



    protected virtual void Start()
    {
        if (autoInit) Init();
    }


    /// <summary>
    /// ��Init��Ҫ����start��ִ��
    /// </summary>
    public void Init()
    {
        rectTransform = GetComponent<RectTransform>();
        scrollRect = GetComponent<ScrollRect>();
        itemDic = new Dictionary<int, T>();
        for (int i = 0; i < num_V + catchNum_V; i++)
        {
            for (int j = 0; j < num_H; j++)
            {
                var tempItem = Instantiate(prefab_Item, scrollRect.content);
                tempItem.GetRectTransform().anchoredPosition = new Vector2(GetIndexPosX(j + 1), GetIndexPosY(i + 1));
                tempItem.SetIndex(i * num_H + j);//���ó�ʼλ��
                tempItem.GetRectTransform().rect.Set(0, 0, itemSize.x, itemSize.y);
                tempItem.GetRectTransform().anchorMin = new Vector2(0.5f, 1f);//���ö���ê��
                tempItem.GetRectTransform().anchorMax = new Vector2(0.5f, 1f);
                itemDic.Add(i * num_H + j, tempItem);
            }
        }
        headIndex = 0;
        endIndex = count - num_H;
    }

    private void Update()
    {
        //Debug.Log(headIndex + "," + endIndex);
        IsCyclicMove_Up();
        IsCyclicMove_Down();

    }


    #region ��ʼ����ʱ����XYλ��
    /// <summary>
    /// ��������Index��X��
    /// </summary>
    /// <param name="increment"></param>
    /// <param name="index">�е�index��Ԫ��</param>
    /// <returns></returns>
    float GetIndexPosX(int index)
    {
        float increment = xincrement;//����
        return (-rectTransform.rect.width / 2) + LR.x + (spacing * (index - 1)) + ((increment / 2) + (index - 1) * increment);//��rect.with/2��ʼ���㣬��ΪXê��������
    }

    /// <summary>
    /// ��������Index��Y��
    /// </summary>
    /// <param name="index">�е�index��Ԫ��</param>
    /// <returns></returns>
    float GetIndexPosY(int index)
    {
        float increment = yincrement;//����
        return 0 - TB.x - (spacing * (index - 1)) - ((increment / 2) + (index - 1) * increment);//��0��ʼ���㣬��Ϊ�Ӽ�Yê���ڶ���
    }

    #endregion;


    #region ѭ���б�����ƶ��߼�
    /// <summary>
    /// �Ƿ������ƶ�
    /// </summary>
    void IsCyclicMove_Up()
    {
        var limit = (int)transform.GetComponent<RectTransform>().TransformVector
                    (
                        new Vector3(0, rectTransform.anchoredPosition.y + rectTransform.rect.min.y, 0)
                    ).y;
        int ori = -1;
        int target = -1;
        //�ж�β�Ƿ���Ҫȥ��
        if (itemDic[endIndex].GetRectTransform().transform.position.y < limit)
        {
            ori = endIndex;
            target = endIndex - count;
        }

        if (ori == -1 && target == -1) return;
        if (target > maxIndex) return;
        if (target < 0) return;
        for (int i = 0; i < num_H; i++)
        {
            var temp = itemDic[ori + i];
            itemDic.Remove(ori + i);

            temp.SetIndex(target + i);
            itemDic.Add(target + i, temp);

            //�����ƶ�
            temp.GetRectTransform().anchoredPosition = new Vector2(
            temp.GetRectTransform().anchoredPosition.x,
            itemDic[target + num_H].GetRectTransform().anchoredPosition.y + yincrement + spacing);
            temp.SetActive(true);
        }

        headIndex = target;
        endIndex = endIndex - num_H;
        RefreshSize(itemDic[endIndex].GetRectTransform());
        //Debug.Log("�����ƶ�");
    }

    /// <summary>
    /// �Ƿ������ƶ� 
    /// </summary>
    void IsCyclicMove_Down()
    {
        var limit = (int)transform.GetComponent<RectTransform>().TransformVector
            (
                new Vector3(0, rectTransform.anchoredPosition.y + rectTransform.rect.max.y, 0)
            ).y;
        int ori = -1;
        int target = -1;
        ///�ж����Ƿ���Ҫȥβ
        if (itemDic[headIndex].GetRectTransform().transform.position.y > limit)
        {
            ori = headIndex;
            target = headIndex + count;
        }

        if (ori == -1 && target == -1) return;
        if (target > maxIndex) return;
        if (target < 0) return;
        for (int i = 0; i < num_H; i++)
        {
            var temp = itemDic[ori + i];
            itemDic.Remove(ori + i);

            temp.SetIndex(target + i);
            itemDic.Add(target + i, temp);

            //�����ƶ�
            temp.GetRectTransform().anchoredPosition = new Vector2(
            temp.GetRectTransform().anchoredPosition.x,
            itemDic[target - num_H].GetRectTransform().anchoredPosition.y - yincrement - spacing);

            //�������ֵ�����ò��ɼ�
            if (target + i >= maxIndex) temp.SetActive(false);
            else temp.SetActive(true);
        }
        endIndex = target;
        headIndex = headIndex + num_H;
        RefreshSize(itemDic[endIndex].GetRectTransform());
        //Debug.Log("�����ƶ�");
    }

    /// <summary>
    /// ˢ�»��������С
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="isDown"></param>
    protected void RefreshSize(RectTransform rect, bool isDown = true)
    {
        if (!rect.gameObject.activeSelf) return;
        var aimSize = Mathf.Abs(rect.anchoredPosition.y) + rect.rect.height / 2;

        ///�Ƿ�����
        if (isDown)
        {
            if (aimSize > scrollRect.content.rect.height)
            {
                scrollRect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, aimSize);
            }
        }
        else
        {
            if (aimSize < scrollRect.content.rect.height)
            {
                scrollRect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, aimSize);
            }
        }

    }

    #endregion


}
