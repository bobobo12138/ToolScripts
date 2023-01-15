using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CardMachine : MonoBehaviour
{
    public List<SingleCardData> cardDatas = new List<SingleCardData>();



    public List<SingleCardData> GetCard(int num,float nowP_Weight)//ȡ����������ǰȨ������λ��
    {
        LinkedList<SingleCardData> selectedData = new LinkedList<SingleCardData>();

        ///�ȸ��������ɸѡ�����޳�
        ///�����ɸѡ������














        ///����Ȩ��ȡ��
        ///�Ȼ������ͨ��ɸѡ�Ŀ�Ƭ
        ///�������ǵ�Ȩ������һ��link
        ///����ʹ��һ��list��װ��selectedData���������indexȡ��
        LinkedList<float> cardIndexList_filtered = new LinkedList<float>();
        List<SingleCardData> cardList_cardIndexList_filtered = new List<SingleCardData>();
        float weightCount=0;//��Ȩ��
        foreach (var v in selectedData)
        {
            cardIndexList_filtered.AddLast(v.default_P_Weight + v.GetPFromCurve(nowP_Weight));
            cardList_cardIndexList_filtered.Add(v);
            weightCount += v.default_P_Weight + v.GetPFromCurve(nowP_Weight);
        }

        ///��list��ȡ��num��Ԫ�أ���¼���ǵ�λ��


        LinkedList<int> cardIndexList_final = new LinkedList<int>();
        for (int i = 0; i < num; i++)
        {
            ///��������ۼ���
            float temp_random = Random.Range(0, weightCount);
            float temp_count=0;
            foreach (var v in cardIndexList_filtered)
            {
                if (temp_random < v+ temp_count)
                {
                    cardIndexList_final.AddLast(i);
                    weightCount -= v;
                    break;
                }
                temp_count += v;
            }
        }

        ///�ɴ�����cardIndexList_final������װ�ص�indexȡ�����յĿ�Ƭ
        List<SingleCardData> cardList_final = new List<SingleCardData>();
        foreach (var v in cardIndexList_final)
        {
            cardList_final.Add(cardList_cardIndexList_filtered[v]);
        }
        return cardList_final;
    }
}
