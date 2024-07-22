using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class Finder
{
    static Dictionary<Transform, Dictionary<string, GameObject>> keyValuePairs = new Dictionary<Transform, Dictionary<string, GameObject>>();


    #region 预加载式，适用于较多find的情况
    /// <summary>
    /// 初始化，产生缓存；将所有transform下的物体（包含本transform）深度遍历，以其name为键存储起来
    /// </summary>
    /// <param name="transform">遍历父级</param>
    /// <param name="maxDeep">最大深度，默认无限深度</param>
    public static void PreloadCache(Transform transform, int maxDeep = -1)
    {
        if (keyValuePairs.ContainsKey(transform)) return;

        keyValuePairs.Add(transform, new Dictionary<string, GameObject>());
        keyValuePairs[transform].Add(transform.name, transform.gameObject);
        PreDeepFind(keyValuePairs[transform], transform, maxDeep);

        Debug.LogWarning("预载" + transform.name + "...");
    }



    /// <summary>
    /// 此方法需要进行预加载
    /// 查找，会先查找存储的键值对，若无法获得则会进行一次深度遍历
    /// 注意一个trans下寻找相同name物体只会返回第一个
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static GameObject Find(Transform transform, string name)
    {
        if (keyValuePairs[transform] == null)
        {
            Debug.LogError(transform.name + "未进行PreloadCache！");
            return null;
        }

        if (keyValuePairs[transform].ContainsKey(name)) return keyValuePairs[transform][name];

        var temp = DeepFind(transform, name);
        if (temp == null)
        {
            Debug.LogError(transform.name + "未找到" + name + "！");
            return null;
        }

        return temp;
    }

    /// <summary>
    /// 此方法需要进行预加载
    /// 查找，会先查找存储的键值对，若无法获得则会进行一次深度遍历
    /// 注意一个trans下寻找相同name物体只会返回第一个
    /// 注意Compont是返回下方组件
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Compont Find<Compont>(Transform transform, string name) where Compont : class
    {
        GameObject v = Find(transform, name);
        if (v != null) return v.GetComponent<Compont>();
        return null;
    }

    /// <summary>
    /// 释放与删除
    /// 记得查找完成后将transform传入进行释放
    /// </summary>
    public static void ReleaseCache(Transform transform)
    {
        keyValuePairs.Remove(transform);

        Debug.LogWarning("卸载" + transform.name + "...");
    }


    static void PreDeepFind(Dictionary<string, GameObject> dic, Transform transform, int maxDeep = -1)
    {
        if (maxDeep == 0) return;
        if (transform.childCount == 0) return;

        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            if (!dic.ContainsKey(child.name))//相同的只保留寻找到的第一个
                dic.Add(child.name, child.gameObject);

            if (child.childCount != 0) PreDeepFind(dic, child, maxDeep - 1);
        }

    }
    #endregion


    /// <summary>
    /// 若不想使用预加载式，则直接调用此
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static GameObject DeepFind(Transform transform, string name)
    {
        if (transform.name == name) return transform.gameObject;
        if (transform.childCount == 0) return null;

        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);

            var temp = DeepFind(child, name);
            if (temp != null)
            {
                return temp;
            }
            else
            {
                continue;
            }
        }
        return null;
    }

}
