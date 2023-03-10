using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 权重放大器处理类
/// </summary>
public class SingleCardData_AfterAmplifier
{
    public SingleCardData singleCardData
    {
        get;
    }
    public float _animationCurve_P_Weight;
    public float AmplifierWeight;//若由CardMachine调动，此值会默认为singleCardData.GetTotalWeight

    public SingleCardData_AfterAmplifier(SingleCardData singleCardData, float _animationCurve_P_Weight,float AmplifierWeight)
    {
        this.singleCardData = singleCardData;
        this._animationCurve_P_Weight = _animationCurve_P_Weight;
        this.AmplifierWeight = AmplifierWeight;
    }
}

public class CardMachine
{
    public List<SingleCardData> cardDatas
    {
        get;
    }

    public List<CardFilterBase> filterList;//默认无筛选器，需要手动添加
    public List<WeightAmplifierBase> AmplifierList;//拥有默认权重放大器，额外的放大器需要手动添加
    public CardMachine(List<SingleCardData> cardDatas)
    {
        this.cardDatas = cardDatas;
        filterList = new List<CardFilterBase>();
        AmplifierList = new List<WeightAmplifierBase>();
    }

    /// <summary>
    /// 抽出不重复的卡片
    /// </summary>
    /// <param name="num"></param>
    /// <param name="_animationCurve_P_Weight">权重曲线位置</param>
    /// <returns></returns>
    public List<SingleCardData> GetCard_NotRepeatable(int num, float _animationCurve_P_Weight)
    {
        Debug.Log("抽卡开始");

        var time = Time.realtimeSinceStartup;
        LinkedList<SingleCardData> selectedData = new LinkedList<SingleCardData>();
        //先转换为linkedlist方便增删
        foreach (var v in cardDatas)
        {
            selectedData.AddLast(v);
        }



        ///先根据特殊的筛选条件剔除
        ///抽象的筛选条件类
        foreach (var v in filterList)
        {
            v.Filter(selectedData);
        }



        ///再进行权重更改操作
        ///抽象的权重放大器类
        ///使用一个LinkedList-singleCardData_AfterAmplifier存放处理权重后的值,从此链表中取出最终卡片
        ///使用一个list-finalcardList来装载最终选择的卡片
        LinkedList<SingleCardData_AfterAmplifier> selectedData_AfterAmplifier_Linklist = new LinkedList<SingleCardData_AfterAmplifier>();
        List<SingleCardData> finalcardList = new List<SingleCardData>();
        foreach (var v in selectedData)
        {
            ///转换为放大器所需的数据类型
            selectedData_AfterAmplifier_Linklist.AddLast(new SingleCardData_AfterAmplifier
                (
                singleCardData: v,
                _animationCurve_P_Weight: _animationCurve_P_Weight,
                AmplifierWeight: v.GetTotalWeight(_animationCurve_P_Weight)
                ));
        }
        ///执行放大器
        foreach (var v in AmplifierList)
        {
            selectedData_AfterAmplifier_Linklist =  v.Amplifier(selectedData_AfterAmplifier_Linklist);
        }
        


        ///随机从selectedData_AfterAmplifier中取出num个数据
        ///开始取出
        float allWeight = 0;
        foreach (var v in selectedData_AfterAmplifier_Linklist)
        {
            //计算总权重
            allWeight += v.AmplifierWeight;
        }
        for (int i = 0; i < num; i++)
        {
            //根据权重取出，权重越大取出的可能性越大
            //随机数和累加数
            float temp_random = Random.Range(0, allWeight);
            float temp_count = 0;

            var node = selectedData_AfterAmplifier_Linklist.First;
            while (true)
            {
                if (temp_random < node.Value.AmplifierWeight + temp_count)
                {
                    finalcardList.Add(node.Value.singleCardData);
                    allWeight -= node.Value.AmplifierWeight;
                    selectedData_AfterAmplifier_Linklist.Remove(node);
                    break;
                }
                temp_count += node.Value.AmplifierWeight;
                node = node.Next;
                if (node == null)
                {
                    Debug.Log("节点溢出");
                    break;
                }
            }
        }
        time = Time.realtimeSinceStartup - time;
        Debug.Log("抽卡结束，耗时：" + time);
        return finalcardList;
    }


    /// <summary>
    /// 抽出可以重复的卡片
    /// </summary>
    /// <param name="num"></param>
    /// <param name="nowP_Weight">权重曲线位置</param>
    /// <returns></returns>
    public List<SingleCardData> GetCard_Repeatable(int num, float nowP_Weight)
    {
        return null;
    }
}
