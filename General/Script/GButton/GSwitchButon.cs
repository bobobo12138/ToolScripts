using System;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ����
/// </summary>
public class GSwitchButon : MonoBehaviour
{
    [Header("turnOff��turnOn��obj")]
    [SerializeField]
    GameObject obj_TurnOn;
    [SerializeField]
    GameObject obj_TurnOff;
    [Header("�����İ�ť")]
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
            Debug.LogError("GButtonȱ�ٰ�ť");
        }
    }

    /// <summary>
    /// ���ûص����ʼ�����Ƿ�Ϊ��״̬
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
    /// ��������
    /// </summary>
    public void Trigger()
    {
        if (obj_TurnOn.activeSelf)//�������أ������ǿ�������Ҫִ�й�
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
    /// ֻ����״̬�������¼�
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
