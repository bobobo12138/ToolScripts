using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ȩ�طŴ���������
/// </summary>
public class SingleCardData_AfterAmplifier
{
    public SingleCardData singleCardData
    {
        get;
    }
    public float _animationCurve_P_Weight;
    public float AmplifierWeight;//����CardMachine��������ֵ��Ĭ��ΪsingleCardData.GetTotalWeight

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

    public List<CardFilterBase> filterList;//Ĭ����ɸѡ������Ҫ�ֶ����
    public List<WeightAmplifierBase> AmplifierList;//ӵ��Ĭ��Ȩ�طŴ���������ķŴ�����Ҫ�ֶ����
    public CardMachine(List<SingleCardData> cardDatas)
    {
        this.cardDatas = cardDatas;
        filterList = new List<CardFilterBase>();
        AmplifierList = new List<WeightAmplifierBase>();
    }

    /// <summary>
    /// ������ظ��Ŀ�Ƭ
    /// </summary>
    /// <param name="num"></param>
    /// <param name="_animationCurve_P_Weight">Ȩ������λ��</param>
    /// <returns></returns>
    public List<SingleCardData> GetCard_NotRepeatable(int num, float _animationCurve_P_Weight)
    {
        Debug.Log("�鿨��ʼ");

        var time = Time.realtimeSinceStartup;
        LinkedList<SingleCardData> selectedData = new LinkedList<SingleCardData>();
        //��ת��Ϊlinkedlist������ɾ
        foreach (var v in cardDatas)
        {
            selectedData.AddLast(v);
        }



        ///�ȸ��������ɸѡ�����޳�
        ///�����ɸѡ������
        foreach (var v in filterList)
        {
            v.Filter(selectedData);
        }



        ///�ٽ���Ȩ�ظ��Ĳ���
        ///�����Ȩ�طŴ�����
        ///ʹ��һ��LinkedList-singleCardData_AfterAmplifier��Ŵ���Ȩ�غ��ֵ,�Ӵ�������ȡ�����տ�Ƭ
        ///ʹ��һ��list-finalcardList��װ������ѡ��Ŀ�Ƭ
        LinkedList<SingleCardData_AfterAmplifier> selectedData_AfterAmplifier_Linklist = new LinkedList<SingleCardData_AfterAmplifier>();
        List<SingleCardData> finalcardList = new List<SingleCardData>();
        foreach (var v in selectedData)
        {
            ///ת��Ϊ�Ŵ����������������
            selectedData_AfterAmplifier_Linklist.AddLast(new SingleCardData_AfterAmplifier
                (
                singleCardData: v,
                _animationCurve_P_Weight: _animationCurve_P_Weight,
                AmplifierWeight: v.GetTotalWeight(_animationCurve_P_Weight)
                ));
        }
        ///ִ�зŴ���
        foreach (var v in AmplifierList)
        {
            selectedData_AfterAmplifier_Linklist =  v.Amplifier(selectedData_AfterAmplifier_Linklist);
        }
        


        ///�����selectedData_AfterAmplifier��ȡ��num������
        ///��ʼȡ��
        float allWeight = 0;
        foreach (var v in selectedData_AfterAmplifier_Linklist)
        {
            //������Ȩ��
            allWeight += v.AmplifierWeight;
        }
        for (int i = 0; i < num; i++)
        {
            //����Ȩ��ȡ����Ȩ��Խ��ȡ���Ŀ�����Խ��
            //��������ۼ���
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
                    Debug.Log("�ڵ����");
                    break;
                }
            }
        }
        time = Time.realtimeSinceStartup - time;
        Debug.Log("�鿨��������ʱ��" + time);
        return finalcardList;
    }


    /// <summary>
    /// ��������ظ��Ŀ�Ƭ
    /// </summary>
    /// <param name="num"></param>
    /// <param name="nowP_Weight">Ȩ������λ��</param>
    /// <returns></returns>
    public List<SingleCardData> GetCard_Repeatable(int num, float nowP_Weight)
    {
        return null;
    }
}
