using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BuffManager抽象基类
/// </summary>
public abstract class BuffManagerBase : UnitySingleton<BuffManagerBase>
{
    public abstract BuffBase GetBuff(int id);
    public abstract void RecycleBuff(BuffBase buff);
}
