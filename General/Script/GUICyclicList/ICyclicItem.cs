using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 循环元素所需继承的接口
/// </summary>
public interface ICyclicItem
{
    /// <summary>
    /// 会返回再列表中的位置
    /// 会在初始与发生交换时触发
    /// </summary>
    /// <param name="index"></param>
    void SetIndex(int index);
}
