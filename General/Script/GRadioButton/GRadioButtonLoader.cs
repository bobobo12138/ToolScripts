using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 单选框装载,单选框必须和装载同时出现
/// 单选框指向此类的对象即可归为同一组
/// </summary>
public class GRadioButtonLoader : MonoBehaviour
{
    [HideInInspector]
    public GRadioButton last;

    /// <summary>
    /// 将所记录的last刷新，这样下一次单选按钮必定触发
    /// </summary>
    public void Refresh()
    {
        last = null;
    }

}
