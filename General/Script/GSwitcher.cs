using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 简易选择器
/// 自动收集子集
/// </summary>
public class GSwitcher : MonoBehaviour
{
    Dictionary<string, GameObject> switcherKeyValuePairs;

    private void Awake()
    {
        Init();
    }
    /// <summary>
    /// 手动初始化
    /// </summary>
    public void Init()
    {
        switcherKeyValuePairs = new Dictionary<string, GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            switcherKeyValuePairs.Add(child.name, child.gameObject);
        }

    }
    /// <summary>
    /// 单选
    /// </summary>
    public void MultipleChoice(int index)
    {
        if (switcherKeyValuePairs == null)
        {
            Debug.LogError("GSwitcher is null");
        }

        foreach (var v in switcherKeyValuePairs)
        {
            if (v.Value.transform.GetSiblingIndex() == index)
            {
                v.Value.SetActive(true);
                continue;
            }
            v.Value.SetActive(false);
        }
    }
    public void MultipleChoice(string name)
    {
        if (switcherKeyValuePairs == null)
        {
            Debug.LogError("GSwitcher is null");
        }

        if (!switcherKeyValuePairs.ContainsKey(name))
        {
            Debug.LogError("GSwitcher cant find key");
        }

        foreach (var v in switcherKeyValuePairs)
        {
            v.Value.SetActive(false);
        }
        switcherKeyValuePairs[name].SetActive(true);
    }

    /// <summary>
    /// 多选
    /// </summary>
    public void MultiChoice(int index)
    {
        if (switcherKeyValuePairs == null)
        {
            Debug.LogError("GSwitcher is null");
        }

        foreach (var v in switcherKeyValuePairs)
        {
            if (v.Value.transform.GetSiblingIndex() == index)
            {
                v.Value.SetActive(true);
                break;
            }
        }
    }
    public void MultiChoice(string name)
    {
        if (switcherKeyValuePairs == null)
        {
            Debug.LogError("GSwitcher is null");
        }

        if (!switcherKeyValuePairs.ContainsKey(name))
        {
            Debug.LogError("GSwitcher cant find key");
        }
        switcherKeyValuePairs[name].SetActive(true);
    }

}
