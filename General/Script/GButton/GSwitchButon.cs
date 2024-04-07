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
    GameObject obj_TurnOn;
    [SerializeField]
    GameObject obj_TurnOff;
    [Header("触发的按钮")]
    [SerializeField]
    Button Button;

    Action ClickTrunOn;
    Action ClickTrunOff;
    private void Awake()
    {
        if (Button != null)
        {
            Button.onClick.AddListener(Trigger);
            ClickTrunOn += () =>
            {
                SetTurnOnStyle();
            };
            ClickTrunOff += () =>
            {
                SetTurnOffStyle();
            };
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
    public void AddListeners(Action _ClickTurnOn, Action _ClickTurnOff, bool initState = true)
    {
        ClickTrunOn += _ClickTurnOn;
        ClickTrunOff += _ClickTurnOff;

        if (initState)
        {
            ClickTrunOn?.Invoke();
        }
        else
        {
            ClickTrunOff?.Invoke();
        }
    }
    /// <summary>
    /// 触发开关
    /// </summary>
    public void Trigger()
    {
        if (obj_TurnOn.activeSelf)//触发开关，现在是开，所以要执行关
        {
            ClickTrunOff?.Invoke();
        }
        else if (!obj_TurnOn.activeSelf)
        {
            ClickTrunOn?.Invoke();
        }

    }

    public void TriggerTrunOn()
    {
        ClickTrunOn?.Invoke();
    }

    public void TriggerTrunOff()
    {
        ClickTrunOff?.Invoke();
    }

    /// <summary>
    /// 只设置状态不触发事件
    /// </summary>
    /// <param name="isTurnOn"></param>
    public void SetState(bool isTurnOn)
    {
        if (isTurnOn)
        {
            SetTurnOnStyle();

        }
        else
        {
            SetTurnOffStyle();
        }
    }



    void SetTurnOnStyle()
    {
        obj_TurnOn.SetActive(true);
        obj_TurnOff.SetActive(false);
    }

    void SetTurnOffStyle()
    {
        obj_TurnOn.SetActive(false);
        obj_TurnOff.SetActive(true);
    }
}
