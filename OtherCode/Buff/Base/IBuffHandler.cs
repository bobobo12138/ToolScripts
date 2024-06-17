using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 继承了此接口代表是buff管理器
/// </summary>
public interface IBuffHandler
{
    public void AddBuff(int id);
    public void AddBuff(BuffBase buffBase);


    public void RemoveBuff(int id);
    public void RemoveBuff(BuffBase buffBase);

}
