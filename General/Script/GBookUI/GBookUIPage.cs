using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class GBookUIPage : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    const float speed = 0.3f;

    [HideInInspector]
    public int order { get; private set; }//界面顺序优先级，判断动画是向左还是向右
    object tempData;     //需要进行传递的数据，为其赋值后，会在show时传入下一个界面
    GBookUIContainer gBookUIContainer;

    Action<int> onPageChange;
    Action<int> onAfterPageChange;
    Action<object> onPageEnd_Head;
    Action<object> onPageEnd_End;

    [SerializeField]
    protected Button btn_Back;
    [SerializeField]
    protected Button btn_Next;

    [Header("上一个group")]
    protected GBookUIPage lastPage;
    [Header("下一个group")]
    protected GBookUIPage nextPage;

    RectTransform _rectTransform;
    protected RectTransform rectTransform
    {
        get
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }
            return _rectTransform;
        }
        set
        {
            _rectTransform = value;
        }
    }
    protected abstract void OnInit();
    protected abstract void OnShow(object data_In);//显示时data进入
    protected abstract void OnHide();
    protected abstract void OnRecycle();//别的界面返回会自动调用一次

    protected virtual void AfterShow() { }
    protected virtual void AfterHide() { }

    /// <summary>
    /// 若需要点击事件之前/后执行某些操作，重写此方法
    /// 操作完成后/前调用complete
    /// </summary>
    /// <param name="complete"></param>
    protected virtual void OnClickBtn_Back(Action complete)
    {
        complete();
    }
    protected virtual void OnClickBtn_Next(Action complete)
    {
        complete();
    }

    protected virtual void Awake()
    {

    }

    public void InitSet
        (int order, GBookUIContainer gBookUIContainer,
        Action<object> onPageEnd_Head, Action<object> onPageEnd_End, Action<int> onPageChange, Action<int> afterPageChange,
        GBookUIPage lastPagw = null, GBookUIPage nextPage = null)
    {

        this.order = order;
        this.gBookUIContainer = gBookUIContainer;
        this.onPageChange = onPageChange;
        this.onAfterPageChange = afterPageChange;
        this.onPageEnd_Head = onPageEnd_Head;
        this.onPageEnd_End = onPageEnd_End;
        this.lastPage = lastPagw;
        this.nextPage = nextPage;

        if (btn_Back != null)
        {
            btn_Back.onClick.AddListener(() =>
            {
                DoBack();
            });
        }
        else
        {
            Debug.LogWarning("这个界面没有btn_Back：" + transform.name);
        }


        if (btn_Next != null)
        {
            btn_Next.onClick.AddListener(() =>
            {
                DoNext();
            });
        }
        else
        {
            Debug.LogWarning("这个界面没有btn_Next：" + transform.name);
        }

        OnInit();
    }


    /// <summary>
    /// 手动调用上一步\下一步
    /// </summary>
    /// <param name="resetPos">是否自动修复坐标</param>
    public void DoBack(bool resetPos = true)
    {
        OnClickBtn_Back(() =>
        {
            if (lastPage != null)
            {
                gBookUIContainer.ShowMask(true);
                lastPage.Show(tempData, order, resetPos, () => gBookUIContainer.ShowMask(false));
                Hide(lastPage.order, resetPos);
                return;
            }
            Hide(order, resetPos);
            onPageEnd_Head?.Invoke(tempData);//没有last，说明是第一个，所以是头
            Debug.Log("没有上一个page");
        });
    }

    public void DoNext(bool resetPos = true)
    {
        OnClickBtn_Next(() =>
        {
            if (nextPage != null)
            {
                gBookUIContainer.ShowMask(true);
                nextPage.Show(tempData, order, resetPos, () => gBookUIContainer.ShowMask(false));
                Hide(nextPage.order, resetPos);
                return;
            }
            Hide(order);
            onPageEnd_End?.Invoke(tempData);//没有next，说明是最后一个，所以是尾
            Debug.Log("没有下一个page");
        });
    }


    /// <summary>
    /// 显示
    /// 此处会获取上一个界面传递的数据
    /// 不要直接调用show，应当调用框架的Jump或者doNext\back
    /// </summary>
    /// <param name="data"></param>
    /// <param name="LastGroundPriority">会根据优先级进行动画，-1不做动画</param>
    /// <param name="resetPos">是否自动修复坐标</param>
    public void Show(object data = null, int LastGroundPriority = 0, bool resetPos = true, Action callback = null)
    {
        SaveData(data);
        //if (gameObject.activeSelf) return;//拖动时可能需要在显示状态调用

        gameObject.SetActive(true);
        if (resetPos) rectTransform.anchoredPosition = Vector2.zero;

        if (LastGroundPriority != -1)
        {
            if (LastGroundPriority != order) onPageChange?.Invoke(order);
            if (LastGroundPriority < order)
            {
                if (resetPos) rectTransform.anchoredPosition += new Vector2(rectTransform.rect.width, 0);
                rectTransform.DOAnchorPosX(0, speed).OnComplete(() =>
                {
                    callback?.Invoke();
                    onAfterPageChange?.Invoke(order);
                    AfterShow();
                });
            }
            else if (LastGroundPriority > order)
            {
                if (resetPos) rectTransform.anchoredPosition -= new Vector2(rectTransform.rect.width, 0);
                rectTransform.DOAnchorPosX(0, speed).OnComplete(() =>
                {
                    callback?.Invoke();
                    onAfterPageChange?.Invoke(order);
                    AfterShow();
                });
            }
        }
        ///若等于则不做动画
        OnShow(data);
    }

    /// <summary>
    /// 隐藏
    /// </summary>
    /// <param name="nextGroundPriority">判断向左还是向右动画</param>
    /// <param name="resetPos">是否自动修复坐标</param>
    public void Hide(int nextGroundPriority = -1, bool resetPos = true)
    {
        //if (!gameObject.activeSelf) return;

        if (resetPos) rectTransform.anchoredPosition = Vector2.zero;

        if (nextGroundPriority == -1)
        {
            gameObject.SetActive(false);
            Recycle();
            OnHide();
            AfterHide();
            return;
        }

        if (nextGroundPriority < order)
        {
            //高于，是返回，本界面向右
            //返回需要刷新一次数据
            rectTransform.DOAnchorPosX(rectTransform.rect.width, speed).OnComplete(() =>
            {
                gameObject.SetActive(false);
                Recycle();
                AfterHide();
            });
        }
        else if (nextGroundPriority > order)
        {
            //低于，下一个界面，本界面向左
            rectTransform.DOAnchorPosX(-rectTransform.rect.width, speed).OnComplete(() =>
            {
                gameObject.SetActive(false);
                AfterHide();
            });
        }
        else if (nextGroundPriority == order)
        {
            gameObject.SetActive(false);
        }
        OnHide();
    }

    /// <summary>
    /// 回收
    /// 只有全局刷新、本界面向右移动（返回）的时候才会回收；本界面向左移动（下一个）的时候不会回收
    /// </summary>
    public void Recycle()
    {
        OnRecycle();
    }

    /// <summary>
    /// 显示\隐藏下一步按钮
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowBtn_Next(bool isShow = true)
    {
        btn_Next.gameObject.SetActive(isShow);
    }

    public void HideBtn_Next()
    {
        btn_Next.gameObject.SetActive(false);
    }

    /// <summary>
    /// 显示\隐藏上一步按钮
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowBtn_Back(bool isShow = true)
    {
        btn_Back.gameObject.SetActive(isShow);
    }

    public void HideBtn_Back()
    {
        btn_Back.gameObject.SetActive(false);
    }

    /// <summary>
    /// 需要对数据进行保存
    /// 框架只负责数据传递，拿到数据以后的操作需要自己处理
    /// show刚进入的时候会默认进行一次数据保存
    /// </summary>
    /// <param name="data"></param>
    public void SaveData(object data)
    {
        this.tempData = data;
    }

    /// <summary>
    /// 重置
    /// </summary>
    public virtual void ResetGroup()
    {
        rectTransform.anchoredPosition = Vector2.zero;
        gameObject.SetActive(false);
        return;
    }






    Direction direction;
    Vector2 tempDelta;
    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.delta.x < 0 && nextPage != null)
        {
            //next正常触发show
            direction = Direction.Right;
            nextPage.Show(tempData, -1);
            nextPage.rectTransform.anchoredPosition = new Vector2(rectTransform.rect.width, nextPage.rectTransform.anchoredPosition.y);
        }
        else if (eventData.delta.x > 0 && lastPage != null)
        {
            direction = Direction.Left;
            lastPage.Show(tempData, -1);
            lastPage.rectTransform.anchoredPosition = new Vector2(-rectTransform.rect.width, lastPage.rectTransform.anchoredPosition.y);
        }
        else
        {
            //其他情况
            direction = Direction.None;
        }
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        tempDelta.x = eventData.delta.x;
        tempDelta.y = 0;
        if (direction == Direction.Right && nextPage != null)
        {
            rectTransform.anchoredPosition += tempDelta;
            nextPage.rectTransform.anchoredPosition += tempDelta;
            if (rectTransform.anchoredPosition.x < -rectTransform.rect.width)
            {
                //简单的区域限制
                rectTransform.anchoredPosition = new Vector2(-rectTransform.rect.width, 0);
                nextPage.rectTransform.anchoredPosition = new Vector2(0, 0);
                return;
            }
            if (rectTransform.anchoredPosition.x > 0)
            {
                rectTransform.anchoredPosition = new Vector2(0, 0);
                nextPage.rectTransform.anchoredPosition = new Vector2(rectTransform.rect.width, 0);
                return;
            }
        }
        else if (direction == Direction.Left && lastPage != null)
        {
            rectTransform.anchoredPosition += tempDelta;
            lastPage.rectTransform.anchoredPosition += tempDelta;
            if (rectTransform.anchoredPosition.x > rectTransform.rect.width)
            {
                //简单的区域限制
                rectTransform.anchoredPosition = new Vector2(rectTransform.rect.width, 0);
                lastPage.rectTransform.anchoredPosition = new Vector2(0, 0);
                return;
            }
            if (rectTransform.anchoredPosition.x < 0)
            {
                rectTransform.anchoredPosition = new Vector2(0, 0);
                lastPage.rectTransform.anchoredPosition = new Vector2(-rectTransform.rect.width, 0);
                return;
            }
        }
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        if (Mathf.Abs(rectTransform.anchoredPosition.x) > rectTransform.rect.width / 15)
        {
            //滑动大于rectTransform.rect.width / 4进行修正
            if (direction == Direction.Right && nextPage != null)
            {
                DoNext(false);
            }
            else if (direction == Direction.Left && lastPage != null)
            {
                DoBack(false);
            }
        }
        else
        {
            //返回之前状态
            if (direction == Direction.Right && nextPage != null)
            {
                gBookUIContainer.ShowMask(true);
                //nextPage正常触发hide，本界面直接动画回去
                rectTransform.DOAnchorPosX(0, speed).OnComplete(() =>
                {
                    gBookUIContainer.ShowMask(false);
                });
                nextPage.Hide(order, false);
            }
            else if (direction == Direction.Left && lastPage != null)
            {
                gBookUIContainer.ShowMask(true);
                rectTransform.DOAnchorPosX(0, speed).OnComplete(() =>
                {
                    gBookUIContainer.ShowMask(false);
                });
                lastPage.Hide(order, false);
            }
        }

    }


}
