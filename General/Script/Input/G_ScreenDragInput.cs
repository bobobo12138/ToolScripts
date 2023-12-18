using Game.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 屏幕拖动控制器
/// 支持多指触控
/// 默认返回的是对上层（最后一个）拖动的对象
/// </summary>
public class G_ScreenDragInput : MonoBehaviour
{
    public bool isEnable { get; private set; } = false;
    public bool isTouch { get; private set; } = false;
    public bool isDraging { get; private set; } = false;

    public Action onEnable;
    public Action onDisEnable;
    public Action<Vector2> onStartDrag;
    public Action<Vector2> onEndDrag;
    public Action<Vector2> onClick;                             //返回单击抬起的目标位置
    public Action<(Vector2 deltaPos, Vector2 mousePos)> onDrag;//第一个是delta，第二个是mousepos

    //拖动区域
    [SerializeField]
    [Header("拖动区域")]
    RectTransform DragArea;
    [SerializeField]
    [Header("是否进行射线检测判断")]
    [Tooltip("不仅仅是区域判定，而且增加更严格的射线检测判断，射线射到的第一个obj必须是DragArea")]
    bool isRaycast = false;
    PointerEventData eventData;
    List<RaycastResult> result;

    [Header("判定拖动距离")]
    [SerializeField]
    float dragLimit_X = 4;
    [SerializeField]
    float dragLimit_Y = 4;

    Camera uiCamera;
    bool isStartPosInRect = false;//若初始位置不在拖动区域则不会触发
    Vector2 startPos = Vector2.zero;
    int lastFingerID = -1;//最后一根手指

    public void SetData(Camera uiCamera)
    {
        ///设置UI相机，用于拖动区域判定
        this.uiCamera = uiCamera;
    }


    private void Reset()
    {
        DragArea = GetComponent<RectTransform>();
        if (DragArea == null)
        {
            Debug.LogError("本界面不是rectTransform，没有设置拖动区域！");
            return;
        }
    }

    private void Start()
    {
        eventData = new PointerEventData(EventSystem.current);
        result = new List<RaycastResult>();

        Debug.LogWarning(Screen.width);
        Debug.LogWarning(Screen.height);
    }


    void Update()
    {
        if (DragArea == null) return;
        if (uiCamera == null) return;
        if (!isEnable) return;

        ///获取手指
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        //编辑器模拟
        Touch touch = new Touch();
        touch.fingerId = 0;
        touch.tapCount = 1;
        if (Input.GetMouseButtonDown(0))
        {
            lastFingerID = 0;
            touch.phase = TouchPhase.Began;
            touch.position = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            touch.phase = TouchPhase.Ended;
            touch.position = Input.mousePosition;

        }
        else if (Input.GetMouseButton(0))
        {
            float delta = Input.GetAxis("Mouse X");
            //Debug.Log(delta);
            if (delta != 0)
            {
                touch.phase = TouchPhase.Moved;
                touch.position = Input.mousePosition;
                touch.deltaPosition = new Vector2(delta, 0);
            }
            else
            {
                touch.phase = TouchPhase.Stationary;
            }
        }
        else
        {
            return;
        }
#else
        if (Input.touchCount <= 0) return;
        Touch touch = Input.GetTouch(Input.touchCount - 1);

        foreach (var v in Input.touches)
        {
            //是新手指
            if (v.phase == TouchPhase.Began)
            {
                lastFingerID = v.fingerId;
                touch = v;
                break;
            }

            //没有新手指，尝试获得之前的手指
            if (v.fingerId == lastFingerID)
            {
                touch = v;
            }
        }
#endif

        ///拖动
        if (touch.phase == TouchPhase.Began)
        {
            //开始
            if (touch.tapCount == 1)
            {
                ///判断是否在拖动区域内
                isStartPosInRect = false;
                if (isRaycast)
                {
                    eventData.position = Input.mousePosition;
                    EventSystem.current.RaycastAll(eventData, result);
                    if (result.Count > 0)
                    {
                        if (result[0].gameObject == DragArea.gameObject)
                        {
                            //Debug.Log(result[0].gameObject.name);
                            isStartPosInRect = true;
                        }
                    }

                }
                else
                {
                    isStartPosInRect = RectTransformUtility.RectangleContainsScreenPoint(DragArea, touch.position, uiCamera);
                }

                if (!isStartPosInRect) return; ;
                isTouch = true;
                startPos = touch.position;
                onStartDrag?.Invoke(touch.position);
            }
        }
        else if (touch.phase == TouchPhase.Ended)
        {
            //结束
            if (touch.fingerId == lastFingerID)
            {

                if (isStartPosInRect)
                {
                    if (isDraging == false)
                    {
                        //没有拖动且移动小于10个像素
                        onClick?.Invoke(touch.position);
                    }
                    else
                    {
                        onEndDrag?.Invoke(touch.position);
                    }
                    isTouch = false;
                    isDraging = false;
                    lastFingerID = -1;
                }
            }
        }
        else if (touch.phase == TouchPhase.Moved &&
            (MathF.Abs(touch.position.x - startPos.x) > dragLimit_X || MathF.Abs(touch.position.y - startPos.y) > dragLimit_Y || isDraging == true))
        {
            //拖动中
            if (isStartPosInRect)
            {
                isDraging = true;
                onDrag?.Invoke((touch.deltaPosition, touch.position));
            }
        }
    }

    public void SetEnable()
    {
        onEnable?.Invoke();
        isEnable = true;
    }

    public void SetDisable()
    {
        onDisEnable?.Invoke();
        isEnable = false;

        isTouch = false;
        isDraging = false;
        lastFingerID = -1;
    }
}
