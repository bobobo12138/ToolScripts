using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RectTransform;
namespace General
{

    /// <summary>
    /// 多层循环列表
    /// 滑动方向是垂直方向，实例化是从上到下，从左到右
    /// 需要插件odin
    /// ICyclicItem
    /// 实例化、循环逻辑都由本类控制，实例化时会返回回调onOnePrefab_ItemCreated，可以再此时设置item参数
    /// </summary>
    public class GUICyclicList_V : MonoBehaviour
    {
        public bool isInit = false;

        /// <summary>
        /// 交换时、刷新时触发,返回item、所处位置（初始化也会触发
        /// 需要在InstantiateItem中监听此回调以方便对item进行操作
        /// </summary>
        Action<ICyclicItem, int> onExchange;

        [Tooltip("interactable暂时无用")]
        public bool interactable;

        public int count { get { return num_H * (num_V + catchNum); } }//当前总量
        [HideInInspector]
        public Dictionary<int, ICyclicItem> itemDic;//索引从0开始

        #region 必要参数
        [Header("Adaptive:将rect铺满，不出现多余;Sequential:顺序安置rect")]
        [Header("注意若是Sequential最好设置更大的缓冲区")]
        public CyclicListCalculateMode cyclicListCalculateMode = CyclicListCalculateMode.Adaptive;//计算方式，默认为自适应
        public GameObject prefab_Item;
        public ICyclicItem prefab_Item_ICyclicItem;
        [HideInInspector]
        public int maxIndex;//最大数量

        //[InfoBox("若是同时勾选了自动计算数量与最大化尺寸，则会导致某行、列最大化且只有一个元素")]
        [Tooltip("勾选后num_H将失效")]
        public bool autoCalculateNum_H = false;//自动计算数量，勾选后,num_H将失效
        //[Tooltip("勾选后num_V将失效")]
        //public bool autoCalculateNum_V = false;//自动计算数量，勾选后,num_V将失效
        [Header("行数量")]
        [SerializeField]
        //[HideIf("autoCalculateNum_H", true)]
        public int num_H = 2;//行可以是自动计算
        int num_V = 2;       //列永远是自动计算
        [Header("缓冲区，建议1-3(若tb、lr较大，则依情况设置更大的缓冲区)")]
        [SerializeField]
        public int catchNum = 1;//容错数量
        [Header("底部距离")]
        [SerializeField]
        public int buttomSpacing = 200;



        [Header("上下左右间隔")]
        [SerializeField] protected Vector2 LR = Vector2.zero;
        [SerializeField] protected Vector2 TB = Vector2.zero;
        [SerializeField]protected int spacing;
        [Header("单元格大小")]
        [SerializeField] bool isControlChildWidth2Max = false;//是否控制子物体宽度至最大
        //[SerializeField][BoxGroup("单元参数")] bool isControlChildHigth2Max = false;//是否控制子物体宽度至最大
        [SerializeField] protected Vector2 itemSize;
        #endregion;

        protected bool isLoad = false;
        /// <summary> 程序会通过首尾行进行计算 </summary>
        protected int headIndex;  //首
        protected int endIndex;   //尾
        protected float xincrement
        {
            get { return ((rectTransform.rect.width - LR.x - LR.y) - (num_H - 1) * spacing) / (num_H); }
        }//x增量
        protected float yincrement
        {

            get
            {
                if (cyclicListCalculateMode == CyclicListCalculateMode.Sequential)
                {
                    return itemSize.y;
                }
                return ((rectTransform.rect.height - TB.x - TB.y) - (num_V - 1) * spacing) / (num_V);
            }
        }//y增量

        protected RectTransform rectTransform;
        protected ScrollRect scrollRect;
        /// <summary>
        /// 可通过SetData或者在inspector中赋值
        /// 注意若有SetData则必须放在Init之前
        /// data会动传入item
        /// </summary>
        public void SetData(ICyclicItem prefab_Item, int maxIndex, int num_H, int num_V, int catchNum_V, Vector2 lR, Vector2 tB, int spacing, Vector2 itemSize)
        {
            this.prefab_Item = prefab_Item.GetRectTransform().gameObject;
            this.prefab_Item_ICyclicItem = prefab_Item;
            this.maxIndex = maxIndex;
            this.num_H = num_H;
            this.num_V = num_V;
            this.catchNum = catchNum_V;
            LR = lR;
            TB = tB;
            this.spacing = spacing;
            this.itemSize = itemSize;
        }

        public void SetPrefab(ICyclicItem prefab_Item)
        {
            this.prefab_Item = prefab_Item.GetRectTransform().gameObject;
            this.prefab_Item_ICyclicItem = prefab_Item;
        }

        protected virtual void OnInit()
        {

        }
        /// <summary>
        /// 此Init需要放在start中执行
        /// onInstantiate当一个prefab完成了实例化，返回该prefab，可进行初始化
        /// </summary>
        public void InstantiateItem(Action<ICyclicItem, int> onInit = null, Action<ICyclicItem, int> onExchange = null, int maxIndex = -1)
        {
            if (isLoad) return;
            OnInit();
            this.onExchange = onExchange;

            if (maxIndex != -1) this.maxIndex = maxIndex;
            if (prefab_Item_ICyclicItem == null) prefab_Item_ICyclicItem = prefab_Item.GetComponent<ICyclicItem>();
            if (prefab_Item_ICyclicItem == null)
            {
                Debug.LogError("错误，prefab未继承ICyclicItem");
                return;
            }
            rectTransform = GetComponent<RectTransform>();
            scrollRect = GetComponent<ScrollRect>();
            if (scrollRect == null) Debug.LogError("需要放置于包含scrollRect的物体下");
            Vector2 last = Vector2.zero;
            scrollRect.onValueChanged.AddListener((pos) =>
            {
                if (!isLoad) return;
                if (pos.y > last.y)
                {
                    //向上
                    IsCyclicMove_Up();
                }
                else
                {
                    //向下
                    IsCyclicMove_Down();
                }
                last = pos;
            });


            //自动计算数量
            if (autoCalculateNum_H)
            {
                Canvas.ForceUpdateCanvases();
                if (isControlChildWidth2Max)
                {
                    //autoCalculateNum_H&&isControlChildWidth2Max的情况下，会导致某行、列最大化且只有一个元素
                    num_H = 1;
                    itemSize.x = (rectTransform.rect.width - LR.x - LR.y);
                }
                else
                {
                    var width = rectTransform.rect.width - LR.x - LR.y;
                    num_H = Mathf.Max(1, (int)Mathf.Floor((width + spacing) / (itemSize.x + spacing)));//y=bx+(x-1)a，y总宽，x单元格数量，b单元格宽度，a间隔；在这里我们的y,b,a都是已知的，求x
                }
            }
            else
            {
                if (isControlChildWidth2Max)
                {
                    itemSize.x = (rectTransform.rect.width - LR.x - LR.y - (num_H - 1) * spacing) / num_H;
                }
            }

            var height = rectTransform.rect.height - TB.x - TB.y;//v的数量永远是自动计算的
            num_V = Mathf.Max(1, (int)Mathf.Floor((height + spacing) / (itemSize.y + spacing)));




            if (num_H <= 0 || num_V <= 0)
            {
                Debug.LogError("无法容纳任何item，会导致除数为0！");
                return;
            }

            itemDic = new Dictionary<int, ICyclicItem>();
            for (int i = 0; i < num_V + catchNum; i++)
            {
                for (int j = 0; j < num_H; j++)
                {
                    var tempItem = Instantiate(prefab_Item, scrollRect.content).GetComponent<ICyclicItem>();
                    onInit?.Invoke(tempItem, i * num_H + j);
                    tempItem.InitSet(itemSize.x, itemSize.y);
                    tempItem.GetRectTransform().anchoredPosition = new Vector2(GetIndexPosX(j + 1), GetIndexPosY(i + 1));
                    if (i * num_H + j >= this.maxIndex)//超过最大值的设置不可见
                    {
                        tempItem.GetRectTransform().gameObject.SetActive(false);
                    }
                    else
                    {
                        tempItem.SetIndex(i * num_H + j);
                        tempItem.GetRectTransform().gameObject.SetActive(true);
                    }
                    tempItem.GetRectTransform().SetSizeWithCurrentAnchors(Axis.Horizontal, itemSize.x);
                    tempItem.GetRectTransform().SetSizeWithCurrentAnchors(Axis.Vertical, itemSize.y);
                    tempItem.GetRectTransform().anchorMin = new Vector2(0.5f, 1f);//设置顶部锚点
                    tempItem.GetRectTransform().anchorMax = new Vector2(0.5f, 1f);
                    RefreshSize_Force(tempItem.GetRectTransform());
                    itemDic.Add(i * num_H + j, tempItem);
                }
            }
            headIndex = 0;
            endIndex = count - num_H;
            isLoad = true;

            Debug.Log("初始化GUICyclicList");
        }

        protected virtual void OnRefresh()
        {

        }

        public void Refresh(int maxIndex = -1)
        {
            if (!isLoad) return;
            OnRefresh();

            if (maxIndex != -1) this.maxIndex = maxIndex;


            Dictionary<int, ICyclicItem> tempDic = new Dictionary<int, ICyclicItem>();
            int i = 0;
            foreach (var v in itemDic)
            {
                v.Value.Refresh();
                tempDic.Add(i, v.Value);
                v.Value.GetRectTransform().anchoredPosition = new Vector2(GetIndexPosX((i % num_H) + 1), GetIndexPosY((i / num_H) + 1));
                RefreshSize_Force(v.Value.GetRectTransform());
                if (i >= this.maxIndex)
                {
                    v.Value.GetRectTransform().gameObject.SetActive(false);
                }
                else
                {
                    //AprilDebug.Log(i);
                    v.Value.SetIndex(i);
                    onExchange?.Invoke(v.Value, i);
                    v.Value.GetRectTransform().gameObject.SetActive(true);
                }
                i++;
            }
            headIndex = 0;
            endIndex = count - num_H;
            itemDic = tempDic;
            scrollRect.content.anchoredPosition = Vector2.zero;
        }



        #region 初始运行时计算XY位置
        /// <summary>
        /// 返回所在Index的X轴
        /// </summary>
        /// <param name="increment"></param>
        /// <param name="index">行第index个元素，与dic不同，最小是1，所以要+1</param>
        /// <returns></returns>
        float GetIndexPosX(int index)
        {
            float increment = xincrement;//增量
            return (-rectTransform.rect.width / 2) + LR.x + (spacing * (index - 1)) + ((increment / 2) + (index - 1) * increment);//从rect.with/2开始计算，因为X锚点在中心
        }

        /// <summary>
        /// 返回所在Index的Y轴
        /// </summary>
        /// <param name="index">列第index个元素，与dic不同，最小是1，所以要+1</param>
        /// <returns></returns>
        float GetIndexPosY(int index)
        {
            float increment = yincrement;//增量
            return 0 - TB.x - (spacing * (index - 1)) - ((increment / 2) + (index - 1) * increment);//从0坐标开始计算，因为子集Y锚点在顶部
        }

        #endregion;
        #region 循环列表核心移动逻辑
        /// <summary>
        /// 是否向上移动
        /// </summary>
        void IsCyclicMove_Up()
        {
            var limit = transform.parent.TransformPoint
                        (
                            new Vector3(0, rectTransform.localPosition.y - rectTransform.rect.height / 2 - itemSize.y / 2 - TB.y, 0)
                        ).y;
            int ori = -1;
            int target = -1;
            //判断尾是否需要去首
            if (itemDic[endIndex].GetRectTransform().transform.position.y < limit)
            {
                ori = endIndex;
                target = endIndex - count;
            }

            if (ori == -1 && target == -1) return;
            if (target >= maxIndex) return;
            if (target < 0) return;
            for (int i = 0; i < num_H; i++)
            {
                var temp = itemDic[ori + i];
                itemDic.Remove(ori + i);
                itemDic.Add(target + i, temp);

                //向上移动
                temp.GetRectTransform().anchoredPosition = new Vector2(
                temp.GetRectTransform().anchoredPosition.x,
                itemDic[target + num_H].GetRectTransform().anchoredPosition.y + yincrement + spacing);
                temp.GetRectTransform().gameObject.SetActive(true);
                temp.SetIndex(target + i);
                onExchange?.Invoke(temp, target + i);
            }

            headIndex = target;
            endIndex = endIndex - num_H;
            RefreshSize(itemDic[endIndex].GetRectTransform());
            //AprilDebug.Log("向上移动");
        }

        /// <summary>
        /// 是否向下移动 
        /// </summary>
        void IsCyclicMove_Down()
        {
            var limit = transform.parent.TransformPoint
                (
                     new Vector3(0, rectTransform.localPosition.y + rectTransform.rect.height / 2 + itemSize.y / 2 + TB.x, 0)
            ).y;
            int ori = -1;
            int target = -1;
            ///判断首是否需要去尾
            if (itemDic[headIndex].GetRectTransform().transform.position.y > limit)
            {
                ori = headIndex;
                target = headIndex + count;
            }

            if (ori == -1 && target == -1) return;
            if (target >= maxIndex) return;
            if (target < 0) return;
            for (int i = 0; i < num_H; i++)
            {
                var temp = itemDic[ori + i];
                itemDic.Remove(ori + i);
                itemDic.Add(target + i, temp);

                //向下移动
                temp.GetRectTransform().anchoredPosition = new Vector2(
                temp.GetRectTransform().anchoredPosition.x,
                itemDic[target - num_H].GetRectTransform().anchoredPosition.y - yincrement - spacing);


                //超过最大值的设置不可见
                if (target + i >= maxIndex)
                {
                    temp.GetRectTransform().gameObject.SetActive(false);
                }
                else
                {
                    temp.SetIndex(target + i);
                    onExchange?.Invoke(temp, target + i);
                    temp.GetRectTransform().gameObject.SetActive(true);
                }
            }
            endIndex = target;
            headIndex = headIndex + num_H;
            RefreshSize(itemDic[endIndex].GetRectTransform());
            //AprilDebug.Log("向下移动");
        }

        /// <summary>
        /// 刷新滑动区域大小
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="isDown"></param>
        protected void RefreshSize(RectTransform rect, bool isDown = true)
        {
            if (!rect.gameObject.activeSelf) return;
            var aimSize = Mathf.Abs(rect.anchoredPosition.y) + rect.rect.height / 2 + buttomSpacing;

            ///是否向下
            if (isDown)
            {
                if (aimSize > scrollRect.content.rect.height)
                {
                    scrollRect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, aimSize);
                }
            }
            else
            {
                if (aimSize < scrollRect.content.rect.height)
                {
                    scrollRect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, aimSize);
                }
            }

        }

        /// <summary>
        /// 强制刷新大小
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="isDown"></param>
        protected void RefreshSize_Force(RectTransform rect)
        {
            if (!rect.gameObject.activeSelf) return;
            var aimSize = Mathf.Abs(rect.anchoredPosition.y) + rect.rect.height / 2 + buttomSpacing;
            scrollRect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, aimSize);
        }

        #endregion
    }

}
