using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// 多层循环列表
/// 仅适用于垂直方向
/// </summary>
/// <typeparam name="T">列表中的元素</typeparam>
public abstract class GUICyclicList<T> : MonoBehaviour where T : MonoBehaviour, ICyclicItem
{
    public bool interactable;
    public bool autoInit=false;
    public int count { get { return num_H * (num_V + catchNum_V); } }//当前总量
    [HideInInspector]
    public Dictionary<int, T> itemDic;//索引从0开始

    #region 必要参数
    public T prefab_Item;
    public int maxIndex;//最大数量

    [Header("行列")]
    [SerializeField]
    int num_H = 2;
    [SerializeField]
    int num_V = 2;
    [Header("列缓冲区，建议1-3")]
    [SerializeField]
    int catchNum_V = 1;//容错数量

    [Header("上下左右间隔")]
    [SerializeField]
    Vector2 LR = Vector2.zero;
    [SerializeField]
    Vector2 TB = Vector2.zero;
    [SerializeField]
    int spacing;
    [Header("单元格大小")]
    [SerializeField]
    Vector2 itemSize;
    #endregion;

    /// <summary> 程序会通过首尾行进行计算 </summary>
    int headIndex;  //首
    int endIndex;   //尾
    float xincrement { get { return ((rectTransform.rect.width - LR.x - LR.y) - (num_H - 1) * spacing) / (num_H); } }//x增量
    float yincrement { get { return ((rectTransform.rect.height - TB.x - TB.y) - (num_V - 1) * spacing) / (num_V); } }//y增量

    RectTransform rectTransform;
    ScrollRect scrollRect;

    /// <summary>
    /// 可通过SetData或者在inspector中赋值
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
    /// 此Init需要放在start中执行
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
                tempItem.SetIndex(i * num_H + j);//设置初始位置
                tempItem.GetRectTransform().rect.Set(0, 0, itemSize.x, itemSize.y);
                tempItem.GetRectTransform().anchorMin = new Vector2(0.5f, 1f);//设置顶部锚点
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


    #region 初始运行时计算XY位置
    /// <summary>
    /// 返回所在Index的X轴
    /// </summary>
    /// <param name="increment"></param>
    /// <param name="index">行第index个元素</param>
    /// <returns></returns>
    float GetIndexPosX(int index)
    {
        float increment = xincrement;//增量
        return (-rectTransform.rect.width / 2) + LR.x + (spacing * (index - 1)) + ((increment / 2) + (index - 1) * increment);//从rect.with/2开始计算，因为X锚点在中心
    }

    /// <summary>
    /// 返回所在Index的Y轴
    /// </summary>
    /// <param name="index">列第index个元素</param>
    /// <returns></returns>
    float GetIndexPosY(int index)
    {
        float increment = yincrement;//增量
        return 0 - TB.x - (spacing * (index - 1)) - ((increment / 2) + (index - 1) * increment);//从0开始计算，因为子集Y锚点在顶部
    }

    #endregion;


    #region 循环列表核心移动逻辑
    /// <summary>
    /// 是否向上移动
    /// </summary>
    void IsCyclicMove_Up()
    {
        var limit = (int)transform.GetComponent<RectTransform>().TransformVector
                    (
                        new Vector3(0, rectTransform.anchoredPosition.y + rectTransform.rect.min.y, 0)
                    ).y;
        int ori = -1;
        int target = -1;
        //判断尾是否需要去首
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

            //向上移动
            temp.GetRectTransform().anchoredPosition = new Vector2(
            temp.GetRectTransform().anchoredPosition.x,
            itemDic[target + num_H].GetRectTransform().anchoredPosition.y + yincrement + spacing);
            temp.SetActive(true);
        }

        headIndex = target;
        endIndex = endIndex - num_H;
        RefreshSize(itemDic[endIndex].GetRectTransform());
        //Debug.Log("向上移动");
    }

    /// <summary>
    /// 是否向下移动 
    /// </summary>
    void IsCyclicMove_Down()
    {
        var limit = (int)transform.GetComponent<RectTransform>().TransformVector
            (
                new Vector3(0, rectTransform.anchoredPosition.y + rectTransform.rect.max.y, 0)
            ).y;
        int ori = -1;
        int target = -1;
        ///判断首是否需要去尾
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

            //向下移动
            temp.GetRectTransform().anchoredPosition = new Vector2(
            temp.GetRectTransform().anchoredPosition.x,
            itemDic[target - num_H].GetRectTransform().anchoredPosition.y - yincrement - spacing);

            //超过最大值的设置不可见
            if (target + i >= maxIndex) temp.SetActive(false);
            else temp.SetActive(true);
        }
        endIndex = target;
        headIndex = headIndex + num_H;
        RefreshSize(itemDic[endIndex].GetRectTransform());
        //Debug.Log("向下移动");
    }

    /// <summary>
    /// 刷新滑动区域大小
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="isDown"></param>
    protected void RefreshSize(RectTransform rect, bool isDown = true)
    {
        if (!rect.gameObject.activeSelf) return;
        var aimSize = Mathf.Abs(rect.anchoredPosition.y) + rect.rect.height / 2;

        ///是否向下
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
