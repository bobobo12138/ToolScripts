using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// buff的接收者
/// 作为组件存在
/// 挂载到所需要添加buff的obj下
/// </summary>
public class BuffHandler : MonoBehaviour, IBuffHandler
{
    public int count
    {
        get
        {
            if (buffDic == null) return 0;
            return buffDic.Count;
        }
    }

    IBuffable buffOwner;//buff拥有者
    Transform buffTrans;//buff存放的obj父级

    Dictionary<int, BuffBase> buffDic;

    public void InitSet(IBuffable buffOwner, Transform buffTrans)
    {
        buffDic = new Dictionary<int, BuffBase>();

        this.buffOwner = buffOwner;
        this.buffTrans = buffTrans;
    }


    #region 添加

    public void AddBuff(int id)
    {
        if (buffDic.ContainsKey(id))
        {
            Debug.LogWarning("已经拥有此buff，不再添加！");
            return;
        }

        //通过mgr获得具体的buff
        BuffBase buff = BuffManagerBase.InstanceNotAutoCreate.GetBuff(id);
        if (buff != null)
        {
            var adaptedData = buff.TrySetData(buffOwner, this);
            if (adaptedData.isAdapted)
            {
                SetBuffTransform(buff, buffTrans);
                buffDic.Add(id, buff);
                buff.Run();
            }
            else
            {
                Debug.LogError("该buff不适配！详细信息信息：" + adaptedData.message);
            }
        }
        else
        {
            Debug.LogError("Buff不能为null！");
        }
    }

    public void AddBuff(BuffBase buffBase)
    {
        if (buffDic.ContainsKey(buffBase.id))
        {
            Debug.LogWarning("已经拥有此buff，不再添加！");
            return;
        }

        if (buffBase != null)
        {
            var adaptedData = buffBase.TrySetData(buffOwner, this);
            if (adaptedData.isAdapted)
            {
                SetBuffTransform(buffBase, buffTrans);
                buffDic.Add(buffBase.id, buffBase);
                buffBase.Run();
            }
            else
            {
                Debug.LogError("该buff不适配！详细信息信息：" + adaptedData.message);
            }
        }
        else
        {
            Debug.LogError("Buff不能为null！");
        }
    }
    #endregion

    #region 移出
    public void RemoveAll()
    {
        foreach (var v in buffDic)
        {
            v.Value.Stop();
            BuffManagerBase.InstanceNotAutoCreate.RecycleBuff(v.Value);
        }

        buffDic.Clear();
    }


    public void RemoveBuff(int id)
    {
        if (buffDic.ContainsKey(id))
        {
            buffDic[id].Stop();
            BuffManagerBase.InstanceNotAutoCreate.RecycleBuff(buffDic[id]);
            buffDic.Remove(id);
        }
        else
        {
            Debug.LogError("不存在buff！" + id);
        }
    }

    public void RemoveBuff(BuffBase buffBase)
    {
        if (buffDic.ContainsKey(buffBase.id))
        {
            buffBase.Stop();
            BuffManagerBase.InstanceNotAutoCreate.RecycleBuff(buffBase);
            buffDic.Remove(buffBase.id);
        }
        else
        {
            Debug.LogError("不存在buff！" + buffBase.id);
        }
    }
    #endregion


    #region 获得

    public BuffBase GetBuff(int id)
    {
        if (buffDic.ContainsKey(id)) return buffDic[id];
        return null;
    }

    public BuffBase GetBuff(BuffBase buffBase)
    {
        if(buffDic.ContainsValue(buffBase))return buffDic[buffBase.id];
        return null;
    }

    public T GetBuff<T>() where T : BuffBase
    {
        foreach (var v in buffDic)
        {
            if (v.Value is T) return v.Value as T;
        }
        return null;
    }

    #endregion


    #region 包含
    public bool Contains(int id)
    {
        return buffDic.ContainsKey(id);
    }

    public bool Contains(BuffBase buffBase)
    {
        return buffDic.ContainsValue(buffBase);
    }

    public bool Contains<T>() where T : BuffBase
    {
        foreach (var v in buffDic)
        {
            if (v.Value is T) return true;
        }
        return false;
    }
    #endregion


    public static void SetBuffTransform(BuffBase buffBase, Transform buffTrans)
    {
        buffBase.transform.parent = buffTrans;
        buffBase.transform.localPosition = Vector3.zero;
        buffBase.transform.localEulerAngles = Vector3.zero;
        buffBase.transform.localScale = Vector3.one;
    }
}
