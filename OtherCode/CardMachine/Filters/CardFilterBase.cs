using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 卡片筛选抽象类
/// </summary>
public abstract class CardFilterBase
{
    /// <summary>
    /// 主要用于预先剔除
    /// </summary>
    /// <param name="selectedData"></param>
    /// <returns></returns>
    public abstract LinkedList<SingleCardData> Filter (LinkedList<SingleCardData> selectedData);

}
