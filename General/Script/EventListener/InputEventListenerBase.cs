using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class KeyEnumKeycode<KeyEnum> where KeyEnum : Enum
{
    public KeyEnum keyEnum;
    public KeyCode keyCode;
}
/// <summary>
/// 事件监听器base，根据需求继承
/// KeyEnum为自定义事件名枚举，列如向前，向后
/// KeyEnum为字典建，对应InputEvent
/// </summary>
public abstract class InputEventListenerBase<KeyEnum> where KeyEnum : Enum
{


    public bool isRun;//是否运行
    protected Dictionary<KeyEnum, InputEvent> inputEventDic;  //KeyEnum对应的Event
    protected Dictionary<KeyEnum, KeyCode> keyEnum_KeycodeDic;//KeyEnum对应的Keycode

    public abstract void Init();    //初始化
    public abstract void Refresh(); //刷新
    /// <summary>
    /// 提供改键保存的地址
    /// </summary>
    /// <returns></returns>
    protected abstract string GetSavePath();







    protected InputEventListenerBase()
    {
        inputEventDic = new Dictionary<KeyEnum, InputEvent>();
        keyEnum_KeycodeDic = new Dictionary<KeyEnum, KeyCode>();

        LoadKeyData();
    }

    /// <summary>
    /// 加载键数据
    /// </summary>
    void LoadKeyData()
    {
        List<KeyEnumKeycode<KeyEnum>> save = new List<KeyEnumKeycode<KeyEnum>>();

        string JsonPath = GetSavePath();

        ///从本地读取文本并转换为json还未写完
        //save= JsonMapper.ToObject<KeyEnumKeycode<KeyEnum>>(save);//读取出存到saveread中

        if (save == null)
        {
            Debug.Log("读取为空");

            return;
        }


        foreach (var v in save)
        {
            keyEnum_KeycodeDic.Add(v.keyEnum, v.keyCode);
        }
    }

    /// <summary>
    /// 外部执行的监听
    /// </summary>
    public void ListenerUpdate()
    {
        if (!isRun) return;
        foreach (var v in inputEventDic)
        {
            v.Value.Listen();
        }
    }
    /// <summary>
    /// 增加事件
    /// </summary>
    public void _AddEvent(KeyEnum inputKey, Action todo_GetKeyDown, Action todo_GetKey = null, Action todo_GetKeyUp = null)
    {
        ///继承者读取改键再_AddEvent
        if (inputEventDic.ContainsKey(inputKey))
        {
            inputEventDic[inputKey].AddEvent(todo_GetKeyDown, todo_GetKey, todo_GetKeyUp);
        }
        else
        {
            ///重写GetKeyCode读取对应keycode
            inputEventDic.Add(inputKey, new InputEvent(keyEnum_KeycodeDic[inputKey], todo_GetKeyDown, todo_GetKey, todo_GetKeyUp));
        }
    }

}
