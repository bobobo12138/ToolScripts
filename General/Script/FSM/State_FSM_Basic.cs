using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 状态
/// 状态继承此类
/// </summary>
/// <typeparam name="T"></typeparam>
[Serializable]
public abstract class State_FSM_Basic<T>: State_FSM where T: IGFSM
{
    protected T user;

    protected State_FSM_Basic(T user)
    {
        this.user = user;
    }
}


/// <summary>
/// State_FSM基类供FSM使用，因为FSM不需要IGFSM
/// 不要将State_FSM与State_FSM_Basic整合，会报错
/// 若要继承状态，去继承State_FSM_Basic
/// </summary>
public abstract class State_FSM
{
    public abstract void WhyChange();

    public abstract void OnUpdate();

    public abstract void OnStart();

    public abstract void OnEnd();
}
