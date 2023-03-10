using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 将马铃薯权重扩大三倍的放大器
/// </summary>
public class WeightAmplifier_SuperPotato : WeightAmplifierBase
{
    public override LinkedList<SingleCardData_AfterAmplifier> Amplifier(LinkedList<SingleCardData_AfterAmplifier> amplifierCardData)
    {
        foreach (var v in amplifierCardData)
        {
            if (v.singleCardData.name == "马铃薯")
            {
                v.AmplifierWeight *= 3;
            }
        }

        return amplifierCardData;
    }
}
