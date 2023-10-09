using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 状态
/// </summary>
/// <typeparam name="T"></typeparam>
[Serializable]
public abstract class State_FSM
{
    GeneralFSM generalFSM = null;

    protected State_FSM(GeneralFSM generalFSM)
    {
        this.generalFSM = generalFSM;
    }

    public abstract void WhyChange();

    public abstract void OnUpdate();

    public abstract void OnStart();

    public abstract void OnEnd();

}

