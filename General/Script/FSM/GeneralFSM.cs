using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 枚举键的状态机_泛型
/// </summary>
public class GeneralFSM
{
    [System.Serializable]
    public class NowEnemyState
    {
        public Enum State;
        public State_FSM Act;
    }
    public Action<Enum> onChange;
    public GameObject user;
    public NowEnemyState nowState = new NowEnemyState();
    private Dictionary<Enum, State_FSM> FSMDictionary = new Dictionary<Enum, State_FSM>();

    public GeneralFSM(GameObject user)
    {
        this.user = user;
    }

    #region 注入移除
    public void AddState(Enum state, State_FSM stateClass)
    {
        if (FSMDictionary.ContainsKey(state))
        {
            Debug.Log(state + "该状态已经存在");
        }
        FSMDictionary.Add(state, stateClass);
    }

    public void RemoveState(Enum state, State_FSM stateClass)
    {
        if (FSMDictionary.ContainsKey(state))
        {
            Debug.Log(state + "未找到该状态");
        }
        FSMDictionary.Remove(state);
    }

    #endregion

    /// <summary>
    /// 传入一个回调，回调执行改变状态枚举，带参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="state"></param>
    /// <param name="action"></param>
    public void ChangeState(Enum state)
    {
        if (!FSMDictionary.ContainsKey(state))
        {
            Debug.Log("该状态不存在：" + state);
            return;
        }

        ///当状态切换时，执行上一个状态的onend和下一个状态的onstart
        ///执行上一个状态的OnEnd
        if (nowState.State != null)
        {
            if (FSMDictionary.ContainsKey(nowState.State))//若nowstate不存在，初始化
            {
                FSMDictionary[nowState.State].OnEnd();
            }
        }
        ///替换状态数据
        nowState.State = state;
        nowState.Act = FSMDictionary[state];
        //状态已经切换，触发事件
        onChange(state);
        ///执行下一个状态的OnStart
        FSMDictionary[nowState.State].OnStart();

    }

    public void Update()
    {
        if (nowState.Act != null)
        {
            nowState.Act.OnUpdate();
            nowState.Act.WhyChange();
        }

    }
}
