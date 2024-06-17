using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Buff使用者基础接口，基础此接口代表可以使用某类buff
/// </summary>
public interface IBuffable
{
    GameObject GetBuffGameobject();
    Image[] images { get; set; }//wm所有显示的图片
}
