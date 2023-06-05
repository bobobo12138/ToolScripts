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
                    ///EVERY暂无修正
                    break;
            }
        }
    }
    DirEnum dirEnum;

    Vector2 Max;
    Vector2 Min;

    Vector2 offset_Follow;//偏移修正,被位移者.pso-uifollowmouse.pos
    bool isAutoOffset;//是否自动修正到最大、最小（接近哪边修正到哪边）

    RectTransform userRectTrans;

    public Action OnOnDrag = null;//当Drag
    public Action AfterOffsetMax = null;//当自动修正到Max
    public Action AfterOffsetMin = null;//当自动修正到Min

    [Header("自动修正范围_Out，0.5则代表上半部分向上修正，下半部分向下修正")]
    [Range(0.1f, 0.9f)]
    public float autoOffsetRange_Out = 0.5f;
    [Header("速度修正_timer时间内若移动了Dis距离则触发修正")]
    public float triggerOffsetDis = 30;
    public float triggerOffsetTimer = 0.4f;
    MoveTime_2D moveTime_2D;


    Vector2 canvasActiveSize;//canvas实际宽高
    Vector2 screenRealSize; //屏幕实际宽高、分辨率
    Vector2 tSize;


    private void Start()
    {
        moveTime_2D = new MoveTime_2D();
        AfterOffsetMin += () => { userRectTrans.DOKill(); };
    }

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
    public void SetData(RectTransform _userRectTrans, DirEnum _dirEnum, Vector2 _max, Vector2 _min, Vector2 _canvasActiveSize, bool _isAutoOffset = true)
    {
        userRectTrans = _userRectTrans;
        dirEnum = _dirEnum;
        Max = _max;
        Min = _min;
        //offset_Follow = _offset_Follow;
        canvasActiveSize = _canvasActiveSize;
        screenRealSize = new Vector2(Screen.width, Screen.height);//自己获取，参数太多了，考虑优化
        isAutoOffset = _isAutoOffset;

        tSize = canvasActiveSize / screenRealSize;
    }
    #region 拖动事件
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
        moveTime_2D.vector2 = eventData.position - moveTime_2D.vector2;//算出增量
        moveTime_2D.timer = Time.realtimeSinceStartup - moveTime_2D.timer;
        moveTime_2D.OffsetMovePos(dirEnum);
        if (moveTime_2D.timer < triggerOffsetTimer && moveTime_2D.vector2.sqrMagnitude > triggerOffsetDis * triggerOffsetDis)
        {
            ///速度触发修正
            if (isAutoOffset)
                AutoOffset_Speed(moveTime_2D.vector2);
        }
        else
        {
            ///坐标触发修正
            if (isAutoOffset)
                AutoOffset_Pos(eventData.position);
        }
        moveTime_2D.Refresh();
    }
    #endregion
    /// <summary>
    /// 外部调动的修正到最大并播放动画
    /// </summary>
    public void PlaySetMax()
    {
        userRectTrans.anchoredPosition = new Vector2(userRectTrans.anchoredPosition.x, Min.y);
        AutoOffset_Pos(Max, false);
    }
    /// <summary>
    /// 外部调动的修正最小并播放动画
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
        vector += offset_Follow;//修正
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
            case DirEnum.LEFTRIGHT://现在左右与双向都不能运作，因为鼠标左下角是（0,0）
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
    /// 根据vector自动修正
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="_isOffsetPos"></param>
    void AutoOffset_Pos(Vector2 vector, bool _isOffsetPos = true)
    {
        vector *= tSize;
        if (_isOffsetPos)
            vector += offset_Follow;//修正
        midPos = (Min + Max) * autoOffsetRange_Out;
        Action endActionMaxOrMin = null;//触发最大或者最小的自动修正事件

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
                ///EVERY暂无修正
                break;
        }
        userRectTrans.DOAnchorPos(tempPos, 0.38f).OnComplete(() => { endActionMaxOrMin?.Invoke(); });
    }
    /// <summary>
    /// 根据move的速度自动修正
    /// </summary>
    /// <param name="move"></param>
    void AutoOffset_Speed(Vector2 move)
    {
        Action endActionMaxOrMin = null;//触发最大或者最小的自动修正事件
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
                ///EVERY暂无修正
                break;
        }
        userRectTrans.DOAnchorPos(tempPos, 0.38f).OnComplete(() => { endActionMaxOrMin?.Invoke(); });
    }

    /// <summary>
    /// 每次点击都应该重置offset_Follow
    /// </summary>
    /// <param name="vector"></param>
    void ReSetOffset(Vector2 vector)
    {
        vector *= canvasActiveSize / screenRealSize;
        offset_Follow = new Vector2(vector.x, -vector.y) + userRectTrans.anchoredPosition;
    }


}
