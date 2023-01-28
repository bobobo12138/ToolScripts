using DG.Tweening;
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

public class UIFollowMouse : MonoBehaviour, IDropHandler, IDragHandler,  IEndDragHandler
{
    DirEnum dirEnum;

    Vector3 Max;
    Vector3 Min;

    Vector3 offset;//ƫ������,��λ����-this
    bool isAutoOffset;//�Ƿ��Զ������������С���ӽ��ı��������ıߣ�

    RectTransform uesrRectTrans;

    public Action OnOnDrag = null;//��Drag
    public Action OnOffsetMax = null;//���Զ�������Max
    public Action OnOffsetMin = null;//���Զ�������Min

    public void SetData(RectTransform _userRectTrans, DirEnum _dirEnum, Vector3 _max, Vector3 _min, Vector3 _offset, bool _isAutoOffset = true)
    {
        uesrRectTrans = _userRectTrans;
        dirEnum = _dirEnum;
        Max = _max;
        Min = _min;
        offset = _offset;
        isAutoOffset = _isAutoOffset;
    }

    public void OnDrop(PointerEventData eventData)
    {
        uesrRectTrans.DOKill();
        OnOnDrag?.Invoke();
        SetPos(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log(eventData.position);
        SetPos(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        SetPos(eventData.position);
        if (isAutoOffset)
            OffsetPos(eventData.position);
    }

    Vector3 tempPos;
    void SetPos(Vector3 vector)
    {
        //����
        vector += offset;
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
            case DirEnum.LEFTRIGHT:
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
    void OffsetPos(Vector3 vector)
    {
        vector += offset;//����
        midPos = (Min + Max)/10;
        Action endActionMaxOrMin =null;//������������С���Զ������¼�

        switch (dirEnum)
        {
            case DirEnum.UPDOWN:
                if (vector.y >= midPos.y)
                {
                    tempPos.x = uesrRectTrans.anchoredPosition.x;
                    tempPos.y = Max.y;
                    endActionMaxOrMin = OnOffsetMax;
                }
                else
                {
                    tempPos.x = uesrRectTrans.anchoredPosition.x;
                    tempPos.y = Min.y;
                    endActionMaxOrMin = OnOffsetMin;

                }
                break;
            case DirEnum.LEFTRIGHT:
                if (vector.x >= midPos.x)
                {
                    tempPos.y = uesrRectTrans.anchoredPosition.y;
                    tempPos.x = Max.x;
                    endActionMaxOrMin = OnOffsetMax;

                }
                else
                {
                    tempPos.y = uesrRectTrans.anchoredPosition.y;
                    tempPos.x = Min.x;
                    endActionMaxOrMin = OnOffsetMin;

                }
                break;
            case DirEnum.EVERY:
                ///EVERY��������
                break;
        }
        uesrRectTrans.DOAnchorPos(tempPos,0.38f).OnComplete(()=> { endActionMaxOrMin?.Invoke(); });
    }


}
