using System;
using UnityEngine;

/// <summary>
/// 单个Input事件类
/// 包括Down、GetKey、Up等
/// </summary>
public class InputEvent
{
    //将InputKey声明为变量方便改键
    public KeyCode inputKey;
    private bool isOnlyGetKeyDown = true;//默认是只判断GetKeyDown

    public Action todo_GetKeyDown;
    public Action todo_GetKey;
    public Action todo_GetKeyUp;

    /// <summary>
    /// 增加事件
    /// </summary>
    /// <param name="inputKey"></param>
    /// <param name="todo_GetKeyDown"></param>
    /// <param name="todo_GetKey"></param>
    /// <param name="todo_GetKeyUp"></param>
    public InputEvent(KeyCode inputKey, Action todo_GetKeyDown, Action todo_GetKey = null, Action todo_GetKeyUp = null)
    {
        this.inputKey = inputKey;
        this.todo_GetKeyDown = todo_GetKeyDown;
        this.todo_GetKey = todo_GetKey;
        this.todo_GetKeyUp = todo_GetKeyUp;

        if (todo_GetKey != null || todo_GetKeyUp != null)
        {
            isOnlyGetKeyDown = false;
        }
    }

    public void AddEvent(Action todo_GetKeyDown, Action todo_GetKey = null, Action todo_GetKeyUp = null)
    {
        this.todo_GetKeyDown += todo_GetKeyDown;
        this.todo_GetKey += todo_GetKey;
        this.todo_GetKeyUp += todo_GetKeyUp;

        if (todo_GetKey != null || todo_GetKeyUp != null)
        {
            isOnlyGetKeyDown = false;
        }
    }

    /// <summary>
    /// 减少事件
    /// </summary>
    /// <param name="todo_GetKeyDown"></param>
    /// <param name="todo_GetKey"></param>
    /// <param name="todo_GetKeyUp"></param>
    public void RemoveEvent(Action todo_GetKeyDown, Action todo_GetKey = null, Action todo_GetKeyUp = null)
    {
        this.todo_GetKeyDown -= todo_GetKeyDown;
        this.todo_GetKey -= todo_GetKey;
        this.todo_GetKeyUp -= todo_GetKeyUp;

        if (todo_GetKey != null || todo_GetKeyUp != null)
        {
            isOnlyGetKeyDown = false;
        }
    }
    /// <summary>
    /// 改键后重置Key
    /// </summary>
    /// <param name="inputKey"></param>
    public void ResetKeyCode(KeyCode inputKey)
    {
        this.inputKey = inputKey;
    }

    /// <summary>
    /// 实际的监听代码，放置于Update中
    /// </summary>
    public void Listen()
    {
        if (isOnlyGetKeyDown)
        {
            if (Input.GetKeyDown(inputKey))
            {
                todo_GetKeyDown?.Invoke();
            }
        }
        else
        {
            if (Input.GetKeyDown(inputKey))
            {
                todo_GetKeyDown?.Invoke();
            }

            if (Input.GetKey(inputKey))
            {
                todo_GetKey?.Invoke();
            }

            if (Input.GetKeyUp(inputKey))
            {
                todo_GetKeyUp?.Invoke();
            }
        }

    }
}