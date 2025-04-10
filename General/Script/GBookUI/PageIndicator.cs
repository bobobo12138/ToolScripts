using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ҳ��ָʾ��
/// С����ʶ����ҳ��
/// </summary>
public class PageIndicator : MonoBehaviour
{
    /// <summary>
    /// ҳ��ָʾ��
    /// ��������ͨ
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
    /// ����
    /// </summary>
    /// <param name="pageCount">һ���ж���page</param>
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
        //����ػ����һЩ�����ŵĶ���SetAsFirstSibling��Ϊ�˽���Щ�����ƶ������

    }


    /// <summary>
    /// ���ø���λ�ã�0��ʼ
    /// </summary>
    /// <param name="page"></param>
    public void SetPage(int page = 0)
    {
        pageIndicator_HightLight.SetSiblingIndex(page);
    }
}
