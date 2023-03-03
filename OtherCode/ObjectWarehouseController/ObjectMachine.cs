using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectMachine
{
    public int machineHash;

    public float timer;
    public float time;

    public Action OnStart;
    public Action OnEnd;

    ObjectWarehouse objectWarehouse;

    protected ObjectMachine(ObjectWarehouse objectWarehouse)
    {
        this.objectWarehouse = objectWarehouse;
        //machineHash = GetHashCode
    }

    public abstract void OnInit();
    public abstract void OnUpdate();

    /// <summary>
    /// вт╩ы
    /// </summary>
    public void SelfDestroy()
    {
        objectWarehouse.RemoveObjectMachine(machineHash);
    }



}
