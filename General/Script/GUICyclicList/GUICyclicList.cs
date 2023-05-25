using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ѭ���б�
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class GUICyclicList<T> : MonoBehaviour where T : ICyclicItem
{
    [HideInInspector]
    public List<T> items;
    public T prefab_Item;

    public bool interactable;


    [Header("����")]
    [SerializeField]
    int num_H = 2;
    [SerializeField]
    int num_V = 2;

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


    RectTransform rectTransform;


    /// <summary>
    /// ��ͨ��SetData������inspector�и�ֵ
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
    /// ��������Index��X��
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
