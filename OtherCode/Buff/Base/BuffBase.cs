using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

/// <summary>
/// 实际的buff逻辑基类
/// 实际的buff应当继承BuffBaseAdapted
/// </summary>
public abstract class BuffBase : MonoBehaviour
{
    public int id { get; protected set; }
    public bool isInit { get; protected set; }
    public bool isRun { get; protected set; }
    public BuffData data { get; protected set; }

    [SerializeField]
    protected bool isDuration;//是否有持续时间
    [SerializeField]
    protected float duration;//持续时间

    protected IBuffable buffOwner;
    protected IBuffHandler buffHandler;

    Timer_Stopwatch _stopwatch;
    protected Timer_Stopwatch stopwatch
    {
        get
        {
            if (_stopwatch == null) _stopwatch = new Timer_Stopwatch(0);
            return _stopwatch;
        }
    }

    private void Awake()
    {
        stopwatch.onFinish = Finish;
        OnAwake();
    }

    /// <summary>
    /// 尝试进行buff赋值，返回是否适配，若不适配则代表这个obj无法添加此buff
    /// </summary>
    /// <param name="buffOwner">适配信息</param>
    /// <returns></returns>
    public (bool isAdapted, string message) TrySetData(IBuffable ibuff,IBuffHandler ibuffHandler)
    {
        var isFit = AdaptedSetData(ibuff);

        if (isFit.isAdapted)
        {
            buffOwner = ibuff;
            buffHandler = ibuffHandler;
        }

        return isFit;
    }

    /// <summary>
    /// 初始化的时候进行基础参数设置，后续不再改变
    /// </summary>
    /// <param name="isDuration"></param>
    /// <param name="duration"></param>
    public void InitSet(BuffData buffConfigData)
    {
        isInit = true;
        this.id = buffConfigData.Id;
        this.isDuration = buffConfigData.IsDuration;
        this.duration = buffConfigData.Duration;
        data = buffConfigData;
        stopwatch.time = this.duration;
        OnInit();
    }

    private void Update()
    {
        if (isRun)
        {
            if (isDuration) stopwatch.Update();
            UpdateBuff();
        }
    }

    public void Run()
    {
        isRun = true;
        if (isDuration) stopwatch.Restart();
        OnRun();
    }

    public void Finish()
    {
        Stop();
        Recycle();
    }
    public void Stop()
    {
        isRun = false;
        if (isDuration) stopwatch.Stop();
        OnFinish();
    }
    protected void Recycle()
    { 
        buffHandler.RemoveBuff(this);
    }


    public void ResetTimer()
    {
        if (isDuration) stopwatch.Restart();
    }

    protected virtual void OnAwake() { }
    protected abstract void OnInit();
    protected abstract (bool isAdapted, string message) AdaptedSetData(IBuffable iBuff);
    protected abstract void OnRun();
    protected abstract void OnFinish();
    protected abstract void UpdateBuff();
}
