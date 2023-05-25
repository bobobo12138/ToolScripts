using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 多层循环列表
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class GUICyclicList<T> : MonoBehaviour where T : ICyclicItem
{
    [HideInInspector]
    public List<T> items;
    public T prefab_Item;

    public bool interactable;


    [Header("行列")]
    [SerializeField]
    int num_H = 2;
    [SerializeField]
    int num_V = 2;

    [Header("上线左右间隔")]
    [SerializeField]
    Vector2 LR = Vector2.zero;
    [SerializeField]
    Vector2 TB = Vector2.zero;
    [SerializeField]
    int spacing;
    [Header("单元格大小")]
    [SerializeField]
    Vector2 itemSize;


    RectTransform rectTransform;


    /// <summary>
    /// 可通过SetData或者在inspector中赋值
    /// </summary>
    public void SetData(T prefab_Item, int num_H, int num_V)
    {
        this.prefab_Item = prefab_Item;
        this.num_H = num_H;
        this.num_V = num_V;


    }


    //public abstract void Init();

    public void Init()
    {
        rectTransform = GetComponent<RectTransform>();
    }


    protected virtual void Start()
    {



    }

    void Update()
    {

    }

    /// <summary>
    /// 返回所在Index的X轴
    /// </summary>
    /// <param name="increment"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    float GetIndexPosX(int index)
    {
        float increment = ((rectTransform.rect.width - LR.x - LR.y) - (num_H - 1) * spacing) / (num_H + 1);
        return (-rectTransform.rect.width / 2) + LR.x + (spacing * (index - 1)) + ((increment / 2) * index);

    }
}
