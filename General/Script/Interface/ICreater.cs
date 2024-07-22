using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICreater<T>
{
    void Recycle(T member);

    List<T> GetAllCreature();//获得所创造物
}
