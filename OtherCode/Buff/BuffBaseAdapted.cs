using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 实际的buff所需继承的类
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class BuffBaseAdapted<T> : BuffBase where T : IBuffable
{

    protected sealed override (bool isAdapted, string message) AdaptedSetData(IBuffable iBuff)
    {
        if (iBuff is T)
        {
            OnSetData((T)iBuff );
            return (true, "");
        }
        else
        { 
            return (false, typeof(T).Name);
        }
    }

    protected  abstract void OnSetData(T ibuff);
}
