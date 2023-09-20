using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class GBookUIPage : MonoBehaviour
{
    [HideInInspector]
    public int order { get; private set; }//界面顺序优先级，判断动画是向左还是向右
    object tempData;     //需要进行传递的数据，为其赋值后，会在show时传入下一个界面

    //Action<int> onPageChange;
    //Action<int> onAfterPageChange;
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

    public void InitSet(int order, Action<object> onPageEnd_Head, Action<object> onPageEnd_End, GBookUIPage lastPagw = null, GBookUIPage nextPage = null)
    {
        this.order = order;
        //this.onPageChange = onPageChange;
        //this.onAfterPageChange = afterPageChange;
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
    public void DoBack()
    {
        OnClickBtn_Back(() =>
        {
            if (lastPage != null)
            {
                lastPage.Show(tempData, order);
                Hide(lastPage.order);
                return;
            }
            Hide(order);
            onPageEnd_Head(tempData);//没有last，说明是第一个，所以是头
            Debug.Log("没有上一个page");
        });
    }

    public void DoNext()
    {
        OnClickBtn_Next(() =>
        {
            if (nextPage != null)
            {
                nextPage.Show(tempData, order);
                Hide(nextPage.order);
                return;
            }
            Hide(order);
            onPageEnd_End(tempData);//没有next，说明是最后一个，所以是尾
            Debug.Log("没有下一个page");
        });
    }


    /// <summary>
    /// 显示
    /// 此处会获取上一个界面传递的数据
    /// 不要直接调用show，应当调用框架的Jump或者doNext\back
    /// </summary>
    /// <param name="data"></param>
    /// <param name="LastGroundPriority">会根据优先级进行动画</param>
    public void Show(object data = null, int LastGroundPriority = 0)
    {
        SaveData(data);
        if (gameObject.activeSelf) return;

        gameObject.SetActive(true);
        rectTransform.anchoredPosition = Vector2.zero;

        ///if (LastGroundPriority != order) onPageChange(order);
        if (LastGroundPriority < order)
        {
            rectTransform.anchoredPosition += new Vector2(rectTransform.rect.width, 0);
            rectTransform.DOAnchorPosX(0, 0.5f).OnComplete(() =>
            {
                //onAfterPageChange(order);
                AfterShow();
            });
        }
        else if (LastGroundPriority > order)
        {
            rectTransform.anchoredPosition -= new Vector2(rectTransform.rect.width, 0);
            rectTransform.DOAnchorPosX(0, 0.5f).OnComplete(() =>
            {
                //onAfterPageChange(order);
                AfterShow();
            });
        }
        ///若等于则不做动画
        OnShow(data);
    }

    /// <summary>
    /// 隐藏
    /// </summary>
    /// <param name="nextGroundPriority">判断向左还是向右动画</param>
    public void Hide(int nextGroundPriority = -1)
    {
        if (!gameObject.activeSelf) return;

        rectTransform.anchoredPosition = Vector2.zero;

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
            rectTransform.DOAnchorPosX(rectTransform.rect.width, 0.5f).OnComplete(() =>
            {
                gameObject.SetActive(false);
                Recycle();
                AfterHide();
            });
        }
        else if (nextGroundPriority > order)
        {
            //低于，下一个界面，本界面向左
            rectTransform.DOAnchorPosX(-rectTransform.rect.width, 0.5f).OnComplete(() =>
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
}
