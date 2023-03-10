using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 无武器剔除器
/// </summary>
public class CardFilter_Noweapons : CardFilterBase
{
    public override LinkedList<SingleCardData> Filter(LinkedList<SingleCardData> selectedData)
    {
        var node = selectedData.First;
        while (true)
        {
            ///剔除武器类型的数据
            if (node.Value.cardTyple == CardTyple.Weapon)
            {
                var temp = node.Next;
                selectedData.Remove(node);
                node = temp;
            }
            else
            {
                node = node.Next;
            }

            if (node == null)
            {
                Debug.Log("武器剔除结束");
                break;
            }
        }
        return selectedData;
    }
}
