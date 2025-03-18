using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRayTarget 
{
    public object data { get; }
    public void OnRayTrigger(RaySource raySource,object data);
}
