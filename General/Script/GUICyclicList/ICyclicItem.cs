using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 循环元素所需继承的接口
/// </summary>
public interface ICyclicItem
{
    /// <summary>
    /// 初始化，会返回宽高
    /// </summary>
    /// <param name="_width"></param>
    /// <param name="_height"></param>
    void InitSet(float _width = 0, float _height = 0);
    /// <summary>
    /// 会返回再列表中的位置
    /// 会在初始与发生交换时触发
    /// </summary>
    /// <param name="index"></param>
    void SetIndex(int index);



    void Refresh();
    ///// <summary>
    ///// 设置数据
    ///// </summary>
    ///// <param name="group"></param>
    //void SetData(object group);


    //Object GetObject();
    RectTransform GetRectTransform();
    //int GetIndex();

}
