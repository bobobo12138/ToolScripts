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

public class UIFollowMouse : MonoBehaviour, IDropHandler, IDragHandler, IEndDragHandler
{
    DirEnum dirEnum;

    Vector2 Max;
    Vector2 Min;

    Vector2 offset_Follow;//偏移修正,被位移者-this
    bool isAutoOffset;//是否自动修正到最大、最小（接近哪边修正到哪边）

    RectTransform uesrRectTrans;

    public Action OnOnDrag = null;//当Drag
    public Action OnOffsetMax = null;//当自动修正到Max
    public Action OnOffsetMin = null;//当自动修正到Min

    [Header("自动修正范围_Out")]
    [Range(0.1f, 0.9f)]
    public float autoOffsetRange_Out = 0.1f;

    Vector2 canvasActiveSize;//canvas实际宽高
    Vector2 screenRealSize; //屏幕实际宽高、分辨率
    /// <summary>
    /// 初始需要setdata
    /// </summary>
    /// <param name="_userRectTrans"></param>
    /// <param name="_dirEnum"></param>
    /// <param name="_max"></param>
    /// <param name="_min"></param>
    /// <param name="_offset_Follow">跟随移动的修正</param>
    /// <param name="_isAutoOffset"></param>
    /// <param name="_canvasActiveSize">canvas实际宽高</param>
    /// <param name="_screenRealSize">屏幕实际宽高</param>
    public void SetData(RectTransform _userRectTrans, DirEnum _dirEnum, Vector2 _max, Vector2 _min, Vector2 _offset_Follow, Vector2 _canvasActiveSize, Vector2 _screenRealSize, bool _isAutoOffset = true)
    {
        uesrRectTrans = _userRectTrans;
        dirEnum = _dirEnum;
        Max = _max;
        Min = _min;
        offset_Follow = _offset_Follow;
        canvasActiveSize = _canvasActiveSize;
        screenRealSize = _screenRealSize;
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
    void SetPos(Vector2 vector)
    {
        vector *= canvasActiveSize / screenRealSize;
        vector += offset_Follow;//修正
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
    void OffsetPos(Vector2 vector)
    {
        vector *= canvasActiveSize / screenRealSize;
        vector += offset_Follow;//修正
        midPos = (Min + Max) * autoOffsetRange_Out;
        Action endActionMaxOrMin = null;//触发最大或者最小的自动修正事件

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
        uesrRectTrans.DOAnchorPos(tempPos, 0.38f).OnComplete(() => { endActionMaxOrMin?.Invoke(); });
    }


}
