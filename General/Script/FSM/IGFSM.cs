using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GeneralFSMʹ���߼̳У�ΪState�ṩ����
/// �ɼ����̳�IGFSMUser�������ض�State
/// </summary>
public interface IGFSM
{
    GameObject GetGameObject();

    GeneralFSM GetGeneralFSM();
}
