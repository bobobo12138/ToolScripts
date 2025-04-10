using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 页面指示器
/// 小点点标识所在页面
/// </summary>
public class PageIndicator : MonoBehaviour
{
    /// <summary>
    /// 页面指示器
    /// 高亮与普通
    /// </summary>
    [SerializeField]
    MonoBehaviour prefab_PageIndicator_HightLight;
    GObjPool_WithPopList<MonoBehaviour> pageIndicator_HightLightPool;

    [SerializeField]
    MonoBehaviour prefab_PageIndicator_Normal;
    GObjPool_WithPopList<MonoBehaviour> pageIndicator_NormalPool;

    Transform pageIndicator_HightLight;

    [SerializeField]
    Transform trans_pointParent;


    public void Init()
    {
        pageIndicator_HightLightPool = new GObjPool_WithPopList<MonoBehaviour>(trans_pointParent, prefab_PageIndicator_HightLight, true);
        pageIndicator_NormalPool = new GObjPool_WithPopList<MonoBehaviour>(trans_pointParent, prefab_PageIndicator_Normal, true);
    }

    /// <summary>
    /// 设置
    /// </summary>
    /// <param name="pageCount">一共有多少page</param>
    public void SetData(int pageCount)
    {
        pageIndicator_HightLightPool.RecycleOutlist();
        pageIndicator_NormalPool.RecycleOutlist();


        pageIndicator_HightLight = pageIndicator_HightLightPool.GetObj().transform;
        for (int i = 0; i < pageCount - 1; i++)
        {
            pageIndicator_NormalPool.GetObj().transform.SetAsFirstSibling();
        }
        pageIndicator_HightLight.SetAsFirstSibling();
        //对象池会产生一些隐藏着的对象，SetAsFirstSibling是为了将这些对象移动至最后

    }


    /// <summary>
    /// 设置高亮位置，0开始
    /// </summary>
    /// <param name="page"></param>
    public void SetPage(int page = 0)
    {
        pageIndicator_HightLight.SetSiblingIndex(page);
    }
}
