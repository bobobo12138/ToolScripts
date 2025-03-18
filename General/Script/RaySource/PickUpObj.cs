using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpObj : MonoBehaviour, IRayTarget
{
    object IRayTarget.data { get => "im monkey!"; }

    void IRayTarget.OnRayTrigger(RaySource raySource, object data)
    {

    }
}