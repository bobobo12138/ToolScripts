using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 状态抽象类，放入状态机
/// </summary>
[Serializable]
public abstract class State_FSM
{
    public abstract void WhyChange();

    public abstract void OnUpdate();

    public abstract void OnStart();

    public abstract void OnEnd();


}

/// <summary>
/// 改进
/// </summary>
/// <typeparam name="T"></typeparam>
[Serializable]
public abstract class State_FSM<T>
{
    public T user;

    protected State_FSM(T _user)
    {
        this.user = _user;
    }

    public abstract void WhyChange();

    public abstract void OnUpdate();

    public abstract void OnStart();

    public abstract void OnEnd();


}
