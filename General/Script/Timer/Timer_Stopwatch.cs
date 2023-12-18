using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 秒表
/// </summary>
public class Timer_Stopwatch
{
    public float time = 0;
    public float timer { get; private set; } = 0;

    public Action onFinish;
    public Action onRestart;

    public Timer_Stopwatch(float time)
    {
        this.time = time;
    }

    public bool isRun
    {
        get; private set;
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
            timer = 0;
            onFinish?.Invoke();
        }
        timer -= Time.deltaTime;
    }

    public void Restart()
    {
        timer = time;
        isRun = true;
        onRestart?.Invoke();
    }

    public void Stop() { isRun = false; }
}
