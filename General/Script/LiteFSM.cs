using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 简易状态机
/// 需要增加退出状态机的事件
/// </summary>
public sealed class LiteFSM<T>
{
    public T nowState;
    Dictionary<T, Action> stateDic;
    bool isInit=false;

    public LiteFSM()
    {
        isInit = true;
        stateDic = new Dictionary<T, Action>();
    }

    public void AddState(T stateName, Action state)
    {
        if (!isInit) Debug.LogError("LiteFSM未初始化");

        if (stateDic.ContainsKey(stateName))
        {
            Debug.LogError("stateName is already exist");
            return;
        }
        stateDic.Add(stateName, state);
    }

    public void RemoveState(T stateName)
    {
        if (!isInit) Debug.LogError("LiteFSM未初始化");

        if (!stateDic.ContainsKey(stateName))
        {
            Debug.LogError("stateName is not exist");
            return;
        }
        stateDic.Remove(stateName);
    }

    public void ChangeState(T stateName)
    {
        if (!isInit) Debug.LogError("LiteFSM未初始化");

        if (!stateDic.ContainsKey(stateName))
        {
            Debug.LogError("stateName is not exist");
            return;
        }
        stateDic[stateName].Invoke();
        nowState = stateName;
    }
}
