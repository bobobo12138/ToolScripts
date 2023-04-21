using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GRotate : MonoBehaviour
{
    public float Speed;
    public Vector3 vector;
    void Update()
    {
        transform.localEulerAngles += vector * Speed * Time.deltaTime;
    }
}
