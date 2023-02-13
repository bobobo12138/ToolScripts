using DG.Tweening;
using Game.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//public enum DirEnum
//{
//    UP,
//    DOWN,
//    LEFT,
//    RIGHT,
//    FORWARD,
//    BACK,


//    UPDOWN,
//    LEFTRIGHT,
//    FORWARDBACK,


//    EVERY,
//}

public class UIFollowMouse : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public class MoveTime_2D
    {
        public Vector2 vector2;
        public float timer;

        public void Refresh()
        {
            vector2 = Vector2.zero;
            timer = 0;
        }

        public void OffsetMovePos(DirEnum _dirEnum)
        {
            switch (_dirEnum)
            {
                case DirEnum.UPDOWN:
                    vector2.x = 0;
                    break;
                case DirEnum.LEFTRIGHT:
                    vector2.y = 0;
                    break;
                case DirEnum.EVERY:
                    ///EVERY��������
                    break;
            }
        }
    }
    DirEnum dirEnum;

    Vector2 Max;
    Vector2 Min;

    Vector2 offset_Follow;//ƫ������,��λ����-this
    bool isAutoOffset;//�Ƿ��Զ������������С���ӽ��ı��������ıߣ�

    RectTransform uesrRectTrans;

    public Action OnOnDrag = null;//��Drag
    public Action AfterOffsetMax = null;//���Զ�������Max
    public Action AfterOffsetMin = null;//���Զ�������Min

    [Header("�Զ�������Χ_Out��0.5������ϰ벿�������������°벿����������")]
    [Range(0.1f, 0.9f)]
    public float autoOffsetRange_Out = 0.5f;
    [Header("�ٶ�����_timerʱ�������ƶ���Dis�����򴥷�����")]
    public float triggerOffsetDis = 30;
    public float triggerOffsetTimer = 0.4f;
    MoveTime_2D moveTime_2D;


    Vector2 canvasActiveSize;//canvasʵ�ʿ��
    Vector2 screenRealSize; //��Ļʵ�ʿ�ߡ��ֱ���
    Vector2 tSize;


    private void Start()
    {
        moveTime_2D = new MoveTime_2D();
    }

    /// <summary>
    /// ��ʼ��Ҫsetdata
    /// </summary>
    /// <param name="_userRectTrans"></param>
    /// <param name="_dirEnum"></param>
    /// <param name="_max"></param>
    /// <param name="_min"></param>
    /// <param name="_offset_Follow">�����ƶ�������</param>
    /// <param name="_isAutoOffset"></param>
    /// <param name="_canvasActiveSize">canvasʵ�ʿ��</param>
    /// <param name="_screenRealSize">��Ļʵ�ʿ��</param>
    public void SetData(RectTransform _userRectTrans, DirEnum _dirEnum, Vector2 _max, Vector2 _min, Vector2 _offset_Follow, Vector2 _canvasActiveSize, bool _isAutoOffset = true)
    {
        uesrRectTrans = _userRectTrans;
        dirEnum = _dirEnum;
        Max = _max;
        Min = _min;
        offset_Follow = _offset_Follow;
        canvasActiveSize = _canvasActiveSize;
        screenRealSize = new Vector2(Screen.width, Screen.height);//�Լ���ȡ������̫���ˣ������Ż�
        isAutoOffset = _isAutoOffset;

        tSize = canvasActiveSize / screenRealSize;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        uesrRectTrans.DOKill();
        OnOnDrag?.Invoke();
        moveTime_2D.vector2 = eventData.position;
        moveTime_2D.timer = Time.realtimeSinceStartup;
        ReSetOffset(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log(eventData.position);
        SetPos(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        SetPos(eventData.position);
        moveTime_2D.vector2 = eventData.position - moveTime_2D.vector2;//�������
        moveTime_2D.timer = Time.realtimeSinceStartup - moveTime_2D.timer;
        moveTime_2D.OffsetMovePos(dirEnum);
        if (moveTime_2D.timer < triggerOffsetTimer && moveTime_2D.vector2.sqrMagnitude > triggerOffsetDis * triggerOffsetDis)
        {
            ///�ٶȴ�������
            if (isAutoOffset)
                OffsetPos_Speed(moveTime_2D.vector2);
        }
        else
        {
            ///���괥������
            if (isAutoOffset)
                OffsetPos_Auto(eventData.position);
        }
        moveTime_2D.Refresh();
    }



    Vector3 tempPos;
    void SetPos(Vector2 vector)
    {
        vector *= tSize;
        vector += offset_Follow;//����
        vector = Vector3.Max(vector, Min);
        vector = Vector3.Min(vector, Max);
        switch (dirEnum)
        {
            case DirEnum.UPDOWN:
                if (vector.y >= Min.y && vector.y <= Max.y)
                {
                    tempPos.x = uesrRectTrans.anchoredPosition.x;
                    tempPos.y = vector.y;
                    uesrRectTrans.anchoredPosition = tempPos;
                }
                break;
            case DirEnum.LEFTRIGHT://����������˫�򶼲�����������Ϊ������½��ǣ�0,0��
                if (vector.x >= Min.x && vector.x <= Max.x)
                {
                    tempPos.y = uesrRectTrans.anchoredPosition.y;
                    tempPos.x = vector.x;
                    uesrRectTrans.anchoredPosition = tempPos;
                }
                break;
            case DirEnum.EVERY:
                if (vector.y >= Min.y && vector.y <= Max.y)
                {
                    if (vector.x > Min.x && vector.x < Max.x)
                    {
                        tempPos = vector;
                        uesrRectTrans.anchoredPosition = tempPos;
                    }
                }
                break;
        }
    }

    Vector3 midPos;
    void OffsetPos_Auto(Vector2 vector)
    {
        vector *= tSize;
        vector += offset_Follow;//����
        midPos = (Min + Max) * autoOffsetRange_Out;
        Action endActionMaxOrMin = null;//������������С���Զ������¼�

        switch (dirEnum)
        {
            case DirEnum.UPDOWN:
                if (vector.y >= midPos.y)
                {
                    tempPos.x = uesrRectTrans.anchoredPosition.x;
                    tempPos.y = Max.y;
                    endActionMaxOrMin = AfterOffsetMax;
                }
                else
                {
                    tempPos.x = uesrRectTrans.anchoredPosition.x;
                    tempPos.y = Min.y;
                    endActionMaxOrMin = AfterOffsetMin;

                }
                break;
            case DirEnum.LEFTRIGHT:
                if (vector.x >= midPos.x)
                {
                    tempPos.y = uesrRectTrans.anchoredPosition.y;
                    tempPos.x = Max.x;
                    endActionMaxOrMin = AfterOffsetMax;

                }
                else
                {
                    tempPos.y = uesrRectTrans.anchoredPosition.y;
                    tempPos.x = Min.x;
                    endActionMaxOrMin = AfterOffsetMin;

                }
                break;
            case DirEnum.EVERY:
                ///EVERY��������
                break;
        }
        uesrRectTrans.DOAnchorPos(tempPos, 0.38f).OnComplete(() => { endActionMaxOrMin?.Invoke(); });
    }

    void OffsetPos_Speed(Vector2 move)
    {
        Action endActionMaxOrMin = null;//������������С���Զ������¼�
        switch (dirEnum)
        {
            case DirEnum.UPDOWN:
                if (move.y >= 0)
                {
                    tempPos.x = uesrRectTrans.anchoredPosition.x;
                    tempPos.y = Max.y;
                    endActionMaxOrMin = AfterOffsetMax;
                }
                else
                {
                    tempPos.x = uesrRectTrans.anchoredPosition.x;
                    tempPos.y = Min.y;
                    endActionMaxOrMin = AfterOffsetMin;

                }
                break;
            case DirEnum.LEFTRIGHT:
                if (move.x >= 0)
                {
                    tempPos.y = uesrRectTrans.anchoredPosition.y;
                    tempPos.x = Max.x;
                    endActionMaxOrMin = AfterOffsetMax;

                }
                else
                {
                    tempPos.y = uesrRectTrans.anchoredPosition.y;
                    tempPos.x = Min.x;
                    endActionMaxOrMin = AfterOffsetMin;

                }
                break;
            case DirEnum.EVERY:
                ///EVERY��������
                break;
        }
        uesrRectTrans.DOAnchorPos(tempPos, 0.38f).OnComplete(() => { endActionMaxOrMin?.Invoke(); });
    }






    /// <summary>
    /// ÿ�ε����Ӧ������offset
    /// </summary>
    /// <param name="vector"></param>
    void ReSetOffset(Vector2 vector)
    {
        vector *= canvasActiveSize / screenRealSize;
        offset_Follow = new Vector2(vector.x, -vector.y);
    }


}
