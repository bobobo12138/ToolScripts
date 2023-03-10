using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class SingleCardData
{
    [Header("唯一ID")]
    public int ID;
    [Header("名字")]
    public string name;
    [Header("卡片类型")]
    public CardTyple cardTyple;
    [Header("默认概率权重，0-100")]
    [Range(0, 100)]
    public float default_P_Weight;
    [Header("几率权重曲线，Y为输出，X为输入（x可以看成等级，概率随等级而变化）")]
    public AnimationCurve animationCurve_P_Weight;
    ///某个抽象类，代表抽到的物品
    ///public xxx




    /// <summary>
    /// 从曲线获得概率
    /// </summary>
    /// <param name="_x">传入x轴值，返回曲线的Y</param>
    public float GetPFromCurve(float _x)
    {
        if (_x > 1 || _x < 0)
        {
            Debug.Log("传入x错误");
            return 0;
        }

        return animationCurve_P_Weight.Evaluate(_x);
    }

    /// <summary>
    /// 返回总的权重
    /// </summary>
    /// <returns></returns>
    public float GetTotalWeight(float _x)
    {
        if (_x > 1 || _x < 0)
        {
            Debug.Log("传入x错误");
            return 0;
        }
        return default_P_Weight + animationCurve_P_Weight.Evaluate(_x);
    }

}


public enum CardTyple
{
    none,
    Normal,
    Weapon,
}

[CreateAssetMenu(menuName = "Config/CardDataConfig")]
public class CardDataConfig : ScriptableObject
{
    public List<SingleCardData> cardDatas = new List<SingleCardData>();

}

