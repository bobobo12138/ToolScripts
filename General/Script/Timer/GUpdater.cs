using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUpdater
{
    float time = 0;
    float timer = 0;

    bool isRun = true;
    Action action_Start;
    Action action_End;
    public GUpdater(float time,Action action_Start=null, Action action_End=null)
    {
        this.time = time;
        this.action_Start = action_Start;
        this.action_End = action_End;
    }
    
    /// <summary>
    /// ���¿�ʼ
    /// </summary>
    public void Restart()
    {
        timer = time;
        isRun = true;
        ///�����ص�-����
        action_Start?.Invoke();
    }

    /// <summary>
    /// ǿ�ƹرռ�ʱ
    /// </summary>
    /// <param name="isTriggerAction_End">�Ƿ񴥷������¼�</param>
    public void TurnOff(bool isTriggerAction_End=false)
    {
        timer = 0;
        isRun = false;
        if (isTriggerAction_End) action_End?.Invoke();
    }

    /// <summary>
    /// ����update�еĸ���
    /// </summary>
    public void Update()
    {
        if (!isRun) return;

        if (timer < 0)
        {
            ///�����ص�-���
            isRun = false;
            action_End?.Invoke();
        }
        timer -= Time.deltaTime;
    }
}
