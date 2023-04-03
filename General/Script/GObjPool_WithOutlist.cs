using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 通用泛型状态机_带出栈清单
/// 待实测，建议是继承重写原始状态机
/// </summary>
public class GObjPool_WithOutlist<T> where T : MonoBehaviour
{
    Stack<T> pool = new Stack<T>();
    List<T> outlist = new List<T>();//出pool清单
    Transform parent;
    T prototype;

    /// <summary>
    /// 父级，生成物，预生成数量
    /// </summary>
    public GObjPool_WithOutlist(Transform _parent, T _spawnObj, int num = 0)
    {
        parent = _parent;
        prototype = GameObject.Instantiate(_spawnObj, parent);//预先克隆一个作为原型，方便外部访问修改
        prototype.name = prototype.name + "prototype";
        for (int i = 0; i < num; i++)
        {
            var v = GameObject.Instantiate(prototype, parent);
            pool.Push(v);
            v.gameObject.SetActive(false);
        }
        prototype.gameObject.SetActive(false);//最后才将原型SetActive(false)否则会出现awake运行问题
    }
    /// <summary>
    /// 获取一个对象
    /// </summary>
    /// <returns></returns>
    public T GetObj()
    {
        if (pool.Count == 0)
        {
            var v = GameObject.Instantiate(prototype, parent);

            //进行初始化，若继承了IObjInit接口的话
            if (v is IObjInit)
            {
                var temp = v as IObjInit;
                temp.Init();
            }

            v.gameObject.SetActive(true);
            Expand_Get(v);
            outlist.Add(v);
            return v;
        }

        var obj = pool.Pop();
        obj.gameObject.SetActive(true);
        Expand_Get(obj);
        outlist.Add(obj);
        return obj;
    }

    /// <summary>
    /// 回收一个对象
    /// </summary>
    /// <param name="obj"></param>
    public void RecycleObj(T obj)
    {
        if (obj == prototype) return;//不能回收原型
        if (pool.Contains(obj)) return;
        if (obj.transform.parent != parent)
            obj.transform.SetParent(parent);
        obj.gameObject.SetActive(false);
        Expand_RecycleObj(obj);
        outlist.Remove(obj);
        pool.Push(obj);
    }

    /// <summary>
    /// 批量生产
    /// </summary>
    /// <param name="num"></param>
    public void Produce(int num = 10)
    {
        for (int i = 0; i < num; i++)
        {
            var v = GameObject.Instantiate(prototype, parent);
            v.gameObject.SetActive(false);
            pool.Push(v);
        }
    }
    /// <summary>
    /// 清除对象池
    /// </summary>
    public void CleanPool()
    {
        if (parent.childCount <= 1) return;

        pool.Clear();
        outlist.Clear();
        for (int i = 1; i < parent.childCount; i++)//谨防误删原型
        {
            GameObject.Destroy(parent.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// 获得克隆的原型，谨慎对原型进行修改
    /// </summary>
    /// <returns></returns>
    public T GetPrototype()
    {
        if (prototype != null)
        {
            return prototype;
        }
        else
        {
            //Debug.LogWarning("cant get the prototype,is objpool init?(constructor)");
            Debug.LogWarning("无法获得原型，对象池初始化否？");
            return null;
        }

    }

    public List<T> GetOutlist()
    {
        return outlist;
    }

    /// <summary>
    /// GetObje方法拓展，若无法满足需求，继承本类，重写后会在Get时自动调用
    /// </summary>
    /// <param name="v"></param>
    protected virtual void Expand_Get(T v)
    {

    }


    /// <summary>
    /// RecycleObj方法拓展，若无法满足需求，继承本类，重写后会在RecycleObj时自动调用
    /// </summary>
    /// <param name="v"></param>
    protected virtual void Expand_RecycleObj(T v)
    {

    }

}
