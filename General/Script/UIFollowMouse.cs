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

    Vector3 offset;//偏移修正,被位移者-this
    bool isAutoOffset;//是否自动修正到最大、最小（接近哪边修正到哪边）

    RectTransform uesrRectTrans;

    public Action OnOnDrag = null;//当Drag
    public Action OnOffsetMax = null;//当自动修正到Max
    public Action OnOffsetMin = null;//当自动修正到Min

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
        //修正
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
        vector += offset;//修正
        midPos = (Min + Max)/10;
        Action endActionMaxOrMin =null;//触发最大或者最小的自动修正事件

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
                ///EVERY暂无修正
                break;
        }
        uesrRectTrans.DOAnchorPos(tempPos,0.38f).OnComplete(()=> { endActionMaxOrMin?.Invoke(); });
    }


}
