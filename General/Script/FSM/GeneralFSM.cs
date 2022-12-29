using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用于改变状态的接口，由使用状态机的gameobject继承
/// 切换状态时的回调
/// </summary>
public interface IGeneralFSM
{
    void UpdateState_CallBack();

}
/// <summary>
/// 枚举键的状态机_泛型
/// </summary>
public class GeneralFSM<T_User, T_Enum>where T_Enum :Enum
{
    [System.Serializable]
    public class NowEnemyState
    {
        public T_Enum State;
        public State_FSM<T_User> Act;
    }

    private Dictionary<T_Enum, State_FSM<T_User>> FSMDictionary = new Dictionary<T_Enum, State_FSM<T_User>>();
    private IGeneralFSM pointIGeneralFSM;//使用状态机的GameObject可继承此接口
    public NowEnemyState nowState = new NowEnemyState();

    /// <summary>
    /// 提供使用者的Gameobject
    /// </summary>
    /// <param name="v"></param>
    public GeneralFSM(GameObject v)
    {
        pointIGeneralFSM = v.GetComponent<IGeneralFSM>();
    }

    #region 注入移除
    public void AddState(T_Enum state, State_FSM<T_User> sabstrat)
    {
        if (FSMDictionary.ContainsKey(state))
        {
            Debug.Log(state + "该状态已经存在");
        }
        FSMDictionary.Add(state, sabstrat);
    }

    public void RemoveState(T_Enum state, State_FSM<T_User> sabstrat)
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
    public void ChangeState(T_Enum state)
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
        ///执行切换回调，若有的话
        if(pointIGeneralFSM!=null) pointIGeneralFSM.UpdateState_CallBack();
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



