using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHurt
{
    /// <summary>
    /// 区分与筛选伤害目标
    /// </summary>
    public HurtEnum type { get; }
    /// <summary>
    /// 直接杀死
    /// </summary>
    public void Kill();
    /// <summary>
    /// 伤害
    /// </summary>
    /// <param name="damage"></param>
    public void Hurt(Damage damage);
    /// <summary>
    /// 设置血量 ##TODO 暂时定
    /// </summary>
    /// <param name="hp"></param>
    public void SetHealth(int hp);
    /// <summary>
    /// 此ihurt因为某种原因不能继续作为目标了，请更换目标
    /// </summary>
    /// <returns></returns>
    public bool IsCanBeTarget();
    public Transform GetHurtTrans();
}
