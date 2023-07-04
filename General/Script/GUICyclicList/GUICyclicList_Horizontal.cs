using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RectTransform;

/// <summary>
/// 多层循环列表
/// 仅适用于垂直方向
/// 需要插件odin
/// ICyclicItem
/// 实例化、循环逻辑都由本类控制，实例化时会返回回调onOnePrefab_ItemCreated，可以再此时设置item参数
/// </summary>
public class GUICyclicList_Horizontal : GUICyclicList_Base
{
    public int count { get { return num_V * (num_H + catchNum); } }//当前总量
    [HideInInspector]
    public Dictionary<int, ICyclicItem> itemDic;//索引从0开始

    #region 必要参数
    [Header("Adaptive:将rect铺满，不出现多余;Sequential:顺序安置rect")]
    [Header("注意若是Sequential最好设置更大的缓冲区")]
    public CyclicListCalculateMode cyclicListCalculateMode = CyclicListCalculateMode.Adaptive;//计算方式，默认为自适应
    public GameObject prefab_Item;
    public ICyclicItem prefab_Item_ICyclicItem;
    [HideInInspector]
    public int maxIndex;//最大数量

    [Header("行列")]
    [SerializeField]
    public int num_H = 2;
    [SerializeField]
    public int num_V = 2;
    [Header("缓冲区，建议1-3")]
    [SerializeField]
    public int catchNum = 1;//容错数量
    [Header("底部距离")]
    [SerializeField]
    public int buttomSpacing = 200;


    [Header("上下左右间隔")]
    [SerializeField]
    Vector2 LR = Vector2.zero;
    [SerializeField]
    Vector2 TB = Vector2.zero;
    [SerializeField]
    int spacing;
    [Header("单元格大小")]
    [SerializeField]
    protected Vector2 itemSize;
    #endregion;

    bool isLoad = false;
    /// <summary> 程序会通过首尾行进行计算 </summary>
    int headIndex;  //首
    int endIndex;   //尾
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
    }//x增量
    float yincrement
    {
        get
        {
            return ((rectTransform.rect.height - TB.x - TB.y) - (num_V - 1) * spacing) / (num_V);
        }
    }//y增量

    RectTransform rectTransform;
    ScrollRect scrollRect;
    /// <summary>
    /// 可通过SetData或者在inspector中赋值
    /// 注意若有SetData则必须放在Init之前
    /// </summary>
    public void SetData(GameObject prefab_Item, int maxIndex, int num_H, int num_V, int catchNum_H, Vector2 lR, Vector2 tB, int spacing, Vector2 itemSize)
    {
        this.prefab_Item = prefab_Item;
        prefab_Item_ICyclicItem = prefab_Item.GetComponent<ICyclicItem>();
        if (prefab_Item_ICyclicItem == null)
        {
            Debug.LogError("错误，prefab未继承ICyclicItem");
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
    /// 此Init需要放在start中执行
    /// onInstantiate当一个prefab完成了实例化，返回该prefab
    /// </summary>
    public void InstantiateItem(Action<ICyclicItem> onInstantiate = null, int Index = -1)
    {
        OnInit();
        if (Index != -1) this.maxIndex = Index;
        prefab_Item_ICyclicItem = prefab_Item.GetComponent<ICyclicItem>();
        if (prefab_Item_ICyclicItem == null)
        {
            Debug.LogError("错误，prefab未继承ICyclicItem");
            return;
        }
        rectTransform = GetComponent<RectTransform>();
        scrollRect = GetComponent<ScrollRect>();
        if (scrollRect == null) Debug.LogError("需要放置于包含scrollRect的物体下");
        Vector2 last = Vector2.zero;
        scrollRect.onValueChanged.AddListener((pos) =>
        {
            if (!isLoad) return;
            if (pos.x > last.x)
            {
                //向右
                IsCyclicMove_Right();
            }
            else
            {
                //向左
                IsCyclicMove_Left();
            }
            last = pos;
        });

        //自动计算数量
        if (autoCalculateNum)
        {
            Canvas.ForceUpdateCanvases();
            var width = rectTransform.rect.width - LR.x - LR.y;
            var height = rectTransform.rect.height - TB.x - TB.y;
            num_H = Mathf.Max(1, (int)Mathf.Floor((width + spacing) / (itemSize.x + spacing)));//y=bx+(x-1)a，y总宽，x单元格数量，b单元格宽度，a间隔；在这里我们的y,b,a都是已知的，求x
            num_V = Mathf.Max(1, (int)Mathf.Floor((height + spacing) / (itemSize.y + spacing)));
        }
        if (num_H <= 0 || num_V <= 0)
        {
            Debug.LogError("无法容纳任何item，会导致除数为0！");
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
                if (i * num_V + j >= maxIndex)//超过最大值的设置不可见
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
                tempItem.GetRectTransform().anchorMin = new Vector2(0f, 0.5f);//设置顶部锚点
                tempItem.GetRectTransform().anchorMax = new Vector2(0f, 0.5f);
                RefreshSize(tempItem.GetRectTransform());
                itemDic.Add(i * num_V + j, tempItem);
            }
        }
        headIndex = 0;
        endIndex = count - num_V;
        isLoad = true;
        Debug.Log("初始化GUICyclicList");
    }

    protected virtual void OnRefresh()
    {

    }

    public void Refresh()//到此，记得检查刷新
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
        Debug.Log("刷新GUICyclicList");
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
    #region 初始运行时计算XY位置
    /// <summary>
    /// 返回所在Index的X轴
    /// </summary>
    /// <param name="increment"></param>
    /// <param name="index">行第index个元素，与dic不同，最小是1，所以要+1</param>
    /// <returns></returns>
    float GetIndexPosX(int index)
    {
        float increment = xincrement;//增量
        return 0 + LR.x + (spacing * (index - 1)) + ((increment / 2) + (index - 1) * increment);//从rect.with/2开始计算，因为X锚点在中心
    }

    /// <summary>
    /// 返回所在Index的Y轴
    /// </summary>
    /// <param name="index">列第index个元素，与dic不同，最小是1，所以要+1</param>
    /// <returns></returns>
    float GetIndexPosY(int index)
    {
        float increment = yincrement;//增量
        return (rectTransform.rect.height / 2) - TB.x - (spacing * (index - 1)) - ((increment / 2) + (index - 1) * increment);//从0坐标开始计算，因为子集Y锚点在顶部
    }

    #endregion;
    #region 循环列表核心移动逻辑
    /// <summary>
    /// 是否向右移动
    /// </summary>
    void IsCyclicMove_Right()
    {
        var limit = (int)transform.parent.TransformPoint
                    (
                        new Vector3(rectTransform.localPosition.x - rectTransform.rect.width / 2 - itemSize.x / 2, 0, 0)
                    ).x;
        int ori = -1;
        int target = -1;
        //判断左是否需要去右
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

            //向右移动
            temp.GetRectTransform().anchoredPosition = new Vector2(
            itemDic[target - num_V].GetRectTransform().anchoredPosition.x + xincrement + spacing,
            temp.GetRectTransform().anchoredPosition.y);
            //超过最大值的设置不可见
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
        //Debug.Log("向右移动");
    }

    /// <summary>
    /// 是否向左移动 
    /// </summary>
    void IsCyclicMove_Left()
    {
        var limit = (int)transform.parent.TransformPoint
            (
                 new Vector3(rectTransform.localPosition.x + rectTransform.rect.width / 2 + itemSize.x / 2, 0, 0)
        ).x;
        int ori = -1;
        int target = -1;
        ///判断右是否需要去左
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

            //向左移动
            temp.GetRectTransform().anchoredPosition = new Vector2(
                itemDic[target + num_V].GetRectTransform().anchoredPosition.x - xincrement - spacing,
                temp.GetRectTransform().anchoredPosition.y);
            temp.GetRectTransform().gameObject.SetActive(true);
            temp.SetIndex(target + i);
        }
        headIndex = target;
        endIndex = endIndex - num_V;
        RefreshSize(itemDic[endIndex].GetRectTransform());
        //Debug.Log("向左移动");
    }

    /// <summary>
    /// 刷新滑动区域大小
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="isRight"></param>
    protected void RefreshSize(RectTransform rect, bool isRight = true)
    {
        if (!rect.gameObject.activeSelf) return;
        var aimSize = Mathf.Abs(rect.anchoredPosition.x) + rect.rect.width / 2 + buttomSpacing;

        ///是否向右
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
    /// 当元素初始化时会调用
    /// </summary>
    /// <param name="item"></param>
    //protected abstract void OnItemInit(ICyclicItem item);
}
