using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectWarehouseController : MonoBehaviour, IMachine
{
    ///������Ҫ���뵥�����
    public ObjectWarehouseController instance;
    /// ��ʱ��int����id
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
    /// �������������ض����������id
    /// </summary>
    /// <param name="_objectMachine"></param>
    /// <returns></returns>
    public int AddObjectMachine(ObjectMachine _objectMachine)
    {

        return 0;
    }


    /// <summary>
    /// �Ӷ���ֿ����Ƴ�
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
