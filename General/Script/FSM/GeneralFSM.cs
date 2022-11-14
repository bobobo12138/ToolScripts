using MonsterFSM;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用于改变状态的接口，由使用状态机的gameobject继承
/// 由于不同怪物枚举不同，所以重写
/// </summary>
public interface IGeneralFSM
{
    void UpdateState(Enum v);

}
#region 旧版状态机
///// <summary>
///// 字符串为键的状态机
///// </summary>
//public class GeneralFSM
//{
//    public class NowEnemyState
//    {
//        public string State;
//        public State_FSM Act;
//    }

//    private Dictionary<string, State_FSM> FSMDictionary = new Dictionary<string, State_FSM>();

//    public GameObject pointGameobject;
//    private IGeneralFSM pointIGeneralFSM;//pointGameobject需要基础pointIGeneralFSM接口
//    public Animator pointAnimator;
//    public NowEnemyState nowState = new NowEnemyState();

//    /// <summary>
//    /// 提供使用者的Gameobject
//    /// </summary>
//    /// <param name="v"></param>
//    public GeneralFSM(GameObject v)
//    {
//        pointGameobject = v;
//        pointIGeneralFSM = v.GetComponent<IGeneralFSM>();
//        pointAnimator = v.GetComponent<Animator>();
//        nowState.State = "None";//初始化状态值，默认为空
//    }

//    #region 注入移除
//    public void AddState(string state, State_FSM sabstrat)
//    {
//        if (FSMDictionary.ContainsKey(state))
//        {
//            Debug.Log(state + "该状态已经存在");
//        }
//        FSMDictionary.Add(state, sabstrat);
//    }

//    public void RemoveState(string state, State_FSM sabstrat)
//    {
//        if (FSMDictionary.ContainsKey(state))
//        {
//            Debug.Log(state + "未找到该状态");
//        }
//        FSMDictionary.Remove(state);
//    }

//    #endregion

//    /// <summary>
//    /// 传入一个回调，回调执行改变状态枚举，带参数
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    /// <param name="state"></param>
//    /// <param name="action"></param>
//    public void ChangeState(string state)
//    {
//        if (!FSMDictionary.ContainsKey(state))
//        {
//            Debug.Log("该状态不存在：" + state);
//            return;
//        }

//        //当状态切换时，执行上一个状态的onend和下一个状态的onstart
//        if (FSMDictionary.ContainsKey(nowState.State))//若nowstate不存在，初始化
//        {
//            FSMDictionary[nowState.State].OnEnd();
//        }
//        nowState.State = state;
//        nowState.Act = FSMDictionary[state];
//        pointIGeneralFSM.UpdateState(state);
//        FSMDictionary[nowState.State].OnStart();

//    }

//    public void Update()
//    {
//        if (nowState.Act != null)
//        {
//            nowState.Act.OnUpdate();
//            nowState.Act.WhyChange();
//        }

//    }
//}
#endregion

/// <summary>
/// 枚举键的状态机_泛型
/// </summary>
public class GeneralFSM<T>
{
    public class NowEnemyState
    {
        public Enum State;
        public State_FSM<T> Act;
    }

    private Dictionary<Enum, State_FSM<T>> FSMDictionary = new Dictionary<Enum, State_FSM<T>>();

    public GameObject pointGameobject;
    private IGeneralFSM pointIGeneralFSM;//pointGameobject需要基础pointIGeneralFSM接口
    public Animator pointAnimator;
    public NowEnemyState nowState = new NowEnemyState();

    /// <summary>
    /// 提供使用者的Gameobject
    /// </summary>
    /// <param name="v"></param>
    public GeneralFSM(GameObject v)
    {
        pointGameobject = v;
        pointIGeneralFSM = v.GetComponent<IGeneralFSM>();
        pointAnimator = v.GetComponent<Animator>();
        nowState.State = Boss_ChargeShooterEnum.aroundFire;//初始化状态值，默认为空
    }

    #region 注入移除
    public void AddState(Enum state, State_FSM<T> sabstrat)
    {
        if (FSMDictionary.ContainsKey(state))
        {
            Debug.Log(state + "该状态已经存在");
        }
        FSMDictionary.Add(state, sabstrat);
    }

    public void RemoveState(Enum state, State_FSM<T> sabstrat)
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

        //当状态切换时，执行上一个状态的onend和下一个状态的onstart
        if (FSMDictionary.ContainsKey(nowState.State))//若nowstate不存在，初始化
        {
            FSMDictionary[nowState.State].OnEnd();
        }
        nowState.State = state;
        nowState.Act = FSMDictionary[state];
        pointIGeneralFSM.UpdateState(state);
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



