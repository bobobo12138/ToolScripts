using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectWarehouseController : MonoBehaviour, IMachine
{
    ///后续需要并入单例框架
    public ObjectWarehouseController instance;
    /// 暂时用int代表id
    Dictionary<int, ObjectMachine> objectWarehouse;


    public void Update()
    {
        if (objectWarehouse.Count>0)
        {
            foreach (var v in objectWarehouse)
            {
                v.Value.Update();
            }
        }
    }


    /// <summary>
    /// 插入对象机，返回对象机所处的id
    /// </summary>
    /// <param name="_objectMachine"></param>
    /// <returns></returns>
    public int AddObjectMachine(ObjectMachine _objectMachine)
    {

        return 0;
    }


    /// <summary>
    /// 从对象仓库中移除
    /// </summary>
    /// <param name="machineId"></param>
    public void RemoveObjectMachine(int machineId)
    {

    }

    public void RemoveObjectMachine(ObjectMachine _objectMachine)
    {

    }

    public void TurnOn()
    {
        throw new System.NotImplementedException();
    }

    public void TurnOff()
    {
        throw new System.NotImplementedException();
    }
}
