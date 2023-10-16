using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GeneralFSM使用者继承，为State提供参数
/// 可继续继承IGFSMUser以适配特定State
/// </summary>
public interface IGFSM
{
    GameObject GetGameObject();

    GeneralFSM GetGeneralFSM();
}
