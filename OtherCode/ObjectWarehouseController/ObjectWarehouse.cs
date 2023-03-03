using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// �ֿ�
/// �ֿ�Ҳ��һ������
/// </summary>
public class ObjectWarehouse : MonoBehaviour, IMachine
{
    bool isRun;

    //�洢���ж������ڿ��ٷ���
    Dictionary<int, ObjectMachine> objectWarehouseDic;
    //��Ҫɾ���Ķ�����ȷ������վ���ȴ�objectWarehouseDic������Ϻ����
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
    /// װ�룬���ض����������id
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
    /// �Ӷ���ֿ����Ƴ�
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
