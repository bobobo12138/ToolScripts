using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ��������Ȩ�����������ķŴ���
/// </summary>
public class WeightAmplifier_SuperPotato : WeightAmplifierBase
{
    public override LinkedList<SingleCardData_AfterAmplifier> Amplifier(LinkedList<SingleCardData_AfterAmplifier> amplifierCardData)
    {
        foreach (var v in amplifierCardData)
        {
            if (v.singleCardData.name == "������")
            {
                v.AmplifierWeight *= 3;
            }
        }

        return amplifierCardData;
    }
}
