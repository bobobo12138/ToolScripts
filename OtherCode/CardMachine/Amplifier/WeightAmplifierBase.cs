using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 权重放大器类
/// 返回经过处理后的权重
/// </summary>
public abstract class WeightAmplifierBase
{
    /// <summary>
    /// 将会获得一个amplifierCardData，对需要修改权重的值进行修改
    /// </summary>
    /// <param name="amplifierCardData"></param>
    /// <returns></returns>
    public abstract LinkedList<SingleCardData_AfterAmplifier> Amplifier(LinkedList<SingleCardData_AfterAmplifier> amplifierCardData);
}
