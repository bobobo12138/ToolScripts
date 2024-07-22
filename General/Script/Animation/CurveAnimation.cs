using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 常见的曲线动画
/// </summary>
public class CurveAnimation : MonoBehaviour
{
    [Header("作用曲线")]
    [SerializeField]
    AnimationCurve curve;
    [SerializeField]
    float cycleTime = 1;//周期时长
    float timer;
    float value;

    [Header("Position")]
    [SerializeField]
    Vector3 pos_Start;
    [SerializeField]
    Vector3 pos_End;

    [Header("Rotate")]
    [SerializeField]
    Vector3 rotate_Start;
    [SerializeField]
    Vector3 rotate_End;

    [Header("Scale")]
    [SerializeField]
    Vector3 scale_Start = Vector3.one;
    [SerializeField]
    Vector3 scale_End = Vector3.one;


    private void Start()
    {
        timer = cycleTime;
    }

    private void Update()
    {
        value = curve.Evaluate(timer / cycleTime);

        transform.localPosition = Vector3.Lerp(pos_Start, pos_End, value);
        transform.localEulerAngles = Vector3.Lerp(rotate_Start, rotate_End, value);
        transform.localScale = Vector3.Lerp(scale_Start, scale_End, value);

        timer += Time.deltaTime;
        if (timer > cycleTime) timer = timer - cycleTime;//周期
    }

}
