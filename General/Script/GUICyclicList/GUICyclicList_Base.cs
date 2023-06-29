using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 循环列表计算方式
/// </summary>
public enum CyclicListCalculateMode
{
    Adaptive,//自适应，将rect铺满，不出现多余
    Sequential//按顺序，顺序安置rect
}


public class GUICyclicList_Base : MonoBehaviour
{
    [Tooltip("interactable暂时无用")]
    public bool interactable;
    [LabelText("自动计算数量")]
    [Tooltip("勾选后，num_H,num_V将失效")]
    public bool autoCalculateNum = false;//自动计算数量，勾选后，num_H,num_V将失效
}
