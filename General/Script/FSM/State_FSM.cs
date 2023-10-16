using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 状态
/// </summary>
/// <typeparam name="T"></typeparam>
[Serializable]
public abstract class State_FSM<T> where T: IGFSM
{
    protected T user;

    protected State_FSM(T user)
    {
        this.user = user;
    }

    public abstract void WhyChange();

    public abstract void OnUpdate();

    public abstract void OnStart();

    public abstract void OnEnd();

}

