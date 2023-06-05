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

    Vector2 offset_Follow;//ƫ������,��λ����.pso-uifollowmouse.pos
    bool isAutoOffset;//�Ƿ��Զ������������С���ӽ��ı��������ıߣ�

    RectTransform userRectTrans;

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
        AfterOffsetMin += () => { userRectTrans.DOKill(); };
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
    public void SetData(RectTransform _userRectTrans, DirEnum _dirEnum, Vector2 _max, Vector2 _min, Vector2 _canvasActiveSize, bool _isAutoOffset = true)
    {
        userRectTrans = _userRectTrans;
        dirEnum = _dirEnum;
        Max = _max;
        Min = _min;
        //offset_Follow = _offset_Follow;
        canvasActiveSize = _canvasActiveSize;
        screenRealSize = new Vector2(Screen.width, Screen.height);//�Լ���ȡ������̫���ˣ������Ż�
        isAutoOffset = _isAutoOffset;

        tSize = canvasActiveSize / screenRealSize;
    }
    #region �϶��¼�
    public void OnBeginDrag(PointerEventData eventData)
    {
        userRectTrans.DOKill();
        OnOnDrag?.Invoke();
        moveTime_2D.vector2 = eventData.position;
        moveTime_2D.timer = Time.realtimeSinceStartup;
        ReSetOffset(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log(eventData.position);
        PositionFollow(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        PositionFollow(eventData.position);
        moveTime_2D.vector2 = eventData.position - moveTime_2D.vector2;//�������
        moveTime_2D.timer = Time.realtimeSinceStartup - moveTime_2D.timer;
        moveTime_2D.OffsetMovePos(dirEnum);
        if (moveTime_2D.timer < triggerOffsetTimer && moveTime_2D.vector2.sqrMagnitude > triggerOffsetDis * triggerOffsetDis)
        {
            ///�ٶȴ�������
            if (isAutoOffset)
                AutoOffset_Speed(moveTime_2D.vector2);
        }
        else
        {
            ///���괥������
            if (isAutoOffset)
                AutoOffset_Pos(eventData.position);
        }
        moveTime_2D.Refresh();
    }
    #endregion
    /// <summary>
    /// �ⲿ��������������󲢲��Ŷ���
    /// </summary>
    public void PlaySetMax()
    {
        userRectTrans.anchoredPosition = new Vector2(userRectTrans.anchoredPosition.x, Min.y);
        AutoOffset_Pos(Max, false);
    }
    /// <summary>
    /// �ⲿ������������С�����Ŷ���
    /// </summary>
    public void PlaySetMin()
    {
        userRectTrans.anchoredPosition = new Vector2(userRectTrans.anchoredPosition.x, Max.y);
        AutoOffset_Pos(Min, false);
    }


    Vector3 tempPos;
    void PositionFollow(Vector2 vector)
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
                    tempPos.x = userRectTrans.anchoredPosition.x;
                    tempPos.y = vector.y;
                    userRectTrans.anchoredPosition = tempPos;
                }
                break;
            case DirEnum.LEFTRIGHT://����������˫�򶼲�����������Ϊ������½��ǣ�0,0��
                if (vector.x >= Min.x && vector.x <= Max.x)
                {
                    tempPos.y = userRectTrans.anchoredPosition.y;
                    tempPos.x = vector.x;
                    userRectTrans.anchoredPosition = tempPos;
                }
                break;
            case DirEnum.EVERY:
                if (vector.y >= Min.y && vector.y <= Max.y)
                {
                    if (vector.x > Min.x && vector.x < Max.x)
                    {
                        tempPos = vector;
                        userRectTrans.anchoredPosition = tempPos;
                    }
                }
                break;
        }
    }

    Vector3 midPos;
    /// <summary>
    /// ����vector�Զ�����
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="_isOffsetPos"></param>
    void AutoOffset_Pos(Vector2 vector, bool _isOffsetPos = true)
    {
        vector *= tSize;
        if (_isOffsetPos)
            vector += offset_Follow;//����
        midPos = (Min + Max) * autoOffsetRange_Out;
        Action endActionMaxOrMin = null;//������������С���Զ������¼�

        switch (dirEnum)
        {
            case DirEnum.UPDOWN:
                if (vector.y >= midPos.y)
                {
                    tempPos.x = userRectTrans.anchoredPosition.x;
                    tempPos.y = Max.y;
                    endActionMaxOrMin = AfterOffsetMax;
                }
                else
                {
                    tempPos.x = userRectTrans.anchoredPosition.x;
                    tempPos.y = Min.y;
                    endActionMaxOrMin = AfterOffsetMin;

                }
                break;
            case DirEnum.LEFTRIGHT:
                if (vector.x >= midPos.x)
                {
                    tempPos.y = userRectTrans.anchoredPosition.y;
                    tempPos.x = Max.x;
                    endActionMaxOrMin = AfterOffsetMax;

                }
                else
                {
                    tempPos.y = userRectTrans.anchoredPosition.y;
                    tempPos.x = Min.x;
                    endActionMaxOrMin = AfterOffsetMin;

                }
                break;
            case DirEnum.EVERY:
                ///EVERY��������
                break;
        }
        userRectTrans.DOAnchorPos(tempPos, 0.38f).OnComplete(() => { endActionMaxOrMin?.Invoke(); });
    }
    /// <summary>
    /// ����move���ٶ��Զ�����
    /// </summary>
    /// <param name="move"></param>
    void AutoOffset_Speed(Vector2 move)
    {
        Action endActionMaxOrMin = null;//������������С���Զ������¼�
        switch (dirEnum)
        {
            case DirEnum.UPDOWN:
                if (move.y >= 0)
                {
                    tempPos.x = userRectTrans.anchoredPosition.x;
                    tempPos.y = Max.y;
                    endActionMaxOrMin = AfterOffsetMax;
                }
                else
                {
                    tempPos.x = userRectTrans.anchoredPosition.x;
                    tempPos.y = Min.y;
                    endActionMaxOrMin = AfterOffsetMin;

                }
                break;
            case DirEnum.LEFTRIGHT:
                if (move.x >= 0)
                {
                    tempPos.y = userRectTrans.anchoredPosition.y;
                    tempPos.x = Max.x;
                    endActionMaxOrMin = AfterOffsetMax;

                }
                else
                {
                    tempPos.y = userRectTrans.anchoredPosition.y;
                    tempPos.x = Min.x;
                    endActionMaxOrMin = AfterOffsetMin;

                }
                break;
            case DirEnum.EVERY:
                ///EVERY��������
                break;
        }
        userRectTrans.DOAnchorPos(tempPos, 0.38f).OnComplete(() => { endActionMaxOrMin?.Invoke(); });
    }

    /// <summary>
    /// ÿ�ε����Ӧ������offset_Follow
    /// </summary>
    /// <param name="vector"></param>
    void ReSetOffset(Vector2 vector)
    {
        vector *= canvasActiveSize / screenRealSize;
        offset_Follow = new Vector2(vector.x, -vector.y) + userRectTrans.anchoredPosition;
    }


}
