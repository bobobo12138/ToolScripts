using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ѭ��Ԫ������̳еĽӿ�
/// </summary>
public interface ICyclicItem
{
    /// <summary>
    /// ��ʼ�����᷵�ؿ��
    /// </summary>
    /// <param name="_width"></param>
    /// <param name="_height"></param>
    void InitSet(float _width = 0, float _height = 0);
    /// <summary>
    /// �᷵�����б��е�λ��
    /// ���ڳ�ʼ�뷢������ʱ����
    /// </summary>
    /// <param name="index"></param>
    void SetIndex(int index);
    /// <summary>
    /// ������������group
    /// �õ���ǵ�ת������
    /// </summary>
    /// <param name="group"></param>
    void SetGroupData(Object group);


    Object GetObject();
    RectTransform GetRectTransform();
    //int GetIndex();

}
