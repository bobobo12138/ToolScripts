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
    Dictionary<T, Action> stateDic_Change;
    Dictionary<T, Action> stateDic_Update;
    bool isInit = false;

    public LiteFSM()
    {
        isInit = true;
        stateDic_Change = new Dictionary<T, Action>();
    }

    public void AddState(T stateName, Action state_Change, Action state_Update)
    {
        if (!isInit) Debug.LogError("LiteFSM未初始化");

        if (stateDic_Change.ContainsKey(stateName))
        {
            Debug.LogError("stateName is already exist");
            return;
        }
        else
        {
            stateDic_Change.Add(stateName, state_Change);
            stateDic_Update.Add(stateName, state_Update);
        }
    }

    public void RemoveState(T stateName)
    {
        if (!isInit) Debug.LogError("LiteFSM未初始化");

        if (!stateDic_Change.ContainsKey(stateName))
        {
            Debug.LogError("stateName is not exist");
            return;
        }
        else
        {
            stateDic_Change.Remove(stateName);
            stateDic_Update.Remove(stateName);

        }
    }

    public void ChangeState(T stateName)
    {
        if (!isInit) Debug.LogError("LiteFSM未初始化");

        if (!stateDic_Change.ContainsKey(stateName))
        {
            Debug.LogError("stateName is not exist");
            return;
        }
        stateDic_Change[stateName]?.Invoke();
        nowState = stateName;
    }

    public void Update()
    {
        stateDic_Update[nowState]?.Invoke();
    }
}
