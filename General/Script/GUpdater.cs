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
    /// 重新开始
    /// </summary>
    public void Restart()
    {
        timer = time;
        isRun = true;
        ///触发回调-重置
        action_Start?.Invoke();
    }

    /// <summary>
    /// 强制关闭计时
    /// </summary>
    /// <param name="isTriggerAction_End">是否触发结束事件</param>
    public void TurnOff(bool isTriggerAction_End=false)
    {
        timer = 0;
        isRun = false;
        if (isTriggerAction_End) action_End?.Invoke();
    }

    /// <summary>
    /// 放在update中的更新
    /// </summary>
    public void Update()
    {
        if (!isRun) return;

        if (timer < 0)
        {
            ///触发回调-完成
            isRun = false;
            action_End?.Invoke();
        }
        timer -= Time.deltaTime;
    }
}
