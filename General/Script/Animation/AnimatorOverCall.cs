using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorOverCall : MonoBehaviour
{
    private string instanceID;
    public void SetInstanceID(string id)
    {
        instanceID = id;
    }
    public void OverCall()
    {
        AnimatorOverCallMgr.Instance.HandleEvent(instanceID);
    }

    private void OnDestroy()
    {
        AnimatorOverCallMgr.Instance.RemoveHandleEvent(instanceID);
    }
}

