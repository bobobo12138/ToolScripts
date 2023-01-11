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
/// �¼�������base����������̳�
/// KeyEnumΪ�Զ����¼���ö�٣�������ǰ�����
/// KeyEnumΪ�ֵ佨����ӦInputEvent
/// </summary>
public abstract class InputEventListenerBase<KeyEnum> where KeyEnum : Enum
{


    public bool isRun;//�Ƿ�����
    protected Dictionary<KeyEnum, InputEvent> inputEventDic;  //KeyEnum��Ӧ��Event
    protected Dictionary<KeyEnum, KeyCode> keyEnum_KeycodeDic;//KeyEnum��Ӧ��Keycode

    public abstract void Init();    //��ʼ��
    public abstract void Refresh(); //ˢ��
    /// <summary>
    /// �ṩ�ļ�����ĵ�ַ
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
    /// ���ؼ�����
    /// </summary>
    void LoadKeyData()
    {
        List<KeyEnumKeycode<KeyEnum>> save = new List<KeyEnumKeycode<KeyEnum>>();

        string JsonPath = GetSavePath();

        ///�ӱ��ض�ȡ�ı���ת��Ϊjson��δд��
        //save= JsonMapper.ToObject<KeyEnumKeycode<KeyEnum>>(save);//��ȡ���浽saveread��

        if (save == null)
        {
            Debug.Log("��ȡΪ��");

            return;
        }


        foreach (var v in save)
        {
            keyEnum_KeycodeDic.Add(v.keyEnum, v.keyCode);
        }
    }

    /// <summary>
    /// �ⲿִ�еļ���
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
    /// �����¼�
    /// </summary>
    public void _AddEvent(KeyEnum inputKey, Action todo_GetKeyDown, Action todo_GetKey = null, Action todo_GetKeyUp = null)
    {
        ///�̳��߶�ȡ�ļ���_AddEvent
        if (inputEventDic.ContainsKey(inputKey))
        {
            inputEventDic[inputKey].AddEvent(todo_GetKeyDown, todo_GetKey, todo_GetKeyUp);
        }
        else
        {
            ///��дGetKeyCode��ȡ��Ӧkeycode
            inputEventDic.Add(inputKey, new InputEvent(keyEnum_KeycodeDic[inputKey], todo_GetKeyDown, todo_GetKey, todo_GetKeyUp));
        }
    }

}
