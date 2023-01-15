using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CardMachine : MonoBehaviour
{
    public List<SingleCardData> cardDatas = new List<SingleCardData>();



    public List<SingleCardData> GetCard(int num,float nowP_Weight)//取出数量，当前权重曲线位置
    {
        LinkedList<SingleCardData> selectedData = new LinkedList<SingleCardData>();

        ///先根据特殊的筛选条件剔除
        ///抽象出筛选条件类














        ///依据权重取出
        ///先获得所有通过筛选的卡片
        ///根据他们的权重制作一个link
        ///重新使用一个list来装载selectedData，方便根据index取出
        LinkedList<float> cardIndexList_filtered = new LinkedList<float>();
        List<SingleCardData> cardList_cardIndexList_filtered = new List<SingleCardData>();
        float weightCount=0;//总权重
        foreach (var v in selectedData)
        {
            cardIndexList_filtered.AddLast(v.default_P_Weight + v.GetPFromCurve(nowP_Weight));
            cardList_cardIndexList_filtered.Add(v);
            weightCount += v.default_P_Weight + v.GetPFromCurve(nowP_Weight);
        }

        ///从list中取出num个元素，记录他们的位置


        LinkedList<int> cardIndexList_final = new LinkedList<int>();
        for (int i = 0; i < num; i++)
        {
            ///随机数和累加数
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

        ///由处理后的cardIndexList_final根据其装载的index取得最终的卡片
        List<SingleCardData> cardList_final = new List<SingleCardData>();
        foreach (var v in cardIndexList_final)
        {
            cardList_final.Add(cardList_cardIndexList_filtered[v]);
        }
        return cardList_final;
    }
}
