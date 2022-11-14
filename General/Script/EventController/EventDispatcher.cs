using Dispatcher;
using System;
using UnityEngine;

/// <summary>
/// 事件派发器
/// </summary>
public class EventDispatcher : Game.UnitySingleton<EventDispatcher>
{
    private EventController ec = new EventController();

    #region 注入事件
    /// <summary>
    /// 注入事件(无参)
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="action">事件</param>
    public void AddEvent(EventNameDataBase _eventName, Action _action)
    {
        ec.AddEvent(_eventName, _action);
    }
    /// <summary>
    /// 注入事件(1个参数)
    /// </summary>
    /// <typeparam name="T">事件类型</typeparam>
    /// <param name="eventName">事件名称</param>
    /// <param name="action">事件</param>
    public void AddEvent<T>(EventNameDataBase _eventName, Action<T> _action)
    {
        ec.AddEvent(_eventName, _action);
    }
    /// <summary>
    /// 注入事件(2个参数)
    /// </summary>
    /// <typeparam name="T">事件1类型</typeparam>
    /// <typeparam name="X">事件2类型</typeparam>
    /// <param name="eventName">事件名称</param>
    /// <param name="action">事件</param>
    public void AddEvent<T, X>(EventNameDataBase _eventName, Action<T, X> _action)
    {
        ec.AddEvent(_eventName, _action);
    }
    /// <summary>
    /// 注入事件(3个参数)
    /// </summary>
    /// <typeparam name="T">事件1类型</typeparam>
    /// <typeparam name="X">事件2类型</typeparam>
    /// <typeparam name="Z">事件3类型</typeparam>
    /// <param name="eventName">事件名称</param>
    /// <param name="action">事件</param>
    public void AddEvent<T, X, Z>(EventNameDataBase _eventName, Action<T, X, Z> _action)
    {
        ec.AddEvent(_eventName, _action);
    }

    #endregion

    #region 移除事件
    /// <summary>
    /// 移除事件(无参)
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="action">事件</param>
    public void RemoveEvent(EventNameDataBase _eventName, Action _action)
    {
        ec.RemoveEvent(_eventName, _action);
    }
    /// <summary>
    /// 移除事件(1个参数)
    /// </summary>
    /// <typeparam name="T">事件类型</typeparam>
    /// <param name="eventName">事件名称</param>
    /// <param name="action">事件</param>
    public void RemoveEvent<T>(EventNameDataBase _eventName, Action<T> _action)
    {
        ec.RemoveEvent(_eventName, _action);
    }
    /// <summary>
    /// 移除事件(2个参数)
    /// </summary>
    /// <typeparam name="T">事件1类型</typeparam>
    /// <typeparam name="X">事件2类型</typeparam>
    /// <param name="eventName">事件名称</param>
    /// <param name="action">事件</param>
    public void RemoveEvent<T, X>(EventNameDataBase _eventName, Action<T, X> _action)
    {
        ec.RemoveEvent(_eventName, _action);
    }
    /// <summary>
    /// 移除事件(3个参数)
    /// </summary>
    /// <typeparam name="T">事件1类型</typeparam>
    /// <typeparam name="X">事件2类型</typeparam>
    /// <typeparam name="Z">事件3类型</typeparam>
    /// <param name="eventName">事件名称</param>
    /// <param name="action">事件</param>
    public void RemoveEvent<T, X, Z>(EventNameDataBase _eventName, Action<T, X, Z> _action)
    {
        ec.RemoveEvent(_eventName, _action);
    }
    #endregion

    #region 触发事件
    /// <summary>
    /// 触发事件(无参)
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="action">事件</param>
    public void TriggerEvent(EventNameDataBase _eventName)
    {
        ec.TriggerEvent(_eventName);
    }
    /// <summary>
    /// 触发事件(1个参数)
    /// </summary>
    /// <typeparam name="T">事件类型</typeparam>
    /// <param name="eventName">事件名称</param>
    /// <param name="action">事件</param>
    public void TriggerEvent<T>(EventNameDataBase _eventName, T arg1)
    {
        ec.TriggerEvent(_eventName, arg1);
    }
    /// <summary>
    /// 触发事件(2个参数)
    /// </summary>
    /// <typeparam name="T">事件1类型</typeparam>
    /// <typeparam name="X">事件2类型</typeparam>
    /// <param name="eventName">事件名称</param>
    /// <param name="action">事件</param>
    public void TriggerEvent<T, X>(EventNameDataBase _eventName, T arg1, X arg2)
    {
        ec.TriggerEvent(_eventName, arg1, arg2);
    }
    /// <summary>
    /// 触发事件(3个参数)
    /// </summary>
    /// <typeparam name="T">事件1类型</typeparam>
    /// <typeparam name="X">事件2类型</typeparam>
    /// <typeparam name="Z">事件3类型</typeparam>
    /// <param name="eventName">事件名称</param>
    /// <param name="action">事件</param>
    public void TriggerEvent<T, X, Z>(EventNameDataBase _eventName, T arg1, X arg2, Z arg3)
    {
        ec.TriggerEvent(_eventName, arg1, arg2, arg3);
    }

    protected override void OnInit()
    {
        //void InitTest()
        //{
        //    Debug.Log("事件派发起初始化成功");
        //}
        ////AddEvent(EventNameDataBase.in, InitTest);
        ////TriggerEvent("InitTest");
    }

    #endregion
}

