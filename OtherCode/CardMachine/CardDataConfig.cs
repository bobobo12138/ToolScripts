using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class SingleCardData
{
    [Header("ΨһID")]
    public int ID;
    [Header("����")]
    public string name;
    [Header("��Ƭ����")]
    public CardTyple cardTyple;
    [Header("Ĭ�ϸ���Ȩ�أ�0-100")]
    [Range(0, 100)]
    public float default_P_Weight;
    [Header("����Ȩ�����ߣ�YΪ�����XΪ���루x���Կ��ɵȼ���������ȼ����仯��")]
    public AnimationCurve animationCurve_P_Weight;
    ///ĳ�������࣬����鵽����Ʒ
    ///public xxx




    /// <summary>
    /// �����߻�ø���
    /// </summary>
    /// <param name="_x">����x��ֵ���������ߵ�Y</param>
    public float GetPFromCurve(float _x)
    {
        if (_x > 1 || _x < 0)
        {
            Debug.Log("����x����");
            return 0;
        }

        return animationCurve_P_Weight.Evaluate(_x);
    }

    /// <summary>
    /// �����ܵ�Ȩ��
    /// </summary>
    /// <returns></returns>
    public float GetTotalWeight(float _x)
    {
        if (_x > 1 || _x < 0)
        {
            Debug.Log("����x����");
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

