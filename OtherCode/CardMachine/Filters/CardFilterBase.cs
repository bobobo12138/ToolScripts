using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ƭɸѡ������
/// </summary>
public abstract class CardFilterBase
{
    /// <summary>
    /// ��Ҫ����Ԥ���޳�
    /// </summary>
    /// <param name="selectedData"></param>
    /// <returns></returns>
    public abstract LinkedList<SingleCardData> Filter (LinkedList<SingleCardData> selectedData);

}
