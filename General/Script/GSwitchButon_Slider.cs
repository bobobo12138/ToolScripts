using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class GSwitchButon_Slider : MonoBehaviour
{
    [SerializeField]
    float aniTime = 0.35f;

    bool isThisInAni = false;//�Ƿ����ڲ�����
    bool nowState = false;//��ǰ״̬��trueΪ����falseΪ��

    [Header("turnOff��turnOn��obj")]
    [SerializeField]
    CanvasGroup canvasGroup_TurnOn;
    [SerializeField]
    CanvasGroup canvasGroup_TurnOff;
    [Header("�����İ�ť")]
    [SerializeField]
    Button Button;

    [Header("����")]
    [SerializeField]
    Transform slider;
    [SerializeField]
    CanvasGroup canvasGroup_Silder_On;
    [SerializeField]
    CanvasGroup canvasGroup_Silder_Off;
    [SerializeField]
    Transform move_On;
    [SerializeField]
    Transform move_Off;


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
            AprilDebug.LogWarning("GButtonȱ�ٰ�ť");
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
            SetTurnOnStyle();
        }
        else
        {
            SetTurnOffStyle();
        }
    }
    /// <summary>
    /// ��������
    /// </summary>
    public void Trigger()
    {
        if (isThisInAni) return;

        if (canvasGroup_TurnOn.gameObject.activeSelf)//�������أ������ǿ�������Ҫִ�й�
        {
            ClickTrunOff?.Invoke();
        }
        else if (!canvasGroup_TurnOn.gameObject.activeSelf)
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
    public void SetState(bool isTurnOn, bool isAni = false)
    {
        if (isTurnOn)
        {
            SetTurnOnStyle(isAni);

        }
        else
        {
            SetTurnOffStyle(isAni);
        }
    }



    void SetTurnOnStyle(bool isAni = true)
    {
        if (isAni && (nowState == false))
        {
            isThisInAni = true;

            canvasGroup_TurnOn.gameObject.SetActive(true);
            canvasGroup_TurnOn.alpha = 0;
            canvasGroup_TurnOn.DOFade(1f, aniTime).OnComplete(() =>
            {
                isThisInAni = false;
            });
            canvasGroup_Silder_On.gameObject.SetActive(true);
            canvasGroup_Silder_On.alpha = 0;
            canvasGroup_Silder_On.DOFade(1f, aniTime);



            canvasGroup_TurnOff.gameObject.SetActive(true);
            canvasGroup_TurnOff.alpha = 1;
            canvasGroup_TurnOff.DOFade(0f, aniTime).OnComplete(() =>
            {
                canvasGroup_TurnOff.gameObject.SetActive(false);
            });
            canvasGroup_Silder_Off.gameObject.SetActive(true);
            canvasGroup_Silder_Off.alpha = 1;
            canvasGroup_Silder_Off.DOFade(0f, aniTime).OnComplete(() =>
            {
                canvasGroup_Silder_Off.gameObject.SetActive(false);
            });

            slider.DOMove(move_On.position, aniTime);
        }
        else
        {
            isThisInAni = false;
            canvasGroup_TurnOn.gameObject.SetActive(true);
            canvasGroup_TurnOn.alpha = 1;
            canvasGroup_Silder_On.gameObject.SetActive(true);
            canvasGroup_Silder_On.alpha = 1;

            canvasGroup_TurnOff.gameObject.SetActive(false);
            canvasGroup_TurnOff.alpha = 1;
            canvasGroup_Silder_Off.gameObject.SetActive(false);
            canvasGroup_Silder_Off.alpha = 1;
            slider.position = move_On.position;
        }
        nowState = true;
    }

    void SetTurnOffStyle(bool isAni = true)
    {
        if (isAni && (nowState == true))
        {
            isThisInAni = true;

            canvasGroup_TurnOff.gameObject.SetActive(true);
            canvasGroup_TurnOff.alpha = 0;
            canvasGroup_TurnOff.DOFade(1f, aniTime).OnComplete(() =>
            {
                isThisInAni = false;
            });
            canvasGroup_Silder_Off.gameObject.SetActive(true);
            canvasGroup_Silder_Off.alpha = 0;
            canvasGroup_Silder_Off.DOFade(1f, aniTime);



            canvasGroup_TurnOn.gameObject.SetActive(true);
            canvasGroup_TurnOn.alpha = 1;
            canvasGroup_TurnOn.DOFade(0f, aniTime).OnComplete(() =>
            {
                canvasGroup_TurnOn.gameObject.SetActive(false);
            });
            canvasGroup_Silder_On.gameObject.SetActive(true);
            canvasGroup_Silder_On.alpha = 1;
            canvasGroup_Silder_On.DOFade(0f, aniTime).OnComplete(() =>
            {
                canvasGroup_Silder_On.gameObject.SetActive(false);
            });

            slider.DOMove(move_Off.position, aniTime);
        }
        else
        {
            isThisInAni = false;
            canvasGroup_TurnOff.gameObject.SetActive(true);
            canvasGroup_TurnOff.alpha = 1;
            canvasGroup_Silder_Off.gameObject.SetActive(true);
            canvasGroup_Silder_Off.alpha = 1;

            canvasGroup_TurnOn.gameObject.SetActive(false);
            canvasGroup_TurnOn.alpha = 1;
            canvasGroup_Silder_On.gameObject.SetActive(false);
            canvasGroup_Silder_On.alpha = 1;
            slider.position = move_Off.position;
        }
        nowState = false;
    }
}
