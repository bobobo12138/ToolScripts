using System;
using UnityEngine;

/// <summary>
/// ����Input�¼���
/// ����Down��GetKey��Up��
/// </summary>
public class InputEvent
{
    //��InputKey����Ϊ��������ļ�
    public KeyCode inputKey;
    private bool isOnlyGetKeyDown = true;//Ĭ����ֻ�ж�GetKeyDown

    public Action todo_GetKeyDown;
    public Action todo_GetKey;
    public Action todo_GetKeyUp;

    /// <summary>
    /// �����¼�
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
    /// �����¼�
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
    /// �ļ�������Key
    /// </summary>
    /// <param name="inputKey"></param>
    public void ResetKeyCode(KeyCode inputKey)
    {
        this.inputKey = inputKey;
    }

    /// <summary>
    /// ʵ�ʵļ������룬������Update��
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