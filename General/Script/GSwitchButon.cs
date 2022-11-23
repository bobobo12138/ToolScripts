using System;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 开关
/// </summary>
public class GSwitchButon : MonoBehaviour
{
    [Header("turnOff与turnOn的obj")]
    [SerializeField]
    GameObject obj_TrunOn;
    [SerializeField]
    GameObject obj_TrunOff;
    [Header("触发的按钮")]
    [SerializeField]
    Button Button;

    Action ClickTrunOn;
    Action ClickTrunOff;
    private void Start()
    {
        if (Button != null)
        {
            Button.onClick.AddListener(Trigger);
        }
        else
        {
            Debug.LogError("GButton缺少按钮");
        }
    }

    /// <summary>
    /// 设置回调与初始开关是否为打开状态
    /// </summary>
    /// <param name="_triggerCallBack"></param>
    /// <param name="initState"></param>
    public void AddListeners(Action _ClickTrunOn, Action _ClickTrunOff, bool initState=true)
    {
        ClickTrunOn = _ClickTrunOn;
        ClickTrunOff = _ClickTrunOff;

        if (initState)
        {
            ClickTrunOn();
            obj_TrunOn.SetActive(true);
            obj_TrunOff.SetActive(false);

        }
        else
        {
            ClickTrunOff();
            obj_TrunOn.SetActive(false);
            obj_TrunOff.SetActive(true);
        }
    }
    /// <summary>
    /// 触发开关
    /// </summary>
    void Trigger()
    {
        if (ClickTrunOn == null)
        {
            Debug.LogWarning("开关中无事件");
            return;
        }
        Debug.LogWarning("测试");
        if (obj_TrunOn.activeSelf)//触发开关，现在是开，所以要执行关
        {
            ClickTrunOff();
            obj_TrunOn.SetActive(false);
            obj_TrunOff.SetActive(true);
        }
        else if (!obj_TrunOn.activeSelf)
        {
            ClickTrunOn();
            obj_TrunOn.SetActive(true);
            obj_TrunOff.SetActive(false);
        }

    }
}
