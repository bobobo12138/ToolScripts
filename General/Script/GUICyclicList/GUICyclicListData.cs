using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace General
{
    /// <summary>
    /// 循环列表计算方式
    /// </summary>
    public enum CyclicListCalculateMode
    {
        Adaptive,//自适应，将rect铺满，不出现多余
        Sequential//按顺序，顺序安置rect
    }


}
