using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// �������޳���
/// </summary>
public class CardFilter_Noweapons : CardFilterBase
{
    public override LinkedList<SingleCardData> Filter(LinkedList<SingleCardData> selectedData)
    {
        var node = selectedData.First;
        while (true)
        {
            ///�޳��������͵�����
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
                Debug.Log("�����޳�����");
                break;
            }
        }
        return selectedData;
    }
}
