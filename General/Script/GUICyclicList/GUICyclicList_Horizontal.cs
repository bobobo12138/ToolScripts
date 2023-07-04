using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RectTransform;

/// <summary>
/// ���ѭ���б�
/// �������ڴ�ֱ����
/// ��Ҫ���odin
/// ICyclicItem
/// ʵ������ѭ���߼����ɱ�����ƣ�ʵ����ʱ�᷵�ػص�onOnePrefab_ItemCreated�������ٴ�ʱ����item����
/// </summary>
public class GUICyclicList_Horizontal : GUICyclicList_Base
{
    public int count { get { return num_V * (num_H + catchNum); } }//��ǰ����
    [HideInInspector]
    public Dictionary<int, ICyclicItem> itemDic;//������0��ʼ

    #region ��Ҫ����
    [Header("Adaptive:��rect�����������ֶ���;Sequential:˳����rect")]
    [Header("ע������Sequential������ø���Ļ�����")]
    public CyclicListCalculateMode cyclicListCalculateMode = CyclicListCalculateMode.Adaptive;//���㷽ʽ��Ĭ��Ϊ����Ӧ
    public GameObject prefab_Item;
    public ICyclicItem prefab_Item_ICyclicItem;
    [HideInInspector]
    public int maxIndex;//�������

    [Header("����")]
    [SerializeField]
    public int num_H = 2;
    [SerializeField]
    public int num_V = 2;
    [Header("������������1-3")]
    [SerializeField]
    public int catchNum = 1;//�ݴ�����
    [Header("�ײ�����")]
    [SerializeField]
    public int buttomSpacing = 200;


    [Header("�������Ҽ��")]
    [SerializeField]
    Vector2 LR = Vector2.zero;
    [SerializeField]
    Vector2 TB = Vector2.zero;
    [SerializeField]
    int spacing;
    [Header("��Ԫ���С")]
    [SerializeField]
    protected Vector2 itemSize;
    #endregion;

    bool isLoad = false;
    /// <summary> �����ͨ����β�н��м��� </summary>
    int headIndex;  //��
    int endIndex;   //β
    float xincrement
    {
        get
        {
            if (cyclicListCalculateMode == CyclicListCalculateMode.Sequential)
            {
                return itemSize.x;
            }
            return ((rectTransform.rect.width - LR.x - LR.y) - (num_H - 1) * spacing) / (num_H);
        }
    }//x����
    float yincrement
    {
        get
        {
            return ((rectTransform.rect.height - TB.x - TB.y) - (num_V - 1) * spacing) / (num_V);
        }
    }//y����

    RectTransform rectTransform;
    ScrollRect scrollRect;
    /// <summary>
    /// ��ͨ��SetData������inspector�и�ֵ
    /// ע������SetData��������Init֮ǰ
    /// </summary>
    public void SetData(GameObject prefab_Item, int maxIndex, int num_H, int num_V, int catchNum_H, Vector2 lR, Vector2 tB, int spacing, Vector2 itemSize)
    {
        this.prefab_Item = prefab_Item;
        prefab_Item_ICyclicItem = prefab_Item.GetComponent<ICyclicItem>();
        if (prefab_Item_ICyclicItem == null)
        {
            Debug.LogError("����prefabδ�̳�ICyclicItem");
            return;
        }
        this.maxIndex = maxIndex;
        this.num_H = num_H;
        this.num_V = num_V;
        this.catchNum = catchNum_H;
        LR = lR;
        TB = tB;
        this.spacing = spacing;
        this.itemSize = itemSize;
    }



    protected virtual void OnInit()
    {

    }
    /// <summary>
    /// ��Init��Ҫ����start��ִ��
    /// onInstantiate��һ��prefab�����ʵ���������ظ�prefab
    /// </summary>
    public void InstantiateItem(Action<ICyclicItem> onInstantiate = null, int Index = -1)
    {
        OnInit();
        if (Index != -1) this.maxIndex = Index;
        prefab_Item_ICyclicItem = prefab_Item.GetComponent<ICyclicItem>();
        if (prefab_Item_ICyclicItem == null)
        {
            Debug.LogError("����prefabδ�̳�ICyclicItem");
            return;
        }
        rectTransform = GetComponent<RectTransform>();
        scrollRect = GetComponent<ScrollRect>();
        if (scrollRect == null) Debug.LogError("��Ҫ�����ڰ���scrollRect��������");
        Vector2 last = Vector2.zero;
        scrollRect.onValueChanged.AddListener((pos) =>
        {
            if (!isLoad) return;
            if (pos.x > last.x)
            {
                //����
                IsCyclicMove_Right();
            }
            else
            {
                //����
                IsCyclicMove_Left();
            }
            last = pos;
        });

        //�Զ���������
        if (autoCalculateNum)
        {
            Canvas.ForceUpdateCanvases();
            var width = rectTransform.rect.width - LR.x - LR.y;
            var height = rectTransform.rect.height - TB.x - TB.y;
            num_H = Mathf.Max(1, (int)Mathf.Floor((width + spacing) / (itemSize.x + spacing)));//y=bx+(x-1)a��y�ܿ�x��Ԫ��������b��Ԫ���ȣ�a��������������ǵ�y,b,a������֪�ģ���x
            num_V = Mathf.Max(1, (int)Mathf.Floor((height + spacing) / (itemSize.y + spacing)));
        }
        if (num_H <= 0 || num_V <= 0)
        {
            Debug.LogError("�޷������κ�item���ᵼ�³���Ϊ0��");
            return;
        }

        itemDic = new Dictionary<int, ICyclicItem>();
        for (int i = 0; i < num_H + catchNum; i++)
        {
            for (int j = 0; j < num_V; j++)
            {
                var tempItem = Instantiate(prefab_Item, scrollRect.content).GetComponent<ICyclicItem>();
                onInstantiate?.Invoke(tempItem);
                tempItem.SetGroupData(this);
                tempItem.InitSet(itemSize.x, itemSize.y);
                tempItem.GetRectTransform().anchoredPosition = new Vector2(GetIndexPosX(i + 1), GetIndexPosY(j + 1));
                if (i * num_V + j >= maxIndex)//�������ֵ�����ò��ɼ�
                {
                    tempItem.GetRectTransform().gameObject.SetActive(false);
                }
                else
                {
                    tempItem.SetIndex(i * num_V + j);
                    tempItem.GetRectTransform().gameObject.SetActive(true);
                }
                tempItem.GetRectTransform().SetSizeWithCurrentAnchors(Axis.Horizontal, itemSize.x);
                tempItem.GetRectTransform().SetSizeWithCurrentAnchors(Axis.Vertical, itemSize.y);
                tempItem.GetRectTransform().anchorMin = new Vector2(0f, 0.5f);//���ö���ê��
                tempItem.GetRectTransform().anchorMax = new Vector2(0f, 0.5f);
                RefreshSize(tempItem.GetRectTransform());
                itemDic.Add(i * num_V + j, tempItem);
            }
        }
        headIndex = 0;
        endIndex = count - num_V;
        isLoad = true;
        Debug.Log("��ʼ��GUICyclicList");
    }

    protected virtual void OnRefresh()
    {

    }

    public void Refresh()//���ˣ��ǵü��ˢ��
    {
        OnRefresh();

        Dictionary<int, ICyclicItem> tempDic = new Dictionary<int, ICyclicItem>();
        int i = 0;
        foreach (var v in itemDic)
        {
            tempDic.Add(i, v.Value);
            v.Value.GetRectTransform().anchoredPosition = new Vector2(GetIndexPosX((i / num_V) + 1), GetIndexPosY((i % num_H) + 1));
            RefreshSize(v.Value.GetRectTransform());
            if (i >= maxIndex)
            {
                v.Value.GetRectTransform().gameObject.SetActive(false);
            }
            else
            {
                //Debug.Log(i);
                v.Value.SetIndex(i);
                v.Value.GetRectTransform().gameObject.SetActive(true);
            }
            i++;
        }
        headIndex = 0;
        endIndex = count - num_V;
        itemDic = tempDic;
        scrollRect.content.anchoredPosition = Vector2.zero;
        Debug.Log("ˢ��GUICyclicList");
    }


    //private void Update()
    //{
    //    if (isLoad)
    //    {
    //        IsCyclicMove_Right();
    //        IsCyclicMove_Left();
    //    }
    //    //Debug.Log(p.position);
    //    //Debug.Log(p.parent.TransformPoint(p.localPosition));
    //}
    #region ��ʼ����ʱ����XYλ��
    /// <summary>
    /// ��������Index��X��
    /// </summary>
    /// <param name="increment"></param>
    /// <param name="index">�е�index��Ԫ�أ���dic��ͬ����С��1������Ҫ+1</param>
    /// <returns></returns>
    float GetIndexPosX(int index)
    {
        float increment = xincrement;//����
        return 0 + LR.x + (spacing * (index - 1)) + ((increment / 2) + (index - 1) * increment);//��rect.with/2��ʼ���㣬��ΪXê��������
    }

    /// <summary>
    /// ��������Index��Y��
    /// </summary>
    /// <param name="index">�е�index��Ԫ�أ���dic��ͬ����С��1������Ҫ+1</param>
    /// <returns></returns>
    float GetIndexPosY(int index)
    {
        float increment = yincrement;//����
        return (rectTransform.rect.height / 2) - TB.x - (spacing * (index - 1)) - ((increment / 2) + (index - 1) * increment);//��0���꿪ʼ���㣬��Ϊ�Ӽ�Yê���ڶ���
    }

    #endregion;
    #region ѭ���б�����ƶ��߼�
    /// <summary>
    /// �Ƿ������ƶ�
    /// </summary>
    void IsCyclicMove_Right()
    {
        var limit = (int)transform.parent.TransformPoint
                    (
                        new Vector3(rectTransform.localPosition.x - rectTransform.rect.width / 2 - itemSize.x / 2, 0, 0)
                    ).x;
        int ori = -1;
        int target = -1;
        //�ж����Ƿ���Ҫȥ��
        if (itemDic[headIndex].GetRectTransform().transform.position.x < limit)
        {
            ori = headIndex;
            target = headIndex + count;
        }

        if (ori == -1 && target == -1) return;
        if (target >= maxIndex) return;
        if (target < 0) return;
        for (int i = 0; i < num_V; i++)
        {
            var temp = itemDic[ori + i];
            itemDic.Remove(ori + i);
            itemDic.Add(target + i, temp);

            //�����ƶ�
            temp.GetRectTransform().anchoredPosition = new Vector2(
            itemDic[target - num_V].GetRectTransform().anchoredPosition.x + xincrement + spacing,
            temp.GetRectTransform().anchoredPosition.y);
            //�������ֵ�����ò��ɼ�
            if (target + i >= maxIndex)
            {
                temp.GetRectTransform().gameObject.SetActive(false);
            }
            else
            {
                temp.SetIndex(target + i);
                temp.GetRectTransform().gameObject.SetActive(true);
            }
        }
        endIndex = target;
        headIndex = headIndex + num_V;
        RefreshSize(itemDic[endIndex].GetRectTransform());
        //Debug.Log("�����ƶ�");
    }

    /// <summary>
    /// �Ƿ������ƶ� 
    /// </summary>
    void IsCyclicMove_Left()
    {
        var limit = (int)transform.parent.TransformPoint
            (
                 new Vector3(rectTransform.localPosition.x + rectTransform.rect.width / 2 + itemSize.x / 2, 0, 0)
        ).x;
        int ori = -1;
        int target = -1;
        ///�ж����Ƿ���Ҫȥ��
        if (itemDic[endIndex].GetRectTransform().transform.position.x > limit)
        {
            ori = endIndex;
            target = endIndex - count;
        }

        if (ori == -1 && target == -1) return;
        if (target >= maxIndex) return;
        if (target < 0) return;
        for (int i = 0; i < num_V; i++)
        {
            var temp = itemDic[ori + i];
            itemDic.Remove(ori + i);
            itemDic.Add(target + i, temp);

            //�����ƶ�
            temp.GetRectTransform().anchoredPosition = new Vector2(
                itemDic[target + num_V].GetRectTransform().anchoredPosition.x - xincrement - spacing,
                temp.GetRectTransform().anchoredPosition.y);
            temp.GetRectTransform().gameObject.SetActive(true);
            temp.SetIndex(target + i);
        }
        headIndex = target;
        endIndex = endIndex - num_V;
        RefreshSize(itemDic[endIndex].GetRectTransform());
        //Debug.Log("�����ƶ�");
    }

    /// <summary>
    /// ˢ�»��������С
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="isRight"></param>
    protected void RefreshSize(RectTransform rect, bool isRight = true)
    {
        if (!rect.gameObject.activeSelf) return;
        var aimSize = Mathf.Abs(rect.anchoredPosition.x) + rect.rect.width / 2 + buttomSpacing;

        ///�Ƿ�����
        if (isRight)
        {
            if (aimSize > scrollRect.content.rect.width)
            {
                scrollRect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, aimSize);
            }
        }
        else
        {
            if (aimSize < scrollRect.content.rect.width)
            {
                scrollRect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, aimSize);
            }
        }

    }

    #endregion

    /// <summary>
    /// ��Ԫ�س�ʼ��ʱ�����
    /// </summary>
    /// <param name="item"></param>
    //protected abstract void OnItemInit(ICyclicItem item);
}
