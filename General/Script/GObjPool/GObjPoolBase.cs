using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GObjPoolBase<T>where T : MonoBehaviour
{
    protected Stack<T> pool = new Stack<T>();
    protected Transform parent;
    protected T prototype;//ע��ԭ�Ͳ����г�ʼ��


    /// <summary>
    /// ��װ��ʵ�������ʼ��
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
        //���г�ʼ�������̳���IObjInit�ӿڵĻ�
        if (obj is IObjInit)
        {
            var temp = obj as IObjInit;
            temp.Init();
        }
    }
}
