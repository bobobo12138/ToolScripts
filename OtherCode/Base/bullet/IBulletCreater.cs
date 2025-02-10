using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBulletCreater
{
    void Recycle(BulletBase bulletBase);
}
