using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ѭ���б���㷽ʽ
/// </summary>
public enum CyclicListCalculateMode
{
    Adaptive,//����Ӧ����rect�����������ֶ���
    Sequential//��˳��˳����rect
}


public class GUICyclicList_Base : MonoBehaviour
{
    [Tooltip("interactable��ʱ����")]
    public bool interactable;
    [LabelText("�Զ���������")]
    [Tooltip("��ѡ��num_H,num_V��ʧЧ")]
    public bool autoCalculateNum = false;//�Զ�������������ѡ��num_H,num_V��ʧЧ
}
