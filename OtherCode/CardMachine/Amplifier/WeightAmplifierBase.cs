using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Ȩ�طŴ�����
/// ���ؾ���������Ȩ��
/// </summary>
public abstract class WeightAmplifierBase
{
    /// <summary>
    /// ������һ��amplifierCardData������Ҫ�޸�Ȩ�ص�ֵ�����޸�
    /// </summary>
    /// <param name="amplifierCardData"></param>
    /// <returns></returns>
    public abstract LinkedList<SingleCardData_AfterAmplifier> Amplifier(LinkedList<SingleCardData_AfterAmplifier> amplifierCardData);
}
