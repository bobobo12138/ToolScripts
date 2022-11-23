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
    GameObject obj_TrunOn;
    [SerializeField]
    GameObject obj_TrunOff;
    [Header("�����İ�ť")]
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
            Debug.LogError("GButtonȱ�ٰ�ť");
        }
    }

    /// <summary>
    /// ���ûص����ʼ�����Ƿ�Ϊ��״̬
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
    /// ��������
    /// </summary>
    void Trigger()
    {
        if (ClickTrunOn == null)
        {
            Debug.LogWarning("���������¼�");
            return;
        }
        Debug.LogWarning("����");
        if (obj_TrunOn.activeSelf)//�������أ������ǿ�������Ҫִ�й�
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
