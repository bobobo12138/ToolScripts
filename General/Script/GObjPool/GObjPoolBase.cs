using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GObjPoolBase<T>where T : MonoBehaviour
{
    protected Stack<T> pool = new Stack<T>();
    protected Transform parent;
    protected T prototype;//注意原型不进行初始化


    /// <summary>
    /// 封装的实例化与初始化
    /// </summary>
    /// <returns></returns>
    protected T Instantiate()
    {
        var v = GameObject.Instantiate(prototype, parent);
        InitObj(v);
        return v;
    }

    protected void InitObj(T obj)
    {
        //进行初始化，若继承了IObjInit接口的话
        if (obj is IObjInit)
        {
            var temp = obj as IObjInit;
            temp.Init();
        }
    }
}
