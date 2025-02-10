using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHurt
{
    /// <summary>
    /// ������ɸѡ�˺�Ŀ��
    /// </summary>
    public HurtEnum type { get; }
    /// <summary>
    /// ֱ��ɱ��
    /// </summary>
    public void Kill();
    /// <summary>
    /// �˺�
    /// </summary>
    /// <param name="damage"></param>
    public void Hurt(Damage damage);
    /// <summary>
    /// ����Ѫ�� ##TODO ��ʱ��
    /// </summary>
    /// <param name="hp"></param>
    public void SetHealth(int hp);
    /// <summary>
    /// ��ihurt��Ϊĳ��ԭ���ܼ�����ΪĿ���ˣ������Ŀ��
    /// </summary>
    /// <returns></returns>
    public bool IsCanBeTarget();
    public Transform GetHurtTrans();
}
