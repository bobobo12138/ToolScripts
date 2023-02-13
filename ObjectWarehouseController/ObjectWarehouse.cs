using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 仓库
/// 仓库也是一个对象
/// </summary>
public class ObjectWarehouse : MonoBehaviour, IMachine
{
    bool isRun;

    //存储所有对象，用于快速访问
    Dictionary<int, ObjectMachine> objectWarehouseDic;
    //将要删除的对象会先放入回收站，等待objectWarehouseDic更新完毕后回收
    List<int> objectRecycleBin;

    public ObjectWarehouse()
    {
        objectWarehouseDic = new Dictionary<int, ObjectMachine>();
        objectRecycleBin = new List<int>();
        isRun = true;
    }
    public void Update()
    {
        if (!isRun) return;

        if (objectWarehouseDic.Count > 0)
        {
            foreach (var v in objectWarehouseDic)
            {
                v.Value.OnUpdate();
            }
        }

        if (objectRecycleBin.Count > 0)
        {
            foreach (var v in objectRecycleBin)
            {
                objectWarehouseDic.Remove(v);
            }

            objectRecycleBin.Clear();
        }

    }


    /// <summary>
    /// 装入，返回对象机所处的id
    /// </summary>
    /// <param name="_objectMachine"></param>
    /// <returns></returns>
    public int AddObjectMachine(ObjectMachine _objectMachine)
    {
        objectWarehouseDic.Add(_objectMachine.machineHash, _objectMachine);
        _objectMachine.OnInit();
        return _objectMachine.machineHash;
    }

    /// <summary>
    /// 从对象仓库中移除
    /// </summary>
    /// <param name="machineId"></param>
    public void RemoveObjectMachine(int _machineId)
    {
        if (objectRecycleBin.Contains(_machineId)) return;

        objectRecycleBin.Add(_machineId);
    }

    public void RemoveObjectMachine(ObjectMachine _objectMachine)
    {
        if (objectRecycleBin.Contains(_objectMachine.machineHash)) return;

        objectRecycleBin.Add(_objectMachine.machineHash);
    }

    public void TurnOn()
    {
        isRun = true;
    }

    public void TurnOff()
    {
        isRun = false;
    }
}
